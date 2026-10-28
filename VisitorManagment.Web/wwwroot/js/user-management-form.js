(function ($) {
    "use strict";

    var form = document.querySelector("[data-user-create-form]");
    if (!form) { return; }

    var personalCode = form.querySelector("[data-personal-code]");
    var personalHelp = form.querySelector("[data-personal-help]");
    var firstName = document.getElementById("fname");
    var lastName = document.getElementById("lname");
    var loader = document.getElementById("loader");
    var roleSelect = document.getElementById("roleId");
    var roleNotice = form.querySelector("[data-role-assignment-notice]");
    var avatarInput = form.querySelector("[data-avatar-input]");
    var avatarError = form.querySelector("[data-avatar-error]");
    var avatarRemove = form.querySelector("[data-avatar-remove]");
    var avatarImage = document.getElementById("imgAvatar");
    var defaultAvatar = avatarImage.getAttribute("src");

    $("#roleId").select2({ placeholder: "نقش کاربر را انتخاب کنید", dir: "rtl" });

    function showToast(icon, message) {
        if (window.Swal) {
            Swal.fire({ icon: icon, title: icon === "error" ? "خطا" : "توجه", text: message, confirmButtonText: "تأیید" });
            return;
        }
        window.alert(message);
    }

    function encode(value) {
        var element = document.createElement("div");
        element.textContent = value || "";
        return element.innerHTML;
    }

    var personnelRequest = 0;
    personalCode.addEventListener("input", function () {
        personnelRequest++;
        personalCode.setCustomValidity("کد پرسنلی جدید را استعلام بگیرید.");
        personalHelp.textContent = "برای بررسی اطلاعات از فیلد خارج شوید یا Tab بزنید.";
        form.dataset.replacementConfirmed = "";
    });

    personalCode.addEventListener("blur", function () {
        var requestId = ++personnelRequest;
        var code = personalCode.value.trim();
        if (!code) { return; }
        loader.classList.remove("d-none");
        personalCode.setAttribute("aria-busy", "true");
        firstName.value = "";
        lastName.value = "";
        personalHelp.className = "user-field__help";
        personalHelp.textContent = "در حال دریافت اطلاعات پرسنلی...";

        $.get("?handler=GetPersonalId", { personalno: code })
            .done(function (data) {
                if (requestId !== personnelRequest) return;
                if (!data || data.message) {
                    personalCode.setCustomValidity((data && data.message) || "اطلاعات پرسنلی یافت نشد.");
                    personalHelp.classList.add("is-error");
                    personalHelp.textContent = (data && data.message) || "اطلاعات پرسنلی یافت نشد.";
                    if (data && data.alreadyExists && data.userId) {
                        var editLink = document.createElement("a");
                        editLink.href = "/Admin/Users/EditUser/" + Number(data.userId);
                        editLink.textContent = " — ویرایش کاربر موجود";
                        personalHelp.appendChild(editLink);
                    }
                    return;
                }
                personalCode.setCustomValidity("");
                firstName.value = data.firstName || "";
                lastName.value = data.lastName || "";
                document.getElementById("rankTitle").value = data.rankTitle || "";
                personalHelp.classList.add("is-success");
                personalHelp.textContent = "اطلاعات پرسنلی با موفقیت دریافت شد.";
            })
            .fail(function () {
                if (requestId !== personnelRequest) return;
                personalCode.setCustomValidity("دریافت اطلاعات پرسنلی با خطا مواجه شد.");
                personalHelp.classList.add("is-error");
                personalHelp.textContent = "ارتباط با سرویس پرسنلی برقرار نشد.";
                showToast("error", personalHelp.textContent);
            })
            .always(function () {
                loader.classList.add("d-none");
                personalCode.removeAttribute("aria-busy");
            });
    });

    $(roleSelect).on("change", function () {
        form.dataset.replacementConfirmed = "";
        var roleId = parseInt(roleSelect.value, 10);
        roleNotice.classList.add("d-none");
        roleNotice.innerHTML = "";
        if (!roleId) { return; }

        roleNotice.className = "role-assignment-notice role-assignment-loading";
        roleNotice.innerHTML = '<i class="ti-reload"></i> در حال بررسی دارندگان فعلی نقش...';
        $.get("?handler=RoleAssignees", { roleId: roleId })
            .done(function (result) {
                if (!result || !result.success) {
                    roleNotice.className = "role-assignment-notice";
                    roleNotice.textContent = (result && result.message) || "بررسی نقش انجام نشد.";
                    return;
                }
                if (!result.users || !result.users.length) {
                    roleNotice.classList.add("d-none");
                    return;
                }
                var rows = result.users.map(function (user) {
                    var details = [user.rankTitle, user.fullName, user.userName, user.unitTitle, user.isActive ? "فعال" : "غیرفعال"].filter(Boolean).join(" - ");
                    return '<div class="role-assignment-user"><span>' + encode(details) + '<small>' + user.openCartableCount + ' درخواست باز</small></span><a href="/Admin/Users/EditUser/' + user.userId + '">ویرایش کاربر</a></div>';
                }).join("");
                roleNotice.className = "role-assignment-notice";
                roleNotice.innerHTML = '<details class="role-assignment-details"><summary style="cursor:pointer;padding:10px"><i class="ti-user"></i> کاربران دارای این نقش (' + result.users.length + ' نفر) — مشاهده جزئیات</summary><div style="max-height:240px;overflow:auto;padding:10px">' + rows + '</div></details>' +
                    '<label class="role-replacement-confirm"><input type="checkbox" name="replaceExistingRoleHolder" value="true" required />' +
                    '<span><strong>جایگزینی مسئول فعلی و انتقال درخواست‌های باز</strong><small>نقش از مسئول قبلی گرفته می‌شود و سوابق هامش‌ها تغییر نمی‌کند.</small></span></label>';
            })
            .fail(function () {
                roleNotice.className = "role-assignment-notice";
                roleNotice.textContent = "ارتباط با سرور برای بررسی نقش برقرار نشد.";
            });
    });

    avatarInput.addEventListener("change", function () {
        var file = avatarInput.files && avatarInput.files[0];
        avatarError.textContent = "";
        if (!file) { return; }
        if (["image/jpeg", "image/png"].indexOf(file.type) === -1 || file.size > 2 * 1024 * 1024) {
            avatarInput.value = "";
            avatarError.textContent = "فقط تصویر JPG یا PNG با حداکثر حجم ۲ مگابایت مجاز است.";
            showToast("error", avatarError.textContent);
            return;
        }
        var reader = new FileReader();
        reader.onload = function (event) { avatarImage.src = event.target.result; };
        reader.readAsDataURL(file);
        avatarRemove.classList.remove("d-none");
    });

    avatarRemove.addEventListener("click", function () {
        avatarInput.value = "";
        avatarImage.src = defaultAvatar;
        avatarError.textContent = "";
        avatarRemove.classList.add("d-none");
    });

    form.addEventListener("submit", function (event) {
        if (!form.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
            var invalid = Array.prototype.find.call(form.elements, function (field) { return field.willValidate && !field.validity.valid; });
            var validationMessage = invalid && invalid.name === "replaceExistingRoleHolder"
                ? "این نقش مسئول دارد. برای ادامه، جایگزینی مسئول فعلی و انتقال درخواست‌های باز را تأیید کنید؛ یا نقش دیگری انتخاب کنید."
                : (invalid && invalid.validationMessage) || "لطفاً فیلدهای الزامی را کامل کنید.";
            if (invalid && invalid.focus) invalid.focus();
            showToast("warning", validationMessage);
            form.classList.add("was-validated");
            return;
        }
        var replacement = form.querySelector('[name="replaceExistingRoleHolder"]:checked');
        if (replacement && !form.dataset.replacementConfirmed) {
            event.preventDefault();
            if (window.Swal) {
                Swal.fire({
                    icon: "warning",
                    title: "جایگزینی مسئول نقش",
                    text: "نقش از مسئول فعلی گرفته و درخواست‌های باز او به کاربر جدید منتقل می‌شود. ادامه می‌دهید؟",
                    showCancelButton: true,
                    confirmButtonText: "بله، جایگزین شود",
                    cancelButtonText: "انصراف"
                }).then(function (result) {
                    if (!result.isConfirmed) { return; }
                    form.dataset.replacementConfirmed = "true";
                    form.requestSubmit();
                });
            } else if (window.confirm("مسئول فعلی جایگزین و درخواست‌های باز منتقل شود؟")) {
                form.dataset.replacementConfirmed = "true";
                form.requestSubmit();
            }
        }
        form.classList.add("was-validated");
    });
})(window.jQuery);

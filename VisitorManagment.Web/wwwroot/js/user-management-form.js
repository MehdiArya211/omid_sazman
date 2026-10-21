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

    personalCode.addEventListener("blur", function () {
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
                if (!data || data.message) {
                    personalCode.setCustomValidity((data && data.message) || "اطلاعات پرسنلی یافت نشد.");
                    personalHelp.classList.add("is-error");
                    personalHelp.textContent = (data && data.message) || "اطلاعات پرسنلی یافت نشد.";
                    showToast("error", personalHelp.textContent);
                    return;
                }
                personalCode.setCustomValidity("");
                firstName.value = data.firstName || "";
                lastName.value = data.lastName || "";
                personalHelp.classList.add("is-success");
                personalHelp.textContent = "اطلاعات پرسنلی با موفقیت دریافت شد.";
            })
            .fail(function () {
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
                    var details = [user.rankTitle, user.fullName, user.userName, user.unitTitle].filter(Boolean).join(" - ");
                    return '<div class="role-assignment-user"><span>' + encode(details) + '</span><a href="/Admin/Users/EditUser/' + user.userId + '">ویرایش کاربر</a></div>';
                }).join("");
                roleNotice.className = "role-assignment-notice";
                roleNotice.innerHTML = '<div class="role-assignment-notice__head"><i class="ti-alert"></i> این نقش هم‌اکنون به کاربر زیر تخصیص دارد:</div>' + rows;
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
            showToast("warning", "لطفاً فیلدهای الزامی را کامل و صحیح وارد کنید.");
        }
        form.classList.add("was-validated");
    });
})(window.jQuery);

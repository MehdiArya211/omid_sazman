(function ($) {
    "use strict";

    var form = document.querySelector("[data-user-edit-form]");
    if (!form) { return; }

    var personalCode = form.querySelector("[data-edit-personal-code]");
    var personalHelp = form.querySelector("[data-edit-personal-help]");
    var loader = document.getElementById("editPersonnelLoader");
    var roleSelect = document.getElementById("listRoles");
    var roleNotice = form.querySelector("[data-role-assignment-notice]");
    var personnelMoveNotice = form.querySelector("[data-personnel-move-notice]");
    var userId = form.querySelector('[name="userId"]').value;
    var lastLoadedCode = personalCode.value.trim();
    var loadedExistingUser = null;

    function encode(value) {
        var element = document.createElement("div");
        element.textContent = value || "";
        return element.innerHTML;
    }

    function showError(message) {
        if (window.Swal) {
            Swal.fire({ icon: "error", title: "خطا", text: message, confirmButtonText: "تأیید" });
        } else {
            window.alert(message);
        }
    }

    function setValue(id, value) {
        var element = document.getElementById(id);
        if (element) { element.value = value == null ? "" : value; }
    }

    function applyPersonnel(data) {
        setValue("fname", data.firstName);
        setValue("lname", data.lastName);
        setValue("rankTitle", data.rankTitle);
        setValue("editUserViewModel_BranchTitle", data.branchTitle);
        setValue("editUserViewModel_BranchCode", data.branchCode);
        setValue("editUserViewModel_JobDes", data.jobDes);
        setValue("editUserViewModel_RankTitle", data.rankTitle);
        setValue("editUserViewModel_RankCode", data.rankCode);
        setValue("editUserViewModel_UnitDutyCode", data.unitDutyCode);
        setValue("editUserViewModel_UnitCode", data.unitCode);
        setValue("editUserViewModel_UnitDutyTitle", data.unitDutyTitle);
        setValue("editUserViewModel_UnitTitle", data.unitTitle);
        setValue("editUserViewModel_CodGhaTitle", data.codGhaTitle);
        setValue("editUserViewModel_CodGha", data.codGha);
    }

    function showPersonnelMove(existingUser) {
        loadedExistingUser = existingUser || null;
        personnelMoveNotice.className = "role-assignment-notice d-none";
        personnelMoveNotice.innerHTML = "";
        if (!existingUser || String(existingUser.userId) === String(userId)) { return; }

        var targetRole = roleSelect.options[roleSelect.selectedIndex];
        var targetRoleTitle = targetRole ? targetRole.text.trim() : "جایگاه انتخابی";
        personnelMoveNotice.className = "role-assignment-notice";
        personnelMoveNotice.innerHTML =
            '<div class="role-assignment-notice__head"><i class="ti-exchange-vertical"></i> جابه‌جایی مسئول جایگاه</div>' +
            '<p><strong>' + encode(existingUser.fullName) + '</strong> از جایگاه «' +
            encode(existingUser.currentRoleTitle) + '» به جایگاه «' + encode(targetRoleTitle) + '» منتقل می‌شود.</p>' +
            '<p class="mb-2">جایگاه قبلی او خالی می‌ماند و درخواست‌های باز آن جایگاه محفوظ خواهد ماند. مسئول فعلی جایگاه مقصد نیز خارج می‌شود.</p>' +
            '<label class="role-replacement-confirm"><input type="checkbox" name="replaceExistingRoleHolder" value="true" required data-personnel-move-confirm />' +
            '<span><strong>خلاصه جابه‌جایی را خواندم و تأیید می‌کنم</strong><small>هویت کاربران و سوابق هامش‌های قبلی تغییر نمی‌کند.</small></span></label>';
    }

    personalCode.addEventListener("blur", function () {
        var code = personalCode.value.trim();
        if (!code || code === lastLoadedCode) { return; }

        loader.classList.remove("d-none");
        personalCode.setAttribute("aria-busy", "true");
        personalHelp.className = "user-field__help";
        personalHelp.textContent = "در حال دریافت اطلاعات پرسنلی...";

        $.get("?handler=GetPersonalId", { personalno: code })
            .done(function (response) {
                if (!response || !response.success || !response.data) {
                    personalHelp.classList.add("is-error");
                    personalHelp.textContent = (response && response.message) || "پرسنلی با این مشخصات یافت نشد.";
                    showError(personalHelp.textContent);
                    return;
                }
                applyPersonnel(response.data);
                showPersonnelMove(response.existingUser);
                lastLoadedCode = code;
                personalHelp.classList.add("is-success");
                personalHelp.textContent = "اطلاعات پرسنلی با موفقیت به‌روزرسانی شد.";
            })
            .fail(function () {
                personalHelp.classList.add("is-error");
                personalHelp.textContent = "ارتباط با سرویس پرسنلی برقرار نشد.";
                showError(personalHelp.textContent);
            })
            .always(function () {
                loader.classList.add("d-none");
                personalCode.removeAttribute("aria-busy");
            });
    });

    function loadRoleAssignees() {
        var roleId = parseInt(roleSelect.value, 10);
        roleNotice.classList.add("d-none");
        roleNotice.innerHTML = "";
        if (!roleId) { return; }

        roleNotice.className = "role-assignment-notice role-assignment-loading";
        roleNotice.innerHTML = '<i class="ti-reload"></i> در حال بررسی دارندگان فعلی نقش...';
        $.get("?handler=RoleAssignees", { roleId: roleId, userId: userId })
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
                roleNotice.innerHTML = '<div class="role-assignment-notice__head"><i class="ti-alert"></i> این نقش مسئول فعلی دارد:</div>' + rows +
                    '<label class="role-replacement-confirm"><input type="checkbox" name="replaceExistingRoleHolder" value="true" required />' +
                    '<span><strong>جایگزینی مسئول فعلی و انتقال درخواست‌های باز</strong><small>نقش از مسئول قبلی گرفته می‌شود و سوابق هامش‌ها تغییر نمی‌کند.</small></span></label>';
            })
            .fail(function () {
                roleNotice.className = "role-assignment-notice";
                roleNotice.textContent = "ارتباط با سرور برای بررسی نقش برقرار نشد.";
            });
    }

    $(roleSelect).on("change", loadRoleAssignees);
    $(roleSelect).on("change", function () { showPersonnelMove(loadedExistingUser); });
    loadRoleAssignees();

    form.addEventListener("submit", function (event) {
        var replacement = form.querySelector('[name="replaceExistingRoleHolder"]:checked');
        if (!replacement || form.dataset.replacementConfirmed) { return; }
        event.preventDefault();
        if (window.Swal) {
            Swal.fire({
                icon: "warning",
                title: "تأیید جابه‌جایی مسئول",
                text: "مسئول جایگاه تغییر می‌کند؛ کارتابل مقصد منتقل و جایگاه قبلی نفر جدید خالی می‌شود. ادامه می‌دهید؟",
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
    });
})(window.jQuery);

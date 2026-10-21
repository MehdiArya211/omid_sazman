(function ($) {
    "use strict";

    var form = document.querySelector("[data-user-edit-form]");
    if (!form) { return; }

    var personalCode = form.querySelector("[data-edit-personal-code]");
    var personalHelp = form.querySelector("[data-edit-personal-help]");
    var loader = document.getElementById("editPersonnelLoader");
    var roleSelect = document.getElementById("listRoles");
    var roleNotice = form.querySelector("[data-role-assignment-notice]");
    var userId = form.querySelector('[name="userId"]').value;
    var lastLoadedCode = personalCode.value.trim();

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
                    var details = [user.rankTitle, user.fullName, user.userName, user.unitTitle].filter(Boolean).join(" - ");
                    return '<div class="role-assignment-user"><span>' + encode(details) + '</span><a href="/Admin/Users/EditUser/' + user.userId + '">ویرایش کاربر</a></div>';
                }).join("");
                roleNotice.className = "role-assignment-notice";
                roleNotice.innerHTML = '<div class="role-assignment-notice__head"><i class="ti-alert"></i> کاربران دیگری نیز این نقش را دارند:</div>' + rows;
            })
            .fail(function () {
                roleNotice.className = "role-assignment-notice";
                roleNotice.textContent = "ارتباط با سرور برای بررسی نقش برقرار نشد.";
            });
    }

    $(roleSelect).on("change", loadRoleAssignees);
    loadRoleAssignees();
})(window.jQuery);

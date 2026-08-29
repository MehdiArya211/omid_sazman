(function ($) {
    "use strict";

    var form = document.querySelector("[data-user-create-form]");
    if (!form) { return; }

    var personalCode = form.querySelector("[data-personal-code]");
    var personalHelp = form.querySelector("[data-personal-help]");
    var firstName = document.getElementById("fname");
    var lastName = document.getElementById("lname");
    var loader = document.getElementById("loader");
    var password = form.querySelector("[data-password]");
    var meter = form.querySelector("[data-password-meter]");
    var passwordStatus = form.querySelector("[data-password-status]");
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

    function passwordScore(value) {
        var score = 0;
        if (value.length >= 8) { score++; }
        if (/[a-z]/.test(value)) { score++; }
        if (/[A-Z]/.test(value)) { score++; }
        if (/\d/.test(value)) { score++; }
        if (/[^A-Za-z0-9]/.test(value)) { score++; }
        return score;
    }

    password.addEventListener("input", function () {
        var score = passwordScore(password.value);
        var states = ["رمز عبور را وارد کنید", "ضعیف", "متوسط", "خوب", "قوی", "بسیار قوی"];
        var colors = ["#c84a5a", "#c84a5a", "#e3912d", "#3157d5", "#148760", "#148760"];
        meter.style.width = (score * 20) + "%";
        meter.style.backgroundColor = colors[score];
        passwordStatus.textContent = states[score];
        passwordStatus.style.color = colors[score];
        password.setCustomValidity(password.value && score < 4 ? "رمز عبور باید قوی باشد." : "");
    });

    form.querySelector("[data-password-toggle]").addEventListener("click", function () {
        var isHidden = password.type === "password";
        password.type = isHidden ? "text" : "password";
        this.innerHTML = isHidden ? '<i class="ti-close"></i>' : '<i class="ti-eye"></i>';
        this.setAttribute("aria-label", isHidden ? "پنهان کردن رمز عبور" : "نمایش رمز عبور");
    });

    personalCode.addEventListener("blur", function () {
        var code = personalCode.value.trim();
        if (!code) { return; }
        loader.classList.remove("d-none");
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
            .always(function () { loader.classList.add("d-none"); });
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

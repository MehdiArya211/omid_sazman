(function () {
    'use strict';

    var password = document.querySelector('[data-new-password]');
    if (!password) return;

    var rules = {
        length: function (value) { return value.length >= 8; },
        upper: function (value) { return /[A-Z]/.test(value); },
        lower: function (value) { return /[a-z]/.test(value); },
        number: function (value) { return /[0-9\u0660-\u0669\u06F0-\u06F9]/.test(value); },
        symbol: function (value) { return /[^A-Za-z0-9\u0660-\u0669\u06F0-\u06F9]/.test(value); }
    };
    var colors = ['#c84a5a', '#d48a28', '#c1a22b', '#398f79', '#148760'];
    var titles = ['بسیار ضعیف', 'ضعیف', 'متوسط', 'خوب', 'قوی'];

    function updateStrength() {
        var value = password.value;
        var score = 0;
        Object.keys(rules).forEach(function (ruleName) {
            var isValid = rules[ruleName](value);
            var rule = document.querySelector('[data-rule="' + ruleName + '"]');
            if (rule) {
                rule.classList.toggle('is-valid', isValid);
                rule.classList.toggle('is-invalid', value.length > 0 && !isValid);
            }
            if (isValid) score += 1;
        });

        var bar = document.querySelector('[data-strength-bar]');
        var title = document.querySelector('[data-strength-title]');
        bar.style.width = value ? (score * 20) + '%' : '0';
        bar.style.background = colors[Math.max(0, score - 1)];
        title.textContent = 'قدرت رمز: ' + (value ? titles[Math.max(0, score - 1)] : 'وارد نشده');
        var validationMessage = document.querySelector('[data-valmsg-for="NewPassword"]');
        if (value && score === Object.keys(rules).length && validationMessage &&
            validationMessage.textContent.indexOf('شامل حرف بزرگ و کوچک') >= 0) {
            validationMessage.textContent = '';
            validationMessage.classList.remove('field-validation-error');
            validationMessage.classList.add('field-validation-valid');
        }
    }

    document.querySelectorAll('[data-toggle-password]').forEach(function (button) {
        button.addEventListener('click', function () {
            var input = button.parentElement.querySelector('input');
            var showPassword = input.type === 'password';
            input.type = showPassword ? 'text' : 'password';
            button.querySelector('i').className = showPassword ? 'ti-eye-off' : 'ti-eye';
            button.setAttribute('aria-pressed', showPassword ? 'true' : 'false');
            button.setAttribute('aria-label', showPassword ? 'مخفی‌کردن رمز عبور' : 'نمایش رمز عبور');
        });
    });

    var passwordForm = document.querySelector('[data-password-change-form]');
    if (passwordForm) {
        passwordForm.addEventListener('submit', function () {
            var form = this;
            var submit = form.querySelector('[type="submit"]');
            if (!form.checkValidity() || !submit || submit.disabled) return;

            var originalContent = submit.innerHTML;
            submit.disabled = true;
            submit.innerHTML = '<span class="spinner-border spinner-border-sm" aria-hidden="true"></span><span>در حال ثبت...</span>';

            window.setTimeout(function () {
                if (!document.documentElement.contains(form) || !submit.disabled) return;
                submit.disabled = false;
                submit.innerHTML = originalContent;

                var notice = form.querySelector('[data-submit-timeout]');
                if (!notice) {
                    notice = document.createElement('div');
                    notice.setAttribute('data-submit-timeout', '');
                    notice.className = 'alert alert-warning mt-2';
                    notice.setAttribute('role', 'alert');
                    form.insertBefore(notice, submit);
                }
                notice.textContent = 'پاسخی از سامانه دریافت نشد. اتصال را بررسی کنید و دوباره تلاش کنید.';
            }, 35000);
        });
    }

    password.addEventListener('input', updateStrength);
    updateStrength();
}());

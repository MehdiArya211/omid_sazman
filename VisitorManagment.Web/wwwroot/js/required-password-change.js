(function () {
    'use strict';

    var password = document.querySelector('[data-new-password]');
    if (!password) return;

    var rules = {
        length: function (value) { return value.length >= 8; },
        upper: function (value) { return /[A-Z]/.test(value); },
        lower: function (value) { return /[a-z]/.test(value); },
        number: function (value) { return /\d/.test(value); },
        symbol: function (value) { return /[^A-Za-z0-9]/.test(value); }
    };
    var colors = ['#c84a5a', '#d48a28', '#c1a22b', '#398f79', '#148760'];
    var titles = ['بسیار ضعیف', 'ضعیف', 'متوسط', 'خوب', 'قوی'];

    function updateStrength() {
        var value = password.value;
        var score = 0;
        Object.keys(rules).forEach(function (ruleName) {
            var isValid = rules[ruleName](value);
            var rule = document.querySelector('[data-rule="' + ruleName + '"]');
            if (rule) rule.classList.toggle('is-valid', isValid);
            if (isValid) score += 1;
        });

        var bar = document.querySelector('[data-strength-bar]');
        var title = document.querySelector('[data-strength-title]');
        bar.style.width = value ? (score * 20) + '%' : '0';
        bar.style.background = colors[Math.max(0, score - 1)];
        title.textContent = 'قدرت رمز: ' + (value ? titles[Math.max(0, score - 1)] : 'وارد نشده');
    }

    document.querySelectorAll('[data-toggle-password]').forEach(function (button) {
        button.addEventListener('click', function () {
            var input = button.parentElement.querySelector('input');
            var showPassword = input.type === 'password';
            input.type = showPassword ? 'text' : 'password';
            button.querySelector('i').className = showPassword ? 'ti-eye-off' : 'ti-eye';
        });
    });

    document.querySelector('[data-password-change-form]').addEventListener('submit', function () {
        var submit = this.querySelector('[type="submit"]');
        if (!this.checkValidity()) return;
        submit.disabled = true;
        submit.innerHTML = '<span class="spinner-border spinner-border-sm" aria-hidden="true"></span><span>در حال ثبت...</span>';
    });

    password.addEventListener('input', updateStrength);
    updateStrength();
}());

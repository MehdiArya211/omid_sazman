(function () {
    "use strict";
    var text = document.querySelector("[data-hamesh-text]");
    var count = document.querySelector("[data-hamesh-count]");
    if (text && count) {
        var updateCount = function () {
            count.textContent = text.value.length.toString().replace(/\d/g, function (digit) { return "۰۱۲۳۴۵۶۷۸۹"[digit]; });
        };
        text.addEventListener("input", updateCount);
        updateCount();
    }

    document.querySelectorAll(".hamesh-page .modal").forEach(function (modal) {
        modal.addEventListener("shown.bs.modal", function () {
            var input = modal.querySelector("input:not([type=hidden]), select, textarea, button");
            if (input) { input.focus(); }
        });
    });
})();

(function () {
    "use strict";

    function fire(options) {
        if (window.Swal && typeof window.Swal.fire === "function") {
            return window.Swal.fire(options);
        }
        window.alert(options.text || options.title || "عملیات انجام شد.");
        return Promise.resolve({ isConfirmed: true });
    }

    function showServerNotification() {
        var element = document.getElementById("operation-notification");
        if (!element) return;

        fire({
            icon: element.dataset.icon || "info",
            title: element.dataset.title || "پیام سیستم",
            text: element.dataset.message || "",
            confirmButtonText: "تأیید",
            confirmButtonColor: "#123f68",
            allowOutsideClick: false
        });
    }

    function enableDeleteConfirmation() {
        document.addEventListener("submit", function (event) {
            var form = event.target;
            if (!(form instanceof HTMLFormElement) || form.dataset.confirmed === "true") return;

            var submitter = event.submitter;
            var requiresConfirmation = form.matches("[data-confirm]") ||
                (submitter && submitter.matches("[data-confirm]"));
            if (!requiresConfirmation) return;

            event.preventDefault();
            var message = (submitter && submitter.dataset.confirm) || form.dataset.confirm ||
                "آیا از انجام این عملیات مطمئن هستید؟";

            fire({
                icon: "warning",
                title: "تأیید عملیات",
                text: message,
                showCancelButton: true,
                confirmButtonText: "بله، انجام شود",
                cancelButtonText: "انصراف",
                confirmButtonColor: "#b42318",
                cancelButtonColor: "#64748b",
                reverseButtons: true
            }).then(function (result) {
                if (!result.isConfirmed) return;
                form.dataset.confirmed = "true";
                if (submitter && form.requestSubmit) form.requestSubmit(submitter);
                else form.submit();
            });
        });
    }

    function enableValidationFeedback() {
        document.addEventListener("invalid", function (event) {
            event.target.classList.add("is-invalid");
        }, true);

        document.addEventListener("input", function (event) {
            if (event.target.matches("input, select, textarea")) {
                event.target.classList.remove("is-invalid");
            }
        });

        document.addEventListener("submit", function (event) {
            var form = event.target;
            if (!(form instanceof HTMLFormElement) || form.noValidate) return;
            if (form.checkValidity()) return;

            event.preventDefault();
            var firstInvalid = form.querySelector(":invalid");
            if (firstInvalid) firstInvalid.focus();
            fire({
                icon: "warning",
                title: "اطلاعات فرم کامل نیست",
                text: "لطفاً فیلدهای مشخص‌شده را بررسی و تکمیل کنید.",
                confirmButtonText: "متوجه شدم",
                confirmButtonColor: "#123f68"
            });
        });
    }

    document.addEventListener("DOMContentLoaded", function () {
        showServerNotification();
        enableDeleteConfirmation();
        enableValidationFeedback();
    });
})();

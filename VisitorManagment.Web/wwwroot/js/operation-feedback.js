(function () {
    "use strict";

    var busyTimeout = 20000;

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

    /** تشخیص می‌دهد دکمه یا فرم جاری یک عملیات حذف واقعی است. */
    function isDeleteOperation(form, submitter) {
        var marker = [
            window.location.pathname,
            form.getAttribute("action"),
            form.dataset.operation,
            submitter && submitter.name,
            submitter && submitter.value,
            submitter && submitter.getAttribute("formaction"),
            submitter && submitter.dataset.operation,
            submitter && submitter.textContent
        ].filter(Boolean).join(" ").toLowerCase();
        return /delete|remove|حذف/.test(marker);
    }

    /** کنترل‌های نامعتبر را مشخص می‌کند و با اولین خطا به کاربر بازخورد می‌دهد. */
    function enableValidationFeedback() {
        document.addEventListener("invalid", function (event) {
            event.target.classList.add("is-invalid");
            event.target.setAttribute("aria-invalid", "true");
        }, true);

        document.addEventListener("input", function (event) {
            if (event.target.matches("input, select, textarea")) {
                event.target.classList.remove("is-invalid");
                event.target.removeAttribute("aria-invalid");
            }
        });

    }

    /** هنگام ارسال موفق فرم از کلیک تکراری جلوگیری می‌کند. */
    function setBusy(form, submitter) {
        if (!submitter || form.dataset.noBusy === "true") return;
        var isInput = submitter.tagName === "INPUT";
        submitter.dataset.originalHtml = isInput ? submitter.value : submitter.innerHTML;
        form.dataset.submitting = "true";
        window.setTimeout(function () {
            submitter.classList.add("is-submitting");
            submitter.disabled = true;
            if (isInput) submitter.value = "در حال انجام...";
            else submitter.innerHTML = '<span class="crud-spinner" aria-hidden="true"></span><span>در حال انجام...</span>';
        }, 0);
        window.setTimeout(function () {
            if (!submitter.isConnected) return;
            delete form.dataset.submitting;
            delete form.dataset.confirmed;
            submitter.disabled = false;
            submitter.classList.remove("is-submitting");
            if (isInput) submitter.value = submitter.dataset.originalHtml || submitter.value;
            else submitter.innerHTML = submitter.dataset.originalHtml || submitter.innerHTML;
        }, busyTimeout);
    }

    /** اعتبارسنجی، تأیید عملیات حساس و وضعیت ارسال را در یک مسیر هماهنگ اجرا می‌کند. */
    function enableSubmitCoordinator() {
        document.addEventListener("submit", function (event) {
            var form = event.target;
            if (!(form instanceof HTMLFormElement) || event.defaultPrevented) return;
            var submitter = event.submitter || form.querySelector('button:not([type]), button[type="submit"], input[type="submit"]');
            if (form.dataset.submitting === "true") {
                event.preventDefault();
                return;
            }

            if (!form.noValidate && !form.checkValidity()) {
                event.preventDefault();
                var firstInvalid = form.querySelector(":invalid");
                if (firstInvalid) {
                    firstInvalid.classList.add("is-invalid");
                    firstInvalid.focus();
                }
                fire({
                    icon: "warning",
                    title: "اطلاعات فرم کامل نیست",
                    text: "لطفاً فیلدهای مشخص‌شده را بررسی و تکمیل کنید.",
                    confirmButtonText: "متوجه شدم",
                    confirmButtonColor: "#123f68"
                });
                return;
            }

            var explicitConfirm = form.dataset.confirm || (submitter && submitter.dataset.confirm);
            if (form.dataset.confirmed !== "true" && (explicitConfirm || isDeleteOperation(form, submitter))) {
                event.preventDefault();
                fire({
                    icon: "warning",
                    title: "تأیید حذف",
                    text: explicitConfirm || "آیا از حذف این مورد مطمئن هستید؟ این عملیات قابل بازگشت نیست.",
                    showCancelButton: true,
                    confirmButtonText: "بله، حذف شود",
                    cancelButtonText: "انصراف",
                    confirmButtonColor: "#b42318",
                    cancelButtonColor: "#64748b",
                    reverseButtons: true,
                    focusCancel: true
                }).then(function (result) {
                    if (!result.isConfirmed) return;
                    form.dataset.confirmed = "true";
                    if (submitter && form.requestSubmit) form.requestSubmit(submitter);
                    else form.submit();
                });
                return;
            }

            setBusy(form, submitter);
        });
    }

    /** فرم‌ها و دکمه‌های قدیمی را بدون تغییر ساختار Razor به الگوی مشترک متصل می‌کند. */
    function enhanceCrudForms(root) {
        var forms = root && root.matches && root.matches("form") ? [root] :
            (root === document ? document.querySelectorAll(".main-content form") : (root || document).querySelectorAll("form"));
        Array.prototype.forEach.call(forms, function (form) {
            if (!form.closest(".main-content")) return;
            if (form.matches(".no-unified-form, .dataTables_filter, [data-ui-skip]")) return;
            var method = (form.getAttribute("method") || "get").toLowerCase();
            if (method !== "post" || !form.querySelector("input, select, textarea")) return;
            form.classList.add("unified-crud-form");

            form.querySelectorAll("[required]").forEach(function (control) {
                if (!control.id) return;
                var label = form.querySelector('label[for="' + control.id.replace(/(["\\])/g, "\\$1") + '"]');
                if (label) label.classList.add("required");
            });

            form.querySelectorAll('button:not([type]), button[type="submit"], input[type="submit"]').forEach(function (button) {
                var marker = ((button.textContent || button.value || "") + " " + (button.name || "") + " " +
                    (button.value || "") + " " + (form.getAttribute("action") || "") + " " + window.location.pathname).toLowerCase();
                button.classList.add("crud-action");
                if (/delete|remove|حذف/.test(marker)) button.classList.add("crud-action--delete");
                else if (/edit|update|ویرایش/.test(marker)) button.classList.add("crud-action--edit");
                else button.classList.add("crud-action--save");
            });
        });
    }

    document.addEventListener("DOMContentLoaded", function () {
        showServerNotification();
        enableValidationFeedback();
        enableSubmitCoordinator();
        enhanceCrudForms(document);
        if (window.MutationObserver) {
            new MutationObserver(function (mutations) {
                mutations.forEach(function (mutation) {
                    Array.prototype.forEach.call(mutation.addedNodes, function (node) {
                        if (node.nodeType === 1) enhanceCrudForms(node);
                    });
                });
            }).observe(document.querySelector(".main-content") || document.body, { childList: true, subtree: true });
        }
    });
})();

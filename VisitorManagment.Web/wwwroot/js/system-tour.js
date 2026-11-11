(function () {
    "use strict";

    var trigger = document.querySelector("[data-system-tour-start]");
    if (!trigger) return;

    var userKey = document.body.dataset.userId || document.querySelector("[data-tour-user]")?.dataset.tourUser || "user";
    var storageKey = "omid-system-tour-v1-" + userKey;
    var steps = [
        {
            title: "به سامانه امید خوش آمدید",
            text: "این راهنمای کوتاه کمک می‌کند سریع‌تر بخش‌های اصلی سامانه را پیدا کنید. با «بعدی» ادامه دهید؛ هر زمان هم بخواهید می‌توانید از دکمه راهنما در نوار بالا دوباره آن را اجرا کنید.",
            selector: null,
            icon: "🎓"
        },
        {
            title: "منوی بخش‌ها",
            text: "بخش‌هایی که برای نقش شما مجاز هستند در این منو دیده می‌شوند. برای دیدن زیرمنوها روی عنوان آن‌ها بزنید؛ سامانه آخرین صفحه‌ی شما را هم مشخص می‌کند.",
            selector: ".navigation-menu-body",
            icon: "☰"
        },
        {
            title: "محیط کار شما",
            text: "محتوای هر صفحه در این قسمت نمایش داده می‌شود. عنوان صفحه و پیام‌های راهنما را بررسی کنید و برای پیدا کردن گزینه‌ها از منوی سمت راست کمک بگیرید.",
            selector: ".main-content",
            icon: "▤"
        },
        {
            title: "اعلان‌های سامانه",
            text: "از زنگوله می‌توانید اعلان‌های جدید و فایل‌های پیوست را ببینید. در صفحه اعلان‌ها هم جستجو، فیلتر و دریافت فایل‌ها در دسترس است.",
            selector: ".system-notification-link",
            icon: "♧",
            optional: true
        },
        {
            title: "تنظیمات نمایش",
            text: "از این گزینه اندازه و فونت نوشته‌ها را متناسب با نیاز خود تغییر دهید. این تنظیمات برای حساب شما ذخیره می‌شود.",
            selector: "#uiSettingsToggle",
            icon: "A",
            optional: true
        },
        {
            title: "راهنما همیشه در دسترس است",
            text: "با انتخاب این دکمه هر زمان نیاز داشتید تور را دوباره ببینید. برای پایان، «پایان راهنما» را بزنید.",
            selector: "[data-system-tour-start]",
            icon: "?"
        }
    ];

    var overlay, card, spotlight, title, content, progress, stepText, previous, next, close;
    var current = 0;
    var visibleSteps = [];

    function createTour() {
        overlay = document.createElement("div");
        overlay.className = "system-tour-overlay";
        overlay.setAttribute("role", "presentation");
        overlay.innerHTML =
            '<div class="system-tour-spotlight" aria-hidden="true"></div>' +
            '<section class="system-tour-card" role="dialog" aria-modal="true" aria-labelledby="systemTourTitle" aria-describedby="systemTourText" dir="rtl">' +
            '<div class="system-tour-card__top"><span class="system-tour-step-icon" data-tour-icon></span><button type="button" class="system-tour-close" aria-label="بستن راهنما">×</button></div>' +
            '<div class="system-tour-card__progress"><span data-tour-progress></span></div>' +
            '<small data-tour-step></small><h2 id="systemTourTitle"></h2><p id="systemTourText"></p>' +
            '<footer><button type="button" class="system-tour-prev">قبلی</button><div><button type="button" class="system-tour-skip">فعلاً رد شو</button><button type="button" class="system-tour-next">بعدی</button></div></footer>' +
            '</section>';
        document.body.appendChild(overlay);
        card = overlay.querySelector(".system-tour-card");
        spotlight = overlay.querySelector(".system-tour-spotlight");
        title = overlay.querySelector("#systemTourTitle");
        content = overlay.querySelector("#systemTourText");
        progress = overlay.querySelector("[data-tour-progress]");
        stepText = overlay.querySelector("[data-tour-step]");
        previous = overlay.querySelector(".system-tour-prev");
        next = overlay.querySelector(".system-tour-next");
        close = overlay.querySelector(".system-tour-close");

        previous.addEventListener("click", function () { showStep(current - 1); });
        next.addEventListener("click", function () {
            if (current >= visibleSteps.length - 1) finish(true);
            else showStep(current + 1);
        });
        overlay.querySelector(".system-tour-skip").addEventListener("click", function () { finish(true); });
        close.addEventListener("click", function () { finish(true); });
        overlay.addEventListener("click", function (event) { if (event.target === overlay) finish(true); });
        document.addEventListener("keydown", onKeydown);
    }

    function onKeydown(event) {
        if (!overlay || !document.body.contains(overlay)) return;
        if (event.key === "Escape") finish(true);
        if (event.key === "ArrowLeft" && current < visibleSteps.length - 1) showStep(current + 1);
        if (event.key === "ArrowRight" && current > 0) showStep(current - 1);
    }

    function getTarget(step) {
        return step.selector ? document.querySelector(step.selector) : null;
    }

    function positionCard(target) {
        var margin = 16;
        card.style.left = "";
        card.style.right = "";
        card.style.top = "";
        card.style.transform = "";
        if (!target) {
            spotlight.hidden = true;
            card.removeAttribute("data-placement");
            card.classList.add("is-centered");
            return;
        }
        spotlight.hidden = false;
        card.classList.remove("is-centered");
        var rect = target.getBoundingClientRect();
        var gap = 10;
        spotlight.style.left = Math.max(4, rect.left - gap) + "px";
        spotlight.style.top = Math.max(4, rect.top - gap) + "px";
        spotlight.style.width = Math.min(window.innerWidth - 8, rect.width + gap * 2) + "px";
        spotlight.style.height = Math.min(window.innerHeight - 8, rect.height + gap * 2) + "px";
        var cardHeight = card.offsetHeight;
        var top = rect.bottom + 18;
        var placement = "below";
        if (top + cardHeight > window.innerHeight - margin) {
            top = rect.top - cardHeight - 18;
            placement = "above";
        }
        if (top < margin) top = margin;
        card.setAttribute("data-placement", placement);
        var left = Math.min(Math.max(margin, rect.left + rect.width / 2 - Math.min(card.offsetWidth, window.innerWidth - margin * 2) / 2), window.innerWidth - card.offsetWidth - margin);
        card.style.top = top + "px";
        card.style.left = left + "px";
    }

    function showStep(index) {
        current = Math.max(0, Math.min(index, visibleSteps.length - 1));
        var step = visibleSteps[current];
        var target = getTarget(step);
        document.querySelectorAll(".system-tour-target").forEach(function (node) { node.classList.remove("system-tour-target"); });
        if (target) {
            target.classList.add("system-tour-target");
            target.scrollIntoView({ behavior: "smooth", block: "center", inline: "nearest" });
        }
        overlay.querySelector("[data-tour-icon]").textContent = step.icon || "•";
        title.textContent = step.title;
        content.textContent = step.text;
        stepText.textContent = "مرحله " + (current + 1) + " از " + visibleSteps.length;
        progress.style.width = ((current + 1) / visibleSteps.length * 100) + "%";
        previous.disabled = current === 0;
        next.textContent = current === visibleSteps.length - 1 ? "پایان راهنما" : "بعدی";
        window.setTimeout(function () { positionCard(target); }, 260);
    }

    function finish(completed) {
        if (overlay && overlay.parentNode) overlay.parentNode.removeChild(overlay);
        document.querySelectorAll(".system-tour-target").forEach(function (node) { node.classList.remove("system-tour-target"); });
        document.body.classList.remove("system-tour-open");
        window.removeEventListener("resize", reposition);
        window.removeEventListener("scroll", reposition, true);
        document.removeEventListener("keydown", onKeydown);
        if (completed) {
            try { window.localStorage.setItem(storageKey, "done"); } catch (ignore) { }
        }
        trigger.focus();
        overlay = null;
    }

    function reposition() {
        if (!overlay || !document.body.contains(overlay)) return;
        positionCard(getTarget(visibleSteps[current]));
    }

    function start() {
        if (overlay) return;
        visibleSteps = steps.filter(function (step) { return !step.selector || getTarget(step) || !step.optional; });
        createTour();
        document.body.classList.add("system-tour-open");
        window.addEventListener("resize", reposition);
        window.addEventListener("scroll", reposition, true);
        showStep(0);
        (close || next).focus();
    }

    trigger.addEventListener("click", start);
    var shouldAutoStart = false;
    try { shouldAutoStart = window.localStorage.getItem(storageKey) !== "done"; } catch (ignore) { shouldAutoStart = true; }
    if (shouldAutoStart) window.setTimeout(start, 900);
})();
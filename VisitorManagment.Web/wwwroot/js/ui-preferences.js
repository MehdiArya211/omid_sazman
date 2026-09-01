(function () {
    "use strict";
    var key = "omid.ui.preferences.v1";
    var root = document.documentElement;
    var fonts = { default: "inherit", tahoma: "Tahoma, Arial, sans-serif", arial: "Arial, Tahoma, sans-serif" };

    function read() {
        try { return JSON.parse(localStorage.getItem(key)) || {}; } catch (_) { return {}; }
    }
    function apply(value) {
        var size = Math.max(90, Math.min(110, Number(value.size) || 100));
        var font = fonts[value.font] ? value.font : "default";
        root.style.setProperty("--user-font-scale", size / 100);
        root.style.setProperty("--user-font-family", fonts[font]);
        root.dataset.userFont = font;
    }
    apply(read());

    document.addEventListener("DOMContentLoaded", function () {
        var panel = document.getElementById("uiSettingsPanel");
        var toggle = document.getElementById("uiSettingsToggle");
        var backdrop = document.getElementById("uiSettingsBackdrop");
        var font = document.getElementById("uiFontFamily");
        var size = document.getElementById("uiFontSize");
        var output = document.getElementById("uiFontSizeValue");
        if (!panel || !toggle || !font || !size) return;
        var value = read();
        font.value = fonts[value.font] ? value.font : "default";
        size.value = Math.max(90, Math.min(110, Number(value.size) || 100));
        output.textContent = size.value + "٪";

        function open(state) {
            panel.classList.toggle("is-open", state);
            panel.setAttribute("aria-hidden", String(!state));
            toggle.setAttribute("aria-expanded", String(state));
            backdrop.hidden = !state;
        }
        function save() {
            var next = { font: font.value, size: Number(size.value) };
            localStorage.setItem(key, JSON.stringify(next));
            output.textContent = size.value + "٪";
            apply(next);
        }
        toggle.addEventListener("click", function () { open(!panel.classList.contains("is-open")); });
        panel.querySelector(".ui-settings-close").addEventListener("click", function () { open(false); });
        backdrop.addEventListener("click", function () { open(false); });
        font.addEventListener("change", save);
        size.addEventListener("input", save);
        document.getElementById("uiSettingsReset").addEventListener("click", function () {
            localStorage.removeItem(key); font.value = "default"; size.value = 100; save();
        });
        document.addEventListener("keydown", function (event) { if (event.key === "Escape") open(false); });
    });
})();

(function () {
    "use strict";
    var dateElement = document.querySelector("[data-current-persian-date]");
    var timeElement = document.querySelector("[data-current-time]");
    if (!dateElement || !timeElement) return;

    var dateFormatter = new Intl.DateTimeFormat("fa-IR-u-ca-persian", {
        weekday: "long", year: "numeric", month: "long", day: "numeric"
    });
    var timeFormatter = new Intl.DateTimeFormat("fa-IR", {
        hour: "2-digit", minute: "2-digit", second: "2-digit", hour12: false
    });

    function updateClock() {
        var now = new Date();
        dateElement.textContent = dateFormatter.format(now);
        timeElement.textContent = "ساعت " + timeFormatter.format(now);
        dateElement.setAttribute("datetime", now.toISOString());
        timeElement.setAttribute("datetime", now.toISOString());
    }

    updateClock();
    window.setInterval(updateClock, 1000);
})();

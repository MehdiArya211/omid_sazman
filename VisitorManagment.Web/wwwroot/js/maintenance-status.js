(function () {
    'use strict';
    var config = document.getElementById('maintenance-client-config');
    if (!config) return;
    var pending = false;
    var navigating = false;
    window.omidMaintenanceCheck = function () {
        if (pending || navigating) return;
        pending = true;
        fetch(config.dataset.statusUrl, { credentials: 'same-origin', cache: 'no-store' })
            .then(function (response) { return response.ok ? response.json() : null; })
            .then(function (state) {
                if (state && state.blocked) {
                    navigating = true;
                    // خروج از صفحه، دوربین/میکروفن و PeerConnectionهای باز را نیز آزاد می‌کند.
                    window.location.replace(config.dataset.maintenanceUrl);
                }
            })
            .catch(function () { })
            .then(function () { pending = false; });
    };
    window.addEventListener('pageshow', window.omidMaintenanceCheck);
    window.addEventListener('focus', window.omidMaintenanceCheck);
    window.setInterval(window.omidMaintenanceCheck, 5000);
    window.omidMaintenanceCheck();
})();

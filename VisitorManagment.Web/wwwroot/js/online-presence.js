(function () {
    "use strict";
    if (!window.signalR) return;
    var connection = new signalR.HubConnectionBuilder().withUrl("/onlineUsersHub").build();
    var retryTimer = null;
    function renderCount(count) { var target = document.getElementById("onlineUsersCount"); if (target) target.textContent = Number(count || 0).toLocaleString("fa-IR"); }
    connection.on("UpdateOnlineUsers", function (count) { renderCount(count); });
    connection.onclose(function () { window.clearTimeout(retryTimer); retryTimer = window.setTimeout(start, 3000); });
    function start() { connection.start().catch(function () { window.clearTimeout(retryTimer); retryTimer = window.setTimeout(start, 5000); }); }
    start();
})();

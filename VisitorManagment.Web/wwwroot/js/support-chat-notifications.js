(function () {
    "use strict";
    if (!window.signalR || window.location.pathname.toLowerCase().indexOf("/admin/supportchatonline") === 0) return;
    var connection = new signalR.HubConnectionBuilder().withUrl("/supporthub").withAutomaticReconnect([0, 2000, 5000, 10000]).build();
    var count = 0;
    function show(sender, message) {
        count += 1;
        var badge = document.getElementById("supportUnreadBadge");
        if (badge) { badge.hidden = false; badge.textContent = count > 99 ? "+۹۹" : count.toLocaleString("fa-IR"); }
        if (window.Swal) {
            Swal.fire({ toast:true, position:"top-start", icon:"info", title:"پیام جدید پشتیبانی", text:(sender || "کاربر") + ": " + (message || ""), showConfirmButton:true, confirmButtonText:"مشاهده", timer:8000 })
                .then(function (result) { if (result.isConfirmed) window.location.href = "/Admin/SupportChatOnline/Index"; });
        }
    }
    connection.on("newSupportMessage", function (_, sender, message) { show(sender, message); });
    function start() { if (connection.state === signalR.HubConnectionState.Disconnected) connection.start().catch(function () { window.setTimeout(start, 5000); }); }
    start();
})();

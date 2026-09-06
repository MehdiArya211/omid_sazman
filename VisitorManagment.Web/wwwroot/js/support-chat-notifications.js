(function () {
    "use strict";
    if (!window.signalR || window.location.pathname.toLowerCase().indexOf("/admin/supportchatonline") === 0) return;
    var connection = new signalR.HubConnectionBuilder().withUrl("/supporthub").withAutomaticReconnect([0, 2000, 5000, 10000]).build();
    var storageKey = "omid-support-unread-count";
    var count = parseInt(window.localStorage.getItem(storageKey) || "0", 10) || 0;
    var isSupportPage = window.location.pathname.toLowerCase().indexOf("/admin/supportchatonline") === 0;
    function updateBadge() {
        var badge = document.getElementById("supportUnreadBadge");
        if (!badge) return;
        badge.hidden = count < 1;
        badge.textContent = count > 99 ? "+۹۹" : count.toLocaleString("fa-IR");
    }
    function show(sender, message) {
        count += 1;
        window.localStorage.setItem(storageKey, String(count));
        updateBadge();
        if (window.Swal) {
            Swal.fire({ toast:true, position:"top-start", icon:"info", title:"پیام جدید پشتیبانی", text:(sender || "کاربر") + ": " + (message || ""), showConfirmButton:true, confirmButtonText:"مشاهده", timer:8000 })
                .then(function (result) { if (result.isConfirmed) window.location.href = "/Admin/SupportChatOnline/Index"; });
        }
    }
    connection.on("newSupportMessage", function (_, value, message) {
        var item = value && typeof value === "object" ? value : { sender:value, message:message };
        if (!isSupportPage) show(item.sender, item.message);
    });
    function start() { if (connection.state === signalR.HubConnectionState.Disconnected) connection.start().catch(function () { window.setTimeout(start, 5000); }); }
    if (isSupportPage) { count = 0; window.localStorage.removeItem(storageKey); }
    updateBadge();
    start();
})();

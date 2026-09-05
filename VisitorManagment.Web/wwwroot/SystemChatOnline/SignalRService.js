(function () {
    "use strict";
    var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").withAutomaticReconnect([0, 2000, 5000, 10000]).build();
    var form, input, messages, button, status;
    function formatPersianDate(value) {
        var date = new Date(value); if (Number.isNaN(date.getTime())) return value || "";
        return new Intl.DateTimeFormat("fa-IR-u-ca-persian", {year:"numeric",month:"2-digit",day:"2-digit",hour:"2-digit",minute:"2-digit"}).format(date);
    }
    function addMessage(sender, message, time) {
        var item = document.createElement("li"), meta = document.createElement("div"), body = document.createElement("div");
        meta.className = "chat-message-meta"; body.className = "messages chat-message-body";
        var fullName = document.getElementById("fullNameUser");
        item.classList.toggle("is-current-user", !!fullName && sender === fullName.value);
        meta.textContent = (sender || "کاربر") + " · " + formatPersianDate(time); body.textContent = message || "";
        item.appendChild(meta); item.appendChild(body); messages.appendChild(item); messages.scrollTop = messages.scrollHeight;
    }
    function setState(text, connected) {
        if (status) { status.textContent = text; status.classList.toggle("is-online", connected); }
        if (button) button.disabled = !connected;
    }
    async function start() {
        if (connection.state !== signalR.HubConnectionState.Disconnected) return;
        try { await connection.start(); setState("آنلاین", true); }
        catch (_) { setState("در حال اتصال مجدد...", false); window.setTimeout(start, 5000); }
    }
    document.addEventListener("DOMContentLoaded", function () {
        form = document.getElementById("NewMessageForm"); input = document.getElementById("MessageInput");
        messages = document.getElementById("Messages"); status = document.getElementById("chatConnectionStatus");
        if (!form || !input || !messages) return; button = form.querySelector("button[type=submit]");
        form.addEventListener("submit", async function (event) {
            event.preventDefault(); var text = input.value.trim();
            if (!text || connection.state !== signalR.HubConnectionState.Connected) return;
            button.disabled = true;
            try { await connection.invoke("SendNewMessage", "", text); input.value = ""; }
            catch (_) { setState("ارسال ناموفق؛ اتصال را بررسی کنید", false); }
            finally { button.disabled = false; input.focus(); }
        });
        input.addEventListener("keydown", function (event) { if (event.key === "Enter" && !event.shiftKey) { event.preventDefault(); form.requestSubmit(); } });
        connection.on("getNewMessage", addMessage);
        connection.on("loadChatHistory", function (items) {
            messages.textContent = "";
            (items || []).forEach(function (item) { addMessage(item.sender, item.message, item.time); });
        });
        connection.onreconnecting(function () { setState("در حال اتصال مجدد...", false); });
        connection.onreconnected(function () { setState("آنلاین", true); });
        connection.onclose(function () { setState("قطع ارتباط", false); start(); });
        start();
    });
})();

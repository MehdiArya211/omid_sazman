(function () {
    "use strict";
    // نسخه SignalR موجود در پروژه withAutomaticReconnect ندارد؛ اتصال مجدد دستی انجام می‌شود.
    var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
    var reconnectTimer = null;
    var form, input, messages, button, status, emptyState, replyPreview, currentUser;
    var selectedReply = null;
    function formatPersianDate(value) {
        var date = new Date(value); if (Number.isNaN(date.getTime())) return value || "";
        return new Intl.DateTimeFormat("fa-IR-u-ca-persian", {year:"numeric",month:"2-digit",day:"2-digit",hour:"2-digit",minute:"2-digit"}).format(date);
    }
    function normalizeMessage(value, message, time) {
        if (value && typeof value === "object") return value;
        return { sender: value, message: message, time: time };
    }
    function clearReply() {
        selectedReply = null;
        if (replyPreview) replyPreview.hidden = true;
    }
    function selectReply(item) {
        if (!item || !item.id) return;
        selectedReply = item;
        replyPreview.querySelector("[data-reply-sender]").textContent = item.sender || "پیام";
        replyPreview.querySelector("[data-reply-message]").textContent = item.message || "";
        replyPreview.hidden = false;
        input.focus();
    }
    function addMessage(value, message, time) {
        var data = normalizeMessage(value, message, time);
        var item = document.createElement("li"), meta = document.createElement("div"), body = document.createElement("div");
        meta.className = "chat-message-meta"; body.className = "chat-message-body";
        item.classList.toggle("is-current-user", currentUser && (data.sender || "").trim().toLowerCase() === currentUser);
        item.dataset.messageId = data.id || ""; item._chatMessage = data;
        if (data.replyToMessage) {
            var quote = document.createElement("button"); quote.type = "button"; quote.className = "chat-message-reply";
            quote.dataset.replyTarget = data.replyToMessageId || "";
            quote.innerHTML = "<strong></strong><span></span>";
            quote.querySelector("strong").textContent = data.replyToSender || "پیام";
            quote.querySelector("span").textContent = data.replyToMessage;
            item.appendChild(quote);
        }
        meta.textContent = (data.sender || "کاربر") + " · " + formatPersianDate(data.time); body.textContent = data.message || "";
        var replyButton = document.createElement("button"); replyButton.type = "button"; replyButton.className = "chat-reply-button"; replyButton.title = "پاسخ به این پیام";
        replyButton.innerHTML = '<i class="ti-back-left"></i><span>پاسخ</span>';
        item.appendChild(meta); item.appendChild(body); item.appendChild(replyButton); messages.appendChild(item);
        if (emptyState) emptyState.classList.add("is-hidden");
        var panel = document.getElementById("MessagePanel"); if (panel) panel.scrollTop = panel.scrollHeight;
    }
    function setState(text, connected) {
        if (status) { status.textContent = text; status.classList.toggle("is-online", connected); }
        if (button) button.disabled = !connected;
        if (input) input.disabled = !connected;
    }
    function resizeComposer() {
        input.style.height = "auto";
        input.style.height = Math.min(input.scrollHeight, 90) + "px";
    }
    async function start() {
        if (connection.state !== signalR.HubConnectionState.Disconnected) return;
        try {
            await connection.start();
            if (reconnectTimer) { window.clearTimeout(reconnectTimer); reconnectTimer = null; }
            setState("آنلاین", true);
        }
        catch (_) {
            setState("در حال اتصال مجدد...", false);
            reconnectTimer = window.setTimeout(start, 5000);
        }
    }
    document.addEventListener("DOMContentLoaded", function () {
        form = document.getElementById("NewMessageForm"); input = document.getElementById("MessageInput");
        messages = document.getElementById("Messages"); status = document.getElementById("chatConnectionStatus");
        emptyState = document.getElementById("clientChatEmptyState"); replyPreview = document.getElementById("clientReplyPreview");
        var page = document.querySelector(".client-chat-page"); currentUser = page ? (page.dataset.currentUser || "").trim().toLowerCase() : "";
        if (!form || !input || !messages) return; button = form.querySelector("button[type=submit]");
        form.addEventListener("submit", async function (event) {
            event.preventDefault(); var text = input.value.trim();
            if (!text || connection.state !== signalR.HubConnectionState.Connected) return;
            button.disabled = true;
            try { await connection.invoke("SendNewMessage", "", text, selectedReply ? selectedReply.id : null); input.value = ""; resizeComposer(); clearReply(); }
            catch (_) { setState("ارسال ناموفق؛ اتصال را بررسی کنید", false); }
            finally { button.disabled = connection.state !== signalR.HubConnectionState.Connected; input.focus(); }
        });
        input.addEventListener("keydown", function (event) { if (event.key === "Enter" && !event.shiftKey) { event.preventDefault(); form.requestSubmit(); } });
        input.addEventListener("input", resizeComposer);
        messages.addEventListener("click", function (event) {
            var reply = event.target.closest(".chat-reply-button");
            if (reply) { selectReply(reply.closest("li")._chatMessage); return; }
            var quote = event.target.closest(".chat-message-reply");
            if (quote && quote.dataset.replyTarget) {
                var target = messages.querySelector('[data-message-id="' + quote.dataset.replyTarget + '"]');
                if (target) { target.scrollIntoView({ behavior:"smooth", block:"center" }); target.classList.add("is-highlighted"); window.setTimeout(function () { target.classList.remove("is-highlighted"); }, 1300); }
            }
        });
        if (replyPreview) replyPreview.querySelector("[data-cancel-reply]").addEventListener("click", clearReply);
        connection.on("getNewMessage", addMessage);
        connection.on("loadChatHistory", function (items) {
            messages.textContent = "";
            (items || []).forEach(function (item) { addMessage(item); });
        });
        connection.onclose(function () {
            setState("در حال اتصال مجدد...", false);
            reconnectTimer = window.setTimeout(start, 2000);
        });
        start();
    });
})();

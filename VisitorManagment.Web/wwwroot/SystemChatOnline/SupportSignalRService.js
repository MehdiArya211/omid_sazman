(function () {
    "use strict";
    var activeRoomId = "";
    var support = new signalR.HubConnectionBuilder().withUrl("/supporthub").withAutomaticReconnect().build();
    var chat = new signalR.HubConnectionBuilder().withUrl("/chathub?support=true").withAutomaticReconnect().build();
    var roomList, messages, form, input, sendButton, search, emptyState, currentUser, replyPreview;
    var selectedReply = null;

    function setEmptyState(title, description, icon) {
        if (!emptyState) return;
        emptyState.classList.remove("is-hidden");
        emptyState.querySelector("i").className = icon || "ti-comments";
        emptyState.querySelector("strong").textContent = title;
        emptyState.querySelector("span").textContent = description;
    }

    function hideEmptyState() {
        if (emptyState) emptyState.classList.add("is-hidden");
    }

    function scrollToLatestMessage() {
        var panel = messages.closest(".message-item");
        if (panel) panel.scrollTop = panel.scrollHeight;
    }

    function resizeComposer() {
        input.style.height = "auto";
        input.style.height = Math.min(input.scrollHeight, 90) + "px";
    }

    function formatPersianDate(value) {
        if (!value) return "";
        var date = new Date(value);
        if (Number.isNaN(date.getTime())) return String(value);
        return new Intl.DateTimeFormat("fa-IR-u-ca-persian", { year: "numeric", month: "2-digit", day: "2-digit", hour: "2-digit", minute: "2-digit" }).format(date);
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
        replyPreview.querySelector("[data-reply-sender]").textContent = item.sender || "کاربر";
        replyPreview.querySelector("[data-reply-message]").textContent = item.message || "";
        replyPreview.hidden = false;
        input.focus();
    }

    function appendMessage(value, message, time, forceSupport) {
        var item = normalizeMessage(value, message, time);
        var li = document.createElement("li"), meta = document.createElement("div"), body = document.createElement("div");
        meta.className = "chat-message-meta"; body.className = "chat-message-body";
        var sender = item.sender || "کاربر";
        var normalizedSender = sender.trim().toLowerCase();
        var isSupport = forceSupport === true || normalizedSender.indexOf("پشتیبان") !== -1 || (currentUser && normalizedSender === currentUser);
        li.classList.toggle("is-support", isSupport);
        li.dataset.messageId = item.id || "";
        li._chatMessage = item;
        meta.textContent = sender + " · " + formatPersianDate(item.time);
        body.textContent = item.message || "";
        if (item.replyToMessage) {
            var quote = document.createElement("button");
            quote.type = "button"; quote.className = "chat-message-reply";
            quote.dataset.replyTarget = item.replyToMessageId || "";
            quote.innerHTML = "<strong></strong><span></span>";
            quote.querySelector("strong").textContent = item.replyToSender || "پیام";
            quote.querySelector("span").textContent = item.replyToMessage;
            li.appendChild(quote);
        }
        var replyButton = document.createElement("button");
        replyButton.type = "button"; replyButton.className = "chat-reply-button"; replyButton.title = "پاسخ به این پیام";
        replyButton.innerHTML = '<i class="ti-back-left"></i><span>پاسخ</span>';
        li.appendChild(meta); li.appendChild(body); li.appendChild(replyButton); messages.appendChild(li); hideEmptyState(); scrollToLatestMessage();
    }
    function loadRooms(rooms) {
        roomList.textContent = "";
        if (!rooms || !rooms.length) {
            var noRoom = document.createElement("div");
            noRoom.className = "chat-room-empty";
            noRoom.textContent = "هنوز گفت‌وگویی ثبت نشده است.";
            roomList.appendChild(noRoom);
            return;
        }
        (rooms || []).forEach(function (room) {
            var link = document.createElement("button");
            link.type = "button"; link.className = "list-group-item list-group-item-action chat-room-item";
            link.dataset.id = room.id; link.dataset.title = room.title || "گفت‌گوی بدون عنوان";
            var title = document.createElement("span"), preview = document.createElement("span"), time = document.createElement("time"), count = document.createElement("span");
            title.className = "chat-room-item__title"; preview.className = "chat-room-item__message"; time.className = "chat-room-item__time"; count.className = "chat-room-item__count";
            title.textContent = link.dataset.title; preview.textContent = room.lastMessage || "بدون پیام"; time.textContent = formatPersianDate(room.lastMessageTime); count.textContent = room.messageCount || 0;
            link.appendChild(title); link.appendChild(preview); link.appendChild(time); link.appendChild(count); roomList.appendChild(link);
        });
    }
    async function switchRoom(button) {
        var id = button.dataset.id; if (!id || id === activeRoomId) return;
        if (activeRoomId && chat.state === signalR.HubConnectionState.Connected) await chat.invoke("LeaveRoom", activeRoomId);
        activeRoomId = id; messages.textContent = "";
        setEmptyState("در حال دریافت پیام‌ها...", "لطفاً چند لحظه صبر کنید.", "ti-reload");
        roomList.querySelectorAll(".chat-room-item").forEach(function (x) { x.classList.toggle("active", x === button); });
        document.getElementById("activeChatTitle").textContent = button.dataset.title;
        document.getElementById("activeChatMeta").textContent = "تاریخ و ساعت پیام‌ها به تقویم شمسی";
        var content = document.querySelector(".support-chat-page .chat-content"); if (content) content.classList.add("mobile-open");
        input.disabled = false; input.removeAttribute("disabled"); sendButton.disabled = false; clearReply();
        try {
            await chat.invoke("JoinRoom", id);
            await support.invoke("LoadMessage", id);
            window.setTimeout(function () {
                if (!messages.children.length && activeRoomId === id) setEmptyState("پیامی وجود ندارد", "این گفت‌وگو هنوز پیامی ندارد.", "ti-comment-alt");
            }, 300);
        } catch (error) {
            activeRoomId = ""; input.disabled = true; sendButton.disabled = true;
            setEmptyState("دریافت پیام‌ها ناموفق بود", "ارتباط با سرور برقرار نشد؛ دوباره تلاش کنید.", "ti-alert");
            throw error;
        }
    }
    async function start(connection) {
        if (connection.state === signalR.HubConnectionState.Disconnected) await connection.start();
    }
    document.addEventListener("DOMContentLoaded", async function () {
        window.localStorage.removeItem("omid-support-unread-count");
        roomList = document.getElementById("roomList"); messages = document.getElementById("chatMessage");
        form = document.getElementById("answerForm"); input = document.getElementById("answerText");
        search = document.getElementById("chatRoomSearch"); emptyState = document.getElementById("chatEmptyState"); replyPreview = document.getElementById("adminReplyPreview");
        var page = document.querySelector(".support-chat-page"); currentUser = page ? (page.dataset.currentUser || "").trim().toLowerCase() : "";
        if (!roomList || !messages || !form || !input) return; sendButton = form.querySelector("button[type=submit]");
        support.on("GetRooms", loadRooms);
        support.on("supportRoomOpened", function () { support.invoke("RefreshRooms").catch(function () {}); });
        support.on("getNewMessage", function (items) { (items || []).forEach(function (m) { appendMessage(m); }); });
        support.on("newSupportMessage", function (roomId, value, message, time) {
            var incoming = normalizeMessage(value, message, time);
            var room = roomList.querySelector('[data-id="' + roomId + '"]');
            if (room) {
                room.querySelector(".chat-room-item__message").textContent = incoming.message || "";
                room.querySelector(".chat-room-item__time").textContent = formatPersianDate(incoming.time);
                var count = room.querySelector(".chat-room-item__count"); count.textContent = (parseInt(count.textContent, 10) || 0) + 1;
                roomList.prepend(room);
            } else { support.invoke("RefreshRooms").catch(function () {}); }
            if (window.Swal && document.hidden) Swal.fire({toast:true,position:"top-start",icon:"info",title:"پیام جدید از " + (incoming.sender || "کاربر"),text:incoming.message || "",showConfirmButton:false,timer:5000});
        });
        chat.on("getNewMessage", appendMessage);
        support.on("messageSent", function (item) { appendMessage(item, null, null, true); });
        roomList.addEventListener("click", function (event) { var button = event.target.closest(".chat-room-item"); if (button) switchRoom(button).catch(console.error); });
        form.addEventListener("submit", async function (event) {
            event.preventDefault(); var text = input.value.trim(); if (!text || !activeRoomId) return;
            sendButton.disabled = true;
            try { await support.invoke("SendMessage", activeRoomId, text, selectedReply ? selectedReply.id : null); input.value = ""; resizeComposer(); clearReply(); }
            catch (error) { if (window.Swal) Swal.fire({icon:"error",title:"ارسال انجام نشد",text:"ارتباط با سرور را بررسی و دوباره تلاش کنید."}); else window.alert("ارسال پیام انجام نشد. دوباره تلاش کنید."); }
            finally { sendButton.disabled = support.state !== signalR.HubConnectionState.Connected; input.focus(); }
        });
        if (search) search.addEventListener("input", function () { var q = search.value.trim().toLowerCase(); roomList.querySelectorAll(".chat-room-item").forEach(function (x) { x.hidden = q && !x.textContent.toLowerCase().includes(q); }); });
        input.addEventListener("keydown", function (event) { if (event.key === "Enter" && !event.shiftKey) { event.preventDefault(); form.requestSubmit(); } });
        input.addEventListener("input", resizeComposer);
        messages.addEventListener("click", function (event) {
            var button = event.target.closest(".chat-reply-button");
            if (button) { selectReply(button.closest("li")._chatMessage); return; }
            var quote = event.target.closest(".chat-message-reply");
            if (quote && quote.dataset.replyTarget) {
                var target = messages.querySelector('[data-message-id="' + quote.dataset.replyTarget + '"]');
                if (target) { target.scrollIntoView({ behavior:"smooth", block:"center" }); target.classList.add("is-highlighted"); window.setTimeout(function () { target.classList.remove("is-highlighted"); }, 1300); }
            }
        });
        if (replyPreview) replyPreview.querySelector("[data-cancel-reply]").addEventListener("click", clearReply);
        var close = document.querySelector(".mobile-chat-close-btn");
        if (close) close.addEventListener("click", function (event) { event.preventDefault(); var content = document.querySelector(".support-chat-page .chat-content"); if (content) content.classList.remove("mobile-open"); });
        support.onreconnecting(function () { var state = document.getElementById("supportConnectionStatus"); if (state) { state.textContent="در حال اتصال مجدد..."; state.classList.remove("is-online"); } input.disabled = true; sendButton.disabled = true; });
        support.onreconnected(function () { var state = document.getElementById("supportConnectionStatus"); if (state) { state.textContent="آنلاین"; state.classList.add("is-online"); } support.invoke("RefreshRooms").catch(function () {}); if (activeRoomId) { input.disabled=false; sendButton.disabled=false; } });
        chat.onreconnected(function () { if (activeRoomId) chat.invoke("JoinRoom", activeRoomId).catch(function () {}); });
        try { await Promise.all([start(support), start(chat)]); var state = document.getElementById("supportConnectionStatus"); if (state) { state.textContent="آنلاین"; state.classList.add("is-online"); } }
        catch (error) { var state = document.getElementById("supportConnectionStatus"); if (state) state.textContent="ارتباط برقرار نشد"; setEmptyState("اتصال به چت برقرار نشد", "صفحه را تازه‌سازی کنید یا وضعیت شبکه را بررسی کنید.", "ti-alert"); console.error("Chat connection failed", error); }
    });
})();

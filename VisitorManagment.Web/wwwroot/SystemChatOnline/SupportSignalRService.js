(function () {
    "use strict";
    var activeRoomId = "";
    var support = new signalR.HubConnectionBuilder().withUrl("/supporthub").withAutomaticReconnect().build();
    var chat = new signalR.HubConnectionBuilder().withUrl("/chathub?support=true").withAutomaticReconnect().build();
    var roomList, messages, form, input, sendButton, search;

    function formatPersianDate(value) {
        if (!value) return "";
        var date = new Date(value);
        if (Number.isNaN(date.getTime())) return String(value);
        return new Intl.DateTimeFormat("fa-IR-u-ca-persian", { year: "numeric", month: "2-digit", day: "2-digit", hour: "2-digit", minute: "2-digit" }).format(date);
    }

    function appendMessage(sender, message, time) {
        var li = document.createElement("li"), meta = document.createElement("div"), body = document.createElement("div");
        meta.className = "chat-message-meta"; body.className = "chat-message-body";
        var isSupport = (sender || "").indexOf("پشتیبان") !== -1;
        li.classList.toggle("is-support", isSupport);
        meta.textContent = (sender || "کاربر") + " · " + formatPersianDate(time); body.textContent = message || "";
        li.appendChild(meta); li.appendChild(body); messages.appendChild(li); messages.scrollTop = messages.scrollHeight;
        var empty = document.getElementById("chatEmptyState"); if (empty) empty.classList.add("is-hidden");
    }
    function loadRooms(rooms) {
        roomList.textContent = "";
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
        roomList.querySelectorAll(".chat-room-item").forEach(function (x) { x.classList.toggle("active", x === button); });
        document.getElementById("activeChatTitle").textContent = button.dataset.title;
        document.getElementById("activeChatMeta").textContent = "تاریخ و ساعت پیام‌ها به تقویم شمسی";
        var content = document.querySelector(".support-chat-page .chat-content"); if (content) content.classList.add("mobile-open");
        input.disabled = false; sendButton.disabled = false;
        await chat.invoke("JoinRoom", id); await support.invoke("LoadMessage", id);
    }
    async function start(connection) {
        if (connection.state === signalR.HubConnectionState.Disconnected) await connection.start();
    }
    document.addEventListener("DOMContentLoaded", async function () {
        roomList = document.getElementById("roomList"); messages = document.getElementById("chatMessage");
        form = document.getElementById("answerForm"); input = document.getElementById("answerText");
        search = document.getElementById("chatRoomSearch");
        if (!roomList || !messages || !form || !input) return; sendButton = form.querySelector("button[type=submit]");
        support.on("GetRooms", loadRooms);
        support.on("getNewMessage", function (items) { (items || []).forEach(function (m) { appendMessage(m.sender, m.message, m.time); }); });
        support.on("newSupportMessage", function (roomId, sender, message, time) {
            var room = roomList.querySelector('[data-id="' + roomId + '"]');
            if (room) {
                room.querySelector(".chat-room-item__message").textContent = message;
                room.querySelector(".chat-room-item__time").textContent = formatPersianDate(time);
                roomList.prepend(room);
            } else { support.invoke("RefreshRooms").catch(function () {}); }
            if (window.Swal && document.hidden) Swal.fire({toast:true,position:"top-start",icon:"info",title:"پیام جدید از " + sender,text:message,showConfirmButton:false,timer:5000});
        });
        chat.on("getNewMessage", appendMessage);
        roomList.addEventListener("click", function (event) { var button = event.target.closest(".chat-room-item"); if (button) switchRoom(button).catch(console.error); });
        form.addEventListener("submit", async function (event) {
            event.preventDefault(); var text = input.value.trim(); if (!text || !activeRoomId) return;
            sendButton.disabled = true;
            try { await support.invoke("SendMessage", activeRoomId, text); appendMessage("پشتیبان", text, new Date()); input.value = ""; }
            finally { sendButton.disabled = false; input.focus(); }
        });
        if (search) search.addEventListener("input", function () { var q = search.value.trim().toLowerCase(); roomList.querySelectorAll(".chat-room-item").forEach(function (x) { x.hidden = q && !x.textContent.toLowerCase().includes(q); }); });
        input.addEventListener("keydown", function (event) { if (event.key === "Enter" && !event.shiftKey) { event.preventDefault(); form.requestSubmit(); } });
        var close = document.querySelector(".mobile-chat-close-btn a");
        if (close) close.addEventListener("click", function (event) { event.preventDefault(); var content = document.querySelector(".support-chat-page .chat-content"); if (content) content.classList.remove("mobile-open"); });
        try { await Promise.all([start(support), start(chat)]); var state = document.getElementById("supportConnectionStatus"); if (state) { state.textContent="آنلاین"; state.classList.add("is-online"); } } catch (error) { console.error("Chat connection failed", error); }
    });
})();

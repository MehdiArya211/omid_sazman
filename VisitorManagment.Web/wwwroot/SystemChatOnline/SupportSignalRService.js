(function () {
    "use strict";
    var activeRoomId = "";
    var support = new signalR.HubConnectionBuilder().withUrl("/supporthub").withAutomaticReconnect().build();
    var chat = new signalR.HubConnectionBuilder().withUrl("/chathub?support=true").withAutomaticReconnect().build();
    var roomList, messages, form, input, sendButton, search;

    function appendMessage(sender, message, time) {
        var li = document.createElement("li"), meta = document.createElement("div"), body = document.createElement("div");
        meta.className = "chat-message-meta"; body.className = "chat-message-body";
        meta.textContent = (sender || "کاربر") + " · " + (time || ""); body.textContent = message || "";
        li.appendChild(meta); li.appendChild(body); messages.appendChild(li); messages.scrollTop = messages.scrollHeight;
    }
    function loadRooms(rooms) {
        roomList.textContent = "";
        (rooms || []).forEach(function (room) {
            var link = document.createElement("button");
            link.type = "button"; link.className = "list-group-item list-group-item-action chat-room-item";
            link.dataset.id = room.id; link.textContent = room.title || "گفت‌گوی بدون عنوان"; roomList.appendChild(link);
        });
    }
    async function switchRoom(button) {
        var id = button.dataset.id; if (!id || id === activeRoomId) return;
        if (activeRoomId && chat.state === signalR.HubConnectionState.Connected) await chat.invoke("LeaveRoom", activeRoomId);
        activeRoomId = id; messages.textContent = "";
        roomList.querySelectorAll(".chat-room-item").forEach(function (x) { x.classList.toggle("active", x === button); });
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
        support.on("getNewMessage", function (items) { (items || []).forEach(function (m) { appendMessage(m.sender, m.message, m.time && new Date(m.time).toLocaleTimeString("fa-IR", {hour:"2-digit", minute:"2-digit"})); }); });
        chat.on("getNewMessage", appendMessage);
        roomList.addEventListener("click", function (event) { var button = event.target.closest(".chat-room-item"); if (button) switchRoom(button).catch(console.error); });
        form.addEventListener("submit", async function (event) {
            event.preventDefault(); var text = input.value.trim(); if (!text || !activeRoomId) return;
            sendButton.disabled = true;
            try { await support.invoke("SendMessage", activeRoomId, text); appendMessage("پشتیبان", text, new Date().toLocaleTimeString("fa-IR", {hour:"2-digit", minute:"2-digit"})); input.value = ""; }
            finally { sendButton.disabled = false; input.focus(); }
        });
        if (search) search.addEventListener("input", function () { var q = search.value.trim().toLowerCase(); roomList.querySelectorAll(".chat-room-item").forEach(function (x) { x.hidden = q && !x.textContent.toLowerCase().includes(q); }); });
        try { await Promise.all([start(support), start(chat)]); } catch (error) { console.error("Chat connection failed", error); }
    });
})();

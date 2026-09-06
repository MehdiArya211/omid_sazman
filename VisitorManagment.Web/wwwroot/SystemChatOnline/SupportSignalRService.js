(function () {
    "use strict";
    var activeRoomId = "";
    // نسخه SignalR موجود در پروژه reconnect خودکار داخلی ندارد.
    var support = new signalR.HubConnectionBuilder().withUrl("/supporthub").build();
    var chat = new signalR.HubConnectionBuilder().withUrl("/chathub?support=true").build();
    var reconnectTimers = [];
    var roomList, messages, form, input, sendButton, search, emptyState, currentUser, replyPreview;
    var messageSearch, fileInput, attachButton, typingIndicator, typingTimer = null, lastDateKey = "";
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
        li.classList.toggle("is-deleted", item.isDeleted === true);
        var messageDate = new Date(item.time), dateKey = Number.isNaN(messageDate.getTime()) ? "" : messageDate.toISOString().slice(0, 10);
        if (dateKey && dateKey !== lastDateKey) { var divider=document.createElement("li");divider.className="chat-date-divider";divider.textContent=new Intl.DateTimeFormat("fa-IR-u-ca-persian",{year:"numeric",month:"long",day:"numeric"}).format(messageDate);messages.appendChild(divider);lastDateKey=dateKey; }
        li.dataset.messageId = item.id || "";
        li._chatMessage = item;
        meta.textContent = sender + " · " + formatPersianDate(item.time) + (item.editedAt ? " · ویرایش‌شده" : "");
        body.textContent = item.isDeleted ? "این پیام حذف شده است" : (item.message || "");
        if (item.replyToMessage) {
            var quote = document.createElement("button");
            quote.type = "button"; quote.className = "chat-message-reply";
            quote.dataset.replyTarget = item.replyToMessageId || "";
            quote.innerHTML = "<strong></strong><span></span>";
            quote.querySelector("strong").textContent = item.replyToSender || "پیام";
            quote.querySelector("span").textContent = item.replyToMessage;
            li.appendChild(quote);
        }
        li.appendChild(meta); li.appendChild(body);
        if (item.attachmentUrl && !item.isDeleted) { var attachment=document.createElement("a");attachment.className="chat-attachment";attachment.href=item.attachmentUrl;attachment.target="_blank";attachment.rel="noopener";if((item.attachmentContentType||"").indexOf("image/")===0){var img=document.createElement("img");img.src=item.attachmentUrl;img.alt=item.attachmentName||"تصویر";attachment.appendChild(img);}var label=document.createElement("span");label.innerHTML='<i class="ti-file"></i> ';label.appendChild(document.createTextNode(item.attachmentName||"دریافت فایل"));attachment.appendChild(label);li.appendChild(attachment); }
        if (!item.isDeleted) { var actions=document.createElement("div");actions.className="chat-message-actions";actions.innerHTML='<button type="button" data-action="reply" title="پاسخ"><i class="ti-back-left"></i></button><button type="button" data-action="copy" title="کپی"><i class="ti-files"></i></button>'+(isSupport?'<button type="button" data-action="edit" title="ویرایش"><i class="ti-pencil"></i></button><button type="button" data-action="delete" title="حذف"><i class="ti-trash"></i></button>':'');li.appendChild(actions); }
        var delivery=document.createElement("small");delivery.className="chat-delivery";delivery.textContent=isSupport?(item.isRead?"✓✓ خوانده شد":(item.isDelivered?"✓✓ تحویل شد":"✓ ارسال شد")):"";li.appendChild(delivery);messages.appendChild(li);hideEmptyState();scrollToLatestMessage();
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
            title.textContent = link.dataset.title; preview.textContent = room.lastMessage || "بدون پیام"; time.textContent = formatPersianDate(room.lastMessageTime); count.textContent = room.unreadCount || 0; count.hidden = !room.unreadCount;
            link.appendChild(title); link.appendChild(preview); link.appendChild(time); link.appendChild(count); roomList.appendChild(link);
        });
    }
    async function switchRoom(button) {
        var id = button.dataset.id; if (!id) return;
        var previousRoomId = activeRoomId;
        if (previousRoomId && previousRoomId !== id && chat.state === signalR.HubConnectionState.Connected) {
            chat.invoke("LeaveRoom", previousRoomId).catch(function (error) { console.warn("LeaveRoom failed", error); });
        }
        activeRoomId = id; messages.textContent = ""; lastDateKey = "";
        setEmptyState("در حال دریافت پیام‌ها...", "لطفاً چند لحظه صبر کنید.", "ti-reload");
        roomList.querySelectorAll(".chat-room-item").forEach(function (x) { x.classList.toggle("active", x === button); });
        var unreadBadge = button.querySelector(".chat-room-item__count"); if (unreadBadge) { unreadBadge.textContent = "0"; unreadBadge.hidden = true; }
        document.getElementById("activeChatTitle").textContent = button.dataset.title;
        document.getElementById("activeChatMeta").textContent = "تاریخ و ساعت پیام‌ها به تقویم شمسی";
        var content = document.querySelector(".support-chat-page .chat-content"); if (content) content.classList.add("mobile-open");
        input.disabled = false; input.removeAttribute("disabled"); sendButton.disabled = false; clearReply();
        try {
            // تاریخچه فقط به اتصال پشتیبانی وابسته است؛ خرابی اتصال دوم نباید مانع نمایش پیام‌ها شود.
            await support.invoke("LoadMessage", id);
            if (chat.state === signalR.HubConnectionState.Connected) {
                chat.invoke("JoinRoom", id).catch(function (error) {
                    console.warn("JoinRoom failed", error);
                    reconnect(chat, function () { return chat.invoke("JoinRoom", activeRoomId); });
                });
            } else {
                reconnect(chat, function () { return chat.invoke("JoinRoom", activeRoomId); });
            }
            window.setTimeout(function () {
                if (!messages.children.length && activeRoomId === id) setEmptyState("پیامی وجود ندارد", "این گفت‌وگو هنوز پیامی ندارد.", "ti-comment-alt");
            }, 300);
        } catch (error) {
            input.disabled = true; sendButton.disabled = true;
            setEmptyState("دریافت پیام‌ها ناموفق بود", "ارتباط با سرور برقرار نشد؛ دوباره تلاش کنید.", "ti-alert");
            throw error;
        }
    }
    function syncChatViewport() {
        var page = document.querySelector(".support-chat-page");
        if (!page) return;
        var top = Math.max(0, page.getBoundingClientRect().top);
        page.style.setProperty("--chat-viewport-height", Math.max(480, window.innerHeight - top - 10) + "px");
    }
    async function start(connection) {
        if (connection.state === signalR.HubConnectionState.Disconnected) await connection.start();
    }
    function setConnectionState(text, connected) {
        var state = document.getElementById("supportConnectionStatus");
        if (state) { state.textContent = text; state.classList.toggle("is-online", connected); }
        if (input) input.disabled = !connected || !activeRoomId;
        if (sendButton) sendButton.disabled = !connected || !activeRoomId;
    }
    function reconnect(connection, afterConnected) {
        var index = connection === support ? 0 : 1;
        if (reconnectTimers[index]) window.clearTimeout(reconnectTimers[index]);
        reconnectTimers[index] = window.setTimeout(async function tryReconnect() {
            try {
                await start(connection);
                reconnectTimers[index] = null;
                if (afterConnected) await afterConnected();
                if (support.state === signalR.HubConnectionState.Connected && chat.state === signalR.HubConnectionState.Connected) {
                    setConnectionState("آنلاین", true);
                }
            } catch (_) {
                reconnectTimers[index] = window.setTimeout(tryReconnect, 5000);
            }
        }, 2000);
    }
    document.addEventListener("DOMContentLoaded", async function () {
        document.documentElement.classList.add("chat-page-lock");
        document.body.classList.add("chat-page-lock");
        syncChatViewport();
        window.addEventListener("resize", syncChatViewport);
        window.localStorage.removeItem("omid-support-unread-count");
        roomList = document.getElementById("roomList"); messages = document.getElementById("chatMessage");
        form = document.getElementById("answerForm"); input = document.getElementById("answerText");
        search = document.getElementById("chatRoomSearch"); emptyState = document.getElementById("chatEmptyState"); replyPreview = document.getElementById("adminReplyPreview");
        messageSearch=document.getElementById("adminMessageSearch");fileInput=document.getElementById("adminAttachmentInput");attachButton=document.getElementById("adminAttachButton");typingIndicator=document.getElementById("adminTypingIndicator");
        var page = document.querySelector(".support-chat-page"); currentUser = page ? (page.dataset.currentUser || "").trim().toLowerCase() : "";
        if (!roomList || !messages || !form || !input) return; sendButton = document.getElementById("answerSendButton");
        if (!sendButton) return;
        support.on("GetRooms", loadRooms);
        support.on("supportRoomOpened", function () { support.invoke("RefreshRooms").catch(function () {}); });
        support.on("getNewMessage", function (items) {
            messages.textContent = "";
            (Array.isArray(items) ? items : []).forEach(function (m) { appendMessage(m); });
            if (!items || !items.length) setEmptyState("پیامی وجود ندارد", "این گفت‌وگو هنوز پیامی ندارد.", "ti-comment-alt");
        });
        support.on("newSupportMessage", function (roomId, value, message, time) {
            var incoming = normalizeMessage(value, message, time);
            support.invoke("MarkAsDelivered", roomId).catch(function () {});
            var room = roomList.querySelector('[data-id="' + roomId + '"]');
            if (room) {
                room.querySelector(".chat-room-item__message").textContent = incoming.message || "";
                room.querySelector(".chat-room-item__time").textContent = formatPersianDate(incoming.time);
                var count = room.querySelector(".chat-room-item__count"); count.textContent = (parseInt(count.textContent, 10) || 0) + 1;
                roomList.prepend(room);
            } else { support.invoke("RefreshRooms").catch(function () {}); }
            if (String(roomId) === String(activeRoomId) && !messages.querySelector('[data-message-id="' + (incoming.id || '') + '"]')) {
                appendMessage(incoming);
                support.invoke("LoadMessage", activeRoomId).catch(function () {});
            }
            if (window.Swal && document.hidden) Swal.fire({toast:true,position:"top-start",icon:"info",title:"پیام جدید از " + (incoming.sender || "کاربر"),text:incoming.message || "",showConfirmButton:false,timer:5000});
        });
        // پیام‌های زنده از SupportHub دریافت می‌شوند تا پاسخ پشتیبان دوبار نمایش داده نشود.
        support.on("messageSent", function (item) { appendMessage(item, null, null, true); });
        support.on("messageUpdated", function (roomOrItem, item) { var roomId=item?roomOrItem:null,data=item||roomOrItem;if(!roomId||String(roomId)===String(activeRoomId))updateMessage(data); });
        support.on("messageDeleted", function (roomOrItem, item) { var roomId=item?roomOrItem:null,data=item||roomOrItem;if(!roomId||String(roomId)===String(activeRoomId))updateMessage(data); });
        support.on("messagesRead", function (roomId, ids) { if(String(roomId)===String(activeRoomId)) markRead(ids); });
        support.on("messagesDelivered", function (roomId, ids) { if(String(roomId)===String(activeRoomId)) markDelivered(ids); });
        support.on("typingChanged", function (roomId, _, typing) { if(typingIndicator&&String(roomId)===String(activeRoomId))typingIndicator.hidden=!typing; });
        roomList.addEventListener("click", function (event) { var button = event.target.closest(".chat-room-item"); if (button) switchRoom(button).catch(console.error); });
        async function sendMessage(event) {
            if (event) event.preventDefault();
            var text = input.value.trim(); if (!text || !activeRoomId || support.state !== signalR.HubConnectionState.Connected) return;
            sendButton.disabled = true;
            try { await support.invoke("SendMessage", activeRoomId, text, selectedReply ? selectedReply.id : null); input.value = ""; resizeComposer(); clearReply(); }
            catch (error) { if (window.Swal) Swal.fire({icon:"error",title:"ارسال انجام نشد",text:"ارتباط با سرور را بررسی و دوباره تلاش کنید."}); else window.alert("ارسال پیام انجام نشد. دوباره تلاش کنید."); }
            finally { sendButton.disabled = support.state !== signalR.HubConnectionState.Connected; input.focus(); }
        }
        // حتی در صورت خطای SignalR، submit معمولی نباید صفحه را Refresh کند.
        form.addEventListener("submit", function (event) { event.preventDefault(); sendMessage(event); });
        sendButton.addEventListener("click", sendMessage);
        if (search) search.addEventListener("input", function () { var q = search.value.trim().toLowerCase(); roomList.querySelectorAll(".chat-room-item").forEach(function (x) { x.hidden = q && !x.textContent.toLowerCase().includes(q); }); });
        input.addEventListener("keydown", function (event) { if (event.key === "Enter" && !event.shiftKey) { event.preventDefault(); sendMessage(event); } });
        input.addEventListener("input", resizeComposer);
        messages.addEventListener("click", function (event) {
            var action=event.target.closest("[data-action]");
            if(action){var data=action.closest("li")._chatMessage,kind=action.dataset.action;if(kind==="reply")selectReply(data);if(kind==="copy")navigator.clipboard.writeText(data.message||"");if(kind==="edit"){var text=window.prompt("ویرایش پیام",data.message||"");if(text!==null&&text.trim())support.invoke("EditMessage",activeRoomId,data.id,text.trim()).catch(showActionError);}if(kind==="delete"&&window.confirm("این پیام حذف شود؟"))support.invoke("DeleteMessage",activeRoomId,data.id).catch(showActionError);return;}
            var quote = event.target.closest(".chat-message-reply");
            if (quote && quote.dataset.replyTarget) {
                var target = messages.querySelector('[data-message-id="' + quote.dataset.replyTarget + '"]');
                if (target) { target.scrollIntoView({ behavior:"smooth", block:"center" }); target.classList.add("is-highlighted"); window.setTimeout(function () { target.classList.remove("is-highlighted"); }, 1300); }
            }
        });
        if(messageSearch)messageSearch.addEventListener("input",function(){var q=messageSearch.value.trim().toLowerCase();messages.querySelectorAll("li:not(.chat-date-divider)").forEach(function(x){x.hidden=!!q&&!x.textContent.toLowerCase().includes(q);});});
        if(attachButton&&fileInput){attachButton.addEventListener("click",function(){fileInput.click();});fileInput.addEventListener("change",function(){sendFile(fileInput.files[0]);});}
        input.addEventListener("input",function(){if(!activeRoomId)return;support.invoke("Typing",activeRoomId,true).catch(function(){});window.clearTimeout(typingTimer);typingTimer=window.setTimeout(function(){support.invoke("Typing",activeRoomId,false).catch(function(){});},1200);});
        if (replyPreview) replyPreview.querySelector("[data-cancel-reply]").addEventListener("click", clearReply);
        var close = document.querySelector(".mobile-chat-close-btn");
        if (close) close.addEventListener("click", function (event) { event.preventDefault(); var content = document.querySelector(".support-chat-page .chat-content"); if (content) content.classList.remove("mobile-open"); });
        support.onclose(function () {
            setConnectionState("در حال اتصال مجدد...", false);
            reconnect(support, function () { return support.invoke("RefreshRooms"); });
        });
        chat.onclose(function () {
            setConnectionState("در حال اتصال مجدد...", false);
            reconnect(chat, function () { return activeRoomId ? chat.invoke("JoinRoom", activeRoomId) : Promise.resolve(); });
        });
        try { await Promise.all([start(support), start(chat)]); setConnectionState("آنلاین", true); }
        catch (error) {
            setConnectionState("در حال اتصال مجدد...", false);
            setEmptyState("اتصال به چت برقرار نشد", "سامانه به‌صورت خودکار دوباره تلاش می‌کند.", "ti-alert");
            if (support.state === signalR.HubConnectionState.Disconnected) {
                reconnect(support, function () { return support.invoke("RefreshRooms"); });
            }
            if (chat.state === signalR.HubConnectionState.Disconnected) {
                reconnect(chat, function () { return activeRoomId ? chat.invoke("JoinRoom", activeRoomId) : Promise.resolve(); });
            }
            console.error("Chat connection failed", error);
        }
    });
    function updateMessage(item){var li=messages.querySelector('[data-message-id="'+item.id+'"]');if(!li)return;li._chatMessage=item;li.classList.toggle("is-deleted",!!item.isDeleted);var body=li.querySelector(".chat-message-body");if(body)body.textContent=item.isDeleted?"این پیام حذف شده است":(item.message||"");var meta=li.querySelector(".chat-message-meta");if(meta)meta.textContent=(item.sender||"کاربر")+" · "+formatPersianDate(item.time)+(item.editedAt?" · ویرایش‌شده":"");if(item.isDeleted)li.querySelectorAll(".chat-attachment,.chat-message-actions").forEach(function(x){x.remove();});}
    function markRead(ids){(ids||[]).forEach(function(id){var x=messages.querySelector('[data-message-id="'+id+'"] .chat-delivery');if(x)x.textContent="✓✓ خوانده شد";});}
    function markDelivered(ids){(ids||[]).forEach(function(id){var x=messages.querySelector('[data-message-id="'+id+'"] .chat-delivery');if(x&&x.textContent!=="✓✓ خوانده شد")x.textContent="✓✓ تحویل شد";});}
    function showActionError(){window.alert("انجام عملیات پیام ممکن نشد.");}
    function sendFile(file){if(!file||!activeRoomId)return;if(file.size>5*1024*1024){window.alert("حجم فایل باید کمتر از ۵ مگابایت باشد.");fileInput.value="";return;}var reader=new FileReader();reader.onload=function(){var base64=String(reader.result).split(",")[1]||"";support.invoke("SendAttachment",activeRoomId,file.name,file.type,base64,input.value.trim(),selectedReply?selectedReply.id:null).then(function(){input.value="";fileInput.value="";clearReply();}).catch(showActionError);};reader.readAsDataURL(file);}
})();

(function () {
    "use strict";

    function tokenFrom(root) {
        var input = (root || document).querySelector('[name="__RequestVerificationToken"]');
        return input ? input.value : "";
    }

    function updateBadge(count) {
        var badge = document.querySelector("[data-notification-badge]");
        if (!badge) return;
        badge.textContent = String(count);
        badge.hidden = count < 1;
        var link = badge.closest("a");
        if (link) link.setAttribute("aria-label", count + " اعلان خوانده‌نشده");
    }

    function markRead(id, root) {
        var body = new URLSearchParams();
        body.append("notificationId", id);
        body.append("__RequestVerificationToken", tokenFrom(root));
        return fetch("/Notifications?handler=MarkRead", {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8", "X-Requested-With": "XMLHttpRequest" },
            body: body.toString(), credentials: "same-origin"
        }).then(function (response) {
            if (!response.ok) throw new Error("ثبت مشاهده اعلان انجام نشد.");
            return response.json();
        }).then(function (result) {
            if (!result.success) throw new Error(result.message || "ثبت مشاهده اعلان انجام نشد.");
            updateBadge(Number(result.unreadCount || 0));
            return result;
        });
    }

    function closeLoginModal(modal) {
        if (!modal) return;
        modal.hidden = true;
        document.body.classList.remove("notification-modal-open");
    }

    var loginModal = document.querySelector("[data-login-notification]");
    if (loginModal) {
        var isNotificationPage = window.location.pathname.toLowerCase().indexOf("/notifications") === 0;
        if (!isNotificationPage) {
            window.setTimeout(function () {
                loginModal.hidden = false;
                document.body.classList.add("notification-modal-open");
            }, 450);
        }

        loginModal.querySelectorAll("[data-notification-dismiss]").forEach(function (button) {
            button.addEventListener("click", function () { closeLoginModal(loginModal); });
        });

        var modalReadButton = loginModal.querySelector("[data-modal-mark-read]");
        modalReadButton.addEventListener("click", function () {
            modalReadButton.disabled = true;
            markRead(loginModal.dataset.notificationId, loginModal).then(function () {
                closeLoginModal(loginModal);
            }).catch(function (error) {
                modalReadButton.disabled = false;
                window.alert(error.message);
            });
        });

        var viewAllLink = loginModal.querySelector("[data-view-all-notifications]");
        if (viewAllLink) {
            viewAllLink.addEventListener("click", function (event) {
                event.preventDefault();
                if (viewAllLink.getAttribute("aria-disabled") === "true") return;
                viewAllLink.setAttribute("aria-disabled", "true");
                viewAllLink.classList.add("is-loading");
                markRead(loginModal.dataset.notificationId, loginModal).then(function () {
                    closeLoginModal(loginModal);
                    window.location.assign(viewAllLink.href);
                }).catch(function (error) {
                    viewAllLink.setAttribute("aria-disabled", "false");
                    viewAllLink.classList.remove("is-loading");
                    window.alert(error.message);
                });
            });
        }

        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape" && !loginModal.hidden) closeLoginModal(loginModal);
        });
    }

    document.querySelectorAll("[data-mark-notification]").forEach(function (button) {
        button.addEventListener("click", function () {
            button.disabled = true;
            markRead(button.dataset.markNotification, document).then(function () {
                var row = document.querySelector('[data-notification-row="' + button.dataset.markNotification + '"]');
                if (row) {
                    row.classList.remove("is-unread");
                    row.classList.add("is-read");
                    var status = row.querySelector(".user-notification__meta span");
                    if (status) status.textContent = "مشاهده‌شده";
                }
                button.remove();
            }).catch(function (error) {
                button.disabled = false;
                window.alert(error.message);
            });
        });
    });
})();

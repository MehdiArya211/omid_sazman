(function () {
    "use strict";

    function tokenFrom(root) {
        var input = (root || document).querySelector('[name="__RequestVerificationToken"]');
        return input ? input.value : "";
    }

    function updateBadge(count) {
        var badge = document.querySelector("[data-notification-badge]");
        if (!badge) return;
        count = Math.max(0, Number(count) || 0);
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
            updateBadge(result.unreadCount);
            return result;
        });
    }

    function dismissedKey(id) { return "notification-dismissed-" + id; }
    function wasDismissed(id) {
        try { return window.sessionStorage.getItem(dismissedKey(id)) === "1"; } catch (ignore) { return false; }
    }
    function rememberDismissal(id) {
        try { window.sessionStorage.setItem(dismissedKey(id), "1"); } catch (ignore) { }
    }

    function applyListFilters() {
        var list = document.querySelector("[data-notification-list]");
        if (!list) return;
        var queryInput = document.querySelector("[data-notification-search]");
        var query = queryInput ? queryInput.value.trim().toLocaleLowerCase() : "";
        var activeFilter = document.querySelector("[data-notification-filter].is-active");
        var filter = activeFilter ? activeFilter.dataset.notificationFilter : "all";
        var visibleCount = 0;
        list.querySelectorAll("[data-notification-row]").forEach(function (row) {
            var status = row.dataset.notificationStatus || "unread";
            var text = (row.dataset.notificationSearch || "").toLocaleLowerCase();
            var visible = (filter === "all" || status === filter) && (!query || text.indexOf(query) >= 0);
            row.hidden = !visible;
            if (visible) visibleCount++;
        });
        var empty = document.querySelector("[data-notification-filter-empty]");
        if (empty) empty.hidden = visibleCount > 0;
        var clear = document.querySelector("[data-clear-notification-search]");
        if (clear) clear.hidden = !query;
        var unread = list.querySelectorAll('[data-notification-status="unread"]').length;
        var total = list.querySelectorAll("[data-notification-row]").length;
        var totalNode = document.querySelector("[data-count-total]");
        var unreadNode = document.querySelector("[data-count-unread]");
        var readNode = document.querySelector("[data-count-read]");
        if (totalNode) totalNode.textContent = total;
        if (unreadNode) unreadNode.textContent = unread;
        if (readNode) readNode.textContent = total - unread;
    }

    function closeLoginModal(modal, snooze) {
        if (!modal) return;
        if (snooze) rememberDismissal(modal.dataset.notificationId);
        modal.hidden = true;
        document.body.classList.remove("notification-modal-open");
    }

    var loginModal = document.querySelector("[data-login-notification]");
    if (loginModal) {
        var isNotificationPage = window.location.pathname.toLowerCase().indexOf("/notifications") === 0;
        if (!isNotificationPage && !wasDismissed(loginModal.dataset.notificationId)) {
            window.setTimeout(function () {
                loginModal.hidden = false;
                document.body.classList.add("notification-modal-open");
            }, 450);
        }

        loginModal.querySelectorAll("[data-notification-dismiss]").forEach(function (button) {
            button.addEventListener("click", function () { closeLoginModal(loginModal, true); });
        });

        var modalReadButton = loginModal.querySelector("[data-modal-mark-read]");
        if (modalReadButton) modalReadButton.addEventListener("click", function () {
            modalReadButton.disabled = true;
            markRead(loginModal.dataset.notificationId, loginModal).then(function () {
                closeLoginModal(loginModal, true);
            }).catch(function (error) {
                modalReadButton.disabled = false;
                window.alert(error.message);
            });
        });

        var viewAllLink = loginModal.querySelector("[data-view-all-notifications]");
        if (viewAllLink) viewAllLink.addEventListener("click", function (event) {
            event.preventDefault();
            closeLoginModal(loginModal, true);
            window.location.assign(viewAllLink.href);
        });

        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape" && !loginModal.hidden) closeLoginModal(loginModal, true);
        });
    }

    var search = document.querySelector("[data-notification-search]");
    if (search) search.addEventListener("input", applyListFilters);
    var clearSearch = document.querySelector("[data-clear-notification-search]");
    if (clearSearch) clearSearch.addEventListener("click", function () {
        if (search) { search.value = ""; search.focus(); }
        applyListFilters();
    });
    document.querySelectorAll("[data-notification-filter]").forEach(function (button) {
        button.addEventListener("click", function () {
            document.querySelectorAll("[data-notification-filter]").forEach(function (item) {
                var selected = item === button;
                item.classList.toggle("is-active", selected);
                item.setAttribute("aria-pressed", selected ? "true" : "false");
            });
            applyListFilters();
        });
    });

    document.querySelectorAll("[data-mark-notification]").forEach(function (button) {
        button.addEventListener("click", function () {
            button.disabled = true;
            markRead(button.dataset.markNotification, document).then(function () {
                var row = document.querySelector('[data-notification-row="' + button.dataset.markNotification + '"]');
                if (row) {
                    row.dataset.notificationStatus = "read";
                    row.classList.remove("is-unread");
                    row.classList.add("is-read");
                    var status = row.querySelector(".user-notification__state");
                    if (status) status.textContent = "مشاهده‌شده";
                    var icon = row.querySelector(".user-notification__icon i");
                    if (icon) icon.className = "ti-check";
                }
                button.remove();
                applyListFilters();
            }).catch(function (error) {
                button.disabled = false;
                window.alert(error.message);
            });
        });
    });
    applyListFilters();
})();
(function () {
    "use strict";

    var modal = document.getElementById("notificationEditModal");
    var deleteForm = document.getElementById("deleteNotificationAttachmentForm");
    if (!modal) return;

    function formatSize(bytes) {
        if (!bytes) return "۰ کیلوبایت";
        var units = ["بایت", "کیلوبایت", "مگابایت"];
        var index = Math.min(Math.floor(Math.log(bytes) / Math.log(1024)), units.length - 1);
        var value = bytes / Math.pow(1024, index);
        return new Intl.NumberFormat("fa-IR", { maximumFractionDigits: 1 }).format(value) + " " + units[index];
    }

    function fileExtension(name) {
        var parts = (name || "").split(".");
        return parts.length > 1 ? parts.pop().toUpperCase() : "FILE";
    }

    function clearPreview(container) {
        container.querySelectorAll("img[data-object-url]").forEach(function (image) {
            window.URL.revokeObjectURL(image.dataset.objectUrl);
        });
        container.innerHTML = "";
    }

    function renderSelectedFiles(input) {
        var section = input.closest(".notification-upload-section");
        var container = section ? section.querySelector("[data-file-preview]") : null;
        var zone = input.closest(".notification-upload-zone");
        if (!container) return;
        clearPreview(container);
        var files = Array.prototype.slice.call(input.files || []);
        if (zone) zone.classList.toggle("has-files", files.length > 0);
        if (!files.length) return;

        files.forEach(function (file) {
            var card = document.createElement("div");
            card.className = "notification-file-card";

            var visual = document.createElement("span");
            visual.className = "notification-file-card__visual";
            if (/^image\//i.test(file.type)) {
                var image = document.createElement("img");
                var objectUrl = window.URL.createObjectURL(file);
                image.src = objectUrl;
                image.alt = "";
                image.dataset.objectUrl = objectUrl;
                visual.appendChild(image);
            } else {
                visual.classList.add("is-document");
                visual.textContent = fileExtension(file.name);
            }

            var info = document.createElement("span");
            info.className = "notification-file-card__info";
            var name = document.createElement("strong");
            name.textContent = file.name;
            var size = document.createElement("small");
            size.textContent = formatSize(file.size);
            info.appendChild(name);
            info.appendChild(size);

            var check = document.createElement("i");
            check.className = "ti-check notification-file-card__check";
            card.appendChild(visual);
            card.appendChild(info);
            card.appendChild(check);
            container.appendChild(card);
        });
    }

    document.querySelectorAll("[data-notification-files]").forEach(function (input) {
        input.addEventListener("change", function () { renderSelectedFiles(input); });
    });

    function addAttachment(container, file) {
        var row = document.createElement("div");
        row.className = "notification-existing-file";

        var visual = document.createElement("span");
        visual.className = "notification-existing-file__icon";
        visual.textContent = fileExtension(file.FileName);

        var link = document.createElement("a");
        link.href = file.FilePath;
        link.target = "_blank";
        link.rel = "noopener";
        link.textContent = file.FileName;

        var button = document.createElement("button");
        button.type = "button";
        button.innerHTML = '<i class="ti-trash"></i><span>حذف</span>';
        button.addEventListener("click", function () {
            if (!window.confirm("این فایل پیوست حذف شود؟")) return;
            deleteForm.querySelector('[name="attachmentId"]').value = file.Id;
            deleteForm.submit();
        });

        row.appendChild(visual);
        row.appendChild(link);
        row.appendChild(button);
        container.appendChild(row);
    }

    document.querySelectorAll("[data-edit-notification]").forEach(function (button) {
        button.addEventListener("click", function () {
            modal.querySelector('[name="id"]').value = button.dataset.id;
            modal.querySelector('[name="title"]').value = button.dataset.title || "";
            modal.querySelector('[name="content"]').value = button.dataset.content || "";
            modal.querySelector('[name="isActive"]').checked = button.dataset.active === "true";

            var fileInput = modal.querySelector("[data-notification-files]");
            if (fileInput) {
                fileInput.value = "";
                var preview = modal.querySelector("[data-file-preview]");
                if (preview) clearPreview(preview);
                var zone = fileInput.closest(".notification-upload-zone");
                if (zone) zone.classList.remove("has-files");
            }

            var container = modal.querySelector("[data-existing-files]");
            container.innerHTML = "";
            var files = [];
            try { files = JSON.parse(button.dataset.attachments || "[]"); } catch (ignore) { files = []; }
            if (!files.length) container.innerHTML = '<div class="notification-no-files"><i class="ti-info-alt"></i> فایل پیوستی ثبت نشده است.</div>';
            else {
                var title = document.createElement("strong");
                title.className = "notification-existing-files__title";
                title.textContent = "پیوست‌های فعلی";
                container.appendChild(title);
            }
            files.forEach(function (file) { addAttachment(container, file); });
            if (window.jQuery) window.jQuery(modal).modal("show");
        });
    });
})();

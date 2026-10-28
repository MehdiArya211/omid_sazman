(function () {
    "use strict";
    var modal = document.getElementById("notificationEditModal");
    var deleteForm = document.getElementById("deleteNotificationAttachmentForm");
    if (!modal) return;

    function addAttachment(container, file) {
        var row = document.createElement("div");
        row.className = "notification-existing-file";
        var link = document.createElement("a");
        link.href = file.FilePath; link.target = "_blank"; link.rel = "noopener"; link.textContent = file.FileName;
        var button = document.createElement("button");
        button.type = "button"; button.innerHTML = '<i class="ti-trash"></i> حذف فایل';
        button.addEventListener("click", function () {
            if (!window.confirm("این فایل پیوست حذف شود؟")) return;
            deleteForm.querySelector('[name="attachmentId"]').value = file.Id;
            deleteForm.submit();
        });
        row.appendChild(link); row.appendChild(button); container.appendChild(row);
    }

    document.querySelectorAll("[data-edit-notification]").forEach(function (button) {
        button.addEventListener("click", function () {
            modal.querySelector('[name="id"]').value = button.dataset.id;
            modal.querySelector('[name="title"]').value = button.dataset.title || "";
            modal.querySelector('[name="content"]').value = button.dataset.content || "";
            modal.querySelector('[name="isActive"]').checked = button.dataset.active === "true";
            var container = modal.querySelector("[data-existing-files]");
            container.innerHTML = "";
            var files = [];
            try { files = JSON.parse(button.dataset.attachments || "[]"); } catch (ignore) { files = []; }
            if (!files.length) container.innerHTML = "<small>فایل پیوستی ثبت نشده است.</small>";
            files.forEach(function (file) { addAttachment(container, file); });
            if (window.jQuery) window.jQuery(modal).modal("show");
        });
    });
})();

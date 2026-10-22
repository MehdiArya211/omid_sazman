(function () {
    "use strict";

    var root = document.querySelector("[data-slide-manager]");
    var modal = document.querySelector("[data-slide-modal]");
    if (!root || !modal) return;

    var form = modal.querySelector("[data-slide-form]");
    var image = modal.querySelector("[data-crop-image]");
    var placeholder = modal.querySelector("[data-crop-placeholder]");
    var fileInput = modal.querySelector("[data-image-input]");
    var croppedData = modal.querySelector("[data-cropped-data]");
    var saveButton = modal.querySelector("[data-save-slide]");
    var title = modal.querySelector("[data-modal-title]");
    var zoom = modal.querySelector("[data-crop-zoom]");
    var offsetX = modal.querySelector("[data-crop-x]");
    var offsetY = modal.querySelector("[data-crop-y]");
    var selectedSource = null;
    var replacementSelected = false;

    function field(name) { return modal.querySelector('[data-field="' + name + '"]'); }

    function updatePreview() {
        image.style.transform = "translate(" + offsetX.value + "%," + offsetY.value + "%) scale(" + (Number(zoom.value) / 100) + ")";
    }

    function setImage(source) {
        selectedSource = source;
        image.onload = function () {
            image.style.display = "block";
            placeholder.style.display = "none";
            updatePreview();
        };
        image.src = source;
    }

    function resetCrop() {
        zoom.value = "100";
        offsetX.value = "0";
        offsetY.value = "0";
        croppedData.value = "";
        fileInput.value = "";
        selectedSource = null;
        replacementSelected = false;
        image.removeAttribute("src");
        image.style.display = "none";
        placeholder.style.display = "flex";
        updatePreview();
    }

    function openModal(data) {
        resetCrop();
        var editing = Boolean(data);
        title.textContent = editing ? "ویرایش تصویر" : "افزودن تصویر";
        form.action = window.location.pathname + (editing ? "?handler=Edit" : "?handler=Create");
        field("id").value = editing ? data.id : "";
        field("title").value = editing ? data.title : "";
        field("description").value = editing ? data.description : "";
        field("alt").value = editing ? data.alt : "";
        field("order").value = editing ? data.order : "1";
        field("active").checked = editing ? data.active === "true" : true;
        if (editing && data.image) setImage(data.image);
        modal.hidden = false;
        document.body.classList.add("slide-modal-open");
        window.setTimeout(function () { field("title").focus(); }, 50);
    }

    function closeModal() {
        modal.hidden = true;
        document.body.classList.remove("slide-modal-open");
        resetCrop();
    }

    root.querySelector("[data-open-create]").addEventListener("click", function () { openModal(null); });
    root.querySelectorAll("[data-edit-slide]").forEach(function (button) {
        button.addEventListener("click", function () {
            openModal({
                id: button.dataset.id,
                image: button.dataset.image,
                title: button.dataset.title || "",
                description: button.dataset.description || "",
                alt: button.dataset.alt || "",
                order: button.dataset.order || "1",
                active: button.dataset.active
            });
        });
    });
    modal.querySelectorAll("[data-close-modal]").forEach(function (button) { button.addEventListener("click", closeModal); });
    [zoom, offsetX, offsetY].forEach(function (control) { control.addEventListener("input", updatePreview); });

    fileInput.addEventListener("change", function () {
        var file = fileInput.files && fileInput.files[0];
        if (!file) return;
        if (!/^image\/(jpeg|png|webp)$/i.test(file.type) || file.size > 12 * 1024 * 1024) {
            window.alert("فقط تصویر JPG، PNG یا WEBP با حجم حداکثر ۱۲ مگابایت قابل انتخاب است.");
            fileInput.value = "";
            return;
        }
        var reader = new FileReader();
        reader.onload = function (event) { replacementSelected = true; setImage(event.target.result); };
        reader.readAsDataURL(file);
    });

    form.addEventListener("submit", function (event) {
        if (!form.checkValidity()) return;
        var isCreate = !field("id").value;
        if (isCreate && !selectedSource) {
            event.preventDefault();
            window.alert("ابتدا تصویر را انتخاب کنید.");
            return;
        }
        if (replacementSelected) {
            event.preventDefault();
            var canvas = document.createElement("canvas");
            canvas.width = 900;
            canvas.height = 980;
            var context = canvas.getContext("2d");
            var scale = Math.max(canvas.width / image.naturalWidth, canvas.height / image.naturalHeight) * (Number(zoom.value) / 100);
            var width = image.naturalWidth * scale;
            var height = image.naturalHeight * scale;
            var availableX = Math.max(0, (width - canvas.width) / 2);
            var availableY = Math.max(0, (height - canvas.height) / 2);
            var x = (canvas.width - width) / 2 + availableX * (Number(offsetX.value) / 100);
            var y = (canvas.height - height) / 2 + availableY * (Number(offsetY.value) / 100);
            context.drawImage(image, x, y, width, height);
            croppedData.value = canvas.toDataURL("image/jpeg", 0.9);
            saveButton.disabled = true;
            saveButton.querySelector("span").textContent = "در حال ذخیره...";
            form.submit();
            return;
        }
        saveButton.disabled = true;
        saveButton.querySelector("span").textContent = "در حال ذخیره...";
    });

    document.addEventListener("keydown", function (event) { if (event.key === "Escape" && !modal.hidden) closeModal(); });
})();

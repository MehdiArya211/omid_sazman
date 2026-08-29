// =============================
// Logger حرفه‌ای برای Console
// =============================
const Logger = {
    info(title, data) {
        console.group(`ℹ️ ${title}`);
        if (data !== undefined) console.log(data);
        console.groupEnd();
    },
    warn(title, data) {
        console.group(`⚠️ ${title}`);
        if (data !== undefined) console.warn(data);
        console.groupEnd();
    },
    error(title, data) {
        console.group(`❌ ${title}`);
        if (data !== undefined) console.error(data);
        console.groupEnd();
    },
    success(title, data) {
        console.group(`✅ ${title}`);
        if (data !== undefined) console.log(data);
        console.groupEnd();
    }
};


(function ($) {

    // =============================
    // ابزارهای عمومی
    // =============================

    let fileValidationPassed = true;

    function clearFields(fields) {
        fields.forEach(field => $(field).val(''));
    }

    function showAlert(message) {
        const alertBox = $(`<div class="alert alert-danger col-md-12 mb-1" role="alert">${message}</div>`);
        $("#alertattachment").append(alertBox);
        setTimeout(() => alertBox.fadeOut(500, () => alertBox.remove()), 3000);
    }

    function handleFileUpload(input, validExtensions, maxSize) {
        const file = input.files[0];
        if (!file) return;

        const ext = file.name.split('.').pop().toLowerCase();
        if (validExtensions && !validExtensions.includes(ext)) {
            showAlert("فرمت فایل نامعتبر است.");
            input.value = '';
            fileValidationPassed = false;
            return;
        }
        if (file.size > maxSize) {
            showAlert("حجم فایل نمی‌تواند بیشتر از 2Mb باشد.");
            input.value = '';
            fileValidationPassed = false;
            return;
        }
        fileValidationPassed = true;
    }

    function toggleLoader(selector, show) {
        $(selector).toggleClass("d-none", !show);
    }

    function validatePersonalCode(code, emptyMsg) {
        if (!code) {
            alert(emptyMsg);
            return false;
        }
        return true;
    }

    // =============================
    // فرمت سه‌رقمی زنده
    // =============================
    function formatNumberInput(selector) {
        $(selector).on("input", function () {
            let cursorPos = this.selectionStart; // ذخیره مکان کرسر
            let value = $(this).val().replace(/\D/g, ""); // فقط عدد
            if (value) {
                let formatted = value.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                $(this).val(formatted);

                // برگرداندن کرسر به موقعیت مناسب
                this.selectionStart = this.selectionEnd = cursorPos + (formatted.length - value.length);
            } else {
                $(this).val("");
            }
        });
    }

    // =============================
    // اطلاعات شخصی
    // =============================
    function getInformation0() {
        const code = $("#personalno").val().trim();

        // اگر خالی است، فقط پاکسازی فیلدها و loader، هشدار نده
        if (!code) {
            clearFields(["#fname", "#lname", "#address", "#codemeli", "#darjeh", "#branchtitle"]);
            $("#personalno-error").text("").hide();
            return;
        }

        toggleLoader("#loderPersonalCode", true);

        $.getJSON(`?handler=GetPersonalId&personalno=${code}`, (data) => {
            handlePersonalDataResponse(
                data,
                ["#fname", "#lname", "#address"],
                ["#isarstatus", "#imgAvatar"],
                "/PersonalAvatar/"
            );
            toggleLoader("#loderPersonalCode", false);
        }).fail(() => {
            alert("خطا در واکشی اطلاعات");
            toggleLoader("#loderPersonalCode", false);
        });
    }

    function getInformation() {
        const code = $("#personalno").val().trim();

        Logger.info("رویداد دریافت اطلاعات پرسنلی", {
            personalCode: code,
            eventTime: new Date().toLocaleString()
        });

        // اگر خالی است
        if (!code) {
            Logger.warn("کد پرسنلی خالی است - پاکسازی فیلدها انجام شد");

            clearFields(["#fname", "#lname", "#address", "#codemeli", "#darjeh", "#branchtitle"]);
            $("#personalno-error").text("").hide();
            return;
        }

        toggleLoader("#loderPersonalCode", true);

        const url = `?handler=GetPersonalId&personalno=${code}`;
        Logger.info("ارسال درخواست به سرویس", { url });

        $.getJSON(url, (data) => {

            Logger.success("پاسخ موفق از سرویس دریافت شد", data);

            handlePersonalDataResponse(
                data,
                ["#fname", "#lname", "#address"],
                ["#isarstatus", "#imgAvatar"],
                "/PersonalAvatar/"
            );

            toggleLoader("#loderPersonalCode", false);

        }).fail((xhr, status, error) => {

            Logger.error("خطا در فراخوانی سرویس", {
                status,
                error,
                responseText: xhr?.responseText
            });

            alert("خطا در واکشی اطلاعات");
            toggleLoader("#loderPersonalCode", false);
        });
    }

    function handlePersonalDataResponse0(data, fieldsToFill, statusFields, avatarPath) {
        $("#personalno-error").hide().text("");

        if (data.message) {
            $("#personalno-error").text(data.message).show();
            clearFields(fieldsToFill);
            return;
        }

        const map = {
            "#fname": data.firstName,
            "#lname": data.lastName,
            "#codemeli": data.melliCode,
            "#darjeh": data.rankTitle,
            "#branchtitle": data.branchTitle,
            "#yeganekhedmati": data.unitDutyTitle,
            "#yeganeomdeh": data.unitTitle,
            "#gharargahemantaghei": data.codGhaTitle,
            "#address": data.addres,
            "#unitcode": data.unitCode,
            "#amaliatikhedmate": data.totAml2,
            "#ghableghatname": data.totAml,
            "#janbaziartesh": data.drsadJa,
            "#janbazibonyad": data.drsadJb,
            "#locationJob": data.locationJob,
            "#tashvighatCount": data.tashvighatCount,
            "#tanbihatCount": data.tanbihatCount,
            "#fararCount": data.fararCount,
            "#nahastCount": data.nahastCount
        };

        Object.entries(map).forEach(([selector, value]) => $(selector).val(value));

        $(statusFields[0]).val(
            ($("#amaliatikhedmate").val() === "00000000" || $("#ghableghatname").val() === "00000000") ? "ندارم" : "دارم"
        );

        $(statusFields[1]).attr('src', avatarPath + data.personalAvatarName);
    }

    function handlePersonalDataResponse(data, fieldsToFill, statusFields, avatarPath) {

        Logger.info("شروع پردازش داده‌های دریافتی", data);

        $("#personalno-error").hide().text("");

        if (data.message) {
            Logger.warn("سرویس پیام خطا برگرداند", data.message);

            $("#personalno-error").text(data.message).show();
            clearFields(fieldsToFill);
            return;
        }

        const map = {
            "#fname": data.firstName,
            "#lname": data.lastName,
            "#codemeli": data.melliCode,
            "#darjeh": data.rankTitle,
            "#branchtitle": data.branchTitle,
            "#yeganekhedmati": data.unitDutyTitle,
            "#yeganeomdeh": data.unitTitle,
            "#gharargahemantaghei": data.codGhaTitle,
            "#address": data.addres,
            "#unitcode": data.unitCode,
            "#amaliatikhedmate": data.totAml2,
            "#ghableghatname": data.totAml,
            "#janbaziartesh": data.drsadJa,
            "#janbazibonyad": data.drsadJb,
            "#locationJob": data.locationJob,
            "#tashvighatCount": data.tashvighatCount,
            "#tanbihatCount": data.tanbihatCount,
            "#fararCount": data.fararCount,
            "#nahastCount": data.nahastCount
        };

        Object.entries(map).forEach(([selector, value]) => {
            $(selector).val(value);
            Logger.info("پر شدن فیلد", { field: selector, value });
        });

        const isarStatus =
            ($("#amaliatikhedmate").val() === "00000000" ||
                $("#ghableghatname").val() === "00000000")
                ? "ندارم"
                : "دارم";

        $(statusFields[0]).val(isarStatus);

        Logger.info("وضعیت ایثارگری محاسبه شد", isarStatus);

        const avatarUrl = avatarPath + data.personalAvatarName;
        $(statusFields[1]).attr('src', avatarUrl);

        Logger.success("آواتار ست شد", avatarUrl);
    }



    // =============================
    // اطلاعات فرمانده
    // =============================
    function getinformationfarmandeh() {
        const code = $("#personalfarmandehno").val();
        if (!validatePersonalCode(code, "لطفاً کد پرسنلی را وارد کنید.")) return;

        toggleLoader("#loderFPersonalCode", true);
        clearFields(["#farmandehname"]);

        $.getJSON(`?handler=GetPersonalFarmandehId&personalno=${code}`, (data) => {
            if (data.message) {
                alert(data.message);
            } else {
                $("#farmandehname").val(`${data.rankTitle} ${data.firstName} ${data.lastName}`);
            }
            toggleLoader("#loderFPersonalCode", false);
        }).fail(() => {
            alert("خطا در واکشی اطلاعات، لطفاً دوباره تلاش کنید.");
            toggleLoader("#loderFPersonalCode", false);
        });
    }

    // =============================
    // فرم و آپلود
    // =============================

    function submitForm() {
        if (!fileValidationPassed) {
            alert("لطفاً خطاهای مربوط به فایل‌های پیوستی را برطرف کنید.");
            return;
        }



        // حالا فرم را ارسال کن
        const form = document.getElementById('requestForm');
        const formData = new FormData(form);

        fetch(form.action, {
            method: 'POST',
            body: formData
        })
            .then(res => res.text()) // پاسخ Razor Page
            .then(html => {
                document.open();
                document.write(html);
                document.close();
            })
            .catch(() => alert('خطا در ارسال فرم به سرور.'));
    }

    function previewFile(event) {
        const file = event.target.files[0];
        if (!file) return;

        const validImageTypes = ['image/jpeg', 'image/png', 'image/gif'];
        if (!validImageTypes.includes(file.type)) {
            alert("لطفاً یک فایل تصویر معتبر انتخاب کنید.");
            event.target.value = '';
            return;
        }

        const reader = new FileReader();
        reader.onload = e => createImagePreview(e.target.result, '200px', 'auto');
        reader.readAsDataURL(file);
    }

    function createImagePreview(src, width, height) {
        const imgPreview = document.createElement('img');
        imgPreview.src = src;
        imgPreview.style.width = width;
        imgPreview.style.height = height;

        const previewContainer = document.getElementById('previewContainer');
        previewContainer.innerHTML = '';
        previewContainer.appendChild(imgPreview);

        $("#clearPreview").show();
    }

    function readURL(input) {
        if (input.files && input.files[0]) {
            const reader = new FileReader();
            reader.onload = e => $('#imgAvatar').attr('src', e.target.result);
            reader.readAsDataURL(input.files[0]);
        }
    }

    // =============================
    // اتصال رویدادها
    // =============================
    $(document).ready(function () {
        //$("#personalno")
        //    .on("input", function () { $(this).val($(this).val().replace(/\D/g, '')); })
        //    .on("blur keydown", function (e) {
        //        if (e.type === "keydown" && (e.key === "Enter" || e.key === "Tab")) {
        //            e.preventDefault();
        //            getInformation();
        //        } else if (e.type === "blur") {
        //            getInformation();
        //        }
        //    });
        $("#personalno")
            .on("input", function () {
                this.value = this.value.replace(/\D/g, '');
                Logger.info("ورودی کد پرسنلی تغییر کرد", this.value);
            })
            .on("blur keydown", function (e) {

                Logger.info("رویداد ورودی", {
                    type: e.type,
                    key: e.key
                });

                if (e.type === "keydown" && (e.key === "Enter" || e.key === "Tab")) {
                    e.preventDefault();
                    getInformation();
                } else if (e.type === "blur") {
                    getInformation();
                }
            });


        $("#personalfarmandehno").on("blur keydown", function (e) {
            if (e.type === "keydown" && (e.key === "Enter" || e.key === "Tab")) {
                e.preventDefault();
                getinformationfarmandeh();
            } else if (e.type === "blur") {
                getinformationfarmandeh();
            }
        });

        $("#attachment").change(function () {
            handleFileUpload(this, ["jpg", "jpeg", "png", "pdf", "doc", "docx", "rar"], 2097152);
        });

        $("#fishAttachment").change(function () {
            handleFileUpload(this, null, 2097152);
        });

        $("#factPersonalViewModel_PersonalAvatar").change(function () {
            handleFileUpload(this, ["png", "jpeg", "jpg", "tiff", "PNG"], 2097152);
            readURL(this);
        });

        $("#clearPreview").on("click", function () {
            $("#previewContainer").empty();
            $("#attachment").val('');
            $(this).hide();
        });

        // فرمت سه‌رقمی زنده برای فیلدهای مالی
        formatNumberInput("#TotalMoney, #ReciveMoney, #CountVam, #SumAghsatVamMahiyaneh");
    });

    // =============================
    // اکسپورت به گلوبال
    // =============================
    window.getInformation = getInformation;
    window.handlePersonalDataResponse = handlePersonalDataResponse;
    window.getinformationfarmandeh = getinformationfarmandeh;
    window.submitForm = submitForm;
    window.previewFile = previewFile;

})(jQuery);


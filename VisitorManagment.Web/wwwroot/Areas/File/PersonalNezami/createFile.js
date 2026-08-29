//(function ($) {
//    "use strict";

//    const SELECTORS = {
//        form: "#createFileForm",

//        personalNo: "#personalno",
//        commanderNo: "#personalfarmandehno",

//        loaderPersonal: "#loderPersonalCode",
//        loaderCommander: ".loderFPersonalCode",

//        avatar: "#imgAvatar",
//        personalAvatarInput: "#personalAvatar",

//        alertBox: "#alertattachment",
//        submit: "#submitsazmani",

//        attachment: "#attachment",
//        fishAttachment: "#fishAttachment",

//        attachmentPreviewContainer: "#attachmentPreviewContainer",
//        fishPreviewContainer: "#fishPreviewContainer",

//        phone: "#phone",

//        totalMoney: "#TotalMoney",
//        totalMoneyHidden: "#TotalMoneyHidden",

//        reciveMoney: "#ReciveMoney",
//        reciveMoneyHidden: "#ReciveMoneyHidden",

//        countVam: "#CountVam",

//        sumAghsat: "#SumAghsatVamMahiyaneh",
//        sumAghsatHidden: "#SumAghsatVamMahiyanehHidden"
//    };

//    let lastPersonalCode = "";
//    let lastCommanderCode = "";

//    let personalRequest = null;
//    let commanderRequest = null;

//    let personalLoaded = false;
//    let commanderLoaded = false;

//    const state = {
//        getPersonalUrl: "",
//        getCommanderUrl: "",
//        defaultAvatarUrl: "/PersonalAvatar/Default.png",
//        avatarBaseUrl: "/PersonalAvatar/"
//    };

//    // =======================================
//    // Init
//    // =======================================
//    $(document).ready(function () {
//        initState();
//        bindEvents();
//        initMoneyInputs();
//        restoreLoadedStateIfServerRenderedValuesExist();
//    });

//    function initState() {
//        const $form = $(SELECTORS.form);

//        state.getPersonalUrl = $form.data("get-personal-url") || "?handler=GetPersonalId";
//        state.getCommanderUrl = $form.data("get-commander-url") || "?handler=GetPersonalFarmandehId";
//        state.defaultAvatarUrl = $form.data("default-avatar-url") || "/PersonalAvatar/Default.png";
//        state.avatarBaseUrl = $form.data("avatar-base-url") || "/PersonalAvatar/";
//    }

//    function bindEvents() {
//        $(SELECTORS.personalNo)
//            .on("input", function () {
//                this.value = onlyNumber(this.value).substring(0, 9);
//                personalLoaded = false;
//                clearPersonalFields(false);
//            })
//            .on("blur", function () {
//                getPersonalInfo(false);
//            })
//            .on("keydown", function (e) {
//                if (e.key === "Enter") {
//                    e.preventDefault();
//                    getPersonalInfo(true);
//                }
//            });

//        $(SELECTORS.commanderNo)
//            .on("input", function () {
//                this.value = onlyNumber(this.value).substring(0, 9);
//                commanderLoaded = false;
//                clearCommanderFields();
//            })
//            .on("blur", function () {
//                getCommanderInfo(false);
//            })
//            .on("keydown", function (e) {
//                if (e.key === "Enter") {
//                    e.preventDefault();
//                    getCommanderInfo(true);
//                }
//            });

//        $(SELECTORS.attachment).on("change", function () {
//            if (validateFile(this, ["jpg", "jpeg", "png", "pdf", "doc", "docx", "rar"], 2 * 1024 * 1024)) {
//                previewAttachmentFile(this, SELECTORS.attachmentPreviewContainer, false);
//            } else {
//                clearFileInput(this, SELECTORS.attachmentPreviewContainer);
//            }
//        });

//        $(SELECTORS.fishAttachment).on("change", function () {
//            if (validateFile(this, ["jpg", "jpeg", "png", "pdf"], 2 * 1024 * 1024)) {
//                previewAttachmentFile(this, SELECTORS.fishPreviewContainer, true);
//            } else {
//                clearFileInput(this, SELECTORS.fishPreviewContainer);
//            }
//        });

//        $(SELECTORS.personalAvatarInput).on("change", function () {
//            if (validateFile(this, ["jpg", "jpeg", "png"], 2 * 1024 * 1024)) {
//                previewImage(this, SELECTORS.avatar);
//            }
//        });

//        $(SELECTORS.phone).on("input", function () {
//            this.value = onlyNumber(this.value).substring(0, 11);
//        });

//        $(SELECTORS.countVam).on("input", function () {
//            this.value = onlyNumber(this.value);
//        });

//        $(SELECTORS.form).on("submit", function (e) {
//            syncMoneyBeforeSubmit();

//            if (!validateBeforeSubmit()) {
//                e.preventDefault();
//                return false;
//            }

//            setSubmitLoading(true);
//            return true;
//        });
//    }

//    function restoreLoadedStateIfServerRenderedValuesExist() {
//        if ($(SELECTORS.personalNo).val() && $("#fname").val() && $("#lname").val()) {
//            personalLoaded = true;
//            lastPersonalCode = $(SELECTORS.personalNo).val();
//        }

//        if ($(SELECTORS.commanderNo).val() && $("#farmandehname").val()) {
//            commanderLoaded = true;
//            lastCommanderCode = $(SELECTORS.commanderNo).val();
//        }
//    }

//    // =======================================
//    // AJAX: Personal
//    // =======================================
//    function getPersonalInfo(force) {
//        const code = onlyNumber($(SELECTORS.personalNo).val());

//        $(SELECTORS.personalNo).val(code);

//        if (!code) {
//            clearPersonalFields(true);
//            return;
//        }

//        if (code.length > 9) {
//            showAlert("کد پرسنلی حداکثر باید ۹ رقم باشد.");
//            clearPersonalFields(true);
//            return;
//        }

//        if (!force && code === lastPersonalCode && personalLoaded) {
//            return;
//        }

//        lastPersonalCode = code;
//        personalLoaded = false;

//        if (personalRequest) {
//            personalRequest.abort();
//        }

//        clearPersonalFields(false);
//        toggleLoader(SELECTORS.loaderPersonal, true);
//        setSubmitLoading(true);

//        personalRequest = $.getJSON(buildUrl(state.getPersonalUrl, "personalno", code))
//            .done(function (res) {
//                const response = normalizeApiResponse(res);

//                if (!response.success || !response.data) {
//                    showAlert(response.message || "پرسنلی با این مشخصات یافت نشد.");
//                    clearPersonalFields(true);
//                    return;
//                }

//                fillPersonalFields(response.data);
//                personalLoaded = true;
//            })
//            .fail(function (xhr, status) {
//                if (status !== "abort") {
//                    showAlert("خطا در ارتباط با سرور هنگام دریافت اطلاعات پرسنلی.");
//                    clearPersonalFields(true);
//                }
//            })
//            .always(function () {
//                toggleLoader(SELECTORS.loaderPersonal, false);
//                setSubmitLoading(false);
//            });
//    }

//    function fillPersonalFields(data) {
//        $("#fname").val(data.firstName || "");
//        $("#lname").val(data.lastName || "");
//        $("#codemeli").val(data.melliCode || "");
//        $("#darjeh").val(data.rankTitle || "");
//        $("#branchtitle").val(data.branchTitle || "");
//        $("#yeganekhedmati").val(data.unitDutyTitle || "");
//        $("#yeganeomdeh").val(data.unitTitle || "");
//        $("#address").val(data.addres || "");
//        $("#amaliatikhedmate").val(data.totAml2 || data.tOT_AML2 || data.toT_AML2 || "");
//        $("#ghableghatname").val(data.totAml || data.tOT_AML || data.toT_AML || "");
//        $("#janbaziartesh").val(data.drsadJa || data.dRSAD_JA || data.drsaD_JA || "");
//        $("#janbazibonyad").val(data.drsadJb || data.dRSAD_JB || data.drsaD_JB || "");

//        const totAml2 = data.totAml2 || data.tOT_AML2 || data.toT_AML2 || "";
//        const totAml = data.totAml || data.tOT_AML || data.toT_AML || "";
//        const drsadJa = data.drsadJa || data.dRSAD_JA || data.drsaD_JA || "";
//        const drsadJb = data.drsadJb || data.dRSAD_JB || data.drsaD_JB || "";

//        $("#isarstatus").val(calculateIsarStatus(totAml2, totAml, drsadJa, drsadJb));

//        const avatarName = data.personalAvatarName || "Default.png";
//        $(SELECTORS.avatar).attr("src", state.avatarBaseUrl + avatarName);
//    }

//    function clearPersonalFields(clearAvatar) {
//        personalLoaded = false;

//        $("#fname").val("");
//        $("#lname").val("");
//        $("#codemeli").val("");
//        $("#darjeh").val("");
//        $("#branchtitle").val("");
//        $("#yeganekhedmati").val("");
//        $("#yeganeomdeh").val("");
//        $("#address").val("");
//        $("#amaliatikhedmate").val("");
//        $("#ghableghatname").val("");
//        $("#janbaziartesh").val("");
//        $("#janbazibonyad").val("");
//        $("#isarstatus").val("");

//        if (clearAvatar) {
//            $(SELECTORS.avatar).attr("src", state.defaultAvatarUrl);
//        }
//    }

//    // =======================================
//    // AJAX: Commander
//    // =======================================
//    function getCommanderInfo(force) {
//        const code = onlyNumber($(SELECTORS.commanderNo).val());

//        $(SELECTORS.commanderNo).val(code);

//        if (!code) {
//            clearCommanderFields();
//            return;
//        }

//        if (code.length > 9) {
//            showAlert("کد پرسنلی فرمانده حداکثر باید ۹ رقم باشد.");
//            clearCommanderFields();
//            return;
//        }

//        if (!force && code === lastCommanderCode && commanderLoaded) {
//            return;
//        }

//        lastCommanderCode = code;
//        commanderLoaded = false;

//        if (commanderRequest) {
//            commanderRequest.abort();
//        }

//        clearCommanderFields();
//        toggleLoader(SELECTORS.loaderCommander, true);
//        setSubmitLoading(true);

//        commanderRequest = $.getJSON(buildUrl(state.getCommanderUrl, "personalno", code))
//            .done(function (res) {
//                const response = normalizeApiResponse(res);

//                if (!response.success || !response.data) {
//                    showAlert(response.message || "فرمانده‌ای با این کد پرسنلی یافت نشد.");
//                    clearCommanderFields();
//                    return;
//                }

//                fillCommanderFields(response.data);
//                commanderLoaded = true;
//            })
//            .fail(function (xhr, status) {
//                if (status !== "abort") {
//                    showAlert("خطا در ارتباط با سرور هنگام دریافت اطلاعات فرمانده.");
//                    clearCommanderFields();
//                }
//            })
//            .always(function () {
//                toggleLoader(SELECTORS.loaderCommander, false);
//                setSubmitLoading(false);
//            });
//    }

//    function fillCommanderFields(data) {
//        const fullName = `${data.rankTitle || ""} ${data.firstName || ""} ${data.lastName || ""}`.trim();

//        if (!fullName) {
//            showAlert("اطلاعات فرمانده ناقص است.");
//            clearCommanderFields();
//            return;
//        }

//        $("#farmandehname").val(fullName);
//    }

//    function clearCommanderFields() {
//        commanderLoaded = false;
//        $("#farmandehname").val("");
//    }

//    // =======================================
//    // Validation
//    // =======================================
//    function validateBeforeSubmit() {
//        if (!personalLoaded) {
//            showAlert("لطفاً ابتدا کد پرسنلی معتبر وارد کنید تا اطلاعات پرسنل دریافت شود.");
//            $(SELECTORS.personalNo).focus();
//            return false;
//        }

//        if (!commanderLoaded) {
//            showAlert("لطفاً ابتدا کد پرسنلی فرمانده معتبر وارد کنید تا نام فرمانده دریافت شود.");
//            $(SELECTORS.commanderNo).focus();
//            return false;
//        }

//        const phone = $(SELECTORS.phone).val();

//        if (!/^09\d{9}$/.test(phone)) {
//            showAlert("شماره همراه معتبر نیست. مثال صحیح: 09123456789");
//            $(SELECTORS.phone).focus();
//            return false;
//        }

//        const fishFile = $(SELECTORS.fishAttachment)[0]?.files?.[0];

//        if (!fishFile) {
//            showAlert("آپلود فایل فیش حقوقی الزامی است.");
//            $(SELECTORS.fishAttachment).focus();
//            return false;
//        }

//        return true;
//    }

//    function validateFile(input, extensions, maxSize) {
//        const file = input.files[0];

//        if (!file) {
//            return true;
//        }

//        const ext = file.name.split(".").pop().toLowerCase();

//        if (extensions && !extensions.includes(ext)) {
//            showAlert("فرمت فایل نامعتبر است.");
//            input.value = "";
//            return false;
//        }

//        if (file.size > maxSize) {
//            showAlert("حجم فایل بیشتر از ۲ مگابایت است.");
//            input.value = "";
//            return false;
//        }

//        return true;
//    }

//    // =======================================
//    // Money Inputs
//    // =======================================
//    function initMoneyInputs() {
//        setupMoneyInput(SELECTORS.totalMoney, SELECTORS.totalMoneyHidden);
//        setupMoneyInput(SELECTORS.reciveMoney, SELECTORS.reciveMoneyHidden);
//        setupMoneyInput(SELECTORS.sumAghsat, SELECTORS.sumAghsatHidden);
//    }

//    function setupMoneyInput(viewSelector, hiddenSelector) {
//        const $view = $(viewSelector);
//        const $hidden = $(hiddenSelector);

//        const hiddenValue = onlyNumber($hidden.val());

//        if (hiddenValue && hiddenValue !== "0") {
//            $view.val(formatNumber(hiddenValue));
//        }

//        $view.on("input", function () {
//            const raw = onlyNumber(this.value);

//            $hidden.val(raw);
//            this.value = raw ? formatNumber(raw) : "";
//        });
//    }

//    function syncMoneyBeforeSubmit() {
//        syncMoneyInput(SELECTORS.totalMoney, SELECTORS.totalMoneyHidden);
//        syncMoneyInput(SELECTORS.reciveMoney, SELECTORS.reciveMoneyHidden);
//        syncMoneyInput(SELECTORS.sumAghsat, SELECTORS.sumAghsatHidden);
//    }

//    function syncMoneyInput(viewSelector, hiddenSelector) {
//        const raw = onlyNumber($(viewSelector).val());

//        $(hiddenSelector).val(raw);
//        $(viewSelector).val(raw ? formatNumber(raw) : "");
//    }

//    function formatNumber(value) {
//        return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
//    }

//    // =======================================
//    // Preview
//    // =======================================
//    // =======================================
//    // Preview
//    // =======================================
//    function previewImage(input, target) {
//        if (!input.files || !input.files[0]) {
//            return;
//        }

//        const reader = new FileReader();

//        reader.onload = function (e) {
//            $(target).attr("src", e.target.result);
//        };

//        reader.readAsDataURL(input.files[0]);
//    }

//    function previewAttachmentFile(input, containerSelector, isRequired) {
//        const $container = $(containerSelector);
//        $container.empty();

//        if (!input.files || !input.files[0]) {
//            return;
//        }

//        const file = input.files[0];
//        const fileName = file.name;
//        const ext = getFileExtension(fileName);
//        const fileSize = formatFileSize(file.size);

//        if (["jpg", "jpeg", "png"].includes(ext)) {
//            previewImageFile(input, containerSelector, fileName, fileSize, isRequired);
//            return;
//        }

//        if (ext === "pdf") {
//            renderFilePreviewCard(
//                input,
//                containerSelector,
//                "فایل PDF انتخاب شد",
//                fileName,
//                fileSize,
//                "fa-file-pdf",
//                isRequired
//            );
//            return;
//        }

//        if (["doc", "docx"].includes(ext)) {
//            renderFilePreviewCard(
//                input,
//                containerSelector,
//                "فایل Word انتخاب شد",
//                fileName,
//                fileSize,
//                "fa-file-word",
//                isRequired
//            );
//            return;
//        }

//        if (ext === "rar") {
//            renderFilePreviewCard(
//                input,
//                containerSelector,
//                "فایل فشرده انتخاب شد",
//                fileName,
//                fileSize,
//                "fa-file-archive",
//                isRequired
//            );
//            return;
//        }

//        renderFilePreviewCard(
//            input,
//            containerSelector,
//            "فایل انتخاب شد",
//            fileName,
//            fileSize,
//            "fa-file",
//            isRequired
//        );
//    }

//    function previewImageFile(input, containerSelector, fileName, fileSize, isRequired) {
//        const $container = $(containerSelector);
//        const reader = new FileReader();

//        reader.onload = function (e) {
//            $container.html(`
//            <div class="attachment-preview-card border rounded p-2 bg-light">
//                <div class="d-flex align-items-start justify-content-between">
//                    <div class="d-flex align-items-start">
//                        <img src="${e.target.result}"
//                             alt="${escapeHtml(fileName)}"
//                             style="width:90px; height:75px; object-fit:cover; border:1px solid #ddd; border-radius:8px; margin-left:10px;" />

//                        <div>
//                            <div class="font-weight-bold text-dark">
//                                ${escapeHtml(fileName)}
//                            </div>
//                            <small class="text-muted">
//                                حجم فایل: ${escapeHtml(fileSize)}
//                            </small>
//                        </div>
//                    </div>

//                    <button type="button"
//                            class="btn btn-sm btn-danger btn-remove-file"
//                            title="حذف فایل انتخاب شده">
//                        حذف
//                    </button>
//                </div>
//            </div>
//        `);

//            bindRemoveFileButton(input, containerSelector, isRequired);
//        };

//        reader.readAsDataURL(input.files[0]);
//    }

//    function renderFilePreviewCard(input, containerSelector, title, fileName, fileSize, iconClass, isRequired) {
//        const $container = $(containerSelector);

//        $container.html(`
//        <div class="attachment-preview-card border rounded p-2 bg-light">
//            <div class="d-flex align-items-center justify-content-between">
//                <div class="d-flex align-items-center">
//                    <div style="font-size:34px; margin-left:10px;">
//                        <i class="fa ${iconClass}"></i>
//                    </div>

//                    <div>
//                        <div class="font-weight-bold text-dark">
//                            ${escapeHtml(title)}
//                        </div>
//                        <div class="text-muted" style="font-size:13px;">
//                            ${escapeHtml(fileName)}
//                        </div>
//                        <small class="text-muted">
//                            حجم فایل: ${escapeHtml(fileSize)}
//                        </small>
//                    </div>
//                </div>

//                <button type="button"
//                        class="btn btn-sm btn-danger btn-remove-file"
//                        title="حذف فایل انتخاب شده">
//                    حذف
//                </button>
//            </div>
//        </div>
//    `);

//        bindRemoveFileButton(input, containerSelector, isRequired);
//    }

//    function bindRemoveFileButton(input, containerSelector, isRequired) {
//        $(containerSelector)
//            .find(".btn-remove-file")
//            .off("click")
//            .on("click", function () {
//                clearFileInput(input, containerSelector);

//                if (isRequired) {
//                    showAlert("فایل فیش حقوقی حذف شد. برای ثبت درخواست، انتخاب فیش حقوقی الزامی است.", "warning");
//                }
//            });
//    }

//    function clearFileInput(input, containerSelector) {
//        input.value = "";
//        $(containerSelector).empty();
//    }

//    function getFileExtension(fileName) {
//        return (fileName || "").split(".").pop().toLowerCase();
//    }

//    function formatFileSize(size) {
//        if (!size || size <= 0) {
//            return "0 KB";
//        }

//        const kb = size / 1024;

//        if (kb < 1024) {
//            return `${kb.toFixed(1)} KB`;
//        }

//        return `${(kb / 1024).toFixed(2)} MB`;
//    }

//    // =======================================
//    // Helpers
//    // =======================================
//    function normalizeApiResponse(res) {
//        if (!res) {
//            return {
//                success: false,
//                message: "پاسخی از سرور دریافت نشد.",
//                data: null
//            };
//        }

//        // خروجی جدید Handler:
//        // { success: true/false, data: object, message: string }
//        if (typeof res.success !== "undefined") {
//            return {
//                success: res.success === true,
//                message: res.message || "",
//                data: res.data || null
//            };
//        }

//        // خروجی قدیمی:
//        // { message: "..." }
//        if (res.message) {
//            return {
//                success: false,
//                message: res.message,
//                data: null
//            };
//        }

//        // خروجی قدیمی مستقیم data
//        return {
//            success: true,
//            message: "",
//            data: res
//        };
//    }

//    function calculateIsarStatus(totAml2, totAml, drsadJa, drsadJb) {
//        const aml2 = normalizeDuration(totAml2);
//        const aml = normalizeDuration(totAml);

//        const ja = toNumber(drsadJa);
//        const jb = toNumber(drsadJb);

//        return aml2 || aml || ja > 0 || jb > 0 ? "دارم" : "ندارم";
//    }

//    function normalizeDuration(value) {
//        const text = (value ?? "").toString().trim();

//        if (
//            text === "" ||
//            text === "0" ||
//            text === "00" ||
//            text === "0000" ||
//            text === "00000000"
//        ) {
//            return "";
//        }

//        return text;
//    }

//    function toNumber(value) {
//        const number = Number((value ?? "0").toString().replace(/[^\d.]/g, ""));
//        return isNaN(number) ? 0 : number;
//    }

//    function onlyNumber(value) {
//        return (value || "").toString().replace(/\D/g, "");
//    }

//    function buildUrl(baseUrl, key, value) {
//        const separator = baseUrl.includes("?") ? "&" : "?";
//        return `${baseUrl}${separator}${encodeURIComponent(key)}=${encodeURIComponent(value)}`;
//    }

//    function toggleLoader(selector, show) {
//        $(selector).toggleClass("d-none", !show);
//    }

//    function setSubmitLoading(isLoading) {
//        $(SELECTORS.submit)
//            .prop("disabled", isLoading)
//            .toggleClass("disabled", isLoading);
//    }

//    function showAlert(message, type) {
//        const alertType = type || "danger";
//        const $box = $(SELECTORS.alertBox);

//        const $alert = $(`
//            <div class="alert alert-${alertType} alert-dismissible fade show" role="alert">
//                ${escapeHtml(message)}
//                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
//                    <span aria-hidden="true">&times;</span>
//                </button>
//            </div>
//        `);

//        $box.append($alert);

//        setTimeout(function () {
//            $alert.fadeOut(500, function () {
//                $(this).remove();
//            });
//        }, 4000);
//    }

//    function escapeHtml(value) {
//        return $("<div>").text(value || "").html();
//    }

//})(jQuery);

(function ($) {
    "use strict";

    /*
     * این آبجکت تمام سلکتورهای صفحه را یکجا نگه می‌دارد.
     * مزیتش این است که اگر بعداً id یک input در ویو تغییر کرد،
     * فقط همین‌جا تغییر می‌دهی و لازم نیست کل فایل JS را بگردی.
     */
    const SELECTORS = {
        form: "#createFileForm",

        personalNo: "#personalno",
        commanderNo: "#personalfarmandehno",

        personalMessage: "#personalCodeMessage",
        commanderMessage: "#commanderCodeMessage",

        loaderPersonal: "#loderPersonalCode",
        loaderCommander: ".loderFPersonalCode",

        avatar: "#imgAvatar",
        personalAvatarInput: "#personalAvatar",

        alertBox: "#alertattachment",
        submit: "#submitsazmani",

        attachment: "#attachment",
        fishAttachment: "#fishAttachment",

        attachmentPreviewContainer: "#attachmentPreviewContainer",
        fishPreviewContainer: "#fishPreviewContainer",

        phone: "#phone",

        totalMoney: "#TotalMoney",
        totalMoneyHidden: "#TotalMoneyHidden",

        reciveMoney: "#ReciveMoney",
        reciveMoneyHidden: "#ReciveMoneyHidden",

        countVam: "#CountVam",

        sumAghsat: "#SumAghsatVamMahiyaneh",
        sumAghsatHidden: "#SumAghsatVamMahiyanehHidden"
    };

    /*
     * آخرین کد پرسنلی و آخرین کد فرمانده که با موفقیت بررسی شده‌اند.
     * از این‌ها استفاده می‌کنیم تا اگر کاربر دوباره همان کد را blur کرد،
     * درخواست AJAX تکراری به سرور ارسال نشود.
     */
    let lastPersonalCode = "";
    let lastCommanderCode = "";

    /*
     * این دو متغیر درخواست AJAX فعال را نگه می‌دارند.
     * اگر کاربر وسط دریافت اطلاعات، کد را تغییر دهد،
     * درخواست قبلی abort می‌شود تا جواب‌های قدیمی با فیلد جدید قاطی نشوند.
     */
    let personalRequest = null;
    let commanderRequest = null;

    /*
     * مشخص می‌کند اطلاعات پرسنل و فرمانده واقعاً از سرور دریافت شده یا نه.
     * موقع submit از این دو مقدار استفاده می‌کنیم.
     */
    let personalLoaded = false;
    let commanderLoaded = false;

    /*
     * این‌ها وضعیت لودینگ فرم را کنترل می‌کنند.
     * اگر اطلاعات پرسنل یا فرمانده در حال دریافت باشد،
     * دکمه ثبت غیرفعال می‌شود تا کاربر زودتر از برگشت AJAX ثبت نزند.
     */
    let personalLoading = false;
    let commanderLoading = false;
    let formSubmitting = false;

    /*
     * آدرس هندلرهای Razor Page و مسیر عکس‌ها از روی خود فرم خوانده می‌شود.
     * این باعث می‌شود آدرس‌ها داخل JS هاردکد نباشند.
     */
    const state = {
        getPersonalUrl: "",
        getCommanderUrl: "",
        defaultAvatarUrl: "/PersonalAvatar/Default.png",
        avatarBaseUrl: "/PersonalAvatar/"
    };

    // =======================================
    // Init
    // =======================================

    /*
     * نقطه شروع فایل JS.
     * بعد از آماده شدن DOM اجرا می‌شود.
     * اول مطمئن می‌شود فرم موردنظر در صفحه وجود دارد.
     * سپس تنظیمات، پیام‌های اختصاصی، رویدادها و ورودی‌های مالی را آماده می‌کند.
     */
    $(document).ready(function () {
        if (!$(SELECTORS.form).length) {
            return;
        }

        initState();
        initFieldMessages();
        bindEvents();
        initMoneyInputs();
        restoreLoadedStateIfServerRenderedValuesExist();
    });

    /*
     * اطلاعات موردنیاز JS را از attributeهای data روی فرم می‌خواند.
     * در ویو این موارد روی فرم قرار داده شده‌اند:
     * data-get-personal-url
     * data-get-commander-url
     * data-default-avatar-url
     * data-avatar-base-url
     */
    function initState() {
        const $form = $(SELECTORS.form);

        state.getPersonalUrl = $form.data("get-personal-url") || "?handler=GetPersonalId";
        state.getCommanderUrl = $form.data("get-commander-url") || "?handler=GetPersonalFarmandehId";
        state.defaultAvatarUrl = $form.data("default-avatar-url") || "/PersonalAvatar/Default.png";
        state.avatarBaseUrl = $form.data("avatar-base-url") || "/PersonalAvatar/";
    }

    /*
     * پیام‌های اختصاصی زیر فیلد کد پرسنلی و کد فرمانده را ایجاد می‌کند.
     * چون نمی‌خواهیم پیام این دو فیلد داخل alert عمومی قاطی شود.
     */
    function initFieldMessages() {
        ensureFieldMessage(SELECTORS.personalNo, "personalCodeMessage");
        ensureFieldMessage(SELECTORS.commanderNo, "commanderCodeMessage");
    }

    /*
     * اگر div پیام برای یک input وجود نداشته باشد، آن را به صورت داینامیک می‌سازد.
     * پیام بعد از span اعتبارسنجی همان فیلد قرار می‌گیرد.
     */
    function ensureFieldMessage(inputSelector, id) {
        if ($("#" + id).length) {
            return;
        }

        const $input = $(inputSelector);
        const $validationSpan = $input.nextAll("span.text-danger").first();

        const html = `
            <div id="${id}"
                 class="small mt-1 d-none js-field-message">
            </div>
        `;

        if ($validationSpan.length) {
            $validationSpan.after(html);
        } else {
            $input.after(html);
        }
    }

    /*
     * تمام eventهای صفحه اینجا bind می‌شوند.
     * شامل:
     * تایپ و blur کد پرسنلی
     * تایپ و blur کد فرمانده
     * تغییر فایل‌ها
     * کنترل شماره موبایل
     * کنترل تعداد وام
     * submit فرم
     */
    function bindEvents0() {
        $(SELECTORS.personalNo)
            .on("input", function () {
                abortPersonalRequest();

                this.value = onlyNumber(this.value).substring(0, 9);

                lastPersonalCode = "";
                personalLoaded = false;

                clearPersonalFields(false);
                clearPersonalMessage();
            })
            .on("blur", function () {
                getPersonalInfo(false);
            })
            .on("keydown", function (e) {
                if (e.key === "Enter") {
                    e.preventDefault();
                    getPersonalInfo(true);
                }
            });

        $(SELECTORS.commanderNo)
            .on("input", function () {
                abortCommanderRequest();

                this.value = onlyNumber(this.value).substring(0, 9);

                lastCommanderCode = "";
                commanderLoaded = false;

                clearCommanderFields();
                clearCommanderMessage();
            })
            .on("blur", function () {
                getCommanderInfo(false);
            })
            .on("keydown", function (e) {
                if (e.key === "Enter") {
                    e.preventDefault();
                    getCommanderInfo(true);
                }
            });

        $(SELECTORS.attachment).on("change", function () {
            if (validateFile(this, ["jpg", "jpeg", "png", "pdf", "doc", "docx", "rar"], 2 * 1024 * 1024)) {
                previewAttachmentFile(this, SELECTORS.attachmentPreviewContainer, false);
            } else {
                clearFileInput(this, SELECTORS.attachmentPreviewContainer);
            }
        });

        $(SELECTORS.fishAttachment).on("change", function () {
            if (validateFile(this, ["jpg", "jpeg", "png", "pdf"], 2 * 1024 * 1024)) {
                previewAttachmentFile(this, SELECTORS.fishPreviewContainer, true);
            } else {
                clearFileInput(this, SELECTORS.fishPreviewContainer);
            }
        });

        $(SELECTORS.personalAvatarInput).on("change", function () {
            if (validateFile(this, ["jpg", "jpeg", "png"], 2 * 1024 * 1024)) {
                previewImage(this, SELECTORS.avatar);
            }
        });

        $(SELECTORS.phone).on("input", function () {
            this.value = onlyNumber(this.value).substring(0, 11);
        });

        $(SELECTORS.countVam).on("input", function () {
            this.value = onlyNumber(this.value);
        });

        $(SELECTORS.form).on("submit", function (e) {
            syncMoneyBeforeSubmit();
            syncLoadedStateFromCurrentFields();

            /*
             * اول اعتبارسنجی خود jQuery Validate را بررسی می‌کنیم.
             * اگر یکی از فیلدهای Required خالی باشد، فرم ارسال نمی‌شود
             * و دکمه ثبت هم نباید غیرفعال بماند.
             */
            if ($.validator && !$(this).valid()) {
                e.preventDefault();
                setFormSubmitting(false);
                return false;
            }

            /*
             * بعد اعتبارسنجی‌های اختصاصی خودمان اجرا می‌شود؛
             * مثل بررسی اینکه اطلاعات پرسنل و فرمانده از AJAX گرفته شده باشد.
             */
            if (!validateBeforeSubmit()) {
                e.preventDefault();
                setFormSubmitting(false);
                return false;
            }

            /*
             * فقط وقتی همه چیز درست بود، دکمه ثبت غیرفعال شود
             * تا کاربر چند بار پشت سر هم ثبت نزند.
             */
            setFormSubmitting(true);
            return true;
        });
    }

    function bindEvents() {
        /*
         * رویدادهای مربوط به کد پرسنلی اصلی
         * با هر بار تایپ:
         * - درخواست قبلی AJAX لغو می‌شود
         * - فقط عدد تا ۹ رقم نگه داشته می‌شود
         * - اطلاعات قبلی پرسنل پاک می‌شود
         * - پیام اختصاصی همان فیلد پاک می‌شود
         */
        $(SELECTORS.personalNo)
            .on("input", function () {
                abortPersonalRequest();

                this.value = onlyNumber(this.value).substring(0, 9);

                lastPersonalCode = "";
                personalLoaded = false;

                clearPersonalFields(false);
                clearPersonalMessage();

                /*
                 * اگر قبلاً دکمه ثبت غیرفعال شده باشد،
                 * با تغییر مقدار فیلد دوباره وضعیتش بررسی شود.
                 */
                setFormSubmitting(false);
            })
            .on("blur", function () {
                getPersonalInfo(false);
            })
            .on("keydown", function (e) {
                if (e.key === "Enter") {
                    e.preventDefault();
                    getPersonalInfo(true);
                }
            });

        /*
         * رویدادهای مربوط به کد پرسنلی فرمانده
         * با هر بار تایپ:
         * - درخواست قبلی فرمانده لغو می‌شود
         * - فقط عدد تا ۹ رقم نگه داشته می‌شود
         * - نام فرمانده پاک می‌شود
         * - پیام اختصاصی فرمانده پاک می‌شود
         */
        $(SELECTORS.commanderNo)
            .on("input", function () {
                abortCommanderRequest();

                this.value = onlyNumber(this.value).substring(0, 9);

                lastCommanderCode = "";
                commanderLoaded = false;

                clearCommanderFields();
                clearCommanderMessage();

                /*
                 * اگر دکمه ثبت به خاطر submit قبلی غیرفعال شده باشد،
                 * با تغییر مقدار فیلد آزاد شود.
                 */
                setFormSubmitting(false);
            })
            .on("blur", function () {
                getCommanderInfo(false);
            })
            .on("keydown", function (e) {
                if (e.key === "Enter") {
                    e.preventDefault();
                    getCommanderInfo(true);
                }
            });

        /*
         * رویداد تغییر فایل پیوست درخواست
         * فرمت‌های مجاز:
         * jpg, jpeg, png, pdf, doc, docx, rar
         * حداکثر حجم: ۲ مگابایت
         */
        $(SELECTORS.attachment).on("change", function () {
            if (validateFile(this, ["jpg", "jpeg", "png", "pdf", "doc", "docx", "rar"], 2 * 1024 * 1024)) {
                previewAttachmentFile(this, SELECTORS.attachmentPreviewContainer, false);
            } else {
                clearFileInput(this, SELECTORS.attachmentPreviewContainer);
            }

            setFormSubmitting(false);
        });

        /*
         * رویداد تغییر فایل فیش حقوقی
         * فرمت‌های مجاز:
         * jpg, jpeg, png, pdf
         * حداکثر حجم: ۲ مگابایت
         */
        $(SELECTORS.fishAttachment).on("change", function () {
            if (validateFile(this, ["jpg", "jpeg", "png", "pdf"], 2 * 1024 * 1024)) {
                previewAttachmentFile(this, SELECTORS.fishPreviewContainer, true);
            } else {
                clearFileInput(this, SELECTORS.fishPreviewContainer);
            }

            setFormSubmitting(false);
        });

        /*
         * رویداد تغییر عکس پرسنلی
         * فقط عکس‌های jpg, jpeg, png مجاز هستند.
         */
        $(SELECTORS.personalAvatarInput).on("change", function () {
            if (validateFile(this, ["jpg", "jpeg", "png"], 2 * 1024 * 1024)) {
                previewImage(this, SELECTORS.avatar);
            }

            setFormSubmitting(false);
        });

        /*
         * کنترل ورودی تلفن همراه
         * فقط عدد قبول می‌کند و حداکثر ۱۱ رقم نگه می‌دارد.
         */
        $(SELECTORS.phone).on("input", function () {
            this.value = onlyNumber(this.value).substring(0, 11);
            setFormSubmitting(false);
        });

        /*
         * کنترل تعداد وام
         * فقط عدد قبول می‌کند.
         */
        $(SELECTORS.countVam).on("input", function () {
            this.value = onlyNumber(this.value);
            setFormSubmitting(false);
        });

        /*
         * وقتی کاربر داخل ورودی‌های مالی تایپ می‌کند،
         * اگر دکمه ثبت قبلاً غیرفعال شده باشد، دوباره آزاد شود.
         */
        $(SELECTORS.totalMoney + ", " + SELECTORS.reciveMoney + ", " + SELECTORS.sumAghsat).on("input", function () {
            setFormSubmitting(false);
        });

        /*
         * وقتی کاربر یکی از selectها یا textareaها را تغییر می‌دهد،
         * اگر دکمه ثبت به خاطر تلاش قبلی غیرفعال شده باشد، دوباره آزاد شود.
         */
        $(SELECTORS.form).on("change input", "select, textarea", function () {
            setFormSubmitting(false);
        });

        /*
         * submit نهایی فرم
         * ترتیب درست:
         * ۱. اول مقادیر مالی sync می‌شوند.
         * ۲. وضعیت اطلاعات پرسنل و فرمانده از روی فیلدهای فعلی بررسی می‌شود.
         * ۳. اول ولیدیشن خود jQuery Validate اجرا می‌شود.
         * ۴. بعد ولیدیشن اختصاصی خودمان اجرا می‌شود.
         * ۵. فقط اگر همه چیز درست بود، دکمه ثبت غیرفعال می‌شود.
         */
        $(SELECTORS.form).on("submit", function (e) {
            syncMoneyBeforeSubmit();
            syncLoadedStateFromCurrentFields();

            /*
             * بررسی اعتبارسنجی jQuery Validate
             * اگر فیلدی مثل required خالی باشد،
             * فرم ارسال نمی‌شود و دکمه ثبت هم نباید غیرفعال بماند.
             */
            if ($.validator && !$(this).valid()) {
                e.preventDefault();

                setFormSubmitting(false);
                refreshSubmitButtonState();

                return false;
            }

            /*
             * بررسی اعتبارسنجی اختصاصی خودمان:
             * - پرسنل باید از AJAX دریافت شده باشد
             * - فرمانده باید از AJAX دریافت شده باشد
             * - موبایل معتبر باشد
             * - فیش حقوقی انتخاب شده باشد
             */
            if (!validateBeforeSubmit()) {
                e.preventDefault();

                setFormSubmitting(false);
                refreshSubmitButtonState();

                return false;
            }

            /*
             * فقط وقتی فرم واقعاً معتبر است،
             * دکمه ثبت غیرفعال می‌شود تا کاربر چند بار پشت سر هم ثبت نزند.
             */
            setFormSubmitting(true);
            refreshSubmitButtonState();

            return true;
        });

        /*
         * اگر jQuery Validate فرم را نامعتبر تشخیص داد،
         * دکمه ثبت دوباره فعال شود.
         * این قسمت جلوی گیر کردن دکمه در حالت disabled را می‌گیرد.
         */
        $(SELECTORS.form).on("invalid-form.validate", function () {
            setFormSubmitting(false);
            refreshSubmitButtonState();
        });
    }

    /*
     * اگر صفحه بعد از برگشت از سرور دوباره رندر شده باشد،
     * ممکن است بعضی فیلدها مقدار داشته باشند.
     * این متد وضعیت loaded را بر اساس مقادیر موجود در صفحه تنظیم می‌کند.
     */
    function restoreLoadedStateIfServerRenderedValuesExist() {
        syncLoadedStateFromCurrentFields();
    }

    /*
     * اگر کد پرسنلی، نام و نام خانوادگی مقدار داشته باشند،
     * یعنی اطلاعات پرسنلی معتبر در صفحه وجود دارد.
     * اگر کد فرمانده و نام فرمانده مقدار داشته باشند،
     * یعنی اطلاعات فرمانده معتبر در صفحه وجود دارد.
     */
    function syncLoadedStateFromCurrentFields() {
        const personalCode = onlyNumber($(SELECTORS.personalNo).val());
        const commanderCode = onlyNumber($(SELECTORS.commanderNo).val());

        if (personalCode && $("#fname").val() && $("#lname").val()) {
            personalLoaded = true;
            lastPersonalCode = personalCode;
        }

        if (commanderCode && $("#farmandehname").val()) {
            commanderLoaded = true;
            lastCommanderCode = commanderCode;
        }
    }

    // =======================================
    // AJAX: Personal
    // =======================================

    /*
     * دریافت اطلاعات پرسنلی بر اساس کد پرسنلی.
     * این متد هنگام blur فیلد کد پرسنلی یا زدن Enter اجرا می‌شود.
     * پیام‌های این بخش فقط زیر فیلد کد پرسنلی نمایش داده می‌شود،
     * نه داخل alert عمومی.
     */
    function getPersonalInfo(force) {
        const code = onlyNumber($(SELECTORS.personalNo).val());

        $(SELECTORS.personalNo).val(code);

        if (!code) {
            abortPersonalRequest();
            clearPersonalFields(true);
            clearPersonalMessage();
            return;
        }

        if (code.length > 9) {
            abortPersonalRequest();
            clearPersonalFields(true);
            showPersonalMessage("کد پرسنلی حداکثر باید ۹ رقم باشد.", "danger");
            return;
        }

        if (!force && code === lastPersonalCode && personalLoaded) {
            return;
        }

        lastPersonalCode = code;
        personalLoaded = false;

        abortPersonalRequest();

        clearPersonalFields(false);
        clearPersonalMessage();
        showPersonalMessage("در حال دریافت اطلاعات پرسنلی...", "info");
        setPersonalLoading(true);

        const requestCode = code;

        const request = $.getJSON(buildUrl(state.getPersonalUrl, "personalno", requestCode));
        personalRequest = request;

        request
            .done(function (res) {
                if (onlyNumber($(SELECTORS.personalNo).val()) !== requestCode) {
                    return;
                }

                const response = normalizeApiResponse(res);

                if (!response.success || !response.data) {
                    clearPersonalFields(true);
                    showPersonalMessage(
                        getPersonalErrorMessage(response.message),
                        "danger"
                    );
                    return;
                }

                fillPersonalFields(response.data);
                personalLoaded = true;
                showPersonalMessage("اطلاعات پرسنلی با موفقیت دریافت شد.", "success");
            })
            .fail(function (xhr, status) {
                if (status === "abort") {
                    return;
                }

                if (onlyNumber($(SELECTORS.personalNo).val()) !== requestCode) {
                    return;
                }

                clearPersonalFields(true);
                showPersonalMessage("خطا در ارتباط با سرور هنگام دریافت اطلاعات پرسنلی.", "danger");
            })
            .always(function () {
                if (personalRequest === request) {
                    personalRequest = null;
                    setPersonalLoading(false);
                }
            });
    }

    /*
     * اطلاعات برگشتی از سرور را داخل فیلدهای پرسنلی قرار می‌دهد.
     * این متد هم نام‌های camelCase و هم PascalCase را پشتیبانی می‌کند،
     * چون ممکن است خروجی JSON پروژه در محیط‌های مختلف متفاوت باشد.
     */
    function fillPersonalFields(data) {
        $("#fname").val(getDataValue(data, "firstName", "FirstName"));
        $("#lname").val(getDataValue(data, "lastName", "LastName"));
        $("#codemeli").val(getDataValue(data, "melliCode", "MelliCode"));
        $("#darjeh").val(getDataValue(data, "rankTitle", "RankTitle"));
        $("#branchtitle").val(getDataValue(data, "branchTitle", "BranchTitle"));
        $("#yeganekhedmati").val(getDataValue(data, "unitDutyTitle", "UnitDutyTitle"));
        $("#yeganeomdeh").val(getDataValue(data, "unitTitle", "UnitTitle"));
        $("#address").val(getDataValue(data, "addres", "Addres", "address", "Address"));

        const totAml2 = getDataValue(data, "totAml2", "tOT_AML2", "toT_AML2", "TOT_AML2");
        const totAml = getDataValue(data, "totAml", "tOT_AML", "toT_AML", "TOT_AML");
        const drsadJa = getDataValue(data, "drsadJa", "dRSAD_JA", "drsaD_JA", "DRSAD_JA");
        const drsadJb = getDataValue(data, "drsadJb", "dRSAD_JB", "drsaD_JB", "DRSAD_JB");

        $("#amaliatikhedmate").val(totAml2);
        $("#ghableghatname").val(totAml);
        $("#janbaziartesh").val(drsadJa);
        $("#janbazibonyad").val(drsadJb);
        $("#isarstatus").val(calculateIsarStatus(totAml2, totAml, drsadJa, drsadJb));

        const avatarName = getDataValue(data, "personalAvatarName", "PersonalAvatarName") || "Default.png";
        $(SELECTORS.avatar).attr("src", state.avatarBaseUrl + avatarName);
    }

    /*
     * فیلدهای اطلاعات پرسنلی را پاک می‌کند.
     * اگر clearAvatar مقدار true باشد، عکس هم به عکس پیش‌فرض برمی‌گردد.
     */
    function clearPersonalFields(clearAvatar) {
        personalLoaded = false;

        $("#fname").val("");
        $("#lname").val("");
        $("#codemeli").val("");
        $("#darjeh").val("");
        $("#branchtitle").val("");
        $("#yگانekhedmati").val("");
        $("#yeganeomdeh").val("");
        $("#address").val("");
        $("#amaliatikhedmate").val("");
        $("#ghableghatname").val("");
        $("#janbaziartesh").val("");
        $("#janbazibonyad").val("");
        $("#isarstatus").val("");

        if (clearAvatar) {
            $(SELECTORS.avatar).attr("src", state.defaultAvatarUrl);
        }
    }

    // =======================================
    // AJAX: Commander
    // =======================================

    /*
     * دریافت اطلاعات فرمانده بر اساس کد پرسنلی فرمانده.
     * پیام‌های این متد فقط زیر فیلد کد فرمانده نمایش داده می‌شود.
     * بنابراین دیگر با پیام کد پرسنلی عادی قاطی نمی‌شود.
     */
    function getCommanderInfo(force) {
        const code = onlyNumber($(SELECTORS.commanderNo).val());

        $(SELECTORS.commanderNo).val(code);

        if (!code) {
            abortCommanderRequest();
            clearCommanderFields();
            clearCommanderMessage();
            return;
        }

        if (code.length > 9) {
            abortCommanderRequest();
            clearCommanderFields();
            showCommanderMessage("کد پرسنلی فرمانده حداکثر باید ۹ رقم باشد.", "danger");
            return;
        }

        if (!force && code === lastCommanderCode && commanderLoaded) {
            return;
        }

        lastCommanderCode = code;
        commanderLoaded = false;

        abortCommanderRequest();

        clearCommanderFields();
        clearCommanderMessage();
        showCommanderMessage("در حال دریافت اطلاعات فرمانده...", "info");
        setCommanderLoading(true);

        const requestCode = code;

        const request = $.getJSON(buildUrl(state.getCommanderUrl, "personalno", requestCode));
        commanderRequest = request;

        request
            .done(function (res) {
                if (onlyNumber($(SELECTORS.commanderNo).val()) !== requestCode) {
                    return;
                }

                const response = normalizeApiResponse(res);

                if (!response.success || !response.data) {
                    clearCommanderFields();
                    showCommanderMessage(
                        getCommanderErrorMessage(response.message),
                        "danger"
                    );
                    return;
                }

                fillCommanderFields(response.data);
                commanderLoaded = true;
                showCommanderMessage("اطلاعات فرمانده با موفقیت دریافت شد.", "success");
            })
            .fail(function (xhr, status) {
                if (status === "abort") {
                    return;
                }

                if (onlyNumber($(SELECTORS.commanderNo).val()) !== requestCode) {
                    return;
                }

                clearCommanderFields();
                showCommanderMessage("خطا در ارتباط با سرور هنگام دریافت اطلاعات فرمانده.", "danger");
            })
            .always(function () {
                if (commanderRequest === request) {
                    commanderRequest = null;
                    setCommanderLoading(false);
                }
            });
    }

    /*
     * اطلاعات فرمانده را داخل فیلد نام فرمانده قرار می‌دهد.
     * نام فرمانده از ترکیب درجه، نام و نام خانوادگی ساخته می‌شود.
     */
    function fillCommanderFields(data) {
        const rankTitle = getDataValue(data, "rankTitle", "RankTitle");
        const firstName = getDataValue(data, "firstName", "FirstName");
        const lastName = getDataValue(data, "lastName", "LastName");

        const fullName = `${rankTitle} ${firstName} ${lastName}`.trim();

        if (!fullName) {
            clearCommanderFields();
            showCommanderMessage("اطلاعات فرمانده ناقص است.", "danger");
            return;
        }

        $("#farmandehname").val(fullName);
    }

    /*
     * فقط اطلاعات فرمانده را پاک می‌کند.
     * این متد به فیلدهای پرسنل کاری ندارد.
     */
    function clearCommanderFields() {
        commanderLoaded = false;
        $("#farmandehname").val("");
    }

    // =======================================
    // Validation
    // =======================================

    /*
     * اعتبارسنجی نهایی قبل از submit فرم.
     * اول مطمئن می‌شود اطلاعات پرسنل دریافت شده.
     * بعد مطمئن می‌شود اطلاعات فرمانده دریافت شده.
     * بعد شماره همراه و فایل فیش حقوقی را بررسی می‌کند.
     */
    function validateBeforeSubmit() {
        clearPersonalMessage();
        clearCommanderMessage();

        if (!personalLoaded) {
            showPersonalMessage("لطفاً ابتدا کد پرسنلی معتبر وارد کنید تا اطلاعات پرسنل دریافت شود.", "danger");
            focusAndScroll(SELECTORS.personalNo);
            return false;
        }

        if (!commanderLoaded) {
            showCommanderMessage("لطفاً ابتدا کد پرسنلی فرمانده معتبر وارد کنید تا نام فرمانده دریافت شود.", "danger");
            focusAndScroll(SELECTORS.commanderNo);
            return false;
        }

        const phone = $(SELECTORS.phone).val();

        if (!/^09\d{9}$/.test(phone)) {
            showAlert("شماره همراه معتبر نیست. مثال صحیح: 09123456789", "danger");
            focusAndScroll(SELECTORS.phone);
            return false;
        }

        const fishInput = $(SELECTORS.fishAttachment)[0];
        const fishFile = fishInput && fishInput.files && fishInput.files.length > 0
            ? fishInput.files[0]
            : null;

        if (!fishFile) {
            showAlert("آپلود فایل فیش حقوقی الزامی است.", "danger");
            focusAndScroll(SELECTORS.fishAttachment);
            return false;
        }

        return true;
    }

    /*
     * اعتبارسنجی فایل انتخاب‌شده.
     * پسوند فایل و حجم فایل را بررسی می‌کند.
     * اگر فایل نامعتبر باشد، input فایل پاک می‌شود.
     */
    function validateFile(input, extensions, maxSize) {
        const file = input.files[0];

        if (!file) {
            return true;
        }

        const ext = file.name.split(".").pop().toLowerCase();

        if (extensions && !extensions.includes(ext)) {
            showAlert("فرمت فایل نامعتبر است.", "danger");
            input.value = "";
            return false;
        }

        if (file.size > maxSize) {
            showAlert("حجم فایل بیشتر از ۲ مگابایت است.", "danger");
            input.value = "";
            return false;
        }

        return true;
    }

    // =======================================
    // Money Inputs
    // =======================================

    /*
     * ورودی‌های مالی صفحه را آماده می‌کند.
     * هر ورودی مالی یک input نمایشی و یک input hidden دارد.
     * input نمایشی عدد را با کاما نشان می‌دهد،
     * hidden مقدار خام عددی را برای ارسال به سرور نگه می‌دارد.
     */
    function initMoneyInputs() {
        setupMoneyInput(SELECTORS.totalMoney, SELECTORS.totalMoneyHidden);
        setupMoneyInput(SELECTORS.reciveMoney, SELECTORS.reciveMoneyHidden);
        setupMoneyInput(SELECTORS.sumAghsat, SELECTORS.sumAghsatHidden);
    }

    /*
     * یک ورودی مالی را تنظیم می‌کند.
     * اگر مقدار hidden از قبل مقدار داشته باشد، در input نمایشی با فرمت کامادار نمایش داده می‌شود.
     * هنگام تایپ هم مقدار خام داخل hidden ذخیره می‌شود.
     */
    function setupMoneyInput(viewSelector, hiddenSelector) {
        const $view = $(viewSelector);
        const $hidden = $(hiddenSelector);

        const hiddenValue = onlyNumber($hidden.val());

        if (hiddenValue && hiddenValue !== "0") {
            $view.val(formatNumber(hiddenValue));
        }

        $view.on("input", function () {
            const raw = onlyNumber(this.value);

            $hidden.val(raw);
            this.value = raw ? formatNumber(raw) : "";
        });
    }

    /*
     * قبل از submit، مقدار همه inputهای مالی را با hiddenهایشان sync می‌کند.
     * این باعث می‌شود سمت سرور عدد خام و بدون کاما دریافت شود.
     */
    function syncMoneyBeforeSubmit() {
        syncMoneyInput(SELECTORS.totalMoney, SELECTORS.totalMoneyHidden);
        syncMoneyInput(SELECTORS.reciveMoney, SELECTORS.reciveMoneyHidden);
        syncMoneyInput(SELECTORS.sumAghsat, SELECTORS.sumAghsatHidden);
    }

    /*
     * یک input مالی مشخص را با hidden خودش هماهنگ می‌کند.
     */
    function syncMoneyInput(viewSelector, hiddenSelector) {
        const raw = onlyNumber($(viewSelector).val());

        $(hiddenSelector).val(raw);
        $(viewSelector).val(raw ? formatNumber(raw) : "");
    }

    /*
     * یک عدد را به صورت سه‌رقمی با کاما فرمت می‌کند.
     * مثال:
     * 1200000 => 1,200,000
     */
    function formatNumber(value) {
        return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }

    // =======================================
    // Preview
    // =======================================

    /*
     * پیش‌نمایش عکس را داخل یک img مشخص نمایش می‌دهد.
     * برای عکس پرسنل استفاده می‌شود.
     */
    function previewImage(input, target) {
        if (!input.files || !input.files[0]) {
            return;
        }

        const reader = new FileReader();

        reader.onload = function (e) {
            $(target).attr("src", e.target.result);
        };

        reader.readAsDataURL(input.files[0]);
    }

    /*
     * برای فایل‌های پیوست، پیش‌نمایش مناسب می‌سازد.
     * اگر فایل عکس باشد، خود عکس را نشان می‌دهد.
     * اگر PDF، Word یا RAR باشد، کارت فایل با آیکون نشان می‌دهد.
     */
    function previewAttachmentFile(input, containerSelector, isRequired) {
        const $container = $(containerSelector);
        $container.empty();

        if (!input.files || !input.files[0]) {
            return;
        }

        const file = input.files[0];
        const fileName = file.name;
        const ext = getFileExtension(fileName);
        const fileSize = formatFileSize(file.size);

        if (["jpg", "jpeg", "png"].includes(ext)) {
            previewImageFile(input, containerSelector, fileName, fileSize, isRequired);
            return;
        }

        if (ext === "pdf") {
            renderFilePreviewCard(input, containerSelector, "فایل PDF انتخاب شد", fileName, fileSize, "fa-file-pdf", isRequired);
            return;
        }

        if (["doc", "docx"].includes(ext)) {
            renderFilePreviewCard(input, containerSelector, "فایل Word انتخاب شد", fileName, fileSize, "fa-file-word", isRequired);
            return;
        }

        if (ext === "rar") {
            renderFilePreviewCard(input, containerSelector, "فایل فشرده انتخاب شد", fileName, fileSize, "fa-file-archive", isRequired);
            return;
        }

        renderFilePreviewCard(input, containerSelector, "فایل انتخاب شد", fileName, fileSize, "fa-file", isRequired);
    }

    /*
     * اگر فایل انتخاب‌شده عکس باشد،
     * عکس را داخل یک کارت کوچک به همراه نام و حجم فایل نمایش می‌دهد.
     */
    function previewImageFile(input, containerSelector, fileName, fileSize, isRequired) {
        const $container = $(containerSelector);
        const reader = new FileReader();

        reader.onload = function (e) {
            $container.html(`
                <div class="attachment-preview-card border rounded p-2 bg-light">
                    <div class="d-flex align-items-start justify-content-between">
                        <div class="d-flex align-items-start">
                            <img src="${e.target.result}"
                                 alt="${escapeHtml(fileName)}"
                                 style="width:90px; height:75px; object-fit:cover; border:1px solid #ddd; border-radius:8px; margin-left:10px;" />

                            <div>
                                <div class="font-weight-bold text-dark">
                                    ${escapeHtml(fileName)}
                                </div>
                                <small class="text-muted">
                                    حجم فایل: ${escapeHtml(fileSize)}
                                </small>
                            </div>
                        </div>

                        <button type="button"
                                class="btn btn-sm btn-danger btn-remove-file"
                                title="حذف فایل انتخاب شده">
                            حذف
                        </button>
                    </div>
                </div>
            `);

            bindRemoveFileButton(input, containerSelector, isRequired);
        };

        reader.readAsDataURL(input.files[0]);
    }

    /*
     * برای فایل‌هایی که عکس نیستند، یک کارت پیش‌نمایش می‌سازد.
     * مثل PDF، Word، RAR یا فایل‌های دیگر.
     */
    function renderFilePreviewCard(input, containerSelector, title, fileName, fileSize, iconClass, isRequired) {
        const $container = $(containerSelector);

        $container.html(`
            <div class="attachment-preview-card border rounded p-2 bg-light">
                <div class="d-flex align-items-center justify-content-between">
                    <div class="d-flex align-items-center">
                        <div style="font-size:34px; margin-left:10px;">
                            <i class="fa ${iconClass}"></i>
                        </div>

                        <div>
                            <div class="font-weight-bold text-dark">
                                ${escapeHtml(title)}
                            </div>
                            <div class="text-muted" style="font-size:13px;">
                                ${escapeHtml(fileName)}
                            </div>
                            <small class="text-muted">
                                حجم فایل: ${escapeHtml(fileSize)}
                            </small>
                        </div>
                    </div>

                    <button type="button"
                            class="btn btn-sm btn-danger btn-remove-file"
                            title="حذف فایل انتخاب شده">
                        حذف
                    </button>
                </div>
            </div>
        `);

        bindRemoveFileButton(input, containerSelector, isRequired);
    }

    /*
     * رویداد دکمه حذف فایل را به کارت پیش‌نمایش وصل می‌کند.
     * اگر فایل الزامی باشد، بعد از حذف فایل پیام هشدار نمایش می‌دهد.
     */
    function bindRemoveFileButton(input, containerSelector, isRequired) {
        $(containerSelector)
            .find(".btn-remove-file")
            .off("click")
            .on("click", function () {
                clearFileInput(input, containerSelector);

                if (isRequired) {
                    showAlert("فایل فیش حقوقی حذف شد. برای ثبت درخواست، انتخاب فیش حقوقی الزامی است.", "warning");
                }
            });
    }

    /*
     * input فایل و container پیش‌نمایش آن را پاک می‌کند.
     */
    function clearFileInput(input, containerSelector) {
        input.value = "";
        $(containerSelector).empty();
    }

    /*
     * پسوند فایل را از روی نام فایل استخراج می‌کند.
     */
    function getFileExtension(fileName) {
        return (fileName || "").split(".").pop().toLowerCase();
    }

    /*
     * حجم فایل را به KB یا MB خوانا تبدیل می‌کند.
     */
    function formatFileSize(size) {
        if (!size || size <= 0) {
            return "0 KB";
        }

        const kb = size / 1024;

        if (kb < 1024) {
            return `${kb.toFixed(1)} KB`;
        }

        return `${(kb / 1024).toFixed(2)} MB`;
    }

    // =======================================
    // Request Control
    // =======================================

    /*
     * اگر درخواست فعال برای دریافت اطلاعات پرسنل وجود داشته باشد، آن را لغو می‌کند.
     * این کار جلوی قاطی شدن جواب درخواست‌های قدیمی با مقدار جدید فیلد را می‌گیرد.
     */
    function abortPersonalRequest() {
        if (personalRequest && personalRequest.readyState !== 4) {
            personalRequest.abort();
        }

        personalRequest = null;
        setPersonalLoading(false);
    }

    /*
     * اگر درخواست فعال برای دریافت اطلاعات فرمانده وجود داشته باشد، آن را لغو می‌کند.
     */
    function abortCommanderRequest() {
        if (commanderRequest && commanderRequest.readyState !== 4) {
            commanderRequest.abort();
        }

        commanderRequest = null;
        setCommanderLoading(false);
    }

    /*
     * وضعیت لودینگ پرسنل را تنظیم می‌کند.
     * هم spinner فیلد پرسنل را کنترل می‌کند و هم وضعیت دکمه ثبت را به‌روزرسانی می‌کند.
     */
    function setPersonalLoading(isLoading) {
        personalLoading = isLoading;
        toggleLoader(SELECTORS.loaderPersonal, isLoading);
        refreshSubmitButtonState();
    }

    /*
     * وضعیت لودینگ فرمانده را تنظیم می‌کند.
     */
    function setCommanderLoading(isLoading) {
        commanderLoading = isLoading;
        toggleLoader(SELECTORS.loaderCommander, isLoading);
        refreshSubmitButtonState();
    }

    /*
     * وقتی فرم در حال ثبت است، دکمه ثبت را غیرفعال نگه می‌دارد.
     */
    function setFormSubmitting(isSubmitting) {
        formSubmitting = isSubmitting;
        refreshSubmitButtonState();
    }

    /*
     * وضعیت دکمه ثبت را بر اساس لودینگ پرسنل، لودینگ فرمانده و submit فرم تنظیم می‌کند.
     */
    function refreshSubmitButtonState() {
        const isDisabled = personalLoading || commanderLoading || formSubmitting;

        $(SELECTORS.submit)
            .prop("disabled", isDisabled)
            .toggleClass("disabled", isDisabled);
    }

    // =======================================
    // Messages
    // =======================================

    /*
     * پیام مخصوص فیلد کد پرسنلی را نمایش می‌دهد.
     */
    function showPersonalMessage(message, type) {
        showFieldMessage(SELECTORS.personalMessage, message, type);
    }

    /*
     * پیام مخصوص فیلد کد فرمانده را نمایش می‌دهد.
     */
    function showCommanderMessage(message, type) {
        showFieldMessage(SELECTORS.commanderMessage, message, type);
    }

    /*
     * پیام فیلد کد پرسنلی را پاک می‌کند.
     */
    function clearPersonalMessage() {
        clearFieldMessage(SELECTORS.personalMessage);
    }

    /*
     * پیام فیلد کد فرمانده را پاک می‌کند.
     */
    function clearCommanderMessage() {
        clearFieldMessage(SELECTORS.commanderMessage);
    }

    /*
     * پیام یک فیلد مشخص را با رنگ مناسب نمایش می‌دهد.
     * type می‌تواند danger، success، warning یا info باشد.
     */
    function showFieldMessage(selector, message, type) {
        let $box = $(selector);

        if (!$box.length) {
            initFieldMessages();
            $box = $(selector);
        }

        const cssClass = getFieldMessageClass(type);

        $box
            .removeClass("d-none text-danger text-success text-warning text-info")
            .addClass(cssClass)
            .text(message || "");
    }

    /*
     * پیام یک فیلد را مخفی و پاک می‌کند.
     */
    function clearFieldMessage(selector) {
        $(selector)
            .addClass("d-none")
            .removeClass("text-danger text-success text-warning text-info")
            .text("");
    }

    /*
     * بر اساس نوع پیام، کلاس Bootstrap مناسب را برمی‌گرداند.
     */
    function getFieldMessageClass(type) {
        if (type === "success") {
            return "text-success";
        }

        if (type === "warning") {
            return "text-warning";
        }

        if (type === "info") {
            return "text-info";
        }

        return "text-danger";
    }

    /*
     * پیام عمومی صفحه را نمایش می‌دهد.
     * این پیام برای خطاهای عمومی مثل موبایل، فایل، حجم فایل و فرمت فایل استفاده می‌شود.
     * پیام پرسنل و فرمانده دیگر از این متد استفاده نمی‌کنند.
     */
    function showAlert(message, type) {
        const alertType = type || "danger";
        const $box = $(SELECTORS.alertBox);

        const $alert = $(`
            <div class="alert alert-${alertType} alert-dismissible fade show" role="alert">
                ${escapeHtml(message)}
                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        `);

        $box.html($alert);

        setTimeout(function () {
            $alert.fadeOut(500, function () {
                $(this).remove();
            });
        }, 4000);
    }

    // =======================================
    // Helpers
    // =======================================

    /*
     * خروجی‌های مختلف سرور را به یک ساختار ثابت تبدیل می‌کند.
     * ساختار نهایی همیشه این شکلی است:
     * {
     *   success: true/false,
     *   message: "...",
     *   data: object/null
     * }
     */
    function normalizeApiResponse(res) {
        if (!res) {
            return {
                success: false,
                message: "پاسخی از سرور دریافت نشد.",
                data: null
            };
        }

        if (typeof res.success !== "undefined") {
            return {
                success: res.success === true,
                message: res.message || "",
                data: res.data || null
            };
        }

        if (res.message) {
            return {
                success: false,
                message: res.message,
                data: null
            };
        }

        return {
            success: true,
            message: "",
            data: res
        };
    }

    /*
     * پیام خطای پرسنل را آماده می‌کند.
     * اگر سرور پیام داده باشد همان پیام نمایش داده می‌شود،
     * وگرنه پیام پیش‌فرض نمایش داده می‌شود.
     */
    function getPersonalErrorMessage(serverMessage) {
        return serverMessage || "پرسنلی با این مشخصات یافت نشد.";
    }

    /*
     * پیام خطای فرمانده را آماده می‌کند.
     * اگر پیام سرور عمومی باشد، آن را به پیام اختصاصی فرمانده تبدیل می‌کند.
     */
    function getCommanderErrorMessage(serverMessage) {
        if (!serverMessage) {
            return "فرمانده‌ای با این کد پرسنلی یافت نشد.";
        }

        if (serverMessage.indexOf("پرسنلی") >= 0 && serverMessage.indexOf("فرمانده") < 0) {
            return "فرمانده‌ای با این کد پرسنلی یافت نشد.";
        }

        return serverMessage;
    }

    /*
     * مقدار یک property را از data می‌گیرد.
     * چند نام مختلف را امتحان می‌کند.
     * برای پشتیبانی همزمان از camelCase و PascalCase استفاده می‌شود.
     */
    function getDataValue(data) {
        for (let i = 1; i < arguments.length; i++) {
            const key = arguments[i];

            if (data && typeof data[key] !== "undefined" && data[key] !== null) {
                return data[key];
            }
        }

        return "";
    }

    /*
     * وضعیت ایثارگری را محاسبه می‌کند.
     * اگر مدت خدمت عملیاتی یا درصد جانبازی مقدار داشته باشد، نتیجه «دارم» می‌شود.
     * در غیر این صورت «ندارم» برمی‌گردد.
     */
    function calculateIsarStatus(totAml2, totAml, drsadJa, drsadJb) {
        const aml2 = normalizeDuration(totAml2);
        const aml = normalizeDuration(totAml);

        const ja = toNumber(drsadJa);
        const jb = toNumber(drsadJb);

        return aml2 || aml || ja > 0 || jb > 0 ? "دارم" : "ندارم";
    }

    /*
     * مقدار مدت خدمت را نرمال می‌کند.
     * مقدارهای خالی یا صفرهای بی‌معنی را خالی در نظر می‌گیرد.
     */
    function normalizeDuration(value) {
        const text = value === null || typeof value === "undefined"
            ? ""
            : value.toString().trim();

        if (
            text === "" ||
            text === "0" ||
            text === "00" ||
            text === "0000" ||
            text === "00000000"
        ) {
            return "";
        }

        return text;
    }

    /*
     * یک مقدار متنی را به عدد تبدیل می‌کند.
     * هر چیزی غیر از عدد و نقطه را حذف می‌کند.
     */
    function toNumber(value) {
        const text = value === null || typeof value === "undefined"
            ? "0"
            : value.toString();

        const number = Number(text.replace(/[^\d.]/g, ""));
        return isNaN(number) ? 0 : number;
    }

    /*
     * فقط اعداد را از مقدار ورودی نگه می‌دارد.
     * برای کد پرسنلی، موبایل، تعداد وام و ورودی‌های مالی استفاده می‌شود.
     */
    function onlyNumber(value) {
        return (value || "").toString().replace(/\D/g, "");
    }

    /*
     * آدرس نهایی AJAX را می‌سازد.
     * اگر baseUrl خودش query string داشته باشد، پارامتر جدید را با & اضافه می‌کند.
     * اگر نداشته باشد، با ? اضافه می‌کند.
     */
    function buildUrl(baseUrl, key, value) {
        const separator = baseUrl.includes("?") ? "&" : "?";
        return `${baseUrl}${separator}${encodeURIComponent(key)}=${encodeURIComponent(value)}`;
    }

    /*
     * spinner یا loader را نمایش یا مخفی می‌کند.
     */
    function toggleLoader(selector, show) {
        $(selector).toggleClass("d-none", !show);
    }

    /*
     * صفحه را به سمت فیلد خطادار اسکرول می‌کند و همان فیلد را focus می‌کند.
     */
    function focusAndScroll(selector) {
        const $element = $(selector);

        if (!$element.length) {
            return;
        }

        $("html, body").animate({
            scrollTop: $element.offset().top - 120
        }, 250);

        $element.focus();
    }

    /*
     * مقدار متنی را escape می‌کند تا داخل HTML امن باشد.
     * برای جلوگیری از تزریق HTML داخل پیش‌نمایش فایل و پیام‌ها استفاده می‌شود.
     */
    function escapeHtml(value) {
        return $("<div>").text(value || "").html();
    }

})(jQuery);
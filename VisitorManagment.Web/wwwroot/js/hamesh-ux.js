(function ($) {
    "use strict";
    if (!$) return;
    var selectors = "#ModalSpecificationPersonal,#ModalShowAttachment,#ModalShowFishAttachment,#ModalSendMain,#ModalSend";
    function message(icon, title, text) {
        if (window.Swal) return Swal.fire({ icon:icon, title:title, text:text, confirmButtonText:"تأیید" });
        window.alert(text);
    }
    $(document).ajaxSend(function (_, __, options) {
        if (!options || !/CreateHamesh|SpecificationPersonal/i.test(options.url || "")) return;
        $(selectors).filter(".show").find(".modal-body").addClass("is-loading");
    });
    $(document).ajaxError(function (_, xhr, options) {
        if (!options || !/CreateHamesh|SpecificationPersonal/i.test(options.url || "")) return;
        var text = xhr.status === 0 ? "ارتباط با سرور برقرار نشد. اتصال شبکه را بررسی و دوباره تلاش کنید." :
            xhr.status === 404 ? "اطلاعات درخواستی پیدا نشد." : "سرور در پاسخ‌گویی با مشکل مواجه شد. لطفاً کمی بعد دوباره تلاش کنید.";
        message("error", "دریافت اطلاعات ناموفق بود", text);
    });
    $(document).ajaxComplete(function () { $(selectors).find(".modal-body").removeClass("is-loading"); });
    $(document).on("shown.bs.modal", selectors, function () {
        var body = $(this).find(".modal-body");
        if (!body.find("table tbody tr,.hamesh-empty-file,a[href]:not([href='']):not([href='#']),input,select,textarea").length)
            body.append('<div class="hamesh-modal-empty"><i class="ti-info-alt"></i><strong>رکوردی یافت نشد</strong><span>اطلاعاتی برای نمایش وجود ندارد.</span></div>');
    });
})(window.jQuery);

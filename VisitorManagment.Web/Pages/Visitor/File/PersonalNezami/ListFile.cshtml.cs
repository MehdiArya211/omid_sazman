using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.User;


namespace VisitorManagment.Web.Pages.Visitor
{
    [Authorize]
    public class ListFileModel : PageModel
    {

        #region Constructor
        // این قسمت سازنده کلاس است و وابستگی‌های مورد نیاز را از طریق تزریق وابستگی (Dependency Injection) دریافت می‌کند.
        private readonly IFileService _fileService;
        private readonly IWorkFlowService _workFlowService;
        private readonly ICartableService _cartableService;
        private readonly IHameshService _hameshService;
        private readonly IWebApiService _webApiService;

        public ListFileModel(
            IFileService fileService,
            IWorkFlowService workFlowService,
            ICartableService cartableService,
            IHameshService hameshService,
            IWebApiService webApiService)
        {
            _fileService = fileService;
            _workFlowService = workFlowService;
            _cartableService = cartableService;
            _hameshService = hameshService;
            _webApiService = webApiService;
        }
        #endregion

        [BindProperty]
        // مدل نمایشی برای لیست فایل‌ها
        public ListFileViewModel ListViewModel { get; set; }
        // لیستی از کاربران که در اینجا استفاده می‌شود.
        public List<Users> Users { get; set; } = new List<Users>();
        // مدل نمایشی برای هامش‌ها (نظرات)
        public ListHameshViewModel ListHameshViewModel { get; set; } = new ListHameshViewModel();

        // متد GET برای بارگذاری صفحه و مقداردهی اولیه فیلترها و داده‌های نمایشی
        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet(int pageId = 1, string filterCaption = "", int requestSubject = 0, int filterAvamerSadereh = 0, string filterGharargah = "")
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);

            // پر کردن ViewData برای فیلترها و داده‌های دیگر
            PopulateViewData(userId);

            // دریافت لیست فایل‌ها بر اساس فیلترهای ورودی
            ListViewModel = _fileService.GetListFile(roleTypeId, userId, pageId, requestSubject, filterAvamerSadereh, filterGharargah, filterCaption);
            ViewData["FileCount"] = ListViewModel.files.Count;
        }

        // متد برای دریافت اطلاعات فایل به صورت JSON

        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public JsonResult OnGetGetFileInfo(int fileid)
        {
            var file = _fileService.GetFile(fileid);

            return new JsonResult(file);
        }

        // دریافت هامش‌ها بر اساس شناسه فایل و بازگشت نتیجه به صورت PartialView
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetHamesh(int fileId)
        {
            ListHameshViewModel = _hameshService.GetHameshIdByFileId(fileId);

            return new PartialViewResult
            {
                ViewName = "_GardeshDarkhast",
                ViewData = new ViewDataDictionary<ListHameshViewModel>(ViewData, ListHameshViewModel)
            };
        }

        // بررسی امکان نوشتن هامش و بازگشت نتیجه به صورت JSON
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public JsonResult OnGetCheckWriteHamesh(int fileId) => new JsonResult(_fileService.GetFile(fileId).Id);

        // جستجوی خودکار برای پیشنهاد نام‌ها و بازگشت نتیجه به صورت JSON
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public JsonResult OnGetSearch(string term) => new JsonResult(_fileService.GetFileForAutoCompliteSearch(term));

        // متد POST برای صفحه‌بندی و بروزرسانی لیست فایل‌ها با فیلترهای جدید
        /// <summary>
        /// درخواست ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPostPagination(int currentPage = 1, string filterCaption = "", int requestSubject = 0, int filterAvamerSadereh = 0, string filterGharargah = "")
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);

            PopulateViewData(userId);

            ListViewModel = _fileService.GetListFile(roleTypeId, userId, currentPage, requestSubject, filterAvamerSadereh, filterGharargah, filterCaption);
            return Page();
        }

        // متد برای آرشیو کردن یک فایل و بازگشت به لیست فایل‌ها
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetArchive1(int fileId)
        {
            return BadRequest("بایگانی فقط از مسیر تأییدشده و امن قابل انجام است.");
        }

        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        [ValidateAntiForgeryToken]
        public IActionResult OnPostArchive(int fileId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("Id").Value);
                _fileService.ArchivedFile(fileId, userId);

                return new JsonResult(new { success = true, message = "فایل با موفقیت آرشیو شد." });
            }
            catch (Exception ex)
            {
                // لاگ کردن خطا در صورت نیاز
                return new JsonResult(new { success = false, message = "خطا در آرشیو فایل. لطفاً دوباره تلاش کنید." });
            }
        }


        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        private void PopulateViewData(int userId)
        {
            ViewData["RequestSubject"] = new SelectList(_fileService.GetRequestSubject(), "Id", "Title");
            ViewData["GharargahList"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["PersonalCodeUserLogined"] = User.FindFirst("UserName").Value;
            ViewData["ListAvamerSadereh"] = new SelectList(_fileService.GetAvamerSadereh(), "Id", "Title");
            ViewData["RoleTypeId"] = _hameshService.GetRoleTypePerson(userId).RoleTypeId;
        }


   
    }
}

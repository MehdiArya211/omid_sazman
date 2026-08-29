using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.VisitorManagment;
using VisitorManagment.Web.Helpers;

namespace VisitorManagment.Web.Pages.Visitor.File.PersonalNezami
{
    [Authorize]
    public class CreateFileModel : PageModel
    {
        #region فیلدها و سازنده

        private readonly IFileService _fileService;
        private readonly IWebApiService _webApiService;
        private readonly IHameshService _hameshService;
        private readonly IPersonService _personService;

        public CreateFileModel(
            IFileService fileService,
            IWebApiService webApiService,
            IHameshService hameshService,
            IPersonService personService)
        {
            _fileService = fileService;
            _webApiService = webApiService;
            _hameshService = hameshService;
            _personService = personService;
        }

        #endregion

        #region پراپرتی‌ها

        [BindProperty]
        public FactPersonalViewModel factPersonalViewModel { get; set; }

        public SMSInfoViewModel smsInfoViewModel { get; set; }

        #endregion

        #region نمایش صفحه

        /// <summary>
        /// بارگذاری اولیه صفحه ثبت درخواست ملاقات
        /// </summary>
        public void OnGet()
        {
            LoadDropdowns();

            factPersonalViewModel = new FactPersonalViewModel();

            ViewData["successcreate"] = false;
        }

        #endregion

        #region ثبت درخواست ملاقات

        /// <summary>
        /// ثبت نهایی درخواست ملاقات، ثبت پرسنل، فایل، کارتابل و هامش
        /// </summary>
        public IActionResult OnPost()
        {
            LoadDropdowns();

            if (factPersonalViewModel == null)
            {
                AddModelError("اطلاعات فرم ارسال نشده است. لطفاً دوباره تلاش کنید.");
                ViewData["successcreate"] = false;
                return Page();
            }

            /*
             * نرمال‌سازی مقادیر مالی قبل از اعتبارسنجی.
             * چون مقادیر مالی ممکن است با کاما یا مقدار خالی ارسال شوند.
             */
            NormalizeMoneyFields();

            /*
             * بعد از تغییر مقدارهای مدل، ModelState قبلی باید پاک شود
             * و مدل دوباره اعتبارسنجی شود تا پیام‌های فارسی Required درست نمایش داده شوند.
             */
            ReValidateFactPersonalViewModel();

            if (!ModelState.IsValid)
            {
                AddModelError("لطفاً خطاهای فرم را بررسی و اصلاح کنید.");
                ViewData["successcreate"] = false;
                return Page();
            }

            if (!TryGetUserClaimInt("Id", out var userId) ||
                !TryGetUserClaimInt("RoleTypeId", out var roleTypeId) ||
                !TryGetUserClaimInt("RoleTypeIdFinal", out var roleTypeIdFinal))
            {
                AddModelError("اطلاعات کاربر لاگین شده ناقص است. لطفاً دوباره وارد سامانه شوید.");
                ViewData["successcreate"] = false;
                return Page();
            }

            var roleTypeTitle = User.FindFirst("RoleTypeTitle")?.Value ?? "";
            var roleTypeTitleFinal = User.FindFirst("RoleTypeTitleFinal")?.Value ?? "";

            /*
             * دریافت اطلاعات پرسنل اصلی از سرویس
             */
            var apiResult = _webApiService.GetPersonalByPersonalNo(factPersonalViewModel.PersonalCode);

            if (apiResult == null || apiResult.IsSuccess == false || apiResult.Data == null)
            {
                AddModelError(apiResult?.Message ?? "اطلاعات پرسنلی از سرویس دریافت نشد.");
                ViewData["successcreate"] = false;
                return Page();
            }

            /*
             * بررسی و دریافت اطلاعات فرمانده
             */
            if (!factPersonalViewModel.FPersonalCode.HasValue ||
                factPersonalViewModel.FPersonalCode.Value <= 0)
            {
                AddModelError("کد پرسنلی فرمانده معتبر نیست.");
                ViewData["successcreate"] = false;
                return Page();
            }

            var commanderPersonalCode = factPersonalViewModel.FPersonalCode.Value.ToString();

            var commanderResult = _webApiService.GetPersonalByPersonalNo(commanderPersonalCode);

            if (commanderResult == null ||
                commanderResult.IsSuccess == false ||
                commanderResult.Data == null)
            {
                AddModelError(commanderResult?.Message ?? "اطلاعات فرمانده از سرویس دریافت نشد.");
                ViewData["successcreate"] = false;
                return Page();
            }

            factPersonalViewModel.FPersonalName =
                $"{commanderResult.Data.RankTitle} {commanderResult.Data.FirstName} {commanderResult.Data.LastName}".Trim();

            factPersonalViewModel.AddUserId = userId;

            /*
             * افزودن یا به‌روزرسانی اطلاعات پرسنلی در دیتابیس داخلی
             */
            var personalId = _fileService.AddPersonToPersonal(factPersonalViewModel);

            if (!HandleServiceError(
                    personalId,
                    "خطا در افزودن یا به‌روزرسانی اطلاعات پرسنلی. لطفاً دوباره تلاش کنید."))
            {
                ViewData["successcreate"] = false;
                return Page();
            }

            factPersonalViewModel.PersonalId = personalId;

            /*
             * ذخیره عکس پرسنلی در صورت انتخاب کاربر
             */
            if (factPersonalViewModel.PersonalAvatar != null &&
                factPersonalViewModel.PersonalAvatar.Length > 0)
            {
                _fileService.ChangePicturePersonelWhenCreateFile(
                    factPersonalViewModel.PersonalAvatar,
                    personalId
                );
            }

            /*
             * ثبت فایل، افزودن به کارتابل و ثبت هامش
             */
            var result = _hameshService.RegFileAndAddToCartableAndRegHamesh(
                factPersonalViewModel,
                userId,
                roleTypeId,
                roleTypeTitle,
                roleTypeIdFinal,
                roleTypeTitleFinal
            );

            if (result == null)
            {
                AddModelError("پاسخی از سرویس ثبت درخواست دریافت نشد.");
                ViewData["successcreate"] = false;
                return Page();
            }

            if (result.Status)
            {
                ViewData["successcreate"] = true;
                return Page();
            }

            AddModelError(result.Message ?? "ثبت درخواست با خطا مواجه شد.");
            ViewData["successcreate"] = false;

            return Page();
        }

        #endregion

        #region دریافت اطلاعات پرسنلی

        /// <summary>
        /// دریافت اطلاعات پرسنل با کد پرسنلی
        /// </summary>
        public JsonResult OnGetGetPersonalId(string personalno)
        {
            if (string.IsNullOrWhiteSpace(personalno) ||
                !Regex.IsMatch(personalno.Trim(), @"^\d{1,9}$"))
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "کد پرسنلی نامعتبر است. فقط عدد تا ۹ رقم مجاز می‌باشد."
                });
            }

            personalno = personalno.Trim();

            var result = _webApiService.GetPersonalByPersonalNo(personalno);

            if (result == null || result.IsSuccess == false || result.Data == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = result?.Message ?? "پرسنلی با این مشخصات یافت نشد."
                });
            }

            var roleIdClaim = User.FindFirst("RoleId")?.Value;
            var username = User.FindFirst("UserName")?.Value;

            int.TryParse(roleIdClaim, out var roleId);

            /*
             * کاربر عادی فقط مجاز است اطلاعات خودش را ببیند.
             */
            if (roleId == 14 && result.Data.PersonalCode != username)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "شما کاربر عادی می‌باشید و دسترسی به اطلاعات این پرسنل را ندارید."
                });
            }

            var personal = _personService.GetPersonalByPersonalCode(personalno);

            result.Data.PersonalAvatarName = string.IsNullOrWhiteSpace(personal?.PersonalAvatar)
                ? "Default.png"
                : personal.PersonalAvatar;

            SessionHelper.SetObjectAsJson(HttpContext.Session, "result", result.Data);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "unitCode", result.Data.UnitCode);

            return new JsonResult(new
            {
                success = true,
                data = result.Data
            });
        }

        #endregion

        #region دریافت اطلاعات فرمانده

        /// <summary>
        /// دریافت اطلاعات فرمانده یگان با کد پرسنلی
        /// </summary>
        public JsonResult OnGetGetPersonalFarmandehId(string personalno)
        {
            if (string.IsNullOrWhiteSpace(personalno) ||
                !Regex.IsMatch(personalno.Trim(), @"^\d{1,9}$"))
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "کد پرسنلی فرمانده نامعتبر است. فقط عدد تا ۹ رقم مجاز می‌باشد."
                });
            }

            personalno = personalno.Trim();

            var result = _webApiService.GetPersonalByPersonalNo(personalno);

            if (result == null || result.IsSuccess == false || result.Data == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = result?.Message ?? "فرمانده‌ای با این کد پرسنلی یافت نشد."
                });
            }

            SessionHelper.SetObjectAsJson(HttpContext.Session, "result2", result.Data);

            return new JsonResult(new
            {
                success = true,
                data = result.Data
            });
        }

        #endregion

        #region بارگذاری اطلاعات کمبوباکس‌ها

        /// <summary>
        /// بارگذاری لیست‌های مورد نیاز صفحه
        /// </summary>
        private void LoadDropdowns()
        {
            /*
             * موضوع درخواست
             */
            var requestSubject = _fileService.GetRequestSubject()
                ?? new List<RequestSubject>();

            ViewData["RequestSubject"] = new SelectList(
                requestSubject,
                "Id",
                "Title"
            );

            /*
             * نوع درخواست
             */
            var fileTypes = _fileService.GetListFileType()
                ?? new List<FileType>();

            ViewData["FileTypeList"] = new SelectList(
                fileTypes,
                "Id",
                "Title"
            );

            /*
             * وضعیت درخواست
             */
            var fileStatus = _fileService.GetFileStatus()
                ?? new List<FileStatus>();

            ViewData["FileStatus"] = new SelectList(
                fileStatus,
                "Id",
                "Title"
            );

            /*
             * اولویت
             */
            var priority = _fileService.GetPriority()
                ?? new List<Priority>();

            ViewData["Priority"] = new SelectList(
                priority,
                "Id",
                "Title"
            );

            /*
             * اطلاعات کاربر جاری
             */
            ViewData["personalCode"] = User.FindFirst("UserName")?.Value ?? "";

            var roleTypeIdClaim = User.FindFirst("RoleTypeId")?.Value;
            ViewData["roleTypeId"] = int.TryParse(roleTypeIdClaim, out var roleTypeId)
                ? roleTypeId
                : 0;

            /*
             * قرارگاه‌ها
             */
            var gharargah = _webApiService.GetGharargah();

            var ghaList = gharargah?.Data
                ?? new List<OrganViewModelDto>();

            ViewData["GharargahList"] = new SelectList(
                ghaList,
                "Id",
                "Title"
            );
        }

        #endregion

        #region متدهای کمکی Claim

        /// <summary>
        /// دریافت مقدار عددی Claim به صورت ایمن
        /// </summary>
        private bool TryGetUserClaimInt(string claimName, out int value)
        {
            value = 0;

            var claimValue = User.FindFirst(claimName)?.Value;

            return !string.IsNullOrWhiteSpace(claimValue) &&
                   int.TryParse(claimValue, out value);
        }

        #endregion

        #region متدهای کمکی اعتبارسنجی مدل

        /// <summary>
        /// پاک‌سازی و اعتبارسنجی دوباره مدل بعد از نرمال‌سازی مقادیر
        /// </summary>
        private void ReValidateFactPersonalViewModel()
        {
            ModelState.Clear();

            TryValidateModel(
                factPersonalViewModel,
                nameof(factPersonalViewModel)
            );
        }

        #endregion

        #region متدهای کمکی مقادیر مالی

        /// <summary>
        /// نرمال‌سازی مقادیر مالی قبل از ثبت
        /// </summary>
        private void NormalizeMoneyFields()
        {
            if (factPersonalViewModel == null)
            {
                return;
            }

            factPersonalViewModel.TotalMoney =
                ParseLongNullableSafe(factPersonalViewModel.TotalMoney);

            factPersonalViewModel.ReciveMoney =
                ParseLongNullableSafe(factPersonalViewModel.ReciveMoney);

            factPersonalViewModel.SumAghsatVamMahiyaneh =
                ParseLongNullableSafe(factPersonalViewModel.SumAghsatVamMahiyaneh);

            factPersonalViewModel.CountVam =
                ParseIntNullableSafe(factPersonalViewModel.CountVam);
        }

        /// <summary>
        /// تبدیل مقدار به long nullable به صورت ایمن
        /// </summary>
        private long? ParseLongNullableSafe(object value)
        {
            var text = value?.ToString()?.Replace(",", "").Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            return long.TryParse(text, out var result)
                ? result
                : null;
        }

        /// <summary>
        /// تبدیل مقدار به int nullable به صورت ایمن
        /// </summary>
        private int? ParseIntNullableSafe(object value)
        {
            var text = value?.ToString()?.Replace(",", "").Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            return int.TryParse(text, out var result)
                ? result
                : null;
        }

        #endregion

        #region متدهای کمکی خطا

        /// <summary>
        /// افزودن خطا به ModelState
        /// </summary>
        private void AddModelError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "عملیات با خطا مواجه شد.";
            }

            ModelState.AddModelError(string.Empty, message);
        }

        /// <summary>
        /// بررسی نتیجه سرویس و افزودن خطا در صورت ناموفق بودن
        /// </summary>
        private bool HandleServiceError(int serviceResult, string errorMessage)
        {
            if (serviceResult <= 0)
            {
                AddModelError(errorMessage);
                return false;
            }

            return true;
        }

        #endregion
    }
}

//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using VisitorManagment.Core.DTOs;
//using VisitorManagment.Core.Services.Interfaces;
//using VisitorManagment.DataLayer.Entities.VisitorManagment;
//using VisitorManagment.Web.Helpers;

//namespace VisitorManagment.Web.Pages.Visitor.File.PersonalNezami
//{
//    [Authorize]
//    public class CreateFileModel : PageModel
//    {
//        #region فیلدها و سازنده

//        private readonly IFileService _fileService;
//        private readonly IWebApiService _webApiService;
//        private readonly IHameshService _hameshService;
//        private readonly IPersonService _personService;

//        public CreateFileModel(
//            IFileService fileService,
//            IWebApiService webApiService,
//            IHameshService hameshService,
//            IPersonService personService)
//        {
//            _fileService = fileService;
//            _webApiService = webApiService;
//            _hameshService = hameshService;
//            _personService = personService;
//        }

//        #endregion

//        #region پراپرتی‌ها

//        [BindProperty]
//        public FactPersonalViewModel factPersonalViewModel { get; set; }

//        public SMSInfoViewModel smsInfoViewModel { get; set; }

//        #endregion

//        #region نمایش صفحه

//        /// <summary>
//        /// بارگذاری اولیه صفحه ثبت درخواست ملاقات
//        /// </summary>
//        public void OnGet()
//        {
//            LoadDropdowns();
//            factPersonalViewModel = new FactPersonalViewModel();
//        }

//        #endregion

//        #region ثبت درخواست ملاقات

//        /// <summary>
//        /// ثبت نهایی درخواست ملاقات، ثبت پرسنل، فایل، کارتابل و هامش
//        /// </summary>
//        public IActionResult OnPost()
//        {
//            LoadDropdowns();

//            if (!TryGetUserClaimInt("Id", out var userId) ||
//                !TryGetUserClaimInt("RoleTypeId", out var roleTypeId) ||
//                !TryGetUserClaimInt("RoleTypeIdFinal", out var roleTypeIdFinal))
//            {
//                AddModelError("اطلاعات کاربر لاگین شده ناقص است. لطفاً دوباره وارد سامانه شوید.");
//                return Page();
//            }

//            var roleTypeTitle = User.FindFirst("RoleTypeTitle")?.Value ?? "";
//            var roleTypeTitleFinal = User.FindFirst("RoleTypeTitleFinal")?.Value ?? "";

//            NormalizeMoneyFields();

//            if (!ModelState.IsValid)
//            {
//                AddModelError("لطفاً تمامی فیلدها را به درستی پر کنید.");
//                return Page();
//            }

//            var apiResult = _webApiService.GetPersonalByPersonalNo(factPersonalViewModel.PersonalCode);

//            if (apiResult == null || apiResult.IsSuccess == false || apiResult.Data == null)
//            {
//                AddModelError(apiResult?.Message ?? "اطلاعات پرسنلی از سرویس دریافت نشد.");
//                return Page();
//            }

//            if (!factPersonalViewModel.FPersonalCode.HasValue || factPersonalViewModel.FPersonalCode.Value <= 0)
//            {
//                AddModelError("کد پرسنلی فرمانده معتبر نیست.");
//                return Page();
//            }

//            var commanderPersonalCode = factPersonalViewModel.FPersonalCode.Value.ToString();

//            var commanderResult = _webApiService.GetPersonalByPersonalNo(commanderPersonalCode);

//            if (commanderResult == null || commanderResult.IsSuccess == false || commanderResult.Data == null)
//            {
//                AddModelError(commanderResult?.Message ?? "اطلاعات فرمانده از سرویس دریافت نشد.");
//                return Page();
//            }

//            factPersonalViewModel.FPersonalName =
//                $"{commanderResult.Data.RankTitle} {commanderResult.Data.FirstName} {commanderResult.Data.LastName}".Trim();

//            factPersonalViewModel.AddUserId = userId;

//            var personalId = _fileService.AddPersonToPersonal(factPersonalViewModel);

//            if (!HandleServiceError(personalId, "خطا در افزودن یا به‌روزرسانی اطلاعات پرسنلی. لطفاً دوباره تلاش کنید."))
//            {
//                return Page();
//            }

//            factPersonalViewModel.PersonalId = personalId;

//            var result = _hameshService.RegFileAndAddToCartableAndRegHamesh(
//                factPersonalViewModel,
//                userId,
//                roleTypeId,
//                roleTypeTitle,
//                roleTypeIdFinal,
//                roleTypeTitleFinal
//            );

//            if (result.Status)
//            {
//                ViewData["successcreate"] = true;
//                return Page();
//            }

//            AddModelError(result.Message ?? "ثبت درخواست با خطا مواجه شد.");
//            ViewData["successcreate"] = false;

//            return Page();
//        }

//        #endregion

//        #region نسخه قبلی ثبت درخواست

//        /// <summary>
//        /// نسخه قبلی ثبت درخواست ملاقات - جهت نگهداری کد قبلی
//        /// </summary>
//        public IActionResult OnPost0()
//        {
//            LoadDropdowns();

//            int userId = int.Parse(User.FindFirst("Id").Value);
//            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);
//            var roleTypeTitle = User.FindFirst("RoleTypeTitle").Value;
//            var roleTypeIdFinal = int.Parse(User.FindFirst("RoleTypeIdFinal").Value);
//            var roleTypeTitleFinal = User.FindFirst("RoleTypeTitleFinal").Value;

//            // حذف کاما و تبدیل مقادیر مالی به عدد
//            factPersonalViewModel.TotalMoney = string.IsNullOrWhiteSpace(factPersonalViewModel.TotalMoney.ToString())
//                ? 0
//                : long.Parse(factPersonalViewModel.TotalMoney.ToString().Replace(",", ""));

//            factPersonalViewModel.ReciveMoney = string.IsNullOrWhiteSpace(factPersonalViewModel.ReciveMoney.ToString())
//                ? 0
//                : long.Parse(factPersonalViewModel.ReciveMoney.ToString().Replace(",", ""));

//            factPersonalViewModel.CountVam = string.IsNullOrWhiteSpace(factPersonalViewModel.CountVam.ToString())
//                ? 0
//                : int.Parse(factPersonalViewModel.CountVam.ToString().Replace(",", ""));

//            factPersonalViewModel.SumAghsatVamMahiyaneh = string.IsNullOrWhiteSpace(factPersonalViewModel.SumAghsatVamMahiyaneh.ToString())
//                ? 0
//                : long.Parse(factPersonalViewModel.SumAghsatVamMahiyaneh.ToString().Replace(",", ""));

//            if (!ModelState.IsValid)
//            {
//                AddModelError("لطفا تمامی فیلدها را به درستی پر کنید.");
//                return Page();
//            }

//            factPersonalViewModel.AddUserId = userId;

//            int personalId = _fileService.AddPersonToPersonal(factPersonalViewModel);

//            if (!HandleServiceError(personalId, "خطا در افزودن اطلاعات پرسنلی. لطفا دوباره تلاش کنید."))
//            {
//                return Page();
//            }

//            factPersonalViewModel.PersonalId = personalId;

//            if (factPersonalViewModel.PersonalAvatar != null)
//            {
//                _fileService.ChangePicturePersonelWhenCreateFile(factPersonalViewModel.PersonalAvatar, personalId);
//            }

//            var result = _hameshService.RegFileAndAddToCartableAndRegHamesh(
//                factPersonalViewModel,
//                userId,
//                roleTypeId,
//                roleTypeTitle,
//                roleTypeIdFinal,
//                roleTypeTitleFinal
//            );

//            if (result.Status)
//            {
//                ViewData["successcreate"] = true;
//                return Page();
//            }

//            ViewData["successcreate"] = false;

//            return Page();
//        }

//        #endregion

//        #region دریافت اطلاعات پرسنلی

//        /// <summary>
//        /// دریافت اطلاعات پرسنل با کد پرسنلی
//        /// </summary>
//        public JsonResult OnGetGetPersonalId(string personalno)
//        {
//            if (string.IsNullOrWhiteSpace(personalno) || !Regex.IsMatch(personalno.Trim(), @"^\d{1,9}$"))
//            {
//                return new JsonResult(new
//                {
//                    success = false,
//                    message = "کد پرسنلی نامعتبر است. فقط عدد تا ۹ رقم مجاز می‌باشد."
//                });
//            }

//            personalno = personalno.Trim();

//            var result = _webApiService.GetPersonalByPersonalNo(personalno);

//            if (result == null || result.IsSuccess == false || result.Data == null)
//            {
//                return new JsonResult(new
//                {
//                    success = false,
//                    message = result?.Message ?? "پرسنلی با این مشخصات یافت نشد."
//                });
//            }

//            var roleIdClaim = User.FindFirst("RoleId")?.Value;
//            var username = User.FindFirst("UserName")?.Value;

//            int.TryParse(roleIdClaim, out var roleId);

//            // کاربر عادی فقط اطلاعات خودش را ببیند
//            if (roleId == 14 && result.Data.PersonalCode != username)
//            {
//                return new JsonResult(new
//                {
//                    success = false,
//                    message = "شما کاربر عادی می‌باشید و دسترسی به اطلاعات این پرسنل را ندارید."
//                });
//            }

//            var personal = _personService.GetPersonalByPersonalCode(personalno);

//            result.Data.PersonalAvatarName = string.IsNullOrWhiteSpace(personal?.PersonalAvatar)
//                ? "Default.png"
//                : personal.PersonalAvatar;

//            SessionHelper.SetObjectAsJson(HttpContext.Session, "result", result.Data);
//            SessionHelper.SetObjectAsJson(HttpContext.Session, "unitCode", result.Data.UnitCode);

//            return new JsonResult(new
//            {
//                success = true,
//                data = result.Data
//            });
//        }

//        /// <summary>
//        /// نسخه قبلی دریافت اطلاعات پرسنل با کد پرسنلی
//        /// </summary>
//        public JsonResult OnGetGetPersonalId0(string personalno)
//        {
//            if (string.IsNullOrEmpty(personalno) || !Regex.IsMatch(personalno, @"^\d{1,9}$"))
//            {
//                return new JsonResult(new { message = "کد پرسنلی نامعتبر است. فقط عدد تا 9 رقم مجاز می‌باشد." });
//            }

//            var result = _webApiService.GetPersonalByPersonalNo(personalno);

//            if (result == null || result.Data == null || result.IsSuccess == false)
//            {
//                return new JsonResult(new { message = result?.Message ?? "پرسنلی با این مشخصات یافت نشد." });
//            }

//            var unitDutyCodeUserLogin = int.Parse(User.FindFirst("UnitDutyCode").Value);
//            var unitCodeUserLogin = int.Parse(User.FindFirst("UnitCode").Value);

//            var personal = _personService.GetPersonalByPersonalCode(personalno);

//            result.Data.PersonalAvatarName = personal?.PersonalAvatar ?? "Default.png";

//            int roleId = int.Parse(User.FindFirst("RoleId").Value);
//            string username = User.FindFirst("UserName").Value;

//            if (roleId == 14 && result.Data.PersonalCode != username)
//            {
//                return new JsonResult(new { message = "شما کاربر عادی میباشید و دسترسی ندارید!" });
//            }

//            SessionHelper.SetObjectAsJson(HttpContext.Session, "result", result.Data);
//            SessionHelper.SetObjectAsJson(HttpContext.Session, "unitCode", result.Data.UnitCode);

//            return new JsonResult(result.Data);
//        }

//        #endregion

//        #region دریافت اطلاعات فرمانده

//        /// <summary>
//        /// دریافت اطلاعات فرمانده یگان با کد پرسنلی
//        /// </summary>
//        public JsonResult OnGetGetPersonalFarmandehId(string personalno)
//        {
//            if (string.IsNullOrWhiteSpace(personalno) || !Regex.IsMatch(personalno.Trim(), @"^\d{1,9}$"))
//            {
//                return new JsonResult(new
//                {
//                    success = false,
//                    message = "کد پرسنلی فرمانده نامعتبر است. فقط عدد تا ۹ رقم مجاز می‌باشد."
//                });
//            }

//            personalno = personalno.Trim();

//            var result = _webApiService.GetPersonalByPersonalNo(personalno);

//            if (result == null || result.IsSuccess == false || result.Data == null)
//            {
//                return new JsonResult(new
//                {
//                    success = false,
//                    message = result?.Message ?? "فرمانده‌ای با این کد پرسنلی یافت نشد."
//                });
//            }

//            SessionHelper.SetObjectAsJson(HttpContext.Session, "result2", result.Data);

//            return new JsonResult(new
//            {
//                success = true,
//                data = result.Data
//            });
//        }

//        /// <summary>
//        /// نسخه قبلی دریافت اطلاعات فرمانده یگان با کد پرسنلی
//        /// </summary>
//        public JsonResult OnGetGetPersonalFarmandehId0(string personalno)
//        {
//            var result2 = _webApiService.GetPersonalByPersonalNo(personalno);

//            if (result2 == null)
//            {
//                return new JsonResult(new { message = "پرسنلی با این مشخصات یافت نشد" });
//            }

//            SessionHelper.SetObjectAsJson(HttpContext.Session, "result2", result2);

//            return new JsonResult(result2.Data);
//        }

//        #endregion

//        #region بارگذاری اطلاعات کمبوباکس‌ها

//        /// <summary>
//        /// بارگذاری لیست‌های مورد نیاز صفحه
//        /// </summary>
//        private void LoadDropdowns()
//        {
//            // موضوع درخواست
//            var requestSubject = _fileService.GetRequestSubject() ?? new List<RequestSubject>();
//            ViewData["RequestSubject"] = new SelectList(requestSubject, "Id", "Title");

//            // نوع درخواست
//            var fileTypes = _fileService.GetListFileType() ?? new List<FileType>();
//            ViewData["FileTypeList"] = new SelectList(fileTypes, "Id", "Title");

//            // وضعیت درخواست
//            var fileStatus = _fileService.GetFileStatus() ?? new List<FileStatus>();
//            ViewData["FileStatus"] = new SelectList(fileStatus, "Id", "Title");

//            // اولویت
//            var priority = _fileService.GetPriority() ?? new List<Priority>();
//            ViewData["Priority"] = new SelectList(priority, "Id", "Title");

//            // اطلاعات کاربر جاری
//            ViewData["personalCode"] = User.FindFirst("UserName")?.Value ?? "";
//            ViewData["roleTypeId"] = int.Parse(User.FindFirst("RoleTypeId")?.Value ?? "0");

//            // قرارگاه‌ها
//            var gharargah = _webApiService.GetGharargah();
//            var ghaList = gharargah?.Data ?? new List<OrganViewModelDto>();
//            ViewData["GharargahList"] = new SelectList(ghaList, "Id", "Title");
//        }

//        #endregion

//        #region متدهای کمکی Claim

//        /// <summary>
//        /// دریافت مقدار عددی Claim به صورت ایمن
//        /// </summary>
//        private bool TryGetUserClaimInt(string claimName, out int value)
//        {
//            value = 0;

//            var claimValue = User.FindFirst(claimName)?.Value;

//            return !string.IsNullOrWhiteSpace(claimValue) && int.TryParse(claimValue, out value);
//        }

//        #endregion

//        #region متدهای کمکی مقادیر مالی

//        /// <summary>
//        /// نرمال‌سازی مقادیر مالی قبل از ثبت
//        /// </summary>
//        private void NormalizeMoneyFields0()
//        {
//            factPersonalViewModel.TotalMoney = ParseLongSafe(factPersonalViewModel.TotalMoney);
//            factPersonalViewModel.ReciveMoney = ParseLongSafe(factPersonalViewModel.ReciveMoney);
//            factPersonalViewModel.SumAghsatVamMahiyaneh = ParseLongSafe(factPersonalViewModel.SumAghsatVamMahiyaneh);
//            factPersonalViewModel.CountVam = ParseIntSafe(factPersonalViewModel.CountVam);
//        }

//        private void NormalizeMoneyFields()
//        {
//            factPersonalViewModel.TotalMoney = ParseLongNullableSafe(factPersonalViewModel.TotalMoney);
//            factPersonalViewModel.ReciveMoney = ParseLongNullableSafe(factPersonalViewModel.ReciveMoney);
//            factPersonalViewModel.SumAghsatVamMahiyaneh = ParseLongNullableSafe(factPersonalViewModel.SumAghsatVamMahiyaneh);
//            factPersonalViewModel.CountVam = ParseIntNullableSafe(factPersonalViewModel.CountVam);
//        }

//        private long? ParseLongNullableSafe(object value)
//        {
//            var text = value?.ToString()?.Replace(",", "").Trim();

//            if (string.IsNullOrWhiteSpace(text))
//            {
//                return null;
//            }

//            return long.TryParse(text, out var result) ? result : null;
//        }

//        private int? ParseIntNullableSafe(object value)
//        {
//            var text = value?.ToString()?.Replace(",", "").Trim();

//            if (string.IsNullOrWhiteSpace(text))
//            {
//                return null;
//            }

//            return int.TryParse(text, out var result) ? result : null;
//        }

//        /// <summary>
//        /// تبدیل مقدار به long به صورت ایمن
//        /// </summary>
//        private long ParseLongSafe(object value)
//        {
//            var text = value?.ToString()?.Replace(",", "").Trim();

//            if (string.IsNullOrWhiteSpace(text))
//            {
//                return 0;
//            }

//            return long.TryParse(text, out var result) ? result : 0;
//        }

//        /// <summary>
//        /// تبدیل مقدار به int به صورت ایمن
//        /// </summary>
//        private int ParseIntSafe(object value)
//        {
//            var text = value?.ToString()?.Replace(",", "").Trim();

//            if (string.IsNullOrWhiteSpace(text))
//            {
//                return 0;
//            }

//            return int.TryParse(text, out var result) ? result : 0;
//        }

//        #endregion

//        #region متدهای کمکی خطا

//        /// <summary>
//        /// افزودن خطا به ModelState
//        /// </summary>
//        private void AddModelError(string message)
//        {
//            ModelState.AddModelError(string.Empty, message);
//        }

//        /// <summary>
//        /// بررسی نتیجه سرویس و افزودن خطا در صورت ناموفق بودن
//        /// </summary>
//        private bool HandleServiceError(int serviceResult, string errorMessage)
//        {
//            if (serviceResult <= 0)
//            {
//                AddModelError(errorMessage);
//                return false;
//            }

//            return true;
//        }

//        #endregion
//    }
//}
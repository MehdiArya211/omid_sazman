using System;
using System.Collections.Generic;
using System.Linq;
using _0_Framework.Application;
using ITOWebApiClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.User;


namespace VisitorManagment.Web.Pages.Visitor.File.HameshInfo
{
    [Authorize]
    public class CreateHameshModel : PageModel
    {

        #region Ctor
        private readonly IFileService _fileService;
        private readonly IMeetingService _meetingService;
        private readonly IPersonService _personService;
        private readonly IWebApiService _webApiService;
        private readonly ICartableService _cartableService;
        private readonly IUserService _userService;
        private readonly IHameshService _hameshService;
        private readonly IWorkFlowService _workFlowService;
        private readonly ISmsService _smsService;
        private readonly ApiTokenCacheClient _apiTokenClient;
        private readonly IVamService _vamService;
        public CreateHameshModel(IFileService fileService, IWebApiService webApiService, ICartableService cartableService,
            IUserService userService, IHameshService hameshService, IWorkFlowService workFlowService, ISmsService smsService,
            ApiTokenCacheClient apiTokenClient, IPersonService personService, IMeetingService meetingService, IVamService vamService)
        {
            _meetingService = meetingService;
            _fileService = fileService;
            _personService = personService;
            _webApiService = webApiService;
            _cartableService = cartableService;
            _userService = userService;
            _hameshService = hameshService;
            _workFlowService = workFlowService;
            _smsService = smsService;
            _apiTokenClient = apiTokenClient;
            _vamService = vamService;
        }
        #endregion

        #region Property
        [BindProperty]
        public HameshFullInfoFileViewModel hameshFullInfoViewModel { get; set; }
        public SMSInfoViewModel smsInfoViewModel { get; set; }
        public List<Users> usersForSend { get; set; }
        public List<Users> usersForAoudat { get; set; }

        /// <summary>
        /// هامش های گردش درخواست
        /// </summary>
        public ListHameshViewModel listHameshViewModel { get; set; }


        /// <summary>
        ///  خلاصه وضعیت نفر
        /// </summary>
        public CollectionSpecificationPersonalViewModel collectionSpecificationPersonalViewModel { get; set; }

        /// <summary>
        /// لیست جلسات ملاقات
        /// </summary>
        public ListMeetingForHameshFormViewModel listMeetingForHameshFormViewModel { get; set; }
        #endregion

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet(int id)
        {
            hameshFullInfoViewModel = new HameshFullInfoFileViewModel();
            try
            {
                // تنظیم ViewData
                var actionTypes = _hameshService.GetActionType();
                if (actionTypes == null)
                {
                    // مدیریت خطا برای عدم دریافت نوع اقدام
                    ViewData["ActionType"] = new SelectList(Enumerable.Empty<object>(), "Id", "Title");
                }
                else
                {
                    ViewData["ActionType"] = new SelectList(actionTypes.Where(x => x.Code != 60005), "Id", "Title");
                }

                var units = _webApiService.GetAllOrganWithGharagahNezaja().Data;
                if (units == null)
                {
                    // مدیریت خطا برای عدم دریافت واحدها
                    ViewData["ListUnit"] = new SelectList(Enumerable.Empty<object>(), "Id", "Title");
                }
                else
                {
                    ViewData["ListUnit"] = new SelectList(units, "Id", "Title");
                }
                ViewData["fileId"] = id;

                #region مقدار دهی اولیه مدل
                collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel
                {
                    ListFile = new ListFileInfoViewModel(),
                    FactPersonal = new FactSpecificationPersonalViewModel()
                };
                listMeetingForHameshFormViewModel = new ListMeetingForHameshFormViewModel();
                listHameshViewModel = new ListHameshViewModel();
                usersForSend = new List<Users>();
                usersForAoudat = new List<Users>();
                #endregion

                // بازیابی اطلاعات Claims
                var userIdClaim = User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    // مدیریت خطا برای عدم وجود شناسه کاربر
                    throw new InvalidOperationException("کاربر شناسایی نشد.");
                }
                var userId = int.Parse(userIdClaim);

                // بازیابی اطلاعات شخصی
                var file = _fileService.GetFileByFileId(id);
                if (file == null)
                {
                    // مدیریت خطا برای عدم یافتن فایل
                    throw new InvalidOperationException("فایل مورد نظر یافت نشد.");
                }
                var personalId = file.PersonalId;

                var personal = _personService.GetPersonByPersonalId(personalId);
                if (personal == null)
                {
                    // مدیریت خطا برای عدم یافتن شخص
                    throw new InvalidOperationException("شخص مورد نظر یافت نشد.");
                }
                var personalCode = personal.PersonalCode;

                var personalData = _webApiService.GetPersonalByPersonalNo(personalCode)?.Data;
                if (personalData == null)
                {
                    // مدیریت خطا برای عدم دریافت اطلاعات شخص
                    throw new InvalidOperationException("اطلاعات شخص مورد نظر دریافت نشد.");
                }

                ViewData["PersonalCode"] = personalCode;
                collectionSpecificationPersonalViewModel.ListFile = _fileService.GetListFile(personalId);
                collectionSpecificationPersonalViewModel.FactPersonal = _personService.GetPersonalForEdit(personalId);
                collectionSpecificationPersonalViewModel.FactPersonal.HomDat = personalData.HomDat;

                // بازیابی اطلاعات جلسات
                listMeetingForHameshFormViewModel = _meetingService.GetListMeetingForFormHamesh();
                listMeetingForHameshFormViewModel.FileId = id;

                // بازیابی اطلاعات فایل و هامش‌ها
                var fileForEdit = _fileService.GetFileForEdit(id);
                if (fileForEdit == null)
                {
                    // مدیریت خطا برای عدم یافتن فایل برای ویرایش
                    throw new InvalidOperationException("فایل برای ویرایش یافت نشد.");
                }

                ViewData["IsAnswerMoavenat"] = fileForEdit.IsAnswerdMoavenat;
                ViewData["IsArchived"] = fileForEdit.IsArchived;

                hameshFullInfoViewModel = _hameshService.GetFullInfoFile(id, userId);
                if (hameshFullInfoViewModel == null)
                {
                    // مدیریت خطا برای عدم دریافت اطلاعات کامل هامش
                    throw new InvalidOperationException("اطلاعات کامل هامش دریافت نشد.");
                }

                hameshFullInfoViewModel.hameshKarshenasGharagahAnsarNezaja = _hameshService.GetlastHameshKarshenasgharagahAnsarNezaja(id);
                hameshFullInfoViewModel.ListVam = _vamService.getAllVamWithFileId(id);

                // بازیابی لیست هامش‌ها
                listHameshViewModel = _hameshService.GetHameshIdByFileId(id);
                if (listHameshViewModel == null)
                {
                    // مدیریت خطا برای عدم دریافت لیست هامش‌ها
                    listHameshViewModel = new ListHameshViewModel(); // یا مدیریت دیگر
                }

                // نمایش صفحه
                ViewData["ShowPage"] = true;
            }
            catch (Exception ex)
            {
                // مدیریت خطاها
                // برای مثال: ثبت خطا در لاگ، نمایش پیام خطا به کاربر، یا غیره
                ViewData["ErrorMessage"] = "خطایی در پردازش درخواست پیش آمده است.";

            }
        }


        #region ارسال

        #region لیست نفراتی رو که میتونیم در خواست ملاقات رو بهشون ارسال کنیم
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetRecieverUserMain(int FileId, int ActionTypeId, string UserDesc)
        {
            // بازیابی Claims
            var claims = new
            {
                RoleId = int.Parse(User.FindFirst("RoleId")?.Value ?? "0"),
                UnitDutyCode = int.Parse(User.FindFirst("UnitDutyCode")?.Value ?? "0"),
                UnitCode = int.Parse(User.FindFirst("UnitCode")?.Value ?? "0"),
                RoleTypeId = int.Parse(User.FindFirst("RoleTypeId")?.Value ?? "0"),
                CodeGha = int.Parse(User.FindFirst("CodGha")?.Value ?? "0")
            };

            // تنظیم ViewData
            ViewData["actionTypeId"] = ActionTypeId;
            ViewData["userDesc"] = UserDesc;
            ViewData["fileId"] = FileId;

            // نمایش لیست نفراتی که دسترسی دارند
            usersForSend = _workFlowService.GetRecieverUserListBySndrRoleId(
                claims.RoleId,
                claims.UnitDutyCode,
                claims.UnitCode,
                claims.CodeGha,
                FileId,
                claims.RoleTypeId
            );


            return new PartialViewResult
            {
                ViewName = "_GridSendList",
                ViewData = new ViewDataDictionary<List<Users>>(ViewData, usersForSend)
            };

        }
        #endregion


        #region انجام عملیات ارسال
        /// <summary>
        /// درخواست ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPostHameshSendFile(List<int> rcvrUserId, int actionTypeId, string userDesc, int fileId)
        {
            // بازیابی اطلاعات کاربر
            if (!int.TryParse(User.FindFirst("Id")?.Value, out var userId))
            {
                ModelState.AddModelError("", "خطا در دریافت شناسه کاربر");
                return Page();
            }

            // بررسی اعتبار ورودی‌ها
            if (actionTypeId <= 0 || string.IsNullOrWhiteSpace(userDesc) || rcvrUserId == null || !rcvrUserId.Any())
            {
                // مقداردهی اولیه ViewModels و ViewData
                ViewData["fileId"] = fileId;
                hameshFullInfoViewModel = new HameshFullInfoFileViewModel();

                usersForSend = new List<Users>();
                usersForAoudat = _workFlowService.GetRecieverUserListByFileId(fileId, userId);

                listMeetingForHameshFormViewModel = new ListMeetingForHameshFormViewModel
                {
                    FileId = fileId
                };
                listMeetingForHameshFormViewModel = _meetingService.GetListMeetingForFormHamesh();

                collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();
                var personalId = _fileService.GetFileByFileId(fileId)?.PersonalId;
                if (personalId.HasValue)
                {
                    var personalCode = _personService.GetPersonByPersonalId(personalId.Value)?.PersonalCode;
                    if (!string.IsNullOrEmpty(personalCode))
                    {
                        collectionSpecificationPersonalViewModel.ListFile = _fileService.GetListFile(personalId.Value);
                        collectionSpecificationPersonalViewModel.FactPersonal = _personService.GetPersonalForEdit(personalId.Value);
                    }
                    else
                    {
                        ModelState.AddModelError("", "مشکلی در دریافت اطلاعات شخصی پیش آمده است.");
                        return Page();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "مشکلی در دریافت اطلاعات فایل پیش آمده است.");
                    return Page();
                }

                hameshFullInfoViewModel = _hameshService.GetFullInfoFile(fileId, userId);
                ViewData["ActionType"] = new SelectList(_hameshService.GetActionType(), "Id", "Title");
                ViewData["ShowPage"] = true;

                ModelState.AddModelError("", "نوع اقدام، متن نظریه و حداقل یک گیرنده را مشخص کنید.");
                return Page();
            }

            // بازیابی اطلاعات Claims
            if (!int.TryParse(User.FindFirst("RoleTypeId")?.Value, out var roleTypeId))
            {
                ModelState.AddModelError("", "خطا در دریافت شناسه نوع نقش");
                return Page();
            }
            var roleTypeTitle = User.FindFirst("RoleTypeTitle")?.Value ?? "نامشخص";

            if (!int.TryParse(User.FindFirst("RoleTypeIdFinal")?.Value, out var RoleTypeIdFinal))
            {
                ModelState.AddModelError("", "خطا در دریافت شناسه نوع نقش");
                return Page();
            }
            var RoleTypeTitleFinal = User.FindFirst("RoleTypeTitleFinal")?.Value ?? "نامشخص";
            // ثبت هامش
            try
            {
                var requestedAmount = hameshFullInfoViewModel?.SumMablaghVamDarkhasti;
                var approvedAmount = hameshFullInfoViewModel?.MablaghVamMohaghaghSode;
                var resHamesh = _hameshService.RegHamesh(actionTypeId, roleTypeId, roleTypeTitle, RoleTypeIdFinal, RoleTypeTitleFinal, userDesc, userId, fileId, requestedAmount, approvedAmount, rcvrUserId);
                if (resHamesh.Status)
                {
                    TempData["OperationTitle"] = "ثبت موفق";
                    TempData["OperationMessage"] = resHamesh.Message;
                    TempData["OperationIcon"] = "success";
                    return RedirectToPage("/Visitor/File/PersonalNezami/ListFile");
                }
                else
                {
                    ModelState.AddModelError("", resHamesh.Message);
                    OnGet(fileId);
                    return Page();
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"خطا در ثبت هامش: {ex.Message}");
                ViewData["successcreate"] = false;
                ViewData["ShowPage"] = false;
            }

            return Page();
        }




        #endregion


        #endregion

        #region عودت

        #region لیست نفراتی که میتونیم درخواست ملاقات رو بهشون عودت بدیم
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetListRecieverUser(int FileId)
        {

            ViewData["fileId"] = FileId;
            var userId = int.Parse(User.FindFirst("Id").Value);


            usersForAoudat = _workFlowService.GetRecieverUserListByFileId(FileId, userId);

            //گزاشتیم تا صفحه در ابتدا لود بشه
            ViewData["ShowPage"] = true;

            return new PartialViewResult
            {
                ViewName = "_GridBackHameshList",
                ViewData = new ViewDataDictionary<List<Users>>(ViewData, usersForAoudat)
            };
        }
        #endregion

        #region انجام عملیات عودت
        /// <summary>
        /// درخواست ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPostBackFile(List<int> rcvrUserId, int actionTypeId, string userDesc, int fileId)
        {
            #region Cliam
            int roleId = int.Parse(User.FindFirst("RoleId").Value);
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);
            var roleTypeTitle = User.FindFirst("RoleTypeTitle").Value;
            var roleTypeIdFinal = int.Parse(User.FindFirst("RoleTypeIdFinal").Value);
            var roleTypeTitleFinal = User.FindFirst("RoleTypeTitleFinal").Value;
            var userId = int.Parse(User.FindFirst("Id").Value);
            var sndUserId = userId;
            #endregion

            if (actionTypeId <= 0 || string.IsNullOrWhiteSpace(userDesc) || rcvrUserId == null || !rcvrUserId.Any())
            {

                #region Initial
                ViewData["fileId"] = fileId;
                hameshFullInfoViewModel = new HameshFullInfoFileViewModel();
                usersForSend = new List<Users>();
                usersForAoudat = new List<Users>();
                usersForAoudat = _workFlowService.GetRecieverUserListByFileId(fileId, userId);
                #region لیست جلسات قابل ارجاع نفر به جلسه
                listMeetingForHameshFormViewModel = new ListMeetingForHameshFormViewModel();
                listMeetingForHameshFormViewModel = _meetingService.GetListMeetingForFormHamesh();
                listMeetingForHameshFormViewModel.FileId = fileId;

                collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

                var personalId = _fileService.GetFileByFileId(fileId).PersonalId;
                var personalCode = _personService.GetPersonByPersonalId(personalId).PersonalCode;
                collectionSpecificationPersonalViewModel.ListFile = _fileService.GetListFile(personalId);
                collectionSpecificationPersonalViewModel.FactPersonal = _personService.GetPersonalForEdit(personalId);
                #endregion
                #endregion

                hameshFullInfoViewModel = _hameshService.GetFullInfoFile(fileId, userId);

                ViewData["ActionType"] = new SelectList(_hameshService.GetActionType(), "Id", "Title");

                ViewData["ShowPage"] = true;

                ModelState.AddModelError("", "نوع اقدام، متن نظریه و حداقل یک گیرنده برای عودت را مشخص کنید.");

                return Page();
            }

            #region  ثبت هامش
            // ثبت هامش
            try

            {
                var requestedAmount = hameshFullInfoViewModel?.SumMablaghVamDarkhasti;
                var approvedAmount = hameshFullInfoViewModel?.MablaghVamMohaghaghSode;
                var res = _hameshService.RegHamesh(actionTypeId, roleTypeId, roleTypeTitle, roleTypeIdFinal, roleTypeTitleFinal, userDesc, userId, fileId, requestedAmount, approvedAmount, rcvrUserId);
                if (res.Status)
                {
                    TempData["OperationTitle"] = "عودت موفق";
                    TempData["OperationMessage"] = res.Message;
                    TempData["OperationIcon"] = "success";
                    return RedirectToPage("/Visitor/File/PersonalNezami/ListFile");
                }
                else
                {
                    ModelState.AddModelError("", res.Message);
                    OnGet(fileId);
                    return Page();
                }
                ViewData["ShowPage"] = false;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"خطا در ثبت هامش: {ex.Message}");
                ViewData["successAoudat"] = false;
                //گزاشتیم تا صفحه در ابتدا لود بشه
                ViewData["ShowPage"] = false;
            }
            #endregion



            return Page();



        }















        #endregion


        #endregion

        #region ارجاع به جلسه

        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetErjaBeMeeting(List<int> fileId, int meetingId, int unitCode)
        {
            #region claim
            if (!int.TryParse(User.FindFirst("Id")?.Value, out var userId))
            {
                ModelState.AddModelError("", "خطا در دریافت شناسه کاربر");
                return Page();
            }
            if (!int.TryParse(User.FindFirst("RoleTypeId")?.Value, out var roleTypeId))
            {
                ModelState.AddModelError("", "خطا در دریافت شناسه نوع نقش");
                return Page();
            }
            var roleTypeTitle = User.FindFirst("RoleTypeTitle")?.Value ?? "نامشخص";

            if (!int.TryParse(User.FindFirst("RoleTypeIdFinal")?.Value, out var RoleTypeIdFinal))
            {
                ModelState.AddModelError("", "خطا در دریافت شناسه نوع نقش");
                return Page();
            }
            var RoleTypeTitleFinal = User.FindFirst("RoleTypeTitleFinal")?.Value ?? "نامشخص";
            #endregion

            var unitCodeTitle = _webApiService.GetAllOrganWithGharagahNezaja().Data.Where(x => x.Id == unitCode).Select(x => x.Title).FirstOrDefault();
            var unitTitle = unitCodeTitle;
            //اضافه کردن شناسه جلسه به جدول درخواست ملاقات
            var resAddToFile = _meetingService.AddSingleMeetingIdToFile(fileId.FirstOrDefault(), meetingId);

            if (resAddToFile.Status)
            {
                //اضافه کردن به جدول نفرات جلسه
                var resAddToMemberMeeting = _meetingService.AddPersonToMemberMeeting(meetingId, fileId.FirstOrDefault(), unitCode, unitTitle);

                if (resAddToMemberMeeting.Status)
                {
                    var meetingReceiver = _userService.GetUserByUserId(139);
                    if (meetingReceiver == null)
                    {
                        return new JsonResult(new { success = false, message = "کاربر مسئول جلسه یافت نشد." });
                    }

                    var receiverIds = new List<int> { meetingReceiver.Id };
                    var userDesc = "بسمه تعالی، درخواست ملاقات به جلسه ارجاع گردید.";
                    var hameshResult = _hameshService.RegHamesh(3, roleTypeId, roleTypeTitle, RoleTypeIdFinal,
                        RoleTypeTitleFinal, userDesc, userId, fileId.FirstOrDefault(), null, null, receiverIds);

                    return new JsonResult(new { success = hameshResult.Status, message = hameshResult.Message });
                }
            }

            return new JsonResult(new { success = false, message = "ارجاع درخواست به جلسه انجام نشد." });

        }
        #endregion

        #region سرویس ها

        #region Tashvighat
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTashvighat(string personalCode)
        {
            //لازمه که تو هر هندلر ویو مدل برای مودالامون رو اینیشیال کنیم!!!
            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var tashvighat = _webApiService.GetTashvighatByPersonalNo(personalCode);

            collectionSpecificationPersonalViewModel.Tashvighat = new ApiResultTashvighatDto();

            if (tashvighat.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.Tashvighat.Data = tashvighat.Data;

                foreach (var item in collectionSpecificationPersonalViewModel.Tashvighat.Data)
                {
                    item.RowBeginDate = Tools.ConvertDigitToDateMain(item.RowBeginDate);
                }
                // var dateCOnvert =ConvertDigitToDateMain(date);
                return new JsonResult(collectionSpecificationPersonalViewModel.Tashvighat.Data.OrderByDescending(x => x.RowBeginDate));
            }

            else
            {
                return new JsonResult("");

            }



        }
        #endregion

        #region Tanbihat
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTanbihat(string personalCode)
        {
            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();
            var tanbihat = _webApiService.GetTanbihatByPersonalNo(personalCode);

            collectionSpecificationPersonalViewModel.Tanbihat = new ApiResultTanbihatDto();

            if (tanbihat.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.Tanbihat.Data = tanbihat.Data;

                foreach (var item in collectionSpecificationPersonalViewModel.Tanbihat.Data)
                {
                    item.RowBeginDate = Tools.ConvertDigitToDateMain(item.RowBeginDate);
                }

                return new JsonResult(collectionSpecificationPersonalViewModel.Tanbihat.Data.OrderByDescending(x => x.RowBeginDate));
            }

            else
            {
                return new JsonResult("");

            }

        }
        #endregion

        #region Entegalat

        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetEnteghal(string personalCode)
        {
            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var enteghal = _webApiService.GetEnteghalByPersonNo(personalCode);

            collectionSpecificationPersonalViewModel.Enteghal = new ApiResultEnteghalDto();

            if (enteghal.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.Enteghal.Data = enteghal.Data;
                foreach (var item in collectionSpecificationPersonalViewModel.Enteghal.Data)
                {
                    item.WentDate = Tools.ConvertDigitToDateMain(item.WentDate);
                }

                return new JsonResult(collectionSpecificationPersonalViewModel.Enteghal.Data.OrderByDescending(x => x.Id));
            }

            else
            {
                return new JsonResult("");

            }


        }
        #endregion

        #region Aele
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetAeleh(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var personFamily = _webApiService.GetPFamilyInfo(personalCode);

            collectionSpecificationPersonalViewModel.PersonFamily = new ApiResultPersonFamilyDto();

            if (personFamily.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.PersonFamily.Data = personFamily.Data;

                return new JsonResult(collectionSpecificationPersonalViewModel.PersonFamily.Data.OrderByDescending(x => x.Id));
            }

            else
            {
                return new JsonResult("");

            }

        }

        #endregion

        #region Maskan
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTashilatMaskan(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var maskan = _webApiService.GetPTashilatMaskan(personalCode);

            collectionSpecificationPersonalViewModel.TashilatMaskan = new ApiResulMaskanDto();

            if (maskan.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.TashilatMaskan.Data = maskan.Data.OrderByDescending(x => x.RegDate).ToList();

                foreach (var item in collectionSpecificationPersonalViewModel.TashilatMaskan.Data)
                {
                    item.MablaqTitle = Tools.FormatNumber(item.Mablaq.ToString());
                }

                foreach (var item in collectionSpecificationPersonalViewModel.TashilatMaskan.Data)
                {
                    item.VagozariDate = Tools.ConvertDigitToDateMain(item.VagozariDate);
                }
                return new JsonResult(collectionSpecificationPersonalViewModel.TashilatMaskan.Data.OrderByDescending(x => x.RegDate));
            }

            else
            {
                return new JsonResult("");

            }


        }

        #endregion

        #region Tashilat DabirKhaneh
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTashilatDabirKhaneh(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var tashiladDabirKhaneh = _webApiService.GetPTashilatDabirkhaneh(personalCode);

            collectionSpecificationPersonalViewModel.TashilatDabirkhaneh = new ApiResulDabirKhanehDto();

            if (tashiladDabirKhaneh.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.TashilatDabirkhaneh.Data = tashiladDabirKhaneh.Data;

                foreach (var item in collectionSpecificationPersonalViewModel.TashilatDabirkhaneh.Data)
                {
                    item.ConfirmMablaqVamTitle = Tools.FormatNumber(item.ConfirmMablaqVam.ToString());
                }

                foreach (var item in collectionSpecificationPersonalViewModel.TashilatDabirkhaneh.Data)
                {
                    item.ConfirmOrderDate = Tools.ConvertDigitToDateMain(item.ConfirmOrderDate);
                }


                return new JsonResult(collectionSpecificationPersonalViewModel.TashilatDabirkhaneh.Data.OrderByDescending(x => x.RegDate));
            }

            else
            {
                return new JsonResult("");


            }

        }
        #endregion

        #region TashilatDastor
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTashilatDastor(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var dastor = _webApiService.GetTashilatDastor(personalCode);

            collectionSpecificationPersonalViewModel.TashilatDastor = new ApiResulDastorDto();

            if (dastor.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.TashilatDastor.Data = dastor.Data;
                foreach (var item in collectionSpecificationPersonalViewModel.TashilatDastor.Data)
                {
                    item.RowBeginDate = Tools.ConvertDigitToDateMain(item.RowBeginDate);
                }

                return new JsonResult(collectionSpecificationPersonalViewModel.TashilatDastor.Data.OrderByDescending(x => x.Id));
            }

            else
            {
                return new JsonResult("");


            }


        }
        #endregion

        #region تسهیلات دیگر
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTashilatOther(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var other = _webApiService.GetTashilatOther(personalCode);

            collectionSpecificationPersonalViewModel.TashilatOther = new ApiResultOtherDto();

            if (other.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.TashilatOther.Data = other.Data;
                foreach (var item in collectionSpecificationPersonalViewModel.TashilatOther.Data)
                {
                    item.MosaedatDate = Tools.ConvertDigitToDateMain(item.MosaedatDate);
                }
                return new JsonResult(collectionSpecificationPersonalViewModel.TashilatOther.Data.OrderByDescending(x => x.Id));
            }

            else
            {
                return new JsonResult("");


            }


        }
        #endregion

        #region تسهیلات بلاعوض
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetTashilatBelaavaz(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var belaavaz = _webApiService.GetTashilatBelaavaz(personalCode);

            collectionSpecificationPersonalViewModel.TashilatBelaavaz = new ApiResultBelaavazDto();

            if (belaavaz.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.TashilatBelaavaz.Data = belaavaz.Data;

                foreach (var item in collectionSpecificationPersonalViewModel.TashilatBelaavaz.Data)
                {
                    item.MablaqTitle = Tools.FormatNumber(item.Mablaq.ToString());
                }

                foreach (var item in collectionSpecificationPersonalViewModel.TashilatBelaavaz.Data)
                {
                    item.RegDate = Tools.ConvertDigitToDateMain(item.RegDate);
                }
                return new JsonResult(collectionSpecificationPersonalViewModel.TashilatBelaavaz.Data.OrderByDescending(x => x.RegDate));
            }

            else
            {
                return new JsonResult("");


            }


        }
        #endregion

        #region Exam
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetExam(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var exam = _webApiService.GetExam(personalCode);

            collectionSpecificationPersonalViewModel.Exam = new ApiResulExamDto();

            if (exam.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.Exam.Data = exam.Data;

                foreach (var item in collectionSpecificationPersonalViewModel.Exam.Data)
                {
                    item.RowBeginDate = Tools.ConvertDigitToDateMain(item.RowBeginDate);
                    item.RowEndDate = Tools.ConvertDigitToDateMain(item.RowEndDate);
                    item.RegisterDate = Tools.ConvertDigitToDateMain(item.RegisterDate);
                }
                return new JsonResult(collectionSpecificationPersonalViewModel.Exam.Data.OrderByDescending(x => x.Id));
            }

            else
            {
                return new JsonResult("");


            }


        }
        #endregion

        #region فیش 
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetFish(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var fish = _webApiService.GetFishByPrsnCode(personalCode);

            collectionSpecificationPersonalViewModel.Fish = new ApiResultFishDto();

            if (fish.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.Fish.Data = fish.Data;
                foreach (var item in collectionSpecificationPersonalViewModel.Fish.Data)
                {
                    item.TotalDebtFormat = Tools.FormatNumber(item.TotalDebt.ToString());
                }

                return new JsonResult(collectionSpecificationPersonalViewModel.Fish.Data.OrderByDescending(x => x.DeductionDate));
            }

            else
            {
                return new JsonResult("");
            }


        }
        #endregion

        #region معسرین 
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetGetMoeeser(string personalCode)
        {

            collectionSpecificationPersonalViewModel = new CollectionSpecificationPersonalViewModel();

            var moeeser = _webApiService.GetMoeeserByPrsnCode(personalCode);

            collectionSpecificationPersonalViewModel.Moeeser = new ApiResultMoeeserDto();

            if (moeeser.IsSuccess)
            {
                collectionSpecificationPersonalViewModel.Moeeser.Data = moeeser.Data;

                foreach (var item in collectionSpecificationPersonalViewModel.Moeeser.Data)
                {
                    item.RegDate = Tools.FormatNumber(item.RegDate.ToString());
                    item.LastUpdDate = Tools.FormatNumber(item.LastUpdDate.ToString());
                }

                return new JsonResult(collectionSpecificationPersonalViewModel.Moeeser.Data.OrderByDescending(x => x.Id));
            }

            else
            {
                return new JsonResult("");


            }


        }
        #endregion



















        #endregion

        #region وضعیت بایگانی درخواست ملاقات رو فعال میکنه
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetArchive(int fileId)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);

            _fileService.ArchivedFile(fileId, userId);
            ViewData["Archived"] = true;
            ViewData["ShowPage"] = false;
            return Page();
        }

        #endregion
    }
}

using ITOWebApiClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.Classes;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Web.Helpers;

namespace VisitorManagment.Web.Pages.Visitor.File.PersonalNezami
{
    [Authorize]
    public class EditFileModel : PageModel
    {
        private readonly IFileService _fileService;
        private readonly ISmsService _smsService;
        private readonly ApiTokenCacheClient _apiTokenClient;
        private readonly IHameshService _hameshService;
        private readonly IWebApiService _webApiService;

        public EditFileModel(IFileService fileService, ISmsService smsService, ApiTokenCacheClient apiTokenClient, IHameshService hameshService
            , IWebApiService webApiService)
        {

            _fileService = fileService;
            _smsService = smsService;
            _apiTokenClient = apiTokenClient;
            _hameshService = hameshService;
            _webApiService = webApiService;
        }

        [BindProperty]
        public EditFactPersonalViewModel editFactPersonalViewModel { get; set; }
        public SMSInfoViewModel smsInfoViewModel { get; set; }
        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public IActionResult OnGet(int id)
        {

            LoadDropdowns();

            editFactPersonalViewModel = _fileService.GetFileForEdit(id);
            //session Attachment
            HttpContext.Session.SetString("attachment", editFactPersonalViewModel.AttachmentFileName);
            Response.Cookies.Append("attachment", editFactPersonalViewModel.AttachmentFileName);
            //end session
            return Page();
        }

        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost()
        {
            LoadDropdowns();
            //get role Type
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);
            var roleTypeTitle = User.FindFirst("RoleTypeId").Value;

            editFactPersonalViewModel.EditUserId = int.Parse(User.FindFirst("Id").Value);

            #region پیوست و عکس

            editFactPersonalViewModel.FishAttachmnet = editFactPersonalViewModel.FishAttachmnet;

            if (string.IsNullOrEmpty(editFactPersonalViewModel.AttachmentFileName))
            {
                editFactPersonalViewModel.AttachmentFileName = HttpContext.Session.GetString("attachment");
            }
            else
            {
                editFactPersonalViewModel.AttachmentFileName = editFactPersonalViewModel.AttachmentFileName;
            }

            if (string.IsNullOrEmpty(editFactPersonalViewModel.PersonalAvatarName))
            {
                editFactPersonalViewModel.PersonalAvatarName = "Default.png";
                ////==== Check FileExtentions

                if (editFactPersonalViewModel.Attachment != null && !FileUploadCheck.CheckFileExtension(editFactPersonalViewModel.Attachment))
                {
                    ModelState.AddModelError("", "فایل انتخابی معتبر نمی باشد");
                    return Page();
                }


            }
            else
            {

                ////==== Check FileExtentions

                if (editFactPersonalViewModel.Attachment != null && !FileUploadCheck.CheckFileExtension(editFactPersonalViewModel.Attachment))
                {
                    ModelState.AddModelError("", "فایل انتخابی معتبر نمی باشد");
                    return Page();
                }

            }
            #endregion

            //edit table file
            var resFile = _fileService.EditFile(editFactPersonalViewModel);
            var resPersonel = new BaseResult();

            if (resFile.Status)
            {
                //edit table Personal
                resPersonel = _fileService.EditPersonal(editFactPersonalViewModel);
            }
            if (resFile.Status && resPersonel.Status)
            {
                ViewData["successcreate"] = true;
            }

            return Page();
        }

        #region گرفتن اطلاعات پرسنلی فرمانده یگان با کد پرسنلی
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public JsonResult OnGetGetPersonalFarmandehId(string personalno)
        {

            var result2 = _webApiService.GetPersonalByPersonalNo(personalno);


            if (result2 == null)
            {
                return new JsonResult(new { message = "پرسنلی با این مشخصات یافت نشد" });
            }

            SessionHelper.SetObjectAsJson(HttpContext.Session, "result2", result2);
            return new JsonResult(result2.Data);
        }
        #endregion

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        private void LoadDropdowns()
        {
            ViewData["RequestSubject"] = new SelectList(_fileService.GetRequestSubject(), "Id", "Title");
            ViewData["FileStatus"] = new SelectList(_fileService.GetFileStatus(), "Id", "Title");
            ViewData["Priority"] = new SelectList(_fileService.GetPriority(), "Id", "Title");
            ViewData["GharargahList"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["FileTypeList"] = new SelectList(_fileService.GetListFileType(), "Id", "Title");
        }
    }
}

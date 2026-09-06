using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Web.Helpers;

namespace VisitorManagment.Web.Pages.Visitor.File.PersonalNezami
{
    public class CreateFileNewModel : PageModel
    {

        private readonly IFileService _fileService;
        private readonly IWebApiService _webApiService;

        private readonly IHameshService _hameshService;
        private readonly IPersonService _personService;

        public CreateFileNewModel(IFileService fileService, IWebApiService webApiService,
             IHameshService hameshService,
             IPersonService personService)
        {
            _fileService = fileService;
            _webApiService = webApiService;
            _hameshService = hameshService;
            _personService = personService;
        }

        [BindProperty]
        public FactPersonalViewModel factPersonalViewModel { get; set; }
        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet()
        {
            LoadDropdowns();
        }

       


        #region گرفتن اطلاعات پرسنلی نفر با کد پرسنلی
        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public JsonResult OnGetGetPersonalId(string personalno)
        {
            var unitDutyCodeUserLogin = int.Parse(User.FindFirst("UnitDutyCode").Value);
            var unitCodeUserLogin = int.Parse(User.FindFirst("UnitCode").Value);
            var result = _webApiService.GetPersonalByPersonalNo(personalno);

            if (result == null)
            {
                return new JsonResult(new { message = "پرسنلی با این مشخصات یافت نشد" });
            }

            var personal = _personService.GetPersonalByPersonalCode(personalno);

            // Default avatar if not present
            result.Data.PersonalAvatarName = personal?.PersonalAvatar ?? "Default.png";

            int roleId = int.Parse(User.FindFirst("RoleId").Value);
            string username = User.FindFirst("UserName").Value;

            if (roleId == 14 && result.Data.PersonalCode != username)
            {
                return new JsonResult(new { message = "شما کاربر عادی میباشید و دسترسی ندارید!" });
            }

            SessionHelper.SetObjectAsJson(HttpContext.Session, "result", result.Data);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "unitCode", result.Data.UnitCode);

            return new JsonResult(result.Data);
        }
        #endregion







        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        private void LoadDropdowns()
        {
            ViewData["RequestSubject"] = new SelectList(_fileService.GetRequestSubject(), "Id", "Title");
            ViewData["FileTypeList"] = new SelectList(_fileService.GetListFileType(), "Id", "Title");
            ViewData["FileStatus"] = new SelectList(_fileService.GetFileStatus(), "Id", "Title");
            ViewData["Priority"] = new SelectList(_fileService.GetPriority(), "Id", "Title");
            ViewData["personalCode"] = User.FindFirst("UserName").Value;
            ViewData["roleTypeId"] = int.Parse(User.FindFirst("RoleTypeId").Value);
            ViewData["GharargahList"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
        }
    }
}

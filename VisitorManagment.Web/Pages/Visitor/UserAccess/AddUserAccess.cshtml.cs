using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Web.Pages.Visitor.UserAccess
{
    [Authorize]
    public class AddUserAccessModel : PageModel
    {
        private readonly IFileService _fileService;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly IWorkFlowService _workFlowService;
        private readonly IHameshService _hameshService;
        public AddUserAccessModel(IFileService fileService, IUserService userService, IPermissionService permissionService , IWorkFlowService workFlowService , IHameshService hameshService)
        {
            _fileService = fileService;
            _userService = userService;
            _permissionService = permissionService;
            _workFlowService = workFlowService;
            _hameshService = hameshService;
        }

        [BindProperty]
        public CreateUserAccessViewModel createUserAccessViewModel { get; set; }
        public List<Users> userlist;
        public int userid;
        #region اعضا و متدهای کلاس

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public void OnGet()
        {
            var userId = User.FindFirst("Id").Value;
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);

            ViewData["RoleList"] = new SelectList(_permissionService.GetRoles(roleTypeId.ToString()), "RoleId", "Title");
            ViewData["RoleListEtayi"] = new SelectList(_permissionService.GetRoles(roleTypeId.ToString()), "RoleId", "Title");
        }

        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public JsonResult OnGetGetPersonal(string personalno)
        {
            var userunitdutycode = int.Parse(User.FindFirst("UnitDutyCode").Value);

            userid = _userService.GetUserIdByPersonalCode(personalno);

            if (userid == 0)
            {
                return new JsonResult(new { message = "این پرسنل در لیست کاربران شما تعریف نشده است . ابتدا پرسنل را تعریف کنید" });
            }
            var user = _userService.GetUserByPersonalCode(personalno);
            if (user.UnitDutyCode != userunitdutycode)
            {
                return new JsonResult(new { message = "این شخص جزء پرسنل یگان شما نمیباشد" });
            }
            return new JsonResult(user);

        }

        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost(List<int> RoleListEtayi, int roleidhidden)
        {
            var addUserId = int.Parse(User.FindFirst("Id").Value);
            _userService.AddAccessTypeToRole(RoleListEtayi, roleidhidden, addUserId);
            ViewData["successcreate"] = true;
            return Page();
        }

        #endregion
    }
}

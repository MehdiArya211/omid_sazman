using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Admin.SendAccess
{
     [Authorize]
    // [AutoValidateAntiforgeryToken]
    public class CreateSendAccessModel : PageModel
    {
        private readonly IWorkFlowService _workFlowService;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;

        public CreateSendAccessModel(IPermissionService permissionService, IUserService userService,
            IWorkFlowService workFlowService)
        {
            _workFlowService = workFlowService;
            _permissionService = permissionService;
            _userService = userService;

        }
        [BindProperty]
        public CreateUserAccessViewModel createUserAccessViewModel { get; set; }
        public List<WorkFlowViewModel> ListAccsessPersonal { get; set; }
        public List<WorkFlowViewModel> ListUnAccsessPersonal { get; set; }

        public int? RoleId;
        #region اعضا و متدهای کلاس


        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public void OnGet(int? roleId = 0)
        {
            RoleId = roleId;

            ViewData["AllRole"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");

            ViewData["WorkFlow"] = new SelectList(_workFlowService.GetrcvrList(roleId ?? 0), "Id", "Title");

            // لیست منو هایی که دسترسی دارد
            ListAccsessPersonal = _workFlowService.GetReciverList(roleId ?? 0);

            // لیست منو هایی که دسترسی ندارد
            ListUnAccsessPersonal = _workFlowService.GetUnAccessList(roleId ?? 0);


            ViewData["ShowForm"] = false;

            if (roleId != 0)
            {
                ViewData["ShowForm"] = true;
                ViewData["RoleId"] = roleId;

            }

        }

        /// <summary>
        /// دسترسی دارند
        /// </summary>
        /// <param name="UnAccessRoleId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IActionResult OnPostRegAccess(List<int> UnAccessRoleId, int roleId)
        {
            if (roleId <= 0 || UnAccessRoleId == null || UnAccessRoleId.Count == 0)
            {
                SetOperationNotification("انتخاب نقش", "حداقل یک نقش را برای افزودن به مسیر ارسال انتخاب کنید.", "warning");
                return RedirectToPage(new { roleId });
            }

            RoleId = roleId;

            ViewData["AllRole"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");

            ViewData["WorkFlow"] = new SelectList(_workFlowService.GetrcvrList(roleId), "Id", "Title");



            var userId = User.FindFirst("Id").Value;

            ViewData["ShowForm"] = true;
            ViewData["RoleId"] = roleId;

            _workFlowService.AddAccessToRole(UnAccessRoleId, roleId, int.Parse(userId));
            SetOperationNotification("ثبت موفق", "نقش‌های انتخاب‌شده با موفقیت به مسیر ارسال اضافه شدند.", "success");
            return RedirectToPage(new { roleId });
        }


        /// <summary>
        /// دسترسی ندارند
        /// </summary>
        /// <param name="AccessRoleId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IActionResult OnPostUnAccess(List<int> AccessRoleId, int roleId)
        {
            if (roleId <= 0 || AccessRoleId == null || AccessRoleId.Count == 0)
            {
                SetOperationNotification("انتخاب نقش", "حداقل یک نقش را برای حذف از مسیر ارسال انتخاب کنید.", "warning");
                return RedirectToPage(new { roleId });
            }

            RoleId = roleId;

            ViewData["AllRole"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");

            ViewData["WorkFlow"] = new SelectList(_workFlowService.GetrcvrList(roleId), "Id", "Title");



            var userId = User.FindFirst("Id").Value;

            ViewData["ShowForm"] = true;
            ViewData["RoleId"] = roleId;


            _workFlowService.RemoveAccessToRole(AccessRoleId, roleId, int.Parse(userId));
            SetOperationNotification("حذف موفق", "نقش‌های انتخاب‌شده با موفقیت از مسیر ارسال حذف شدند.", "success");
            return RedirectToPage(new { roleId });
        }

        /// <summary>
        /// پیام نتیجه عملیات مسیر ارسال را برای نمایش پس از انتقال صفحه تنظیم می‌کند.
        /// </summary>
        private void SetOperationNotification(string title, string message, string icon)
        {
            TempData["OperationTitle"] = title;
            TempData["OperationMessage"] = message;
            TempData["OperationIcon"] = icon;
        }

        #endregion
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Core.Services.Interfaces.RolePermissions;

namespace VisitorManagment.Web.Pages.Admin.RolePermission
{
    [Authorize]
    public class MainIndexModel : PageModel
    {
        private readonly IWorkFlowService _workFlowService;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly IRolePermissionService _rolePermissionService;

        public MainIndexModel(IPermissionService permissionService, IUserService userService,
            IWorkFlowService workFlowService, IRolePermissionService rolePermissionService)
        {
            _workFlowService = workFlowService;
            _permissionService = permissionService;
            _userService = userService;
            _rolePermissionService = rolePermissionService;

        }
        [BindProperty]
        public CreateUserAccessViewModel createUserAccessViewModel { get; set; }
        public List<PermissionViewModel> ListAccsessPersonal { get; set; }
        public List<PermissionViewModel> ListUnAccsessPersonal { get; set; }

        public int? RoleId;
        #region اعضا و متدهای کلاس


        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public void OnGet(int? roleId = 0)
        {
            RoleId = roleId;

            ViewData["RoleList"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");

            ViewData["PermissionList"] = new SelectList(_permissionService.GetAllPermission(), "PermissionId", "PermissionTitle");

            // لیست پرسنلی که دسترسی دارد
            ListAccsessPersonal = _permissionService.GetAccessReciverMenuList(roleId ?? 0);

            // لیست پرسنلی که دسترسی ندارد
            ListUnAccsessPersonal = _permissionService.GetUnAccessReciverMenuList(roleId ?? 0);


            ViewData["ShowForm"] = false;

            if (roleId != 0)
            {
                ViewData["ShowForm"] = true;
                ViewData["RoleId"] = roleId;

            }

        }


        /// <summary>
        /// درخواست ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPostAddAccess(int roleId , List<int> permissionIds)
        {
            ViewData["RoleList"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");

            ViewData["PermissionList"] = new SelectList(_permissionService.GetAllPermission(), "PermissionId", "PermissionTitle");

            // لیست پرسنلی که دسترسی دارد
            ListAccsessPersonal = _permissionService.GetAccessReciverMenuList(roleId);

            // لیست پرسنلی که دسترسی ندارد
            ListUnAccsessPersonal = _permissionService.GetUnAccessReciverMenuList(roleId);


            _rolePermissionService.AddPermissionToRole(roleId, permissionIds);
            ViewData["ShowForm"] = true;
            ViewData["RoleId"] = roleId;
            return Redirect("/Admin/rolepermission/mainindex?roleId=" + roleId);
        }

        /// <summary>
        /// درخواست ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPostRemoveAccess(int roleId, List<int> permissionIds)
        {
            ViewData["RoleList"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");

            ViewData["PermissionList"] = new SelectList(_permissionService.GetAllPermission(), "PermissionId", "PermissionTitle");

            // لیست پرسنلی که دسترسی دارد
            ListAccsessPersonal = _permissionService.GetAccessReciverMenuList(roleId);

            // لیست پرسنلی که دسترسی ندارد
            ListUnAccsessPersonal = _permissionService.GetUnAccessReciverMenuList(roleId);


            _rolePermissionService.RemovePermissionToRole(roleId, permissionIds);
            ViewData["ShowForm"] = true;
            ViewData["RoleId"] = roleId;
            return Redirect("/Admin/rolepermission/mainindex?roleId=" + roleId) ;
        }
        #endregion
    }
}

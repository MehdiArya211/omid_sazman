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

        [BindProperty(SupportsGet = true)]
        public int? RoleId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? UnitCode { get; set; }
        public bool HasUnitProfile { get; private set; }
        #region اعضا و متدهای کلاس


        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public void OnGet(int? roleId = 0, int? unitCode = null)
        {
            RoleId = roleId;
            UnitCode = unitCode > 0 ? unitCode : null;

            ViewData["RoleList"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");
            ViewData["UnitList"] = new SelectList(_permissionService.GetUnitsForAccessManagement(), "UnitCode", "UnitTitle");

            ViewData["PermissionList"] = new SelectList(_permissionService.GetAllPermission(), "PermissionId", "PermissionTitle");

            // لیست پرسنلی که دسترسی دارد
            ListAccsessPersonal = _permissionService.GetAccessReciverMenuList(roleId ?? 0, UnitCode);

            // لیست پرسنلی که دسترسی ندارد
            ListUnAccsessPersonal = _permissionService.GetUnAccessReciverMenuList(roleId ?? 0, UnitCode);
            HasUnitProfile = UnitCode.HasValue && _permissionService.HasUnitAccessProfile(roleId ?? 0, UnitCode.Value);


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
        public IActionResult OnPostAddAccess(int roleId, int? unitCode, List<int> permissionIds)
        {
            if (roleId <= 0 || permissionIds == null || permissionIds.Count == 0)
            {
                SetOperationNotification("انتخاب دسترسی", "حداقل یک منو را برای افزودن انتخاب کنید.", "warning");
                return RedirectToPage(new { roleId, unitCode });
            }

            ViewData["RoleList"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");

            ViewData["PermissionList"] = new SelectList(_permissionService.GetAllPermission(), "PermissionId", "PermissionTitle");

            // لیست پرسنلی که دسترسی دارد
            ListAccsessPersonal = _permissionService.GetAccessReciverMenuList(roleId);

            // لیست پرسنلی که دسترسی ندارد
            ListUnAccsessPersonal = _permissionService.GetUnAccessReciverMenuList(roleId);


            var addedCount = _rolePermissionService.AddPermissionToRole(roleId, permissionIds, unitCode);
            SetOperationNotification(addedCount > 0 ? "ثبت موفق" : "بدون تغییر", addedCount > 0 ? $"{addedCount} دسترسی با موفقیت به نقش اضافه شد." : "دسترسی‌های انتخاب‌شده قبلاً برای این نقش ثبت شده‌اند.", addedCount > 0 ? "success" : "info");
            return RedirectToPage(new { roleId, unitCode });
        }

        /// <summary>
        /// درخواست ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPostRemoveAccess(int roleId, int? unitCode, List<int> permissionIds)
        {
            if (roleId <= 0 || permissionIds == null || permissionIds.Count == 0)
            {
                SetOperationNotification("انتخاب دسترسی", "حداقل یک منو را برای حذف انتخاب کنید.", "warning");
                return RedirectToPage(new { roleId, unitCode });
            }

            ViewData["RoleList"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");

            ViewData["PermissionList"] = new SelectList(_permissionService.GetAllPermission(), "PermissionId", "PermissionTitle");

            // لیست پرسنلی که دسترسی دارد
            ListAccsessPersonal = _permissionService.GetAccessReciverMenuList(roleId);

            // لیست پرسنلی که دسترسی ندارد
            ListUnAccsessPersonal = _permissionService.GetUnAccessReciverMenuList(roleId);


            var removedCount = _rolePermissionService.RemovePermissionToRole(roleId, permissionIds, unitCode);
            SetOperationNotification(removedCount > 0 ? "حذف موفق" : "بدون تغییر", removedCount > 0 ? $"{removedCount} دسترسی با موفقیت حذف شد." : "هیچ دسترسی ثبت‌شده‌ای برای حذف یافت نشد.", removedCount > 0 ? "success" : "info");
            return RedirectToPage(new { roleId, unitCode });
        }

        public IActionResult OnPostResetUnitAccess(int roleId, int unitCode)
        {
            if (roleId <= 0 || unitCode <= 0)
            {
                SetOperationNotification("اطلاعات نامعتبر", "نقش و یگان معتبر انتخاب نشده است.", "warning");
                return RedirectToPage(new { roleId, unitCode });
            }

            var reset = _rolePermissionService.ResetUnitPermissionToRole(roleId, unitCode);
            SetOperationNotification(reset ? "بازنشانی موفق" : "بدون تغییر",
                reset ? "دسترسی اختصاصی حذف شد و این یگان از این پس از دسترسی پایه نقش استفاده می‌کند." : "برای این یگان دسترسی اختصاصی ثبت نشده بود.",
                reset ? "success" : "info");
            return RedirectToPage(new { roleId, unitCode });
        }

        /// <summary>
        /// پیام نتیجه عملیات مدیریت دسترسی را برای نمایش پس از انتقال صفحه تنظیم می‌کند.
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

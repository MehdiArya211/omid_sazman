using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.User;
using VisitorManagment.Web.Helpers;
using VisitorManagment.Core.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VisitorManagment.Web.Pages.Admin.Roles
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IPermissionService _permissionService;
        public IndexModel(IPermissionService permissionService) { _permissionService = permissionService; }
        public System.Collections.Generic.List<Role> Roles { get; private set; }
        public SelectList RoleTypeOptions { get; private set; }
        public SelectList RoleFinalTypeOptions { get; private set; }

        /// <summary>
        /// عنوان خوانای سمت سازمانی را برای نمایش در جدول نقش‌ها برمی‌گرداند.
        /// </summary>
        public string GetRoleTypeTitle(int roleType)
        {
            var options = SystemRoleTypes.GetManagementOptions();
            return options.ContainsKey(roleType) ? options[roleType] : $"کد {roleType}";
        }

        /// <summary>
        /// عنوان خوانای سطح سازمانی نهایی را برای نمایش در جدول نقش‌ها برمی‌گرداند.
        /// </summary>
        public string GetRoleFinalTypeTitle(int? roleFinalType)
        {
            if (!roleFinalType.HasValue) return "—";
            var options = SystemRoleFinalTypes.GetManagementOptions();
            return options.ContainsKey(roleFinalType.Value) ? options[roleFinalType.Value] : $"کد {roleFinalType.Value}";
        }

        public IActionResult OnGet()
        {
            if (!IsSystemAdmin()) return Forbid();
            Roles = _permissionService.GetAllRoles().Where(role => !role.IsDelete).OrderBy(role => role.SortNum).ThenBy(role => role.Title).ToList();
            LoadOptions();
            return Page();
        }

        public IActionResult OnPostCreate(string title, int code, int roleType, int? roleTypeFinalId, int? sortNum)
        {
            if (!IsSystemAdmin()) return Forbid();
            title = (title ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(title) || code <= 0 || roleType <= 0)
                return RedirectWithMessage("اطلاعات ناقص", "عنوان، کد و نوع نقش الزامی است.", "warning");
            if (_permissionService.RoleTitleOrCodeExists(title, code))
                return RedirectWithMessage("رکورد تکراری", "نقشی با همین عنوان یا کد وجود دارد.", "warning");
            _permissionService.AddRole(new Role { Title = title, Code = code, RoleType = roleType, RoleTypeFinalId = roleTypeFinalId, SortNum = sortNum, IsDelete = false });
            return RedirectWithMessage("ثبت موفق", "نقش جدید با موفقیت ثبت شد.", "success");
        }

        public IActionResult OnPostEdit(int roleId, string title, int code, int roleType, int? roleTypeFinalId, int? sortNum)
        {
            if (!IsSystemAdmin()) return Forbid();
            var role = _permissionService.GetRoleById(roleId);
            title = (title ?? string.Empty).Trim();
            if (role == null) return RedirectWithMessage("خطا", "نقش موردنظر یافت نشد.", "error");
            if (string.IsNullOrWhiteSpace(title) || code <= 0 || roleType <= 0)
                return RedirectWithMessage("اطلاعات ناقص", "عنوان، کد و نوع نقش الزامی است.", "warning");
            if (_permissionService.RoleTitleOrCodeExists(title, code, roleId))
                return RedirectWithMessage("رکورد تکراری", "نقشی با همین عنوان یا کد وجود دارد.", "warning");
            role.Title = title; role.Code = code; role.RoleType = roleType; role.RoleTypeFinalId = roleTypeFinalId; role.SortNum = sortNum;
            _permissionService.UpdateRole(role);
            return RedirectWithMessage("ویرایش موفق", "اطلاعات نقش با موفقیت ویرایش شد.", "success");
        }

        public IActionResult OnPostDelete(int roleId)
        {
            if (!IsSystemAdmin()) return Forbid();
            var role = _permissionService.GetRoleById(roleId);
            if (role == null) return RedirectWithMessage("خطا", "نقش موردنظر یافت نشد.", "error");
            if (_permissionService.IsRoleInUse(roleId)) return RedirectWithMessage("حذف انجام نشد", "این نقش به کاربر یا دسترسی منو متصل است؛ ابتدا وابستگی‌ها را حذف کنید.", "warning");
            _permissionService.DeleteRole(role);
            return RedirectWithMessage("حذف موفق", "نقش با موفقیت حذف شد.", "success");
        }

        private bool IsSystemAdmin() => User.IsSystemAdministrator();
        private void LoadOptions()
        {
            var roleTypes = SystemRoleTypes.GetManagementOptions().ToDictionary(item => item.Key, item => item.Value);
            foreach (var role in Roles.Where(role => !roleTypes.ContainsKey(role.RoleType)))
                roleTypes[role.RoleType] = $"{role.Title} (کد فعلی {role.RoleType})";
            RoleTypeOptions = new SelectList(roleTypes.OrderBy(item => item.Key), "Key", "Value");
            var finalTypes = SystemRoleFinalTypes.GetManagementOptions().ToDictionary(item => item.Key, item => item.Value);
            foreach (var role in Roles.Where(role => role.RoleTypeFinalId.HasValue && !finalTypes.ContainsKey(role.RoleTypeFinalId.Value)))
                finalTypes[role.RoleTypeFinalId.Value] = $"سطح فعلی با کد {role.RoleTypeFinalId.Value}";
            RoleFinalTypeOptions = new SelectList(finalTypes.OrderBy(item => item.Key), "Key", "Value");
        }
        private IActionResult RedirectWithMessage(string title, string message, string icon) { TempData["OperationTitle"] = title; TempData["OperationMessage"] = message; TempData["OperationIcon"] = icon; return RedirectToPage(); }
    }
}

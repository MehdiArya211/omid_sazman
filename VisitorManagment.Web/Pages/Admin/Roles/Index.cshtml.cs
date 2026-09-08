using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Web.Pages.Admin.Roles
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IPermissionService _permissionService;
        public IndexModel(IPermissionService permissionService) { _permissionService = permissionService; }
        public System.Collections.Generic.List<Role> Roles { get; private set; }

        public IActionResult OnGet()
        {
            if (!IsSystemAdmin()) return Forbid();
            Roles = _permissionService.GetAllRoles().Where(role => !role.IsDelete).OrderBy(role => role.SortNum).ThenBy(role => role.Title).ToList();
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

        private bool IsSystemAdmin() => User.FindFirst("RoleTypeId")?.Value == "100";
        private IActionResult RedirectWithMessage(string title, string message, string icon) { TempData["OperationTitle"] = title; TempData["OperationMessage"] = message; TempData["OperationIcon"] = icon; return RedirectToPage(); }
    }
}

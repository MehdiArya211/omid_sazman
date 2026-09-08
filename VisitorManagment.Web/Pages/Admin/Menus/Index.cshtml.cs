using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.Permissions;

namespace VisitorManagment.Web.Pages.Admin.Menus
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IPermissionService _permissionService;
        public IndexModel(IPermissionService permissionService) { _permissionService = permissionService; }
        public List<Permission> Menus { get; private set; }
        public IActionResult OnGet() { if (!IsSystemAdmin()) return Forbid(); Load(); return Page(); }

        public IActionResult OnPostCreate(string title, int? parentId, string parentUrl, string subUrl, string menuUrl, string iconName, int order, bool isActive, bool showAll)
        {
            if (!IsSystemAdmin()) return Forbid();
            title = (title ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(title)) return Notify("اطلاعات ناقص", "عنوان منو الزامی است.", "warning");
            if (_permissionService.GetAllPermission().Any(menu => menu.PermissionTitle == title && menu.ParentID == parentId)) return Notify("رکورد تکراری", "منویی با همین عنوان در این سطح وجود دارد.", "warning");
            if (parentId.HasValue && !IsValidParent(parentId.Value)) return Notify("والد نامعتبر", "منوی والد انتخاب‌شده معتبر نیست.", "warning");
            _permissionService.AddPermission(new Permission { PermissionTitle = title, ParentID = parentId, ParentUrl = Clean(parentUrl), SubUrl = Clean(subUrl), MenuUrl = Clean(menuUrl), IconName = Clean(iconName), Order = order, IsActive = isActive, ShowAll = showAll });
            return Notify("ثبت موفق", "منوی جدید با موفقیت ثبت شد.", "success");
        }

        public IActionResult OnPostEdit(int permissionId, string title, int? parentId, string parentUrl, string subUrl, string menuUrl, string iconName, int order, bool isActive, bool showAll)
        {
            if (!IsSystemAdmin()) return Forbid();
            var menu = _permissionService.GetPermissionById(permissionId); title = (title ?? string.Empty).Trim();
            if (menu == null) return Notify("خطا", "منوی موردنظر یافت نشد.", "error");
            if (string.IsNullOrWhiteSpace(title)) return Notify("اطلاعات ناقص", "عنوان منو الزامی است.", "warning");
            if (parentId == permissionId || (parentId.HasValue && !IsValidParent(parentId.Value))) return Notify("والد نامعتبر", "یک منو نمی‌تواند فرزند خودش یا یک زیرمنو باشد.", "warning");
            if (_permissionService.GetAllPermission().Any(item => item.PermissionId != permissionId && item.PermissionTitle == title && item.ParentID == parentId)) return Notify("رکورد تکراری", "منویی با همین عنوان در این سطح وجود دارد.", "warning");
            menu.PermissionTitle = title; menu.ParentID = parentId; menu.ParentUrl = Clean(parentUrl); menu.SubUrl = Clean(subUrl); menu.MenuUrl = Clean(menuUrl); menu.IconName = Clean(iconName); menu.Order = order; menu.IsActive = isActive; menu.ShowAll = showAll;
            _permissionService.UpdatePermission(menu);
            return Notify("ویرایش موفق", "اطلاعات منو با موفقیت ویرایش شد.", "success");
        }

        public IActionResult OnPostDelete(int permissionId)
        {
            if (!IsSystemAdmin()) return Forbid();
            var menu = _permissionService.GetPermissionById(permissionId);
            if (menu == null) return Notify("خطا", "منوی موردنظر یافت نشد.", "error");
            if (_permissionService.IsPermissionInUse(permissionId)) return Notify("حذف انجام نشد", "این منو دارای زیرمنو یا دسترسی نقش است؛ ابتدا وابستگی‌ها را حذف کنید.", "warning");
            _permissionService.DeletePermission(permissionId);
            return Notify("حذف موفق", "منو با موفقیت حذف شد.", "success");
        }

        public IActionResult OnPostSaveOrder([FromBody] List<PermissionOrderViewModel> items)
        {
            if (!IsSystemAdmin()) return new JsonResult(new { success = false, message = "دسترسی غیرمجاز" }) { StatusCode = 403 };
            if (items == null || items.Count == 0) return new JsonResult(new { success = false, message = "چیدمان معتبری دریافت نشد." });
            var saved = _permissionService.UpdatePermissionOrder(items);
            return new JsonResult(new { success = saved, message = saved ? "ترتیب منوها ذخیره شد." : "ساختار ارسالی معتبر نیست؛ صفحه را تازه‌سازی و دوباره تلاش کنید." });
        }

        private void Load() { Menus = _permissionService.GetAllPermission().OrderBy(menu => menu.Order).ThenBy(menu => menu.PermissionTitle).ToList(); }
        private bool IsValidParent(int id) { var parent = _permissionService.GetPermissionById(id); return parent != null && !parent.ParentID.HasValue; }
        private bool IsSystemAdmin() => User.FindFirst("RoleTypeId")?.Value == "100";
        private static string Clean(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        private IActionResult Notify(string title, string message, string icon) { TempData["OperationTitle"] = title; TempData["OperationMessage"] = message; TempData["OperationIcon"] = icon; return RedirectToPage(); }
    }
}

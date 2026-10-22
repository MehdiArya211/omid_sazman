using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities;
using VisitorManagment.Web.Helpers;

namespace VisitorManagment.Web.Pages.Admin.LoginSlides
{
    /// <summary>
    /// مدیریت تصاویر صفحه ورود فقط برای مدیر سامانه.
    /// تصویر ورودی در مرورگر با نسبت ثابت قاب لاگین کراپ و به JPEG استاندارد تبدیل می‌شود.
    /// </summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private const int MaximumImageBytes = 8 * 1024 * 1024;
        private const string ManagedFolder = "LoginSlides";
        private readonly VisitorManagmentContext _context;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(VisitorManagmentContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public List<LoginSlide> Slides { get; private set; } = new List<LoginSlide>();

        public IActionResult OnGet()
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            LoadSlides();
            return Page();
        }

        public IActionResult OnPostCreate(string title, string description, string altText, int sortOrder,
            bool isActive, string croppedImageData)
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            var validation = ValidateFields(title, croppedImageData, imageIsRequired: true);
            if (validation != null) return RedirectWithMessage("ثبت انجام نشد", validation, "warning");

            var storedPath = SaveCroppedImage(croppedImageData);
            if (storedPath == null)
                return RedirectWithMessage("تصویر نامعتبر", "فایل تصویر معتبر نیست یا حجم آن بیش از حد مجاز است.", "error");

            _context.LoginSlides.Add(new LoginSlide
            {
                ImagePath = storedPath,
                Title = title.Trim(),
                Description = Clean(description),
                AltText = Clean(altText) ?? title.Trim(),
                SortOrder = sortOrder,
                IsActive = isActive,
                CreatedAt = DateTime.Now
            });
            _context.SaveChanges();
            return RedirectWithMessage("ثبت موفق", "تصویر جدید به اسلایدر صفحه ورود اضافه شد.", "success");
        }

        public IActionResult OnPostEdit(int id, string title, string description, string altText, int sortOrder,
            bool isActive, string croppedImageData)
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            var slide = _context.LoginSlides.FirstOrDefault(item => item.Id == id);
            if (slide == null) return RedirectWithMessage("خطا", "تصویر موردنظر یافت نشد.", "error");

            var validation = ValidateFields(title, croppedImageData, imageIsRequired: false);
            if (validation != null) return RedirectWithMessage("ویرایش انجام نشد", validation, "warning");

            string previousPath = null;
            if (!string.IsNullOrWhiteSpace(croppedImageData))
            {
                var newPath = SaveCroppedImage(croppedImageData);
                if (newPath == null)
                    return RedirectWithMessage("تصویر نامعتبر", "فایل تصویر معتبر نیست یا حجم آن بیش از حد مجاز است.", "error");

                previousPath = slide.ImagePath;
                slide.ImagePath = newPath;
            }

            slide.Title = title.Trim();
            slide.Description = Clean(description);
            slide.AltText = Clean(altText) ?? title.Trim();
            slide.SortOrder = sortOrder;
            slide.IsActive = isActive;
            slide.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            if (previousPath != null) DeleteManagedFile(previousPath);
            return RedirectWithMessage("ویرایش موفق", "اطلاعات تصویر صفحه ورود به‌روزرسانی شد.", "success");
        }

        public IActionResult OnPostDelete(int id)
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            var slide = _context.LoginSlides.FirstOrDefault(item => item.Id == id);
            if (slide == null) return RedirectWithMessage("خطا", "تصویر موردنظر یافت نشد.", "error");

            _context.LoginSlides.Remove(slide);
            _context.SaveChanges();
            DeleteManagedFile(slide.ImagePath);
            return RedirectWithMessage("حذف موفق", "تصویر از اسلایدر صفحه ورود حذف شد.", "success");
        }

        public IActionResult OnPostToggle(int id)
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            var slide = _context.LoginSlides.FirstOrDefault(item => item.Id == id);
            if (slide == null) return RedirectWithMessage("خطا", "تصویر موردنظر یافت نشد.", "error");
            slide.IsActive = !slide.IsActive;
            slide.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return RedirectWithMessage("وضعیت تغییر کرد", slide.IsActive ? "تصویر فعال شد." : "تصویر غیرفعال شد.", "success");
        }

        #region Image storage

        private string SaveCroppedImage(string dataUrl)
        {
            const string prefix = "data:image/jpeg;base64,";
            if (string.IsNullOrWhiteSpace(dataUrl) || !dataUrl.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return null;

            try
            {
                var bytes = Convert.FromBase64String(dataUrl.Substring(prefix.Length));
                if (bytes.Length == 0 || bytes.Length > MaximumImageBytes ||
                    bytes.Length < 3 || bytes[0] != 0xFF || bytes[1] != 0xD8 || bytes[2] != 0xFF)
                    return null;

                var folder = Path.Combine(_environment.WebRootPath, ManagedFolder);
                Directory.CreateDirectory(folder);
                var fileName = Guid.NewGuid().ToString("N") + ".jpg";
                System.IO.File.WriteAllBytes(Path.Combine(folder, fileName), bytes);
                return "/" + ManagedFolder + "/" + fileName;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private void DeleteManagedFile(string publicPath)
        {
            var expectedPrefix = "/" + ManagedFolder + "/";
            if (string.IsNullOrWhiteSpace(publicPath) || !publicPath.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
                return;

            var folder = Path.GetFullPath(Path.Combine(_environment.WebRootPath, ManagedFolder));
            var filePath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, publicPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));
            if (!filePath.StartsWith(folder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) return;
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
        }

        #endregion

        #region Helpers

        private void LoadSlides()
        {
            Slides = _context.LoginSlides.AsNoTracking()
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.Id)
                .ToList();
        }

        private static string ValidateFields(string title, string imageData, bool imageIsRequired)
        {
            if (string.IsNullOrWhiteSpace(title)) return "عنوان تصویر الزامی است.";
            if (title.Trim().Length > 150) return "عنوان تصویر نباید بیشتر از ۱۵۰ کاراکتر باشد.";
            if (imageIsRequired && string.IsNullOrWhiteSpace(imageData)) return "انتخاب و کراپ تصویر الزامی است.";
            return null;
        }

        private static string Clean(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private IActionResult RedirectWithMessage(string title, string message, string icon)
        {
            TempData["OperationTitle"] = title;
            TempData["OperationMessage"] = message;
            TempData["OperationIcon"] = icon;
            return RedirectToPage();
        }

        #endregion
    }
}

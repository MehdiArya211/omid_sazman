using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VisitorManagment.DataLayer;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities;
using VisitorManagment.Web.Helpers;

namespace VisitorManagment.Web.Pages.Admin.Notifications
{
    /// <summary>ثبت، ویرایش و حذف اعلان‌های عمومی سامانه و فایل‌های پیوست آن‌ها.</summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private const long MaximumFileSize = 10 * 1024 * 1024;
        private const string AttachmentFolder = "NotificationAttachment";
        private static readonly HashSet<string> AllowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".zip" };

        private readonly VisitorManagmentContext _context;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(VisitorManagmentContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public List<Notification> Notifications { get; private set; } = new List<Notification>();
        public Dictionary<int, int> ViewCounts { get; private set; } = new Dictionary<int, int>();

        public IActionResult OnGet()
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            LoadData();
            return Page();
        }

        public IActionResult OnPostCreate(string title, string content, bool isActive, List<IFormFile> attachments)
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            var validation = Validate(title, content, attachments);
            if (validation != null) return Notify("ثبت انجام نشد", validation, "warning");

            var notification = new Notification
            {
                Title = title.Trim(), Content = content.Trim(), IsActive = isActive,
                PublishDate = DateTime.Now, Attachments = new List<NotificationAttachment>()
            };
            var savedFiles = SaveFiles(attachments);
            if (!savedFiles.Success) return Notify("فایل نامعتبر", savedFiles.Error, "warning");
            foreach (var attachment in savedFiles.Files) notification.Attachments.Add(attachment);

            _context.Notifications.Add(notification);
            try { _context.SaveChanges(); }
            catch
            {
                savedFiles.Files.ForEach(item => DeleteManagedFile(item.FilePath));
                return Notify("خطا در ثبت", "ذخیره اعلان انجام نشد. دوباره تلاش کنید.", "error");
            }
            return Notify("ثبت موفق", "اعلان جدید ثبت شد و برای کاربران قابل مشاهده است.", "success");
        }

        public IActionResult OnPostEdit(int id, string title, string content, bool isActive, List<IFormFile> attachments)
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            var notification = _context.Notifications.Include(item => item.Attachments).FirstOrDefault(item => item.Id == id);
            if (notification == null) return Notify("خطا", "اعلان موردنظر پیدا نشد.", "error");
            var validation = Validate(title, content, attachments);
            if (validation != null) return Notify("ویرایش انجام نشد", validation, "warning");

            var savedFiles = SaveFiles(attachments);
            if (!savedFiles.Success) return Notify("فایل نامعتبر", savedFiles.Error, "warning");
            notification.Title = title.Trim();
            notification.Content = content.Trim();
            notification.IsActive = isActive;
            foreach (var attachment in savedFiles.Files) notification.Attachments.Add(attachment);
            try { _context.SaveChanges(); }
            catch
            {
                savedFiles.Files.ForEach(item => DeleteManagedFile(item.FilePath));
                return Notify("خطا در ویرایش", "ذخیره تغییرات اعلان انجام نشد.", "error");
            }
            return Notify("ویرایش موفق", "اطلاعات اعلان به‌روزرسانی شد.", "success");
        }

        public IActionResult OnPostDelete(int id)
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            var notification = _context.Notifications.Include(item => item.Attachments).FirstOrDefault(item => item.Id == id);
            if (notification == null) return Notify("خطا", "اعلان موردنظر پیدا نشد.", "error");
            var paths = notification.Attachments.Select(item => item.FilePath).ToList();
            _context.Notifications.Remove(notification);
            _context.SaveChanges();
            paths.ForEach(DeleteManagedFile);
            return Notify("حذف موفق", "اعلان و سوابق مشاهده آن حذف شد.", "success");
        }

        public IActionResult OnPostDeleteAttachment(int attachmentId)
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            var attachment = _context.NotificationAttachments.FirstOrDefault(item => item.Id == attachmentId);
            if (attachment == null) return Notify("خطا", "فایل پیوست پیدا نشد.", "error");
            var path = attachment.FilePath;
            _context.NotificationAttachments.Remove(attachment);
            _context.SaveChanges();
            DeleteManagedFile(path);
            return Notify("حذف موفق", "فایل پیوست حذف شد.", "success");
        }

        private void LoadData()
        {
            Notifications = _context.Notifications.AsNoTracking().Include(item => item.Attachments)
                .OrderByDescending(item => item.PublishDate).ThenByDescending(item => item.Id).ToList();
            ViewCounts = _context.NotificationUsers.AsNoTracking().GroupBy(item => item.NotificationId)
                .ToDictionary(group => group.Key, group => group.Count());
        }

        private static string Validate(string title, string content, IEnumerable<IFormFile> attachments)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content)) return "عنوان و متن اعلان الزامی است.";
            if (title.Trim().Length > 100) return "عنوان اعلان نباید بیشتر از ۱۰۰ کاراکتر باشد.";
            if (content.Trim().Length > 10000) return "متن اعلان نباید بیشتر از ۱۰ هزار کاراکتر باشد.";
            foreach (var file in attachments ?? Enumerable.Empty<IFormFile>())
            {
                if (file == null || file.Length == 0) continue;
                if (file.Length > MaximumFileSize) return $"حجم فایل «{file.FileName}» بیشتر از ۱۰ مگابایت است.";
                if (!AllowedExtensions.Contains(Path.GetExtension(file.FileName))) return $"نوع فایل «{file.FileName}» مجاز نیست.";
            }
            return null;
        }

        private (bool Success, string Error, List<NotificationAttachment> Files) SaveFiles(IEnumerable<IFormFile> uploads)
        {
            var result = new List<NotificationAttachment>();
            try
            {
                var folder = Path.Combine(_environment.WebRootPath, AttachmentFolder);
                Directory.CreateDirectory(folder);
                foreach (var file in uploads ?? Enumerable.Empty<IFormFile>())
                {
                    if (file == null || file.Length == 0) continue;
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    var storedName = Guid.NewGuid().ToString("N") + extension;
                    using (var stream = System.IO.File.Create(Path.Combine(folder, storedName))) file.CopyTo(stream);
                    result.Add(new NotificationAttachment
                    {
                        FileName = Path.GetFileName(file.FileName), FilePath = "/" + AttachmentFolder + "/" + storedName,
                        FileType = file.ContentType, FileSize = file.Length
                    });
                }
                return (true, null, result);
            }
            catch
            {
                result.ForEach(item => DeleteManagedFile(item.FilePath));
                return (false, "ذخیره فایل پیوست انجام نشد. دوباره تلاش کنید.", new List<NotificationAttachment>());
            }
        }

        private void DeleteManagedFile(string publicPath)
        {
            var prefix = "/" + AttachmentFolder + "/";
            if (string.IsNullOrWhiteSpace(publicPath) || !publicPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return;
            var folder = Path.GetFullPath(Path.Combine(_environment.WebRootPath, AttachmentFolder));
            var filePath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, publicPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));
            try
            {
                if (filePath.StartsWith(folder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            catch { /* حذف فایل نباید نتیجه ثبت‌شده در دیتابیس را به خطا تبدیل کند. */ }
        }

        private IActionResult Notify(string title, string message, string icon)
        {
            TempData["OperationTitle"] = title; TempData["OperationMessage"] = message; TempData["OperationIcon"] = icon;
            return RedirectToPage();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VisitorManagment.DataLayer;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.NotificationInfo;

namespace VisitorManagment.Web.Pages.Notifications
{
    /// <summary>مرکز اعلان‌های کاربر و ثبت قطعی مشاهده هر اعلان.</summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly VisitorManagmentContext _context;
        public IndexModel(VisitorManagmentContext context) { _context = context; }

        public List<Notification> Notifications { get; private set; } = new List<Notification>();
        public HashSet<int> ReadNotificationIds { get; private set; } = new HashSet<int>();

        public IActionResult OnGet()
        {
            if (!TryGetUserId(out var userId)) return Forbid();
            Notifications = _context.Notifications.AsNoTracking().Include(item => item.Attachments)
                .Where(item => item.IsActive && item.PublishDate <= DateTime.Now)
                .OrderByDescending(item => item.PublishDate).ThenByDescending(item => item.Id).ToList();
            ReadNotificationIds = _context.NotificationUsers.AsNoTracking().Where(item => item.UserId == userId)
                .Select(item => item.NotificationId).ToHashSet();
            return Page();
        }

        public IActionResult OnPostMarkRead(int notificationId)
        {
            if (!TryGetUserId(out var userId)) return new JsonResult(new { success = false, message = "هویت کاربر معتبر نیست." });
            var exists = _context.Notifications.Any(item => item.Id == notificationId && item.IsActive && item.PublishDate <= DateTime.Now);
            if (!exists) return new JsonResult(new { success = false, message = "اعلان موردنظر در دسترس نیست." });

            if (!_context.NotificationUsers.Any(item => item.NotificationId == notificationId && item.UserId == userId))
            {
                var viewRecord = new NotificationUser { NotificationId = notificationId, UserId = userId, ViewedDate = DateTime.Now };
                _context.NotificationUsers.Add(viewRecord);
                try { _context.SaveChanges(); }
                catch (DbUpdateException) { _context.Entry(viewRecord).State = EntityState.Detached; /* درخواست هم‌زمان قبلاً مشاهده را ثبت کرده است. */ }
            }

            var unreadCount = _context.Notifications.Count(item => item.IsActive && item.PublishDate <= DateTime.Now &&
                !_context.NotificationUsers.Any(view => view.NotificationId == item.Id && view.UserId == userId));
            return new JsonResult(new { success = true, unreadCount });
        }

        private bool TryGetUserId(out int userId) => int.TryParse(User.FindFirst("Id")?.Value, out userId) && userId > 0;
    }
}

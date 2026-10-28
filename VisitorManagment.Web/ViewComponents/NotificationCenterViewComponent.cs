using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorManagment.DataLayer;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.Web.ViewComponents
{
    public class NotificationCenterViewModel
    {
        public int UnreadCount { get; set; }
        public Notification FirstUnread { get; set; }
    }

    /// <summary>نشان اعلان و اولین اعلان خوانده‌نشده را در تمام صفحات سامانه نمایش می‌دهد.</summary>
    public class NotificationCenterViewComponent : ViewComponent
    {
        private readonly VisitorManagmentContext _context;
        public NotificationCenterViewComponent(VisitorManagmentContext context) { _context = context; }

        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            var unreadQuery = _context.Notifications.AsNoTracking()
                .Where(item => item.IsActive && item.PublishDate <= DateTime.Now &&
                    !_context.NotificationUsers.Any(view => view.NotificationId == item.Id && view.UserId == userId));
            var model = new NotificationCenterViewModel
            {
                UnreadCount = await unreadQuery.CountAsync(),
                FirstUnread = await unreadQuery.Include(item => item.Attachments)
                    .OrderBy(item => item.PublishDate).ThenBy(item => item.Id).FirstOrDefaultAsync()
            };
            return View("Default", model);
        }
    }
}

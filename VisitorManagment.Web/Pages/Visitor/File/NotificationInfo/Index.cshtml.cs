using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Core.Services.Notification;
using VisitorManagment.DataLayer;
using VisitorManagment.DataLayer.Entities.NotificationInfo;

namespace VisitorManagment.Web.Pages.Visitor.File.NotificationInfo
{
    public class IndexModel : PageModel
    {
        private readonly INotificationManager _notificationService;

        public IndexModel(INotificationManager notificationService)
        {
            _notificationService = notificationService;
        }


        public List<Notification> model { get; set; } 
        public void OnGet()
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            model = _notificationService.GetAllNotificationNoWatchingUser(userId).Model;

        }
    }
}

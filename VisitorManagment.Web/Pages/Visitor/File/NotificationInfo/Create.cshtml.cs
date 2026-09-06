using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs.Notification;
using VisitorManagment.Core.Services.Notification;

namespace VisitorManagment.Web.Pages.Visitor.File.NotificationInfo
{
    public class CreateModel : PageModel
    {
        private readonly INotificationManager _notificationManager;
        public CreateModel(INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }
        [BindProperty]
        public NotificationViewModel Notification { get; set; }
        #region اعضا و متدهای کلاس


        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public void OnGet()
        {
        }

        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

         //   var result = _notificationManager.Create();

            return Page();
        }
        #endregion
    }
}

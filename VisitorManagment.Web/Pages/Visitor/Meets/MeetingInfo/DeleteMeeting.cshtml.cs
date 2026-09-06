using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Visitor.File.MeetingInfo
{

    [Authorize]
    public class DeleteMeetingModel : PageModel
    {
        private readonly IMeetingService _meetingservice;
        public DeleteMeetingModel(IMeetingService meetingservice)
        {
            _meetingservice = meetingservice;
        }
        [BindProperty]
        public DeleteMeetingViewModel deleteMeetingViewModel { get; set; }
        #region اعضا و متدهای کلاس

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public void OnGet(int id)
        {
            deleteMeetingViewModel = _meetingservice.GetMeetingInformation(id);
        }
        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost(int Id)
        {
            if (Id <= 0)
            {
                TempData["OperationTitle"] = "خطا در حذف";
                TempData["OperationMessage"] = "شناسه جلسه معتبر نیست.";
                TempData["OperationIcon"] = "error";
                return RedirectToPage("/Visitor/Meets/MeetingInfo/Index");
            }

            _meetingservice.DeleteMeeting(Id);
            TempData["OperationTitle"] = "حذف موفق";
            TempData["OperationMessage"] = "جلسه با موفقیت حذف شد.";
            TempData["OperationIcon"] = "success";
            return RedirectToPage("/Visitor/Meets/MeetingInfo/Index");
        }
        #endregion
    }
}

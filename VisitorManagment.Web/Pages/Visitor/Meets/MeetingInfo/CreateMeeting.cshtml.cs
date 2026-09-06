using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Web.Pages.Visitor.File.MeetingInfo
{
    [Authorize]
    public class CreateMeetingModel : PageModel
    {
        private readonly IMeetingService _meetingService;
        public CreateMeetingModel(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }
        [BindProperty]
        public Meeting meetingViewModel { get; set; }
        #region اعضا و متدهای کلاس

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public void OnGet()
        {
            ViewData["MeetingPlaceList"] = new SelectList(_meetingService.GetMeetingPlace(), "Id", "Title");
            ViewData["BoseMeetingList"] = new SelectList(_meetingService.GetBoseMeeting(), "Id", "FullName");
            ViewData["ClerkMeetingList"] = new SelectList(_meetingService.GetClerkMeeting(), "Id", "FullName");
            ViewData["MeetingStatusList"] = new SelectList(_meetingService.GetMeetingStatus(), "Id", "Title");
        }

        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost(int meetingStatusId, int meetingPlaceId, int boseMeetingId, int clerkMeetingId)
        {
            OnGet();
            if (!ModelState.IsValid)
                return Page();
            if (meetingStatusId <= 0 || meetingPlaceId <= 0 || boseMeetingId <= 0 || clerkMeetingId <= 0)
            {
                ModelState.AddModelError("", "وضعیت، محل، رئیس و منشی جلسه باید انتخاب شوند.");
                return Page();
            }

            var userid = int.Parse(User.FindFirst("Id").Value);
            meetingViewModel.MeetingStatusId = meetingStatusId;
            meetingViewModel.MeetingPlaceId = meetingPlaceId;
            meetingViewModel.BoseMeetingId = boseMeetingId;
            meetingViewModel.ClerkMeetingId = clerkMeetingId;
            _meetingService.CreateMeeting(userid, meetingViewModel);
            TempData["OperationTitle"] = "ثبت موفق";
            TempData["OperationMessage"] = "جلسه با موفقیت ثبت شد.";
            TempData["OperationIcon"] = "success";
            return RedirectToPage("/Visitor/Meets/MeetingInfo/Index");

        }
        #endregion
    }
}

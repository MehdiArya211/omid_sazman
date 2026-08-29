using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Visitor.File.MeetingInfo
{
    [Authorize]
    public class EditMeetingModel : PageModel
    {
        private readonly IMeetingService _meetingService;
        private readonly ISmsService _smsService;
        public EditMeetingModel(IMeetingService meetingService, ISmsService smsService)
        {
            _meetingService = meetingService;
            _smsService = smsService;
        }
        [BindProperty]
        public EditMeetingViewModel editMeetingViewModel { get; set; }
        #region اعضا و متدهای کلاس

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public void OnGet(int id)
        {
            ViewData["MeetingPlaceList"] = new SelectList(_meetingService.GetMeetingPlace(), "Id", "Title");
            ViewData["BoseMeetingList"] = new SelectList(_meetingService.GetBoseMeeting(), "Id", "FullName");
            ViewData["ClerkMeetingList"] = new SelectList(_meetingService.GetClerkMeeting(), "Id", "FullName");
            ViewData["MeetingStatusList"] = new SelectList(_meetingService.GetMeetingStatus(), "Id", "Title");
            editMeetingViewModel = _meetingService.GetMeetingForEdit(id);
        }

        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                OnGet(editMeetingViewModel?.Id ?? 0);
                return Page();
            }
            if (editMeetingViewModel.Id <= 0 || editMeetingViewModel.MeetingStatusId <= 0 ||
                editMeetingViewModel.MeetingPlaceId <= 0 || editMeetingViewModel.BoseMeetingId <= 0 ||
                editMeetingViewModel.ClerkMeetingId <= 0)
            {
                ModelState.AddModelError("", "اطلاعات جلسه و گزینه‌های انتخابی را کامل کنید.");
                OnGet(editMeetingViewModel.Id);
                return Page();
            }

            editMeetingViewModel.EditUserId = int.Parse(User.FindFirst("Id").Value);
            //edit table Meeting
            _meetingService.EditMeeting(editMeetingViewModel);
            _smsService.SendSmsToMemberAddToMeeting(editMeetingViewModel.Id);

            TempData["OperationTitle"] = "ویرایش موفق";
            TempData["OperationMessage"] = "جلسه با موفقیت ویرایش شد.";
            TempData["OperationIcon"] = "success";
            return RedirectToPage("/Visitor/Meets/MeetingInfo/Index");
        }
        #endregion
    }
}

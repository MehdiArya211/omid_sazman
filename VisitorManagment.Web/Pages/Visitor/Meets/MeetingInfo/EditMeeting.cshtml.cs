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
        public void OnGet(int id)
        {
            ViewData["MeetingPlaceList"] = new SelectList(_meetingService.GetMeetingPlace(), "Id", "Title");
            ViewData["BoseMeetingList"] = new SelectList(_meetingService.GetBoseMeeting(), "Id", "FullName");
            ViewData["ClerkMeetingList"] = new SelectList(_meetingService.GetClerkMeeting(), "Id", "FullName");
            ViewData["MeetingStatusList"] = new SelectList(_meetingService.GetMeetingStatus(), "Id", "Title");
            editMeetingViewModel = _meetingService.GetMeetingForEdit(id);
        }

        public IActionResult OnPost()
        {
            editMeetingViewModel.EditUserId = int.Parse(User.FindFirst("Id").Value);
            //edit table Meeting
            _meetingService.EditMeeting(editMeetingViewModel);
            _smsService.SendSmsToMemberAddToMeeting(editMeetingViewModel.Id);

            return RedirectToPage("/Visitor/Meets/MeetingInfo/Index");
        }
    }
}

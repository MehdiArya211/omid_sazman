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
        public void OnGet()
        {
            ViewData["MeetingPlaceList"] = new SelectList(_meetingService.GetMeetingPlace(), "Id", "Title");
            ViewData["BoseMeetingList"] = new SelectList(_meetingService.GetBoseMeeting(), "Id", "FullName");
            ViewData["ClerkMeetingList"] = new SelectList(_meetingService.GetClerkMeeting(), "Id", "FullName");
            ViewData["MeetingStatusList"] = new SelectList(_meetingService.GetMeetingStatus(), "Id", "Title");
        }

        public IActionResult OnPost(int meetingStatusId, int meetingPlaceId, int boseMeetingId, int clerkMeetingId)
        {

            var userid = int.Parse(User.FindFirst("Id").Value);
            meetingViewModel.MeetingStatusId = meetingStatusId;
            meetingViewModel.MeetingPlaceId = meetingPlaceId;
            meetingViewModel.BoseMeetingId = boseMeetingId;
            meetingViewModel.ClerkMeetingId = clerkMeetingId;
            _meetingService.CreateMeeting(userid, meetingViewModel);
            ViewData["successcreate"] = true;
            return Page();

        }
    }
}

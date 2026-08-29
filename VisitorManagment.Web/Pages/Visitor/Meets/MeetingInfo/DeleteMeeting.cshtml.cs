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
        public void OnGet(int id)
        {
            deleteMeetingViewModel = _meetingservice.GetMeetingInformation(id);
        }
        public IActionResult OnPost(int Id)
        {
            _meetingservice.DeleteMeeting(Id);
            return RedirectToPage("/Visitor/Meets/MeetingInfo/Index");
        }
    }
}

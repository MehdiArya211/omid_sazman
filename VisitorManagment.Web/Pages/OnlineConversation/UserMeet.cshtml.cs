using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.OnlineConversation
{
    [Authorize]
    public class UserMeetModel : PageModel
    {
        private readonly IMeetingService _meetingService;
        private readonly IWebApiService _webApiService;
        public UserMeetModel(IMeetingService meetingService , IWebApiService webApiService)
        {
            _meetingService = meetingService;
            _webApiService  =webApiService;
        }

        [BindProperty]
        public ListMeetingViewModel listMeetingViewModel { get; set; }
        public void OnGet(int pageId = 1, int filterMeetingStatus = 1, string filterCaption = "")
        {
            ViewData["UnitCode"]  = User.FindFirst("UnitCode").Value; 
            listMeetingViewModel = _meetingService.GetListMeeting(pageId, filterMeetingStatus, filterCaption);
            ViewData["Meets"] = new SelectList(_meetingService.GetListMeetingForOnlineConversation().Meetings, "Id", "Title");
            ViewData["Organ"] = new SelectList(_webApiService.GetAllOrgan1().Data, "Id", "Title");

        }

        public JsonResult OnGetYeganList(int id)
        {
            var meetingId = id;
            var result = _meetingService.GetListOrganMemberMeeting(meetingId);

            return new JsonResult(result);
        }
    }
}

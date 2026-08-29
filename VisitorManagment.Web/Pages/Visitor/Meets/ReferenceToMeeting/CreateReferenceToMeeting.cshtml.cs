using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Visitor.Meets.ReferenceToMeeting
{
    [Authorize]
    public class CreateReferenceToMeetingModel : PageModel
    {
        private readonly IMeetingService _meetingService;
        private readonly IWebApiService _webApiService;


        public CreateReferenceToMeetingModel(IMeetingService MeetingService, IWebApiService webApiService)
        {
            _meetingService = MeetingService;
            _webApiService = webApiService;

        }
        [BindProperty]
        public ListFileReferenceViewModel listviewmodel { get; set; }
        public void OnGet(int pageId = 1, string filterCaption = "", int SubjectId = 0, int filterCodGha = 0 , string filterGharargah = "")
        {
            ViewData["RequestSubjects"] = new SelectList(_meetingService.GetRequestSubjects(), "Id", "Title");
            ViewData["CodGhaTitle"] = new SelectList(_meetingService.GetCodGhaTitle(), "CodGha", "CodGhaTitle");
            ViewData["MeetingTitleList"] = new SelectList(_meetingService.GetMeetingTitle(), "MeetingId", "MeetingTitle");
           // ViewData["GharargahList"] = new SelectList(_webApiService.GetGharargah(), "Id", "Title");

            var rcvrUserId = int.Parse(User.FindFirst("Id").Value);
            listviewmodel = _meetingService.GetListFileForReference(rcvrUserId, pageId, filterCaption, SubjectId, filterCodGha, filterGharargah);

        }

        public IActionResult OnPost(List<int> fileId, int MeetingId)
        {
            _meetingService.AddMeetingIdToFile(fileId, MeetingId);

            if (fileId.Count == 0)
            {
                //زمانیکه کسی رو اضافه نکرده بود
                ViewData["successcreate"] = false;
            }
            else
            {
                ViewData["successcreate"] = true;
            }
            return Page();

        }
    }
}

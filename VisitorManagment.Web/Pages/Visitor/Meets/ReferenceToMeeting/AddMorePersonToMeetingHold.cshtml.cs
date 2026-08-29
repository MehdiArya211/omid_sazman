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
    public class AddMorePersonToMeetingHoldModel : PageModel
    {
        private readonly IMeetingService _meetingService;
        private readonly IWebApiService _webApiService;


        public AddMorePersonToMeetingHoldModel(IMeetingService MeetingService , IWebApiService webApiService)
        {
            _meetingService = MeetingService;
            _webApiService = webApiService;

        }
        [BindProperty]
        public ListFileReferenceViewModel listviewmodel { get; set; }
        #region اعضا و متدهای کلاس

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public void OnGet(int id, int pageId = 1, string filterCaption = "", int SubjectId = 0, int filterCodGha = 0)
        {
            ViewData["RequestSubjects"] = new SelectList(_meetingService.GetRequestSubjects(), "Id", "Title");
            ViewData["CodGhaTitle"] = new SelectList(_meetingService.GetCodGhaTitle(), "CodGha", "CodGhaTitle");
            ViewData["MeetingTitleList"] = new SelectList(_meetingService.GetMeetingTitle(), "MeetingId", "MeetingTitle");
            //ViewData["GharargahList"] = new SelectList(_webApiService.GetGharargah(), "Id", "Title");

            var userid = int.Parse(User.FindFirst("Id").Value);
            listviewmodel = _meetingService.GetListFileForAddPersonToMeeting(userid,id, pageId, filterCaption, SubjectId, filterCodGha);

        }

        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost(List<int> fileId)
        {
            int meetingId = listviewmodel.MeetingId;

            _meetingService.AddMeetingIdToFile(fileId, meetingId);


            if (fileId.Count==0)
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

        #endregion
    }
}


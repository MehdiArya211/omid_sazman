using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Web.Pages.Visitor.Meets.ReferenceToMeeting
{
    [Authorize]
    public class EditReferenceToMeetingModel : PageModel
    {
        private readonly IMeetingService _meetingService;
        private readonly IWorkFlowService _workFlowService;
        private readonly ICartableService _cartableService;
        private readonly IHameshService _hameshService;


        public EditReferenceToMeetingModel(IMeetingService MeetingService , IWorkFlowService workFlowService , ICartableService cartableService , IHameshService hameshService)
        {
            _meetingService = MeetingService;
            _workFlowService = workFlowService;
            _hameshService = hameshService;
            _cartableService = cartableService;

        }
        [BindProperty]
        public ListFileReferenceViewModel listviewmodel { get; set; }
        public List<Users> users { get; set; }
       // public List<int> listFileViewModel { get; set; }
        #region اعضا و متدهای کلاس

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public void OnGet(int id, int pageId = 1, string filterCaption = "", int SubjectId = 0, int filterCodGha = 0)
        {
            ViewData["RequestSubjects"] = new SelectList(_meetingService.GetRequestSubjects(), "Id", "Title");
            ViewData["CodGhaTitle"] = new SelectList(_meetingService.GetCodGhaTitle(), "CodGha", "CodGhaTitle");
            ViewData["MeetingTitleList"] = new SelectList(_meetingService.GetMeetingTitle(), "MeetingId", "MeetingTitle");

            var userid = int.Parse(User.FindFirst("Id").Value);
            listviewmodel = _meetingService.GetListFileForEditReference(id, pageId, filterCaption, SubjectId, filterCodGha);
            users = new List<Users>();

        }


        /// <summary>
        /// درخواست ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPostDeletePersonFromMeeting(int FileId)
        {
            _meetingService.DeletePersonInMeeting(FileId);

            return RedirectToPage("/Visitor/Meets/ReferenceToMeeting/EditReferenceToMeeting");

        }
        #endregion
    }
}

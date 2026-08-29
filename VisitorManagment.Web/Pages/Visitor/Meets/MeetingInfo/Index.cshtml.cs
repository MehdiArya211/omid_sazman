using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Web.Pages.Visitor.MeetingInfo
{

    [Authorize]
    public class IndexModel : PageModel
    {

        private readonly IMeetingService _meetingService;
        private readonly IWorkFlowService _workFlowService;
        private readonly ICartableService _cartableService;
        private readonly IHameshService _hameshService;


        public IndexModel(IMeetingService MeetingService, IWorkFlowService workFlowService, ICartableService cartableService, IHameshService hameshService)
        {
            _meetingService = MeetingService;
            _workFlowService = workFlowService;
            _hameshService = hameshService;
            _cartableService = cartableService;

        }
        [BindProperty]
        public ListMeetingViewModel listMeetingViewModel { get; set; }
        public List<Users> users { get; set; }

        public void OnGet(int pageId = 1, int filterMeetingStatus = 1, string filterCaption = "")
        {
            users = new List<Users>();

            ViewData["MeetingPlaceList"] = new SelectList(_meetingService.GetMeetingPlace(), "Id", "Title");
            ViewData["BoseMeetingList"] = new SelectList(_meetingService.GetBoseMeeting(), "Id", "FullName");
            ViewData["ClerkMeetingList"] = new SelectList(_meetingService.GetClerkMeeting(), "Id", "FullName");
            ViewData["MeetingStatusList"] = new SelectList(_meetingService.GetMeetingStatus(), "Id", "Title");
            //ViewData["Organ"] = new SelectList(_meetingService.GetMeetingStatus(), "Id", "Title");
            listMeetingViewModel = _meetingService.GetListMeeting(pageId, filterMeetingStatus, filterCaption);
        }


        public IActionResult OnGetGetRecieverUser(int meetingId)
        {
            //session meetingId
            HttpContext.Session.SetString("sessionMeetingId", meetingId.ToString());
            Response.Cookies.Append("sessionMeetingId", meetingId.ToString());
            //end session

            int userId = int.Parse(User.FindFirst("Id").Value);
            int roleId = int.Parse(User.FindFirst("RoleId").Value);
            int unitDutyCode = int.Parse(User.FindFirst("UnitDutyCode").Value);
            int unitCode = int.Parse(User.FindFirst("UnitCode").Value);

            var result = _workFlowService.GetRecieverFarmandehiNezajaList(roleId);

            return new PartialViewResult
            {
                ViewName = "_GridSendListToFarmandeNezaja",
                ViewData = new ViewDataDictionary<List<Users>>(ViewData, result)
            };
        }


        public IActionResult OnPostSendFileToFarmandehiNezaja(List<int> rcvrUserId)
        {
            int userId = int.Parse(User.FindFirst("Id").Value);
            var meetingId = HttpContext.Session.GetString("sessionMeetingId");

            var files = _meetingService.GetListFileByMeetingId(int.Parse(meetingId));

            var meeting = _meetingService.GetMeetingByMeetingId(int.Parse(meetingId));

            meeting.IsSend = true;
            //وقتی به هیئت رییسه جلسه ارسال شد آپدیت کنه ملاقات رو
            _meetingService.UpdateMeeting(meeting);

            //get roletyprId && roleTypetitle
            var RoleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value); 
            var RoleTypeTitle = User.FindFirst("RoleTypeTitle").Value;
            var RoleTypeIdFinal = int.Parse(User.FindFirst("RoleTypeIdFinal").Value);
            var RoleTypeTitleFinal = User.FindFirst("RoleTypeTitleFinal").Value;

            //********************

            int sndUserId = int.Parse(User.FindFirst("Id").Value);


            //add to table cartable
            _cartableService.SendListFileToCartable(rcvrUserId, sndUserId, files);

            _hameshService.AddToHameshWhenSendListFileToCartable(sndUserId, files, rcvrUserId, RoleTypeId, RoleTypeTitle);
            // یه هامش خالی باید برای کسی که میفرستیم هم بزنیم
           _hameshService.AddToHameshWhenSendListFileToFarmandehiNezaja(rcvrUserId, files, RoleTypeId, RoleTypeTitle , RoleTypeIdFinal , RoleTypeTitleFinal , sndUserId);

            ViewData["successcreate"] = true;

            return Page();
        }

        public IActionResult OnPost()
        {
            return Page();
        }

        #region autocomplete search
        public IActionResult OnGetSearch(string term)
        {
            var names = _meetingService.GetMeetingForAutoCompliteSearch(term);
            return new JsonResult(names);
        }
        #endregion

        #region تغییر وضعیت جلسات ملاقات
        public IActionResult OnGetChangeStatusMeeting(int meetingId)
        {
            var result = _meetingService.ChangeStatusMeeting(meetingId);

            return new JsonResult(result.Status);
        }
        #endregion
    }
}

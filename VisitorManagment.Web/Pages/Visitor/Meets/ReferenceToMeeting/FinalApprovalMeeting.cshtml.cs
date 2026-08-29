using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITOWebApiClient;
using ITOWebApiClient.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Visitor.Meets.ReferenceToMeeting
{
    [Authorize]
    public class FinalApprovalMeetingModel : PageModel
    {
        private readonly IMeetingService _meetingService;
        private readonly ISmsService _smsService;

        private readonly ApiTokenCacheClient _apiTokenClient;

        public FinalApprovalMeetingModel(IMeetingService MeetingService, ApiTokenCacheClient apiTokenClient , ISmsService smsService)
        {
            _meetingService = MeetingService;
            _apiTokenClient = apiTokenClient;
            _smsService = smsService;

        }
        [BindProperty]
        public ListFileReferenceViewModel listviewmodel { get; set; }
        public ListSMSInfoViewModel listSMSInfoViewModel { get; set; }

        public void OnGet(int id)
        {
            listviewmodel = _meetingService.GetFileForFinalApprovalMeeting(id);
        }
        public IActionResult OnPost(int MeetingId)
        {
            //listSMSInfoViewModel = _smsService.GetFileForSMSInfo(MeetingId);
            //var access_token = _apiTokenClient.GetApiToken(
            //      CustomSettings.Instance.ClientId,
            //      CustomSettings.Instance.Scope,
            //      CustomSettings.Instance.ClientSecret,
            //      CustomSettings.Instance.ROPC_UserName,
            //      CustomSettings.Instance.ROPC_Password
            //  ).Result;

            //foreach (var item in listSMSInfoViewModel.sMSInfos)
            //{
            //    _smsService.AddSMSInfo(item, access_token);
            //}

            _smsService.SendSmsToMemberAddToMeeting(MeetingId);

            var userid = int.Parse(User.FindFirst("Id").Value);
            _meetingService.AddFinalApprovalMeeting(MeetingId, userid);

            return RedirectToPage("/Visitor/Meets/MeetingInfo/Index");
          
        }
    }
}

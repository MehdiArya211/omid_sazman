using System.Collections.Generic;
using System.Linq;
using ITOWebApiClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Visitor.File.MeetingHold
{
    [Authorize]
    public class CreateMeetingHoldModel : PageModel
    {
        private readonly IMeetingService _meetingService;
        private readonly IPermissionService _permissionservice;
        private readonly IHameshService _hameshService;
        private readonly ISmsService _smsService;
        private readonly ApiTokenCacheClient _apiTokenClient;
        public CreateMeetingHoldModel(IMeetingService meetingService, IPermissionService permissionService, IHameshService hameshService , ISmsService smsService , ApiTokenCacheClient apiTokenClient)
        {
            _meetingService = meetingService;
            _permissionservice = permissionService;
            _hameshService = hameshService;
            _smsService = smsService;
            _apiTokenClient = apiTokenClient;
        }
        [BindProperty]
        public MeetingHoldViewModel meetingHoldViewModel { get; set; }
        public SMSInfoViewModel smsInfoViewModel { get; set; }
        #region اعضا و متدهای کلاس


        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public void OnGet(int id)
        {

            ViewData["ActionType"] = new SelectList(_hameshService.GetActionType(), "Id", "Title");
            ViewData["MeetingForHold"] = _meetingService.GetMeetingList(id).Select(a =>
                       new SelectListItem
                       {
                           //value hamishe bayad string bashe
                           Value = a.Id.ToString(),
                           Text = a.Title + " ** " + a.StartMeetingDate
                       }).ToList();

        }

        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public JsonResult OnGetGetHamesh(int membermeetingid, int titlemeetingid)
        {
            var result = _meetingService.getHmaeshByFileId(membermeetingid, titlemeetingid);
            return new JsonResult(result);

        }

        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost(int meetingId, int personId, int ActionTypeId, List<int> rcvrId)

        {
            var userid = int.Parse(User.FindFirst("Id").Value);

            //get Role Type
            
            var roleTypeTitle = _hameshService.GetRoleTypePerson(userid).RoleTypeTitle;

            var fileId = _meetingService.GetFileIdByMeetingIdAndPersonId(meetingId, personId);

            var hamesh = _hameshService.GetHameshByUserIdAndFileId(userid, fileId);

            meetingHoldViewModel.ActionTypeId = ActionTypeId;

            return RedirectToPage("/Visitor/Meets/MeetingInfo/MeetingHold/CreateMeetingHold");
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Visitor.Meets.MeetingInfo.MeetingHold
{
    [Authorize]
    public class ListPersonalMeetingHoldModel : PageModel
    {

        private readonly IMeetingService _meetingService;
        private readonly IHameshService _hameshService;
        private readonly IFileService _fileService;

        public ListPersonalMeetingHoldModel(IMeetingService meetingService, IHameshService hameshService, IFileService fileService)
        {

            _meetingService = meetingService;
            _hameshService = hameshService;
            _fileService = fileService;
        }
        [BindProperty]
        public ListMeetingHoldViewModel listMeetingHoldViewModel { get; set; } = new ListMeetingHoldViewModel
        {
            MeetingHolds = new List<MeetingHoldInfoViewModel>()
        };

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public IActionResult OnGet(int id, int pageId = 1, string filterCaption = "")
        {
            if (id <= 0) return BadRequest("شناسه جلسه معتبر نیست.");
            listMeetingHoldViewModel = _meetingService.GetPersonalMemberForMeetingList(id, Math.Max(pageId, 1), filterCaption?.Trim() ?? "")
                ?? new ListMeetingHoldViewModel { MeetingHolds = new List<MeetingHoldInfoViewModel>() };
            listMeetingHoldViewModel.MeetingHolds = listMeetingHoldViewModel.MeetingHolds ?? new List<MeetingHoldInfoViewModel>();
            ViewData["meetingId"] = id;
            return Page();
        }
    }
}

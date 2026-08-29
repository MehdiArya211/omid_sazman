using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stimulsoft.System.Web.UI;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Visitor.Meets.MeetingInfo.MeetingHold
{
    [Authorize]
    public class ReportPdfModel : PageModel
    {
        private readonly IMeetingService _meetingService;
        private readonly IReportStimulService _reportService;
        private readonly IFileService _fileService;

        public ReportPdfModel(IMeetingService meetingService, IReportStimulService reportService,
            IFileService fileService  )
        {

            _meetingService = meetingService;
            _reportService = reportService;
            _fileService = fileService;
        }
        [BindProperty]
        public ReportTestInfoViewModel reportTestInfoViewModel { get; set; }
        public ReportTestInfoViewModelV2 reportTestInfoViewModelv2 { get; set; }
        public void OnGet(int id)
        {
            reportTestInfoViewModel = _reportService.GetReportFullPersonal(id);
            reportTestInfoViewModelv2 = _reportService.GetReportFullPersonalV2(id);

           // reportTestInfoViewModel.RolePersonLogin = User.FindFirst("RoleTypeTitle").Value;

        }
    }
}

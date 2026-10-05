using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs.Inspection;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Admin.Inspection.Attendance
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IInspectionService _inspectionService;
        public IndexModel(IInspectionService inspectionService) => _inspectionService = inspectionService;
        public IReadOnlyList<InspectionAttendanceSummaryViewModel> History { get; private set; }
        public SelectList Meetings { get; private set; }
        public int? MeetingId { get; private set; }
        public string Search { get; private set; }
        public void OnGet(int? meetingId, string search)
        {
            MeetingId = meetingId; Search = search;
            History = _inspectionService.GetAttendanceHistory(meetingId, search);
            Meetings = new SelectList(_inspectionService.GetMeetings(), "Id", "Title", meetingId);
        }
    }
}

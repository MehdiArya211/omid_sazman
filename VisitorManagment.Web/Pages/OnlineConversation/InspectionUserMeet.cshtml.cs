using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.Inspection;

namespace VisitorManagment.Web.Pages.OnlineConversation
{
    /// <summary>صفحه مستقل ورود یگان‌ها به جلسات بازرسی.</summary>
    [Authorize]
    public class InspectionUserMeetModel : PageModel
    {
        private readonly IInspectionService _inspectionService;
        public InspectionUserMeetModel(IInspectionService inspectionService) => _inspectionService = inspectionService;
        public IReadOnlyList<InspectionMeeting> Meetings { get; private set; }
        public int UnitCode { get; private set; }
        public string UnitTitle { get; private set; }

        public IActionResult OnGet()
        {
            if (!int.TryParse(User.FindFirst("UnitCode")?.Value, out var unitCode)) return Forbid();
            UnitCode = unitCode;
            UnitTitle = User.FindFirst("UnitCodeTitle")?.Value ?? $"یگان {unitCode}";
            Meetings = _inspectionService.GetAvailableMeetings(unitCode);
            return Page();
        }

        public JsonResult OnGetValidateMeeting(int meetingId)
        {
            var validUnit = int.TryParse(User.FindFirst("UnitCode")?.Value, out var unitCode);
            return new JsonResult(new { isAllowed = validUnit && _inspectionService.CanUnitEnterMeeting(meetingId, unitCode) });
        }
    }
}

using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs.Inspection;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.Inspection;

namespace VisitorManagment.Web.Pages.Admin.Inspection.Meetings
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IInspectionService _inspectionService;
        public IndexModel(IInspectionService inspectionService) => _inspectionService = inspectionService;
        [BindProperty] public InspectionMeetingCreateViewModel Input { get; set; } = new InspectionMeetingCreateViewModel();
        public IReadOnlyList<InspectionUnitViewModel> Units { get; private set; }
        public IReadOnlyList<InspectionMeeting> Meetings { get; private set; }
        public void OnGet() => LoadPage();
        public IActionResult OnPost()
        {
            LoadPage();
            if (!ModelState.IsValid) return Page();
            var userId = int.TryParse(User.FindFirst("Id")?.Value, out var parsed) ? parsed : 0;
            var result = _inspectionService.CreateMeeting(Input, userId);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            if (result.IsSuccess) return RedirectToPage();
            ModelState.AddModelError(string.Empty, result.Message);
            return Page();
        }
        private void LoadPage() { Units = _inspectionService.GetRegisteredUnits(); Meetings = _inspectionService.GetMeetings(); }
    }
}

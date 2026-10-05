using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.Inspection;

namespace VisitorManagment.Web.Pages.Admin.Inspection.People
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IInspectionService _inspectionService;
        public IndexModel(IInspectionService inspectionService) => _inspectionService = inspectionService;

        [BindProperty] public string PersonalCode { get; set; }
        public IReadOnlyList<InspectionPerson> People { get; private set; }

        public void OnGet() => LoadPage();

        public JsonResult OnGetLookup(string personalCode) => new JsonResult(_inspectionService.LookupPerson(personalCode));

        public IActionResult OnPost()
        {
            var userId = int.TryParse(User.FindFirst("Id")?.Value, out var parsed) ? parsed : 0;
            var result = _inspectionService.RegisterPerson(PersonalCode, userId);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            if (result.IsSuccess) return RedirectToPage();
            LoadPage();
            return Page();
        }

        private void LoadPage() => People = _inspectionService.GetPeople();
    }
}

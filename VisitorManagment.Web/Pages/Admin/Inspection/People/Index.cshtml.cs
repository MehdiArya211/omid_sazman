using System.Collections.Generic;
using System.Linq;
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

        /// <summary>فرد را ثبت می‌کند و داده تازه جدول را بدون بارگذاری مجدد صفحه برمی‌گرداند.</summary>
        public JsonResult OnPostRegister()
        {
            var userId = int.TryParse(User.FindFirst("Id")?.Value, out var parsed) ? parsed : 0;
            var result = _inspectionService.RegisterPerson(PersonalCode, userId);
            if (!result.IsSuccess)
                return new JsonResult(new { isSuccess = false, message = result.Message });

            return new JsonResult(new { isSuccess = true, message = result.Message, people = BuildPeopleRows() });
        }

        /// <summary>فرد را به‌شکل نرم حذف می‌کند تا سوابق شرکت او در جلسات باقی بماند.</summary>
        public JsonResult OnPostDeletePerson(int id)
        {
            var result = _inspectionService.DeletePerson(id);
            return new JsonResult(new
            {
                isSuccess = result.IsSuccess,
                message = result.Message,
                people = result.IsSuccess ? BuildPeopleRows() : null
            });
        }

        private object BuildPeopleRows() => _inspectionService.GetPeople().Select(person => new
        {
            id = person.Id,
            personalCode = person.PersonalCode,
            fullName = (person.FirstName + " " + person.LastName).Trim(),
            rankTitle = person.RankTitle,
            unitTitle = person.UnitTitle,
            unitDutyTitle = person.UnitDutyTitle,
            jobDescription = person.JobDescription
        }).ToList();

        private void LoadPage() => People = _inspectionService.GetPeople();
    }
}

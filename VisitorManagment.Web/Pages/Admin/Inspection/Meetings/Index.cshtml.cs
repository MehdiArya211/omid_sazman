using System.Collections.Generic;
using System.Linq;
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
        public IReadOnlyList<InspectionPerson> People { get; private set; }
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

        /// <summary>جلسه را ثبت می‌کند و ردیف‌های تازه جدول را بدون بارگذاری مجدد صفحه برمی‌گرداند.</summary>
        public JsonResult OnPostCreate()
        {
            if (!ModelState.IsValid)
                return new JsonResult(new { isSuccess = false, message = FirstModelError() });
            var userId = CurrentUserId();
            var result = _inspectionService.CreateMeeting(Input, userId);
            return new JsonResult(new { isSuccess = result.IsSuccess, message = result.Message, meetings = result.IsSuccess ? BuildMeetingRows() : null });
        }

        /// <summary>نفرات جلسه را بدون حذف یا ایجاد مجدد جلسه به‌روزرسانی می‌کند.</summary>
        public JsonResult OnPostUpdatePeople(int meetingId, List<int> personIds)
        {
            var result = _inspectionService.UpdateMeetingPeople(meetingId, personIds, CurrentUserId());
            return new JsonResult(new { isSuccess = result.IsSuccess, message = result.Message, meetings = result.IsSuccess ? BuildMeetingRows() : null });
        }

        private int CurrentUserId() => int.TryParse(User.FindFirst("Id")?.Value, out var parsed) ? parsed : 0;

        private string FirstModelError() => ModelState.Values.SelectMany(value => value.Errors)
            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "اطلاعات فرم معتبر نیست." : error.ErrorMessage)
            .FirstOrDefault() ?? "اطلاعات فرم معتبر نیست.";

        private object BuildMeetingRows() => _inspectionService.GetMeetings().Select(meeting => new
        {
            id = meeting.Id,
            title = meeting.Title,
            meetingDate = meeting.MeetingDate,
            startTime = meeting.StartTime,
            unitNames = string.Join("، ", meeting.Units.Select(unit => unit.UnitTitle).Where(title => !string.IsNullOrWhiteSpace(title)).Distinct()),
            unitCodes = meeting.Units.Select(unit => unit.UnitCode).Distinct().ToList(),
            personIds = meeting.People.Select(person => person.InspectionPersonId).Distinct().ToList(),
            peopleCount = meeting.People.Select(person => person.InspectionPersonId).Distinct().Count(),
            statusTitle = meeting.Status == InspectionMeetingStatus.Draft ? "پیش‌نویس" : meeting.Status == InspectionMeetingStatus.Published ? "منتشرشده" : meeting.Status.ToString()
        }).ToList();
        private void LoadPage() { Units = _inspectionService.GetRegisteredUnits(); People = _inspectionService.GetPeople(); Meetings = _inspectionService.GetMeetings(); }
    }
}

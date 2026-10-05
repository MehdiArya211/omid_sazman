using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisitorManagment.DataLayer.Entities.Inspection;

namespace VisitorManagment.Core.DTOs.Inspection
{
    public class InspectionPersonLookupViewModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string PersonalCode { get; set; }
        public string FullName { get; set; }
        public string RankTitle { get; set; }
        public string UnitTitle { get; set; }
        public string UnitDutyTitle { get; set; }
        public string JobDescription { get; set; }
    }

    public class InspectionMeetingCreateViewModel
    {
        [Required(ErrorMessage = "عنوان جلسه را وارد کنید."), MaxLength(200)]
        [Display(Name = "عنوان جلسه")]
        public string Title { get; set; }

        [Required(ErrorMessage = "تاریخ جلسه را وارد کنید."), RegularExpression(@"^\d{4}/\d{2}/\d{2}$", ErrorMessage = "تاریخ باید به شکل ۱۴۰۵/۰۷/۱۳ باشد.")]
        [Display(Name = "تاریخ جلسه")]
        public string MeetingDate { get; set; }

        [Required(ErrorMessage = "ساعت شروع را وارد کنید."), RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "ساعت معتبر نیست.")]
        [Display(Name = "ساعت شروع")]
        public string StartTime { get; set; }

        [MaxLength(1000)]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "انتشار و قابل ورود")]
        public bool Publish { get; set; } = true;
        public List<int> UnitCodes { get; set; } = new List<int>();
    }

    public class InspectionUnitViewModel
    {
        public int UnitCode { get; set; }
        public string UnitTitle { get; set; }
        public int PeopleCount { get; set; }
    }

    public class InspectionOperationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public static InspectionOperationResult Success(string message) => new InspectionOperationResult { IsSuccess = true, Message = message };
        public static InspectionOperationResult Failure(string message) => new InspectionOperationResult { Message = message };
    }
}

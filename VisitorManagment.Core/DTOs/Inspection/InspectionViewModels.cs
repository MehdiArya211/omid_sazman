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
        private string _meetingDate;
        private string _startTime;

        [Required(ErrorMessage = "عنوان جلسه را وارد کنید."), MaxLength(200)]
        [Display(Name = "عنوان جلسه")]
        public string Title { get; set; }

        [Required(ErrorMessage = "تاریخ جلسه را وارد کنید."), RegularExpression(@"^\d{4}/\d{2}/\d{2}$", ErrorMessage = "تاریخ باید به شکل ۱۴۰۵/۰۷/۱۳ باشد.")]
        [Display(Name = "تاریخ جلسه")]
        public string MeetingDate
        {
            get => _meetingDate;
            set => _meetingDate = NormalizeDigits(value);
        }

        [Required(ErrorMessage = "ساعت شروع را وارد کنید."), RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "ساعت معتبر نیست.")]
        [Display(Name = "ساعت شروع")]
        public string StartTime
        {
            get => _startTime;
            set => _startTime = NormalizeDigits(value);
        }

        [MaxLength(1000)]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "انتشار و قابل ورود")]
        public bool Publish { get; set; } = true;
        public List<int> UnitCodes { get; set; } = new List<int>();
        public List<int> PersonIds { get; set; } = new List<int>();

        /// <summary>اعداد فارسی و عربی ورودی تقویم را پیش از اعتبارسنجی به اعداد لاتین تبدیل می‌کند.</summary>
        private static string NormalizeDigits(string value) => value?.Trim()
            .Replace('۰', '0').Replace('۱', '1').Replace('۲', '2').Replace('۳', '3').Replace('۴', '4')
            .Replace('۵', '5').Replace('۶', '6').Replace('۷', '7').Replace('۸', '8').Replace('۹', '9')
            .Replace('٠', '0').Replace('١', '1').Replace('٢', '2').Replace('٣', '3').Replace('٤', '4')
            .Replace('٥', '5').Replace('٦', '6').Replace('٧', '7').Replace('٨', '8').Replace('٩', '9');
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

    public class InspectionParticipationSummaryViewModel
    {
        public string PersonalCode { get; set; }
        public string FullName { get; set; }
        public string RankTitle { get; set; }
        public int UnitCode { get; set; }
        public string UnitTitle { get; set; }
        public int MeetingsCount { get; set; }
        public List<InspectionParticipationDetailViewModel> Details { get; set; } = new List<InspectionParticipationDetailViewModel>();
    }

    public class InspectionParticipationDetailViewModel
    {
        public int MeetingId { get; set; }
        public string MeetingTitle { get; set; }
        public string MeetingDate { get; set; }
        public string StartTime { get; set; }
        public System.DateTime RegisteredAt { get; set; }
    }
}

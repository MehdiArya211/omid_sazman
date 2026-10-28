using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.Core.DTOs
{
    /// <summary>
    /// یک ردیف از فایل ورود گروهی کاربران را برای بازبینی مدیر نگهداری می‌کند.
    /// </summary>
    public class BulkUserImportRowViewModel
    {
        public int RowNumber { get; set; }
        public bool Selected { get; set; } = true;

        [Display(Name = "کد پرسنلی")]
        public string PersonalCode { get; set; }

        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Display(Name = "درجه")]
        public string RankTitle { get; set; }

        [Display(Name = "نام یگان")]
        public string UnitTitle { get; set; }

        public int? UnitCode { get; set; }
        public int? UnitDutyCode { get; set; }
        public string UnitDutyTitle { get; set; }
        public int? CodGha { get; set; }
        public string CodGhaTitle { get; set; }

        [Display(Name = "شماره همراه")]
        public string Mobile { get; set; }

        public int RoleId { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public bool UnitMatched { get; set; }
        public bool PersonnelVerified { get; set; }
        public bool Imported { get; set; }
    }

    /// <summary>
    /// نتیجه نهایی ورود گروهی را با ردیف‌های موفق و ناموفق نمایش می‌دهد.
    /// </summary>
    public class BulkUserImportResultViewModel
    {
        public int ImportedCount { get; set; }
        public int FailedCount { get; set; }
        public int SkippedCount { get; set; }
        public List<BulkUserImportRowViewModel> Rows { get; set; } = new List<BulkUserImportRowViewModel>();
    }
}

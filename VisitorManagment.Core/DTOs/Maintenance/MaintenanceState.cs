using System;

namespace VisitorManagment.Core.DTOs.Maintenance
{
    public class MaintenanceState
    {
        public const string DefaultMessage = "سامانه موقتاً در حال به‌روزرسانی است. لطفاً کمی بعد دوباره مراجعه کنید.";
        public bool IsEnabled { get; set; }
        public string Message { get; set; } = DefaultMessage;
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedByUserId { get; set; }
        public int Revision { get; set; }
    }
}

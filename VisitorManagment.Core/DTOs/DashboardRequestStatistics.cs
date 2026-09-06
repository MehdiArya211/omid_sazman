namespace VisitorManagment.Core.DTOs
{
    /// <summary>
    /// آمار یکتای درخواست‌های قابل مشاهده در داشبورد کاربر.
    /// هر درخواست صرف‌نظر از تعداد گردش‌ها و نظریه‌های آن فقط یک بار محاسبه می‌شود.
    /// </summary>
    public class DashboardRequestStatistics
    {
        public int TotalRequests { get; set; }
        public int ResolvedRequests { get; set; }
        public int OpinionRequests { get; set; }
        public int ReturnedRequests { get; set; }
        public int OtherRequests { get; set; }

        public int CompletedRequests => ResolvedRequests + OpinionRequests;
        public int CompletionPercentage => TotalRequests == 0
            ? 0
            : (int)System.Math.Round(CompletedRequests * 100d / TotalRequests);
    }
}

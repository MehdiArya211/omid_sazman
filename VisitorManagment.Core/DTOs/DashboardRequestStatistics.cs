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
        public System.Collections.Generic.List<DashboardMonthlyRequestStatistics> MonthlyTrend { get; set; }
            = new System.Collections.Generic.List<DashboardMonthlyRequestStatistics>();
        public System.Collections.Generic.List<DashboardUnitOption> AvailableUnits { get; set; }
            = new System.Collections.Generic.List<DashboardUnitOption>();
        public System.Collections.Generic.List<DashboardUnitStatistics> UnitBreakdown { get; set; }
            = new System.Collections.Generic.List<DashboardUnitStatistics>();

        public int CompletedRequests => ResolvedRequests + OpinionRequests;
        public int CompletionPercentage => TotalRequests == 0
            ? 0
            : (int)System.Math.Round(CompletedRequests * 100d / TotalRequests);
    }

    /// <summary>
    /// آمار یکتای درخواست‌ها در یک ماه برای نمودار مدیریتی.
    /// </summary>
    public class DashboardMonthlyRequestStatistics
    {
        public string Label { get; set; }
        public int TotalRequests { get; set; }
        public int ResolvedRequests { get; set; }
        public int OpinionRequests { get; set; }
        public int ReturnedRequests { get; set; }
    }

    public class DashboardUnitOption
    {
        public int UnitCode { get; set; }
        public string UnitTitle { get; set; }
    }

    public class DashboardUnitStatistics
    {
        public string UnitTitle { get; set; }
        public int RequestCount { get; set; }
    }
}

namespace VisitorManagment.Core.DTOs
{
    /// <summary>
    /// یگان قابل انتخاب در مدیریت دسترسی‌ها.
    /// </summary>
    public class UnitAccessOptionViewModel
    {
        public int UnitCode { get; set; }
        public string UnitTitle { get; set; }
        public int UserCount { get; set; }
    }
}

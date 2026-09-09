using System.Collections.Generic;

namespace VisitorManagment.Core.DTOs.Access
{
    /// <summary>
    /// اطلاعات یکپارچه نقش و محل خدمت کاربر برای تصمیم‌گیری‌های دسترسی.
    /// نقش قابلیت‌های کاربر را تعیین می‌کند و کدهای سازمانی فقط محدوده داده را مشخص می‌کنند.
    /// </summary>
    public class UserOrganizationalContext
    {
        public int UserId { get; set; }
        public int UnitCode { get; set; }
        public int? UnitDutyCode { get; set; }
        public int? GharargahCode { get; set; }
        public IReadOnlyCollection<int> RoleIds { get; set; } = new List<int>();
        public IReadOnlyCollection<int> RoleTypes { get; set; } = new List<int>();

        /// <summary>
        /// مشخص می‌کند کاربر مدیر کل سامانه است و محدودیت سازمانی ندارد.
        /// </summary>
        public bool IsSystemAdministrator { get; set; }
    }
}

using System.Collections.Generic;
using System.Text.RegularExpressions;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Core.Constants
{
    /// <summary>فقط نقش‌های بازرسی مشترک‌اند؛ سایر جایگاه‌ها منطق تک‌مسئولی قبلی را حفظ می‌کنند.</summary>
    public static class InspectionRolePolicy
    {
        // عناوین صریح استفاده می‌شوند؛ جستجوی Contains ممکن است نقش مدیریتی دیگری را اشتباهاً مشترک کند.
        private static readonly HashSet<string> SharedTitles = new HashSet<string>
        {
            "بازرسی یگان", "بازرسی قرارگاه", "بازرسی ارشد نظامی",
            "بازرسی قرارگاه/ارشد نظامی", "بازرسی قرارگاه / ارشد نظامی",
            "بازرسی نیرو", "بازرسی نزاجا"
        };

        public static bool IsShared(Role role)
        {
            if (role == null || role.IsDelete) return false;
            var title = Regex.Replace((role.Title ?? "").Replace('ي', 'ی').Replace('ك', 'ک').Replace('\u200c', ' ').Trim(), @"\s+", " ");
            return role.RoleId == SystemRoleIds.InspectionOffice || SharedTitles.Contains(title);
        }
    }
}

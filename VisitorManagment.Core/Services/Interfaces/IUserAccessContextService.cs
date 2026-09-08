using VisitorManagment.Core.DTOs.Access;

namespace VisitorManagment.Core.Services.Interfaces
{
    /// <summary>
    /// نقش و محدوده سازمانی کاربر را از یک نقطه واحد در اختیار سرویس‌ها قرار می‌دهد.
    /// </summary>
    public interface IUserAccessContextService
    {
        /// <summary>
        /// زمینه دسترسی کاربر را شامل نقش‌ها، یگان محل خدمت و قرارگاه مربوطه دریافت می‌کند.
        /// اگر کاربر معتبر نباشد مقدار null برگردانده می‌شود.
        /// </summary>
        UserOrganizationalContext GetUserContext(int userId);

        /// <summary>
        /// بررسی می‌کند کاربر اجازه مشاهده داده‌های یک یگان را دارد یا خیر.
        /// مدیر سامانه همه یگان‌ها و سایر کاربران فقط یگان خود را مشاهده می‌کنند.
        /// </summary>
        bool CanAccessUnit(int userId, int unitCode);

        /// <summary>
        /// بررسی می‌کند کاربر اجازه مشاهده داده‌های یک قرارگاه را دارد یا خیر.
        /// مدیر سامانه همه قرارگاه‌ها و سایر کاربران فقط قرارگاه خود را مشاهده می‌کنند.
        /// </summary>
        bool CanAccessGharargah(int userId, int gharargahCode);
    }
}

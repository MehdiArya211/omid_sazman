using System.Security.Claims;
using VisitorManagment.Core.Constants;

namespace VisitorManagment.Web.Helpers
{
    /// <summary>
    /// دسترسی امن و خوانا به Claimهای کاربر واردشده را فراهم می‌کند.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        #region Claim readers

        /// <summary>
        /// مقدار عددی Claim را دریافت می‌کند؛ در صورت نبودن یا نامعتبر بودن مقدار، صفر برمی‌گرداند.
        /// </summary>
        public static int GetIntClaim(this ClaimsPrincipal user, string claimType)
        {
            if (user == null || string.IsNullOrWhiteSpace(claimType)) return 0;
            return int.TryParse(user.FindFirst(claimType)?.Value, out var value) ? value : 0;
        }

        /// <summary>
        /// بررسی می‌کند نوع سمت سازمانی کاربر با مقدار موردنظر برابر است یا خیر.
        /// </summary>
        public static bool HasRoleType(this ClaimsPrincipal user, int roleType)
        {
            return user.GetIntClaim("RoleTypeId") == roleType;
        }

        /// <summary>
        /// بررسی می‌کند شناسه نقش فعلی کاربر با مقدار موردنظر برابر است یا خیر.
        /// </summary>
        public static bool HasRoleId(this ClaimsPrincipal user, int roleId)
        {
            return user.GetIntClaim("RoleId") == roleId;
        }

        /// <summary>
        /// بررسی می‌کند نقش فعلی کاربر در مجموعه نقش‌های مجاز قرار دارد یا خیر.
        /// </summary>
        public static bool HasAnyRoleId(this ClaimsPrincipal user, params int[] roleIds)
        {
            if (roleIds == null || roleIds.Length == 0) return false;
            var currentRoleId = user.GetIntClaim("RoleId");
            return System.Array.IndexOf(roleIds, currentRoleId) >= 0;
        }

        /// <summary>
        /// بررسی می‌کند نوع سمت کاربر در مجموعه سمت‌های مجاز قرار دارد یا خیر.
        /// </summary>
        public static bool HasAnyRoleType(this ClaimsPrincipal user, params int[] roleTypes)
        {
            if (roleTypes == null || roleTypes.Length == 0) return false;
            var currentRoleType = user.GetIntClaim("RoleTypeId");
            return System.Array.IndexOf(roleTypes, currentRoleType) >= 0;
        }

        /// <summary>
        /// مشخص می‌کند کاربر مدیر کل سامانه است یا خیر.
        /// </summary>
        public static bool IsSystemAdministrator(this ClaimsPrincipal user)
        {
            return user.HasRoleType(SystemRoleTypes.SystemAdministrator);
        }

        #endregion
    }
}

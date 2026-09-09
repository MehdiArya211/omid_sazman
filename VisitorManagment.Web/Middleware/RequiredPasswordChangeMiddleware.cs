using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace VisitorManagment.Web.Middleware
{
    /// <summary>
    /// کاربری را که رمز او توسط مدیر بازنشانی شده است تا زمان ثبت رمز جدید
    /// به صفحه تغییر اجباری رمز محدود می‌کند.
    /// </summary>
    public class RequiredPasswordChangeMiddleware
    {
        #region Fields and constructor

        private readonly RequestDelegate _next;

        public RequiredPasswordChangeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Middleware pipeline

        /// <summary>
        /// Claim تغییر اجباری رمز را بررسی و درخواست‌های غیرمجاز را به فرم تغییر رمز هدایت می‌کند.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User;
            var requestPath = context.Request.Path;
            var passwordChangeRequired = user.Identity?.IsAuthenticated == true &&
                                         bool.TryParse(user.FindFirst("MustChangePassword")?.Value, out var required) &&
                                         required;

            var isAllowedPath = requestPath.StartsWithSegments("/ChangePasswordRequired") ||
                                requestPath.StartsWithSegments("/DevelopmentImpersonation") ||
                                requestPath.StartsWithSegments("/Admin/Users/Logout");

            if (passwordChangeRequired && !isAllowedPath)
            {
                context.Response.Redirect("/ChangePasswordRequired");
                return;
            }

            await _next(context);
        }

        #endregion
    }
}

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Middleware
{
    /// <summary>
    /// تا زمان ثبت رمز جدید یا پاسخ‌های امنیتی لازم، کاربر را به صفحه‌ی تکمیل اطلاعات هدایت می‌کند.
    /// </summary>
    public class RequiredPasswordChangeMiddleware
    {
        private readonly RequestDelegate _next;

        public RequiredPasswordChangeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            var user = context.User;
            if (user.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var requestPath = context.Request.Path;
            if (IsAllowedPath(requestPath))
            {
                await _next(context);
                return;
            }

            if (!int.TryParse(user.FindFirst("Id")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId) ||
                userId <= 0)
            {
                await _next(context);
                return;
            }

            if (userService.IsPasswordChangeRequired(userId))
            {
                context.Response.Redirect("/ChangePasswordRequired");
                return;
            }

            if (!userService.HasSecurityAnswers(userId))
            {
                context.Response.Redirect("/AccountSecurityQuestions");
                return;
            }

            await _next(context);
        }

        private static bool IsAllowedPath(PathString path)
        {
            return path.StartsWithSegments("/AccountSecurityQuestions") ||
                   path.StartsWithSegments("/ChangePasswordRequired") ||
                   path.StartsWithSegments("/ForgetPassword") ||
                   path.StartsWithSegments("/Index") ||
                   path.StartsWithSegments("/DevelopmentImpersonation") ||
                   path.StartsWithSegments("/Admin/Users/Logout");
        }
    }
}

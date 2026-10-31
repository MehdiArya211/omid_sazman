using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Middleware
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;
        public MaintenanceMiddleware(RequestDelegate next) { _next = next; }

        public async Task InvokeAsync(HttpContext context, IMaintenanceService maintenance)
        {
            if (IsRecoveryPath(context.Request.Path))
            {
                await _next(context);
                return;
            }
            var state = await maintenance.GetStateAsync();
            if (!state.IsEnabled || await maintenance.IsDeveloperAsync(context.User))
            {
                await _next(context);
                return;
            }
            context.Response.Headers["Cache-Control"] = "no-store, no-cache";
            context.Response.Headers["Retry-After"] = "60";
            var isPageNavigation = HttpMethods.IsGet(context.Request.Method) &&
                context.Request.Headers["Accept"].ToString().IndexOf("text/html", StringComparison.OrdinalIgnoreCase) >= 0 &&
                !string.Equals(context.Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
            if (isPageNavigation)
            {
                context.Response.Redirect(context.Request.PathBase + "/UpdateSamane");
                return;
            }
            // POST و Ajax دوباره ارسال یا به صفحه HTML تبدیل نمی‌شوند.
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new { isMaintenance = true, message = state.Message });
        }

        private static bool IsRecoveryPath(PathString path)
        {
            var value = (path.Value ?? string.Empty).TrimEnd('/');
            return value.Length == 0 ||
                string.Equals(value, "/Index", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "/UpdateSamane", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "/MaintenanceStatus", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "/Developer/Maintenance", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "/Admin/Users/Logout", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "/DevelopmentImpersonation", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "/ChangePasswordRequired", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "/Captcha", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "/Captcha/Index", StringComparison.OrdinalIgnoreCase);
        }
    }
}

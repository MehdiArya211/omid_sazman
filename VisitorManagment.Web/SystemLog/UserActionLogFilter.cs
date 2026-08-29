using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.SystemLog;
using VisitorManagment.Core.Services.SystemLog;

namespace VisitorManagment.Web.Filters
{
    public class UserActionLogFilter : IAsyncActionFilter
    {
        private readonly IUserActionLogger _logger;

        public UserActionLogFilter(IUserActionLogger logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var http = context.HttpContext;
            var user = http.User;

            var logDto = new UserActionLog
            {
                UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                UserName = user.Identity?.Name,
                Controller = context.RouteData.Values["controller"]?.ToString(),
                Action = context.RouteData.Values["action"]?.ToString(),
                IpAddress = http.Connection.RemoteIpAddress?.ToString(),
                UserAgent = http.Request.Headers["User-Agent"],
                CreatedAt = DateTime.Now
            };

            try
            {
                var result = await next();
                logDto.IsSuccess = result.Exception == null;
                logDto.ErrorMessage = result.Exception?.Message;
            }
            catch (Exception ex)
            {
                logDto.IsSuccess = false;
                logDto.ErrorMessage = ex.Message;
                throw;
            }
            finally
            {
                await _logger.LogAsync(logDto);
            }

        }
    }
}

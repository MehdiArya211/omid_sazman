using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Hubs
{
    /// <summary>درخواست‌های اتصال‌های WebSocket موجود هم در هر فراخوانی دوباره بررسی می‌شوند.</summary>
    public class MaintenanceHubFilter : IHubFilter
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public MaintenanceHubFilter(IServiceScopeFactory scopeFactory) { _scopeFactory = scopeFactory; }

        private async Task<bool> IsBlockedAsync(HubCallerContext context)
        {
            using var scope = _scopeFactory.CreateScope();
            var maintenance = scope.ServiceProvider.GetRequiredService<IMaintenanceService>();
            var state = await maintenance.GetStateAsync();
            return state.IsEnabled && !await maintenance.IsDeveloperAsync(context.User);
        }

        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext context, Func<HubInvocationContext, ValueTask<object>> next)
        {
            if (await IsBlockedAsync(context.Context))
            {
                context.Context.Abort();
                throw new HubException("سامانه در حال به‌روزرسانی است.");
            }
            return await next(context);
        }

        public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            if (await IsBlockedAsync(context.Context))
            {
                context.Context.Abort();
                return;
            }
            await next(context);
        }
    }
}

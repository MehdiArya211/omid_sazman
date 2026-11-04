using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisitorManagment.Web.Hubs
{
    public class OnlineUsersHub1 : Hub
    {
        private static readonly ConcurrentDictionary<string, string> OnlineUsers = new ConcurrentDictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name ?? $"مهمان-{Context.ConnectionId.Substring(0, 5)}";
            OnlineUsers[Context.ConnectionId] = userName;
            await NotifyAllClients();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            OnlineUsers.TryRemove(Context.ConnectionId, out _);
            await NotifyAllClients();
            await base.OnDisconnectedAsync(exception);
        }

        private Task NotifyAllClients()
        {
            var users = OnlineUsers.Values;
            return Clients.All.SendAsync("UpdateOnlineUsers", users.Count(), users);
        }
    }

    public class OnlineUsersHub : Hub
    {
        private static readonly ConcurrentDictionary<string, OnlineUserSnapshot> Connections =
            new ConcurrentDictionary<string, OnlineUserSnapshot>();

        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            var snapshot = new OnlineUserSnapshot
            {
                UserId = user?.FindFirst("Id")?.Value ?? user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                PersonalCode = user?.FindFirst("PersonalCode")?.Value ?? string.Empty,
                FullName = user?.FindFirst("FullName")?.Value ?? user?.Identity?.Name ?? "کاربر سامانه",
                UnitTitle = user?.FindFirst("UnitCodeTitle")?.Value ?? string.Empty,
                IsAuthenticated = user?.Identity?.IsAuthenticated == true
            };

            Connections[Context.ConnectionId] = snapshot;
            await NotifyClients();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Connections.TryRemove(Context.ConnectionId, out _);
            await NotifyClients();
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>اتصال‌های چندگانه را به یک ردیف برای هر کاربر واردشده تبدیل می‌کند.</summary>
        public static IReadOnlyList<OnlineUserSnapshot> GetOnlineUsers()
        {
            return Connections.Values
                .Where(user => user.IsAuthenticated && !string.IsNullOrWhiteSpace(user.UserId))
                .GroupBy(user => user.UserId, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(user => user.FullName)
                .ToList();
        }

        private Task NotifyClients()
        {
            var uniqueUsers = Connections.Values
                .Where(user => !string.IsNullOrWhiteSpace(user.PersonalCode))
                .Select(user => user.PersonalCode)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            return Clients.All.SendAsync("UpdateOnlineUsers", uniqueUsers.Count, uniqueUsers);
        }
    }

    public class OnlineUserSnapshot
    {
        public string UserId { get; set; }
        public string PersonalCode { get; set; }
        public string FullName { get; set; }
        public string UnitTitle { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}

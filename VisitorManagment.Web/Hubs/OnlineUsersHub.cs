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
        private static readonly ConcurrentDictionary<string, string> OnlineUsers = new();

        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name ?? $"مهمان-{Context.ConnectionId.Substring(0, 5)}";
            OnlineUsers[Context.ConnectionId] = userName;

            await NotifyAllClients();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
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
        private static HashSet<string> ConnectedUsers = new();

        public override Task OnConnectedAsync()
        {
            ConnectedUsers.Add(Context.ConnectionId);
            Console.WriteLine($"[Hub] Connected: {Context.ConnectionId}, Total: {ConnectedUsers.Count}");

            Clients.All.SendAsync("UpdateOnlineUsers", ConnectedUsers.Count, ConnectedUsers.ToList());

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectedUsers.Remove(Context.ConnectionId);
            Console.WriteLine($"[Hub] Disconnected: {Context.ConnectionId}, Total: {ConnectedUsers.Count}");

            Clients.All.SendAsync("UpdateOnlineUsers", ConnectedUsers.Count, ConnectedUsers.ToList());

            return base.OnDisconnectedAsync(exception);
        }
    }




}

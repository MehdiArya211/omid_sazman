using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Web.Hubs
{
    public class NezRTCHub : Hub
    {
        private static RoomManager roomManager = new RoomManager();

        public override  Task OnConnectedAsync()
        {
             return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            roomManager.DeleteRoom(Context.ConnectionId);
            _ = NotifyRoomInfoAsync(false);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task CreateRoom(string personalCode, string MeetId)
        {
            RoomInfo roomInfo = roomManager.CreateRoom(Context.ConnectionId, personalCode, MeetId);
            if (roomInfo != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomInfo.RoomId);
                await Clients.Caller.SendAsync("created", roomInfo.RoomId);
                await NotifyRoomInfoAsync(false);
                
             }
            else
            {
                await Clients.Caller.SendAsync("error", "error occurred when creating a new room.");
            }
        }

        public async Task Join(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Caller.SendAsync("joined", roomId);
            await Clients.Group(roomId).SendAsync("ready", roomId);

            ////remove the room from room list.
            //if (int.TryParse(roomId, out int id))
            //{
            //    roomManager.DeleteRoom(id);
            //    await NotifyRoomInfoAsync(false);
            //}
        }

        public async Task LeaveRoom(string roomId)
        {
            await Clients.Group(roomId).SendAsync("bye");
        }

        public async Task GetRoomInfo()
        {
            await NotifyRoomInfoAsync(true);
            await ansarNotifyRoomInfoAsync(true);
        }

        public async Task SendMessage(string roomId, object message)
        {
            await Clients.OthersInGroup(roomId).SendAsync("message", message);
        }

        public async Task NotifyRoomInfoAsync(bool notifyOnlyCaller)
        {
            List<RoomInfo> roomInfos = roomManager.GetAllRoomInfo();
            var list = from room in roomInfos
                       select new
                       {
                           RoomId = room.RoomId,
                           PersonalCode = room.PersonalCode,
                           MeetId = room.MeetId,
                           Button = "<button class=\"connectBtn\">برقراری تماس</button>"
                       };
            var data = JsonConvert.SerializeObject(list);

            if (notifyOnlyCaller)
            {
                await Clients.Caller.SendAsync("updateRoom", data);
            }
            else
            {
                await Clients.All.SendAsync("updateRoom", data);
            }
        }


        // ansar tasks
        // added by mahdi fakhr
        private static AnsarRoomManager ansarRoomManager = new AnsarRoomManager();
        public async Task ansarCreateRoom(string name)
        {
            AnsarRoomInfo ansarRoomInfo = ansarRoomManager.CreateRoom(Context.ConnectionId, name);
            if (ansarRoomInfo != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, ansarRoomInfo.RoomId);
                await Clients.Caller.SendAsync("ansarCreated", ansarRoomInfo.RoomId, ansarRoomInfo.Name);
                await ansarNotifyRoomInfoAsync(false);

            }
            else
            {
                await Clients.Caller.SendAsync("error", "error occurred when creating a new room.");
            }
        }
        public async Task ansarJoin(string roomId, string name)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Caller.SendAsync("ansarJoined", roomId, name);
            await Clients.Group(roomId).SendAsync("ansarReady", roomId, name);

            //remove the room from room list.
            if (int.TryParse(roomId, out int id))
            {
                ansarRoomManager.DeleteRoom(id);
                await ansarNotifyRoomInfoAsync(false);
            }
        }
        public async Task ansarLeaveRoom(string roomId)
        {
            await Clients.Group(roomId).SendAsync("bye");
        }
        public async Task ansarGetRoomInfo()
        {
            await ansarNotifyRoomInfoAsync(true);
        }
        public async Task ansarNotifyRoomInfoAsync(bool notifyOnlyCaller)
        {
            List<AnsarRoomInfo> ansarRoomInfos = ansarRoomManager.GetAllRoomInfo();
            var list = from room in ansarRoomInfos
                       select new
                       {
                           RoomId = room.RoomId,
                           Name = room.Name
                       };
            var data = JsonConvert.SerializeObject(list);

            if (notifyOnlyCaller)
            {
                await Clients.Caller.SendAsync("ansarUpdateRoom", data);
            }
            else
            {
                await Clients.All.SendAsync("ansarUpdateRoom", data);
            }
        }



        // chatroom hub 
        public async Task ChatJoin(string roomId)
        {
            // joining to given room id
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            
            // notify self and all other users
            await Clients.Caller.SendAsync("chatjoined_self", roomId);
            await Clients.Group(roomId).SendAsync("chatjoined", roomId);
        }
        // Leave
        public async Task LeaveChatRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("chatBye");
        }

        // ChatMessage
        public async Task ChatMessage(string roomId, string message, string uuid)
        {
            await Clients.OthersInGroup(roomId).SendAsync("on_chatroom_message", message, uuid);
        }

        public async Task RemoveAllChats(string roomId)
        {
            await Clients.Group(roomId).SendAsync("remove_chat_messages", roomId);
        }

        public async Task RemoveSingleChatMessage(string roomId, string uuid)
        {
            await Clients.OthersInGroup(roomId).SendAsync("remove_single_chat_messages", uuid);
        }

    }

    /// <summary>
    /// Room management for WebRTCHub
    /// </summary>
    public class RoomManager
    {
        private int nextRoomId;
        /// <summary>
        /// Room List (key:RoomId)
        /// </summary>
        private ConcurrentDictionary<int, RoomInfo> rooms;

        public RoomManager()
        {
            nextRoomId = 1;
            rooms = new ConcurrentDictionary<int, RoomInfo>();
        }

        public RoomInfo CreateRoom(string connectionId, string personalCode = "not set", string MeetId = "not set")
        {

            rooms.TryRemove(nextRoomId, out _);

            //create new room info
            var roomInfo = new RoomInfo
            {
                RoomId = nextRoomId.ToString(),
                PersonalCode = personalCode,
                MeetId = MeetId,
                HostConnectionId = connectionId
            };
            bool result = rooms.TryAdd(nextRoomId, roomInfo);

            if (result)
            {
                nextRoomId++;
                return roomInfo;
            }
            else
            {
                return null;
            }
        }

        public void DeleteRoom(int roomId)
        {
            rooms.TryRemove(roomId, out _);
        }

        public void DeleteRoom(string connectionId)
        {
            int? correspondingRoomId = null;
            foreach (var pair in rooms)
            {
                if (pair.Value.HostConnectionId.Equals(connectionId))
                {
                    correspondingRoomId = pair.Key;
                }
            }

            if (correspondingRoomId.HasValue)
            {
                rooms.TryRemove(correspondingRoomId.Value, out _);
            }
        }

        public List<RoomInfo> GetAllRoomInfo()
        {
            return rooms.Values.ToList();
        }
    }


    // added by mahdi fakhr
    public class AnsarRoomManager
    {
        private int nextRoomId;
        /// <summary>
        /// Room List (key:RoomId)
        /// </summary>
        private ConcurrentDictionary<int, AnsarRoomInfo> ansarRooms;

        public AnsarRoomManager()
        {
            nextRoomId = 999;
            ansarRooms = new ConcurrentDictionary<int, AnsarRoomInfo>();
        }

        public AnsarRoomInfo CreateRoom(string connectionId, string name)
        {

            ansarRooms.TryRemove(nextRoomId, out _);

            //create new room info
            var ansarRoomInfo = new AnsarRoomInfo
            {
                RoomId = nextRoomId.ToString(),
                Name = name,
                HostConnectionId = connectionId
            };
            bool result = ansarRooms.TryAdd(nextRoomId, ansarRoomInfo);

            if (result)
            {
                nextRoomId++;
                return ansarRoomInfo;
            }
            else
            {
                return null;
            }
        }

        public void DeleteRoom(int roomId)
        {
            ansarRooms.TryRemove(roomId, out _);
        }

        public void DeleteRoom(string connectionId)
        {
            int? correspondingRoomId = null;
            foreach (var pair in ansarRooms)
            {
                if (pair.Value.HostConnectionId.Equals(connectionId))
                {
                    correspondingRoomId = pair.Key;
                }
            }

            if (correspondingRoomId.HasValue)
            {
                ansarRooms.TryRemove(correspondingRoomId.Value, out _);
            }
        }

        public List<AnsarRoomInfo> GetAllRoomInfo()
        {
            return ansarRooms.Values.ToList();
        }
    }


    public class RoomInfo
    {
        public string RoomId { get; set; }
        public string Name { get; set; }
        public string PersonalCode { get; set; }
        public string MeetId { get; set; }
        public string HostConnectionId { get; set; }
    }


    // added by mahdi fakhr
    public class AnsarRoomInfo
    {
        public string RoomId { get; set; }
        public string Name { get; set; }
        public string HostConnectionId { get; set; }
    }


    public class ChatInfo
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string HostConnectionId { get; set; }
    }
}


/*
 * عنوان چت روم
 * اماکن برای مدیر سیستم جهت پاک کردن چت روم
 * پاک کردن برای چت های خود نفر در صورت امکان
 * نام کاربر در کنار چت
 * ساعت ارسال پیام
 */
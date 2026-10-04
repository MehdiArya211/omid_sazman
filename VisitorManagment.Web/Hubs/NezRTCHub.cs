using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using VisitorManagment.DataLayer.Entities.VisitorManagment;
using VisitorManagment.DataLayer.Entities.OnlineConversation;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.Web.Hubs
{
    [Authorize]
    public class NezRTCHub : Hub
    {
        private readonly VisitorManagmentContext _context;
        private static RoomManager roomManager = new RoomManager();

        public NezRTCHub(VisitorManagmentContext context)
        {
            _context = context;
        }

        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public override  Task OnConnectedAsync()
        {
             return base.OnConnectedAsync();
        }

        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            roomManager.DeleteRoom(Context.ConnectionId);
            _ = NotifyRoomInfoAsync(false);
            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
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

        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
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

        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public async Task LeaveRoom(string roomId)
        {
            await Clients.Group(roomId).SendAsync("bye");
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public async Task GetRoomInfo()
        {
            await NotifyRoomInfoAsync(true);
            await ansarNotifyRoomInfoAsync(true);
        }

        /// <summary>
        /// اطلاعات را به مقصد موردنظر ارسال می‌کند.
        /// </summary>
        public async Task SendMessage(string roomId, object message)
        {
            await Clients.OthersInGroup(roomId).SendAsync("message", message);
        }

        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
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
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
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
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
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
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public async Task ansarLeaveRoom(string roomId)
        {
            await Clients.Group(roomId).SendAsync("bye");
        }
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public async Task ansarGetRoomInfo()
        {
            await ansarNotifyRoomInfoAsync(true);
        }
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
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



        #region چت پایدار جلسه

        /// <summary>
        /// کاربر را به چت جلسه متصل کرده و تاریخچه پیام‌ها را برای او ارسال می‌کند.
        /// این متد برای سازگاری با کد قبلی نام ChatJoin را حفظ کرده است.
        /// </summary>
        public async Task ChatJoin(string roomId)
        {
            int meetingId;
            if (!TryGetMeetingId(roomId, out meetingId))
            {
                throw new HubException("شناسه جلسه معتبر نیست.");
            }

            await JoinMeetingChat(meetingId);
        }

        /// <summary>
        /// اتصال کاربر به گروه چت جلسه و بازیابی آخرین پیام‌های ذخیره‌شده.
        /// </summary>
        public async Task JoinMeetingChat(int meetingId)
        {
            await EnsureMeetingExists(meetingId);

            var groupName = GetChatGroupName(meetingId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await _context.OnlineConversationMessages
                .AsNoTracking()
                .Where(message => message.MeetingId == meetingId)
                .OrderByDescending(message => message.SentAtUtc)
                .Take(200)
                .OrderBy(message => message.SentAtUtc)
                .Select(message => new
                {
                    message.Id,
                    message.MeetingId,
                    message.SenderUserId,
                    message.SenderName,
                    message.SenderAvatar,
                    message.Message,
                    message.SentAtUtc,
                    message.IsDelivered,
                    message.DeliveredAtUtc,
                    message.IsRead,
                    message.ReadAtUtc
                })
                .ToListAsync();

            await Clients.Caller.SendAsync("chat_history", meetingId, messages);
            await Clients.Caller.SendAsync("chatjoined_self", groupName);
            await Clients.OthersInGroup(groupName).SendAsync("chatjoined", groupName);
        }

        /// <summary>
        /// کاربر را از گروه چت جلسه خارج می‌کند.
        /// </summary>
        public async Task LeaveChatRoom(string roomId)
        {
            int meetingId;
            if (!TryGetMeetingId(roomId, out meetingId))
            {
                return;
            }

            var groupName = GetChatGroupName(meetingId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.OthersInGroup(groupName).SendAsync("chatBye");
        }

        /// <summary>
        /// پیام را ابتدا در دیتابیس ثبت و سپس نتیجه قطعی را برای اعضای جلسه ارسال می‌کند.
        /// </summary>
        public async Task ChatMessage(string roomId, string message, string clientMessageId)
        {
            int meetingId;
            if (!TryGetMeetingId(roomId, out meetingId))
            {
                throw new HubException("شناسه جلسه معتبر نیست.");
            }

            await SaveChatMessage(meetingId, message, clientMessageId);
        }

        /// <summary>
        /// نسخه صریح متد ارسال پیام برای کدهای جدید سمت کاربر.
        /// </summary>
        public async Task SendMeetingChatMessage(int meetingId, string message, string clientMessageId)
        {
            await SaveChatMessage(meetingId, message, clientMessageId);
        }

        private async Task SaveChatMessage(int meetingId, string message, string clientMessageId)
        {
            await EnsureMeetingExists(meetingId);

            var normalizedMessage = (message ?? string.Empty).Trim();
            if (normalizedMessage.Length == 0)
            {
                throw new HubException("متن پیام نمی‌تواند خالی باشد.");
            }

            if (normalizedMessage.Length > 4000)
            {
                throw new HubException("متن پیام بیشتر از حد مجاز است.");
            }

            var userId = GetCurrentUserId();
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == userId);

            if (user == null)
            {
                throw new HubException("اطلاعات کاربر فرستنده یافت نشد.");
            }

            var now = DateTime.UtcNow;
            var entity = new OnlineConversationMessage
            {
                Id = Guid.NewGuid(),
                MeetingId = meetingId,
                SenderUserId = user.Id,
                SenderName = BuildSenderName(user.RankTitle, user.FirstName, user.LastName),
                SenderAvatar = NormalizeAvatar(user.UserAvatar),
                Message = normalizedMessage,
                SentAtUtc = now,
                IsDelivered = false,
                IsRead = false
            };

            _context.OnlineConversationMessages.Add(entity);
            await _context.SaveChangesAsync();

            await Clients.Group(GetChatGroupName(meetingId)).SendAsync("chat_message_saved", new
            {
                entity.Id,
                entity.MeetingId,
                entity.SenderUserId,
                entity.SenderName,
                entity.SenderAvatar,
                entity.Message,
                entity.SentAtUtc,
                entity.IsDelivered,
                entity.DeliveredAtUtc,
                entity.IsRead,
                entity.ReadAtUtc,
                ClientMessageId = clientMessageId
            });
        }

        /// <summary>
        /// دریافت یا خوانده‌شدن پیام توسط طرف مقابل را ثبت می‌کند.
        /// </summary>
        public async Task AcknowledgeChatMessage(Guid messageId, bool isRead)
        {
            var userId = GetCurrentUserId();
            var entity = await _context.OnlineConversationMessages
                .FirstOrDefaultAsync(message => message.Id == messageId);

            if (entity == null || entity.SenderUserId == userId)
            {
                return;
            }

            var now = DateTime.UtcNow;
            if (!entity.IsDelivered)
            {
                entity.IsDelivered = true;
                entity.DeliveredAtUtc = now;
            }

            if (isRead && !entity.IsRead)
            {
                entity.IsRead = true;
                entity.ReadAtUtc = now;
            }

            await _context.SaveChangesAsync();

            await Clients.Group(GetChatGroupName(entity.MeetingId)).SendAsync("chat_message_status", new
            {
                entity.Id,
                entity.IsDelivered,
                entity.DeliveredAtUtc,
                entity.IsRead,
                entity.ReadAtUtc
            });
        }

        /// <summary>
        /// تمام پیام‌های یک جلسه را از دیتابیس و رابط کاربران حذف می‌کند.
        /// </summary>
        public async Task RemoveAllChats(string roomId)
        {
            int meetingId;
            if (!TryGetMeetingId(roomId, out meetingId))
            {
                return;
            }

            var messages = await _context.OnlineConversationMessages
                .Where(message => message.MeetingId == meetingId)
                .ToListAsync();

            if (messages.Count > 0)
            {
                _context.OnlineConversationMessages.RemoveRange(messages);
                await _context.SaveChangesAsync();
            }

            await Clients.Group(GetChatGroupName(meetingId)).SendAsync("remove_chat_messages");
        }

        /// <summary>
        /// پیام فقط توسط فرستنده آن قابل حذف است.
        /// </summary>
        public async Task RemoveSingleChatMessage(string roomId, string messageId)
        {
            int meetingId;
            Guid parsedMessageId;
            if (!TryGetMeetingId(roomId, out meetingId) || !Guid.TryParse(messageId, out parsedMessageId))
            {
                return;
            }

            var userId = GetCurrentUserId();
            var entity = await _context.OnlineConversationMessages.FirstOrDefaultAsync(message =>
                message.Id == parsedMessageId &&
                message.MeetingId == meetingId &&
                message.SenderUserId == userId);

            if (entity == null)
            {
                return;
            }

            _context.OnlineConversationMessages.Remove(entity);
            await _context.SaveChangesAsync();
            await Clients.Group(GetChatGroupName(meetingId)).SendAsync("remove_single_chat_messages", entity.Id);
        }

        private async Task EnsureMeetingExists(int meetingId)
        {
            if (meetingId <= 0 || !await _context.Meetings.AsNoTracking().AnyAsync(meeting => meeting.Id == meetingId))
            {
                throw new HubException("جلسه انتخاب‌شده وجود ندارد یا غیرفعال شده است.");
            }
        }

        private int GetCurrentUserId()
        {
            int userId;
            var value = Context.User == null ? null : Context.User.FindFirst("Id")?.Value;
            if (!int.TryParse(value, out userId))
            {
                throw new HubException("هویت کاربر معتبر نیست.");
            }

            return userId;
        }

        private static bool TryGetMeetingId(string roomId, out int meetingId)
        {
            meetingId = 0;
            if (string.IsNullOrWhiteSpace(roomId))
            {
                return false;
            }

            var normalized = roomId.EndsWith("_chat", StringComparison.OrdinalIgnoreCase)
                ? roomId.Substring(0, roomId.Length - 5)
                : roomId;

            return int.TryParse(normalized, out meetingId) && meetingId > 0;
        }

        private static string GetChatGroupName(int meetingId)
        {
            return string.Format("meeting-{0}-chat", meetingId);
        }

        private static string BuildSenderName(string rankTitle, string firstName, string lastName)
        {
            return string.Join(" ", new[] { rankTitle, firstName, lastName }
                .Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private static string NormalizeAvatar(string avatar)
        {
            if (string.IsNullOrWhiteSpace(avatar))
            {
                return "/UserAvatar/Default.jpg";
            }

            if (avatar.StartsWith("/", StringComparison.Ordinal))
            {
                return avatar;
            }

            return "/UserAvatar/" + avatar.TrimStart('/');
        }

        #endregion

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

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
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

        /// <summary>
        /// اطلاعات مشخص‌شده را حذف می‌کند.
        /// </summary>
        public void DeleteRoom(int roomId)
        {
            rooms.TryRemove(roomId, out _);
        }

        /// <summary>
        /// اطلاعات مشخص‌شده را حذف می‌کند.
        /// </summary>
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

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
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

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
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

        /// <summary>
        /// اطلاعات مشخص‌شده را حذف می‌کند.
        /// </summary>
        public void DeleteRoom(int roomId)
        {
            ansarRooms.TryRemove(roomId, out _);
        }

        /// <summary>
        /// اطلاعات مشخص‌شده را حذف می‌کند.
        /// </summary>
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

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
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
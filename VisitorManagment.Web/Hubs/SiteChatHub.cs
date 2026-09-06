using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs.SystemChatOnline;
using VisitorManagment.Core.Services.SystemChatOnline;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Web.Hubs
{
    public class SiteChatHub : Hub
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IMessageService _messageService;
        private readonly IHubContext<SupportHub> _supportHub;
        private readonly IWebHostEnvironment _environment;
        public SiteChatHub(IChatRoomService chatRoomService, IMessageService messageService, IHubContext<SupportHub> supportHub, IWebHostEnvironment environment)
        {
            _chatRoomService = chatRoomService;
            _messageService = messageService;
            _supportHub = supportHub;
            _environment = environment;
        }

        public async Task EditMessage(Guid messageId, string text)
        {
            var roomId = await _chatRoomService.GetChatRoomForConnection(Context.ConnectionId);
            var message = await _messageService.EditMessage(roomId, messageId, Context.User.Identity.Name, text);
            await Clients.Group(roomId.ToString()).SendAsync("messageUpdated", message);
            await _supportHub.Clients.All.SendAsync("messageUpdated", roomId, message);
        }

        public async Task DeleteMessage(Guid messageId)
        {
            var roomId = await _chatRoomService.GetChatRoomForConnection(Context.ConnectionId);
            var message = await _messageService.DeleteMessage(roomId, messageId, Context.User.Identity.Name);
            await Clients.Group(roomId.ToString()).SendAsync("messageDeleted", message);
            await _supportHub.Clients.All.SendAsync("messageDeleted", roomId, message);
        }

        public async Task Typing(bool isTyping)
        {
            var roomId = await _chatRoomService.GetChatRoomForConnection(Context.ConnectionId);
            await _supportHub.Clients.All.SendAsync("typingChanged", roomId, Context.User.Identity.Name, isTyping);
        }

        public async Task MarkAsRead()
        {
            var roomId = await _chatRoomService.GetChatRoomForConnection(Context.ConnectionId);
            var ids = await _messageService.MarkMessagesAsRead(roomId, Context.User.Identity.Name);
            if (ids.Count > 0) await _supportHub.Clients.All.SendAsync("messagesRead", roomId, ids);
        }

        public async Task MarkAsDelivered()
        {
            var roomId = await _chatRoomService.GetChatRoomForConnection(Context.ConnectionId);
            var ids = await _messageService.MarkMessagesAsDelivered(roomId, Context.User.Identity.Name);
            if (ids.Count > 0) await _supportHub.Clients.All.SendAsync("messagesDelivered", roomId, ids);
        }

        public async Task SendAttachment(string fileName, string contentType, string base64Data, string caption, Guid? replyToMessageId = null)
        {
            var roomId = await _chatRoomService.GetChatRoomForConnection(Context.ConnectionId);
            var attachment = SaveAttachment(fileName, contentType, base64Data);
            var message = await _messageService.SaveChatMessage(roomId, new MessageDto {
                Sender=Context.User.Identity.Name, Message=caption, Time=DateTime.Now, ReplyToMessageId=replyToMessageId,
                AttachmentUrl=attachment.url, AttachmentName=attachment.name, AttachmentContentType=attachment.type, AttachmentSize=attachment.size
            });
            await Clients.Group(roomId.ToString()).SendAsync("getNewMessage", message);
            await _supportHub.Clients.All.SendAsync("newSupportMessage", roomId, message);
        }

        private (string url, string name, string type, long size) SaveAttachment(string fileName, string contentType, string base64Data)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".zip" };
            var extension = Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant();
            if (!allowed.Contains(extension)) throw new HubException("نوع فایل مجاز نیست.");
            byte[] bytes; try { bytes = Convert.FromBase64String(base64Data ?? string.Empty); } catch { throw new HubException("فایل معتبر نیست."); }
            if (bytes.Length == 0 || bytes.Length > 5 * 1024 * 1024) throw new HubException("حجم فایل باید کمتر از ۵ مگابایت باشد.");
            var safeName = Guid.NewGuid().ToString("N") + extension;
            var folder = Path.Combine(_environment.WebRootPath, "ChatAttachments"); Directory.CreateDirectory(folder);
            File.WriteAllBytes(Path.Combine(folder, safeName), bytes);
            return ("/ChatAttachments/" + safeName, Path.GetFileName(fileName), contentType, bytes.LongLength);
        }
        #region اعضا و متدهای کلاس


        /// <summary>
        /// اطلاعات را به مقصد موردنظر ارسال می‌کند.
        /// </summary>

        public async Task SendNewMessage(string Sender, string Message, Guid? replyToMessageId = null)
        {
            if (string.IsNullOrWhiteSpace(Message)) return;
            var roomId = await _chatRoomService.GetChatRoomForConnection(Context.ConnectionId);

            MessageDto messageDto = new MessageDto()
            {
                Message = Message.Trim(),
                Sender = Context.User.Identity.Name ?? Sender ?? "کاربر",
                Time = DateTime.Now,
                ReplyToMessageId = replyToMessageId
            };

            messageDto = await _messageService.SaveChatMessage(roomId,messageDto);
            await Clients.Group(roomId.ToString())
                .SendAsync("getNewMessage", messageDto);
            await _supportHub.Clients.All.SendAsync("newSupportMessage", roomId, messageDto);
        }

        /// <summary>
        /// پیوستن پشتیبان ها به گروه
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        public async Task JoinRoom(Guid roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        }

        /// <summary>
        /// ترک گروه توسط پشتیبان
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [Authorize]
        public async Task LeaveRoom(Guid roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
        }


        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var supportQueryValue = Context.GetHttpContext()?.Request.Query["support"].ToString();
            var isSupportConnection = string.Equals(supportQueryValue, "true", StringComparison.OrdinalIgnoreCase);
            if (isSupportConnection && Context.User.Identity.IsAuthenticated)
            {
                await base.OnConnectedAsync();
                return;
            }
            var user = Context.User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(user))
                throw new HubException("برای استفاده از چت باید وارد سامانه شوید.");
            var roomId = await _chatRoomService.CreateChatRoom(Context.ConnectionId , user);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
            var history = await _messageService.GetChatMessage(roomId);
            if (history.Count > 0)
                await Clients.Caller.SendAsync("loadChatHistory", history);
            else
            {
                var welcomeMessage = await _messageService.SaveChatMessage(roomId, new MessageDto
                {
                    Sender = "پشتیبانی سامانه امید",
                    Message = "سلام، وقت بخیر 👋 چطور می‌توانم کمکتان کنم؟",
                    Time = DateTime.Now
                });
                await Clients.Caller.SendAsync("getNewMessage", welcomeMessage);
                await _supportHub.Clients.All.SendAsync("supportRoomOpened", roomId);
            }
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
        #endregion
    }
}

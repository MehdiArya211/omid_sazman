using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs.SystemChatOnline;
using VisitorManagment.Core.Services.SystemChatOnline;
using VisitorManagment.Web.Hubs;

namespace VisitorManagment.Web.Hubs
{
  
    [Authorize]
    public class SupportHub:Hub
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IMessageService _messageService;

        private readonly IHubContext<SiteChatHub> _siteChathub;
        private readonly IWebHostEnvironment _environment;
        public SupportHub(IChatRoomService chatRoomService,
            IMessageService messageService
            , IHubContext<SiteChatHub> hubContext, IWebHostEnvironment environment)
        {
            _chatRoomService = chatRoomService;
            _messageService = messageService;
            _siteChathub = hubContext;
            _environment = environment;
        }
        #region اعضا و متدهای کلاس

        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>

        public async override Task OnConnectedAsync()
        {
            var rooms = await _chatRoomService.GetAllrooms();
            await Clients.Caller.SendAsync("GetRooms", rooms);
            await base.OnConnectedAsync(); 
        }

        /// <summary>فهرست گفت‌وگوها را پس از دریافت اولین پیام تازه‌سازی می‌کند.</summary>
        public async Task RefreshRooms()
        {
            var rooms = await _chatRoomService.GetAllrooms();
            await Clients.Caller.SendAsync("GetRooms", rooms);
        }


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public async Task LoadMessage(Guid roomId)
        {
            var message = await _messageService.GetChatMessage(roomId);
            await Clients.Caller.SendAsync("getNewMessage", message);
            var readIds = await _messageService.MarkMessagesAsRead(roomId, Context.User.Identity.Name);
            if (readIds.Count > 0) await _siteChathub.Clients.Group(roomId.ToString()).SendAsync("messagesRead", readIds);
        }

        public async Task MarkAsDelivered(Guid roomId)
        {
            var ids = await _messageService.MarkMessagesAsDelivered(roomId, Context.User.Identity.Name);
            if (ids.Count > 0) await _siteChathub.Clients.Group(roomId.ToString()).SendAsync("messagesDelivered", ids);
        }

        public async Task EditMessage(Guid roomId, Guid messageId, string text)
        {
            var message = await _messageService.EditMessage(roomId, messageId, Context.User.Identity.Name, text);
            await _siteChathub.Clients.Group(roomId.ToString()).SendAsync("messageUpdated", message);
            await Clients.Caller.SendAsync("messageUpdated", message);
        }

        public async Task DeleteMessage(Guid roomId, Guid messageId)
        {
            var message = await _messageService.DeleteMessage(roomId, messageId, Context.User.Identity.Name);
            await _siteChathub.Clients.Group(roomId.ToString()).SendAsync("messageDeleted", message);
            await Clients.Caller.SendAsync("messageDeleted", message);
        }

        public Task Typing(Guid roomId, bool isTyping) =>
            _siteChathub.Clients.Group(roomId.ToString()).SendAsync("typingChanged", Context.User.Identity.Name, isTyping);

        public async Task SendAttachment(Guid roomId, string fileName, string contentType, string base64Data, string caption, Guid? replyToMessageId = null)
        {
            var attachment = SaveAttachment(fileName, contentType, base64Data);
            var message = await _messageService.SaveChatMessage(roomId, new MessageDto {
                Sender=Context.User.Identity.Name, Message=caption, Time=DateTime.Now, ReplyToMessageId=replyToMessageId,
                AttachmentUrl=attachment.url, AttachmentName=attachment.name, AttachmentContentType=attachment.type, AttachmentSize=attachment.size
            });
            await _siteChathub.Clients.Group(roomId.ToString()).SendAsync("getNewMessage", message);
            await Clients.Caller.SendAsync("messageSent", message);
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

        /// <summary>
        /// اطلاعات را به مقصد موردنظر ارسال می‌کند.
        /// </summary>
        public async Task SendMessage(Guid roomId,string text, Guid? replyToMessageId = null)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var message = new MessageDto
            {
                Sender = Context.User.Identity.Name,
                Message = text.Trim(),
                Time = DateTime.Now,
                ReplyToMessageId = replyToMessageId
            };

            message = await _messageService.SaveChatMessage(roomId, message);

            await _siteChathub.Clients.Group(roomId.ToString())
                .SendAsync("getNewMessage", message);
            await Clients.Caller.SendAsync("messageSent", message);
                
        }

        #endregion
    }
}

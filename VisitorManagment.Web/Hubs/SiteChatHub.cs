using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
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
        public SiteChatHub(IChatRoomService chatRoomService, IMessageService messageService)
        {
            _chatRoomService = chatRoomService;
            _messageService = messageService;
        }
        #region اعضا و متدهای کلاس


        /// <summary>
        /// اطلاعات را به مقصد موردنظر ارسال می‌کند.
        /// </summary>

        public async Task SendNewMessage(string Sender, string Message)
        {
            if (string.IsNullOrWhiteSpace(Message)) return;
            var roomId = await _chatRoomService.GetChatRoomForConnection(Context.ConnectionId);

            MessageDto messageDto = new MessageDto()
            {
                Message = Message.Trim(),
                Sender = Context.User.Identity.Name ?? Sender ?? "کاربر",
                Time = DateTime.Now,
            };

            await _messageService.SaveChatMessage(roomId,messageDto);
            await Clients.Groups(roomId.ToString())
                .SendAsync("getNewMessage", messageDto.Sender, messageDto.Message, messageDto.Time.ToShortTimeString());
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
            var isSupportConnection = string.Equals(Context.GetHttpContext()?.Request.Query["support"], "true", StringComparison.OrdinalIgnoreCase);
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
            await Clients.Caller.
                SendAsync("getNewMessage", "پشتیبانی سامانه امید", "سلام وقت بخیر 👋 . چطور میتونم کمکتون کنم؟", DateTime.Now.ToShortTimeString());
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

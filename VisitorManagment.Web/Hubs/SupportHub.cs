using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs.SystemChatOnline;
using VisitorManagment.Core.Services.SystemChatOnline;
using VisitorManagment.Web.Hubs;

namespace VisitorManagment.Web.Hubs
{
  
    public class SupportHub:Hub
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IMessageService _messageService;

        private readonly IHubContext<SiteChatHub> _siteChathub;
        public SupportHub(IChatRoomService chatRoomService,
            IMessageService messageService
            , IHubContext<SiteChatHub> hubContext)
        {
            _chatRoomService = chatRoomService;
            _messageService = messageService;
            _siteChathub = hubContext;
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


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public async Task LoadMessage(Guid roomId)
        {
            var message = await _messageService.GetChatMessage(roomId);
            await Clients.Caller.SendAsync("getNewMessage", message);
        }

        /// <summary>
        /// اطلاعات را به مقصد موردنظر ارسال می‌کند.
        /// </summary>
        public async Task SendMessage(Guid roomId,string text)
        {
            var message = new MessageDto
            {
                Sender = Context.User.Identity.Name,
                Message = text,
                Time = DateTime.Now,
            };

            await _messageService.SaveChatMessage(roomId, message);

            await _siteChathub.Clients.Group(roomId.ToString())
                .SendAsync("getNewMessage", message.Sender, message.Message, message.Time.ToShortTimeString());
                
        }
        #endregion
    }
}

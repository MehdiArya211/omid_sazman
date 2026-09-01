using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs.SystemChatOnline;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.SystemChatRoom;

namespace VisitorManagment.Core.Services.SystemChatOnline
{
    public class MessageService : IMessageService
    {
        private readonly VisitorManagmentContext _context;
        public MessageService(VisitorManagmentContext context)
        {
            _context = context;
        }
        #region اعضا و متدهای کلاس


        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>

        public Task<List<MessageDto>> GetChatMessage(Guid RoomId)
        {
            var messages = _context.ChatMessages.Where(p => p.ChatRoomId == RoomId)
                .Select(p => new MessageDto
                {
                    Message = p.Message,
                    Sender = p.Sender,
                    Time = p.Time
                }).OrderBy(p => p.Time).ToList();
            return Task.FromResult(messages);
        }

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public Task SaveChatMessage(Guid RoomId, MessageDto message)
        {
            var room = _context.ChatRooms.SingleOrDefault(p => p.Id == RoomId);
            if (room == null)
                throw new InvalidOperationException("اتاق گفت‌وگو وجود ندارد.");
            if (message == null || string.IsNullOrWhiteSpace(message.Message))
                throw new ArgumentException("متن پیام نمی‌تواند خالی باشد.", nameof(message));
            var text = message.Message.Trim();
            ChatMessage chatMessage = new ChatMessage()
            {
                ChatRoom = room,
                Message = text.Length > 2000 ? text.Substring(0, 2000) : text,
                Sender = message.Sender,
                Time = message.Time,
            };
            _context.ChatMessages.Add(chatMessage);
            return _context.SaveChangesAsync();
        }
        #endregion
    }
}

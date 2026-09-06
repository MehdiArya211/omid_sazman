using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            var messages = _context.ChatMessages.AsNoTracking().Where(p => p.ChatRoomId == RoomId)
                .Select(p => new MessageDto
                {
                    Id = p.Id,
                    Message = p.Message,
                    Sender = p.Sender,
                    Time = p.Time,
                    ReplyToMessageId = p.ReplyToMessageId,
                    ReplyToSender = p.ReplyToSender,
                    ReplyToMessage = p.ReplyToMessage
                }).OrderBy(p => p.Time).ToListAsync();
            return messages;
        }

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public async Task<MessageDto> SaveChatMessage(Guid RoomId, MessageDto message)
        {
            var room = await _context.ChatRooms.SingleOrDefaultAsync(p => p.Id == RoomId);
            if (room == null)
                throw new InvalidOperationException("اتاق گفت‌وگو وجود ندارد.");
            if (message == null || string.IsNullOrWhiteSpace(message.Message))
                throw new ArgumentException("متن پیام نمی‌تواند خالی باشد.", nameof(message));
            var text = message.Message.Trim();
            ChatMessage repliedMessage = null;
            if (message.ReplyToMessageId.HasValue)
            {
                repliedMessage = await _context.ChatMessages.AsNoTracking()
                    .SingleOrDefaultAsync(p => p.Id == message.ReplyToMessageId.Value && p.ChatRoomId == RoomId);
                if (repliedMessage == null)
                    throw new InvalidOperationException("پیام انتخاب‌شده برای پاسخ در این گفت‌وگو وجود ندارد.");
            }
            ChatMessage chatMessage = new ChatMessage()
            {
                Id = Guid.NewGuid(),
                ChatRoom = room,
                Message = text.Length > 2000 ? text.Substring(0, 2000) : text,
                Sender = string.IsNullOrWhiteSpace(message.Sender) ? "کاربر" : message.Sender.Trim(),
                Time = message.Time == default ? DateTime.Now : message.Time,
                ReplyToMessageId = repliedMessage?.Id,
                ReplyToSender = repliedMessage?.Sender,
                ReplyToMessage = repliedMessage == null ? null :
                    (repliedMessage.Message.Length > 180 ? repliedMessage.Message.Substring(0, 180) : repliedMessage.Message)
            };
            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            return new MessageDto
            {
                Id = chatMessage.Id,
                Sender = chatMessage.Sender,
                Message = chatMessage.Message,
                Time = chatMessage.Time,
                ReplyToMessageId = chatMessage.ReplyToMessageId,
                ReplyToSender = chatMessage.ReplyToSender,
                ReplyToMessage = chatMessage.ReplyToMessage
            };
        }
        #endregion
    }
}

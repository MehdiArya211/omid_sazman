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
                    ,EditedAt = p.EditedAt, DeletedAt = p.DeletedAt, IsDeleted = p.IsDeleted,
                    IsRead = p.IsRead, ReadAt = p.ReadAt, AttachmentUrl = p.AttachmentUrl,
                    IsDelivered = p.IsDelivered, DeliveredAt = p.DeliveredAt,
                    AttachmentName = p.AttachmentName, AttachmentContentType = p.AttachmentContentType,
                    AttachmentSize = p.AttachmentSize
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
            if (message == null || (string.IsNullOrWhiteSpace(message.Message) && string.IsNullOrWhiteSpace(message.AttachmentUrl)))
                throw new ArgumentException("متن یا پیوست پیام الزامی است.", nameof(message));
            var text = (message.Message ?? string.Empty).Trim();
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
                    ((repliedMessage.Message ?? repliedMessage.AttachmentName ?? "پیوست").Length > 180
                        ? (repliedMessage.Message ?? repliedMessage.AttachmentName ?? "پیوست").Substring(0, 180)
                        : (repliedMessage.Message ?? repliedMessage.AttachmentName ?? "پیوست")),
                AttachmentUrl = message.AttachmentUrl,
                AttachmentName = message.AttachmentName,
                AttachmentContentType = message.AttachmentContentType,
                AttachmentSize = message.AttachmentSize
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
                ,IsRead = chatMessage.IsRead, AttachmentUrl = chatMessage.AttachmentUrl,
                IsDelivered = chatMessage.IsDelivered, DeliveredAt = chatMessage.DeliveredAt,
                AttachmentName = chatMessage.AttachmentName, AttachmentContentType = chatMessage.AttachmentContentType,
                AttachmentSize = chatMessage.AttachmentSize
            };
        }

        public async Task<MessageDto> EditMessage(Guid roomId, Guid messageId, string sender, string text)
        {
            var message = await _context.ChatMessages.SingleOrDefaultAsync(x => x.Id == messageId && x.ChatRoomId == roomId);
            if (message == null || message.IsDeleted) throw new InvalidOperationException("پیام پیدا نشد.");
            if (!string.Equals(message.Sender, sender, StringComparison.OrdinalIgnoreCase)) throw new UnauthorizedAccessException("ویرایش این پیام مجاز نیست.");
            text = (text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(text) && string.IsNullOrWhiteSpace(message.AttachmentUrl)) throw new ArgumentException("متن پیام خالی است.");
            message.Message = text.Length > 2000 ? text.Substring(0, 2000) : text;
            message.EditedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return ToDto(message);
        }

        public async Task<MessageDto> DeleteMessage(Guid roomId, Guid messageId, string sender)
        {
            var message = await _context.ChatMessages.SingleOrDefaultAsync(x => x.Id == messageId && x.ChatRoomId == roomId);
            if (message == null) throw new InvalidOperationException("پیام پیدا نشد.");
            if (!string.Equals(message.Sender, sender, StringComparison.OrdinalIgnoreCase)) throw new UnauthorizedAccessException("حذف این پیام مجاز نیست.");
            message.IsDeleted = true; message.DeletedAt = DateTime.Now; message.Message = string.Empty;
            await _context.SaveChangesAsync();
            return ToDto(message);
        }

        public async Task<List<Guid>> MarkMessagesAsRead(Guid roomId, string reader)
        {
            var messages = await _context.ChatMessages.Where(x => x.ChatRoomId == roomId && !x.IsRead && x.Sender != reader).ToListAsync();
            var now = DateTime.Now;
            foreach (var message in messages) { message.IsDelivered = true; message.DeliveredAt = message.DeliveredAt ?? now; message.IsRead = true; message.ReadAt = now; }
            if (messages.Count > 0) await _context.SaveChangesAsync();
            return messages.Select(x => x.Id).ToList();
        }

        public async Task<List<Guid>> MarkMessagesAsDelivered(Guid roomId, string receiver)
        {
            var messages = await _context.ChatMessages.Where(x => x.ChatRoomId == roomId && !x.IsDelivered && x.Sender != receiver).ToListAsync();
            var now = DateTime.Now;
            foreach (var message in messages) { message.IsDelivered = true; message.DeliveredAt = now; }
            if (messages.Count > 0) await _context.SaveChangesAsync();
            return messages.Select(x => x.Id).ToList();
        }

        private static MessageDto ToDto(ChatMessage p) => new MessageDto
        {
            Id=p.Id, Sender=p.Sender, Message=p.Message, Time=p.Time, ReplyToMessageId=p.ReplyToMessageId,
            ReplyToSender=p.ReplyToSender, ReplyToMessage=p.ReplyToMessage, EditedAt=p.EditedAt,
            DeletedAt=p.DeletedAt, IsDeleted=p.IsDeleted, IsRead=p.IsRead, ReadAt=p.ReadAt,
            IsDelivered=p.IsDelivered, DeliveredAt=p.DeliveredAt,
            AttachmentUrl=p.AttachmentUrl, AttachmentName=p.AttachmentName,
            AttachmentContentType=p.AttachmentContentType, AttachmentSize=p.AttachmentSize
        };
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs.SystemChatOnline;

namespace VisitorManagment.Core.Services.SystemChatOnline
{
    public interface IMessageService
    {
        Task<MessageDto> SaveChatMessage(Guid RoomId, MessageDto message);
        Task<List<MessageDto>> GetChatMessage(Guid RoomId);
        Task<MessageDto> EditMessage(Guid roomId, Guid messageId, string sender, string text);
        Task<MessageDto> DeleteMessage(Guid roomId, Guid messageId, string sender);
        Task<List<Guid>> MarkMessagesAsRead(Guid roomId, string reader);
        Task<List<Guid>> MarkMessagesAsDelivered(Guid roomId, string receiver);
    }
}

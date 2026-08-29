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
        Task SaveChatMessage(Guid RoomId, MessageDto message);
        Task<List<MessageDto>> GetChatMessage(Guid RoomId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs.SystemChatOnline;

namespace VisitorManagment.Core.Services.SystemChatOnline
{
    public interface IChatRoomService
    {
        Task<Guid> CreateChatRoom(string ConnectionId , string personalCode);
        Task<Guid> GetChatRoomForConnection(string CoonectionId);
        //Task<List<Guid>> GetAllrooms();
        Task<List<ChatRoomDTO>> GetAllrooms();
    }
}

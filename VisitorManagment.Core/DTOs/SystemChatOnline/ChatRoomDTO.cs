using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Core.DTOs.SystemChatOnline
{
    public class ChatRoomDTO
    {
        public Guid Id { get; set; }
        public string ConnectionId { get; set; }
        public int? UserId { get; set; }
        public Users User { get; set; }
        public string Title { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public int MessageCount { get; set; }
    }
}

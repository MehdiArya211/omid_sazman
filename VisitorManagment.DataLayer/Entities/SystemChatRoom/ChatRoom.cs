using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.DataLayer.Entities.SystemChatRoom
{
    public class ChatRoom
    {
        public Guid Id { get; set; }
        public string ConnectionId { get; set; }
        public int? UserId { get; set; }
        public Users User { get; set; }
        public string Title { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }

    }
}

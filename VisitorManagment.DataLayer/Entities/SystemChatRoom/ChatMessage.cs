using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.DataLayer.Entities.SystemChatRoom
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public ChatRoom ChatRoom { get; set; }
        public Guid ChatRoomId { get; set; }
    }
}

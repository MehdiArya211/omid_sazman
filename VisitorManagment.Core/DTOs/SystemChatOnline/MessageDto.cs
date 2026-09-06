using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs.SystemChatOnline
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public Guid? ReplyToMessageId { get; set; }
        public string ReplyToSender { get; set; }
        public string ReplyToMessage { get; set; }
        public DateTime? EditedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsDelivered { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string AttachmentUrl { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentContentType { get; set; }
        public long? AttachmentSize { get; set; }
    }
}

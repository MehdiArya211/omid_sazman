using System;
using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.DataLayer.Entities.OnlineConversation
{
    /// <summary>
    /// پیام پایدار چت مربوط به یک جلسه ارتباط تصویری.
    /// اطلاعات فرستنده به‌صورت Snapshot نگهداری می‌شود تا تاریخچه مستقل از تغییرات بعدی کاربر باقی بماند.
    /// </summary>
    public class OnlineConversationMessage
    {
        [Key]
        public Guid Id { get; set; }

        public int MeetingId { get; set; }

        public int SenderUserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string SenderName { get; set; }

        [MaxLength(300)]
        public string SenderAvatar { get; set; }

        [Required]
        [MaxLength(4000)]
        public string Message { get; set; }

        public DateTime SentAtUtc { get; set; }

        public bool IsDelivered { get; set; }

        public DateTime? DeliveredAtUtc { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadAtUtc { get; set; }
    }
}

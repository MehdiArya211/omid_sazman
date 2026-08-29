using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System;
using VisitorManagment.DataLayer.Entities;
using VisitorManagment.DataLayer.Entities.NotificationInfo;

namespace VisitorManagment.DataLayer
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }

        public bool IsActive { get; set; }

        // ارتباط با پیوست‌ها
        public ICollection<NotificationAttachment> Attachments { get; set; } = new List<NotificationAttachment>();

        // ارتباط با مشاهده اطلاعیه توسط کاربران
        public ICollection<NotificationUser> NotificationUsers { get; set; } = new List<NotificationUser>();
    }
}

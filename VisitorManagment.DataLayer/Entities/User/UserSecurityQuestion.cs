using System;
using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.DataLayer.Entities.User
{
    /// <summary>هش پاسخ‌های امنیتی و محدودیت تلاش بازیابی هر حساب.</summary>
    public class UserSecurityQuestion
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(200)]
        public string Answer1Hash { get; set; }

        [Required, MaxLength(200)]
        public string Answer2Hash { get; set; }

        [Required, MaxLength(200)]
        public string Answer3Hash { get; set; }

        public int FailedAttempts { get; set; }
        public DateTime? LockedUntilUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.DataLayer.Entities
{
    /// <summary>
    /// تصویر قابل مدیریت اسلایدر صفحه ورود را نگهداری می‌کند.
    /// فایل تصویر در wwwroot و اطلاعات نمایشی آن در دیتابیس ذخیره می‌شود.
    /// </summary>
    public class LoginSlide
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string ImagePath { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [MaxLength(150)]
        public string AltText { get; set; }

        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

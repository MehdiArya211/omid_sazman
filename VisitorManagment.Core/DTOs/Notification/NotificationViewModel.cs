using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs.Notification
{
    public class NotificationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان اطلاعیه الزامی است")]
        public string Title { get; set; }

        [Required(ErrorMessage = "متن اطلاعیه الزامی است")]
        public string Content { get; set; }

        [Required(ErrorMessage = "تاریخ انتشار الزامی است")]
        public DateTime PublishDate { get; set; }

        public bool IsActive { get; set; }

        // فایل‌های پیوست
        [Display(Name = "Attachments")]
        public List<IFormFile> Attachments { get; set; } // لیست از IFormFile
    }

}

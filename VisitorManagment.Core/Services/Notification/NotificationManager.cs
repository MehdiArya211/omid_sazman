using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.Core.Generator;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities;
using VisitorManagment.DataLayer.Entities.NotificationInfo;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Core.Services.Notification
{
    public class NotificationManager : INotificationManager
    {
        private readonly VisitorManagmentContext _context;
        public NotificationManager(VisitorManagmentContext context)
        {
            _context = context;
        }
        #region اعضا و متدهای کلاس


        /// <summary>
        /// لیست تمام اطلاعیه هایی که کاربران مشاهده نکرده اند
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public BaseResult GetAllNotificationNoWatchingUser(int userId)
        {
            var Notifications = _context.Notifications
                         .Where(n => !_context.NotificationUsers
                             .Any(nu => nu.NotificationId == n.Id && nu.UserId == userId))
                         .ToList();

            if (Notifications != null)
            {
                return new BaseResult
                {
                    Message = "",
                    Model = Notifications,
                    Status = true
                };
            }
            return new BaseResult
            {
                Message = "",
                //Model = ,
                Status = false
            };
        }

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public BaseResult Create(DataLayer.Notification Notification)
        {
            var notification = new DataLayer.Notification
            {
                Title = Notification.Title,
                Content = Notification.Content,
                PublishDate = Notification.PublishDate,
                IsActive = Notification.IsActive,
                Attachments = new List<NotificationAttachment>()
            };

            // ذخیره فایل‌های پیوست
            if (Notification.Attachments != null && Notification.Attachments.Count > 0)
            {
                foreach (var file in Notification.Attachments)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/NotificationAttachment");


                    var filePath = Path.Combine(uploadsFolder, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                       // file.CopyTo(stream); // ذخیره‌سازی فایل
                    }

                    notification.Attachments.Add(new NotificationAttachment
                    {
                        FileName = file.FileName,
                        FilePath = "/uploads/" + file.FileName, // مسیر ذخیره شده در پایگاه داده
                        FileType = file.FileType,
                        FileSize = file.FileSize
                    });
                }
            }

            _context.Notifications.Add(notification);
            //  await _context.SaveChangesAsync();

            return new BaseResult
            {
            };
        }
        #endregion
    }
}

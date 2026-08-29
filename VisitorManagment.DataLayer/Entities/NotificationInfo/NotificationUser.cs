using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.DataLayer.Entities.NotificationInfo
{
    public class NotificationUser
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public DateTime ViewedDate { get; set; }

        public Notification Notification { get; set; }
    }
}

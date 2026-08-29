using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.DataLayer.Entities.NotificationInfo;

namespace VisitorManagment.Core.Services.Notification
{
    public interface INotificationManager
    {
        BaseResult GetAllNotificationNoWatchingUser(int userId);
        BaseResult  Create(DataLayer.Notification Notification );
    }
}

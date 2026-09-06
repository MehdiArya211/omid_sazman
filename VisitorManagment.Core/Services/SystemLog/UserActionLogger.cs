using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs.SystemLog;

namespace VisitorManagment.Core.Services.SystemLog
{
    public class UserActionLogger : IUserActionLogger
    {
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public Task LogAsync(UserActionLog log)
        {
            Log.ForContext("LogType", "UserAction")
               .ForContext("UserId", log.UserId)
               .ForContext("UserName", log.UserName)
               .ForContext("Controller", log.Controller)
               .ForContext("Action", log.Action)
               .ForContext("IsSuccess", log.IsSuccess)
               .ForContext("IpAddress", log.IpAddress)
               .Information("User action executed");

            return Task.CompletedTask;

        }
    }

}

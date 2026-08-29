using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs.SystemLog;

namespace VisitorManagment.Core.Services.SystemLog
{
    public interface IUserActionLogger
    {
        Task LogAsync(UserActionLog log);
    }

}

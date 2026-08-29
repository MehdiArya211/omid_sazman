using System;

namespace VisitorManagment.Core.DTOs.SystemLog
{
    public class UserActionLog
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }

}

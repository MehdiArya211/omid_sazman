using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs
{
    public  class PermissionViewModel
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public string RoleTitle { get; set; }
        public string PermossionTitle { get; set; }
    }
}

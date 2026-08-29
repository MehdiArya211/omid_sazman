using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs
{
    public class RoomInfo
    {
        public string RoomId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string HostConnectionId { get; set; }
    }
}
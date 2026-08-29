using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs.Fajr
{
    public class ItoFajrDTO
    {
        public long Id { get; set; }
        public DateTime? Time { get; set; }
        public string SoftwareVersion { get; set; }
        public string SoftwareId { get; set; }
        public string Url { get; set; }
        public string SoftwareName { get; set; }
        public string ServerIp { get; set; }
        public string PortNumber { get; set; }
        public string ServerHostname { get; set; }
        public string ClientHostname { get; set; }
        public string ClientIp { get; set; }
        public string PageTitle { get; set; }
        public string UserUniqueId { get; set; }
        public string Username { get; set; }
        public string Sensitivity { get; set; }
        public string Importance { get; set; }
        public string ActionType { get; set; }
        public string SubType { get; set; }
        public string SubTypeDescription { get; set; }
        public string Flag { get; set; }
        public string SendDate { get; set; }
        public byte? SendStat { get; set; }
        public short? SendCount { get; set; }
    }
}

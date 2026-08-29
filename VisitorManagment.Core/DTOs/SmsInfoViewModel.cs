using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs
{

    public class ListSMSInfoViewModel
    {
        public List<SMSInfoViewModel> sMSInfos { get; set; }
    }
    public class SMSInfoViewModel
    {

        public DateTime CreateDate { get; set; }
        public string Mobile { get; set; }
        public string PrsnNo { get; set; }
        public string NationalNo { get; set; }
        public int SMSTypeId { get; set; }
        public int SystemTypeId { get; set; }
        public string FullName { get; set; }
        public string SmsBody { get; set; }
    }
}


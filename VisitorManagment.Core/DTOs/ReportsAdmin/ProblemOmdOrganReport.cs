using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs.ReportsAdmin
{
    public class ProblemOmdOrganReport
    {
        public int TCount { get; set; }
        public int? UnitCode { get; set; }
        public string UnitTitle { get; set; }
        public int? RequestSubjectId { get; set; }
        public string RequestSubjectTitle { get; set; }
        public int? CodeGha { get; set; }
        public int? UnitDutyCode { get; set; }

    }
}

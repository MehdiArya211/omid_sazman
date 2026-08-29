using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.DataLayer.Entities.Views
{
    public class ViwHamesh
    {
        [Key]
        public int Id { get; set; }
        public int FileId { get; set; }
        public int UserId { get; set; }
        public int? ParentId { get; set; }
        public int ActionTypeId { get; set; }
        public string UserDesc { get; set; }
        public DateTime RegDate { get; set; }
        public int RoleTypeId { get; set; }
        public string RoleTypeTitle { get; set; }
        public int? UserSubId { get; set; }
        public int RequestSubjectId { get; set; }
        public string RequestSubjectTitle { get; set; }
        public string ActionTypeTitle { get; set; }
        public int? CodGha { get; set; }
        public string CodGhaTitle { get; set; }
        public int UnitCode { get; set; }
        public int UnitDutyCode { get; set; }
        public string UnitDutyTitle { get; set; }
        public string UnitTitle { get; set; }

        public int TCount { get; set; }
    }
}

   
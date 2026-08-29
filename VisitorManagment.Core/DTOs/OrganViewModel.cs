using System;
using System.Collections.Generic;
using System.Text;

namespace VisitorManagment.Core.DTOs
{
   public class OrganViewModel
    {
        public int? Id { get; set; }
        public int ParentId { get; set; }
        public int OmdId { get; set; }
        public string UnitTitle { get; set; }
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string ArshadTitle { get; set; }
        public int? CityId { get; set; }
        public string AqidatiCode { get; set; }
        public bool? IsActive { get; set; }
        public int OrganTypeId { get; set; }

    }

    public class OrganViewModelDto
    {
        public int Id { get; set; }
        public int UnitCode { get; set; }
        public string Title { get; set; }


    }
}

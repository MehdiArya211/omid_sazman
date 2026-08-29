using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs
{
    public class VamViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string VamCodeTitle { get; set; }
        public long VamPrice { get; set; }
        public int FileId { get; set; }
        public int VamCodeId { get; set; }
        public int CodeVam { get; set; }
        public int RegUserId { get; set; }
        public DateTime RegDate { get; set; }
        public bool IsDelete { get; set; }
    }

    public class ListVamViewModel
    {
        List<VamViewModel> listVam { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
    public class Vam
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }
        public int FileId { get; set; }
        public int VamCodeId { get; set; }

        [Display(Name = "کد وام")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public int CodeVam { get; set; }
        public int RegUserId { get; set; }
        public DateTime RegDate { get; set; }
        public bool IsDelete { get; set; }


        #region Relations
        public VamCode VamCode { get; set; }
        public Files File { get; set; }

        #endregion
    }
}

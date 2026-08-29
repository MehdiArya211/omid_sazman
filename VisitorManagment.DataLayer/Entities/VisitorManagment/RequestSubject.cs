using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
   public class RequestSubject
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "موضوع درخواست")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }
        public int Code { get; set; }

        #region Relations
        public List<Files> Documents { get; set; }

        #endregion
    }
}

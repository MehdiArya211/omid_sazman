using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
   public class MeetingStatus
    {

        [Key]
        public int Id { get; set; }
        [Display(Name = "وضعیت جلسه")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Required(ErrorMessage = "{0} را وارد کنید")]
        public int Code { get; set; }
        public bool IsDelete { get; set; }

        #region Relations

        #endregion
    }
}

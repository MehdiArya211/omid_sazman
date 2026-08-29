using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
   public class BoseMeeting
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "نام ")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string FullName { get; set; }

        [Display(Name = "درجه ")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string RankTitle { get; set; }

        public bool IsDelete { get; set; }

        #region Relations
        #endregion
    }
}

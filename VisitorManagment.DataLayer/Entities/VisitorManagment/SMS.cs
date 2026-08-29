using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
  public  class SMS
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = " متن پیام فارسی")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(400, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string TitleFa { get; set; }

        [Display(Name = " متن پیام انگلیسی")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string TitleEn { get; set; }
        public int SortNumber { get; set; }
        public bool IsActive { get; set; }

    }
}

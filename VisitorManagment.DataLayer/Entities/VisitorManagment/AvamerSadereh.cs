using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
    public class AvamerSadereh
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "اوامر")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }
        public int Code { get; set; }
    }
}

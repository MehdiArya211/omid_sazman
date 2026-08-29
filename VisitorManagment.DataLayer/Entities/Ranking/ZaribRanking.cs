using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.DataLayer.Entities.Ranking
{
    public class ZaribRanking
    {
        [Key]
        public int Id   { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Display(Name = "کد")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public int Code { get; set; }

        [Display(Name = "ضریب")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public int Zarib { get; set; }
    }
}

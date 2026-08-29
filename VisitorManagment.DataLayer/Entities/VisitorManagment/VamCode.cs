using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
    public class VamCode
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }
        [Display(Name = "کد وام")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public int Code { get; set; }
        [Display(Name = "مبلغ وام")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public long Price { get; set; }
        public int SortNum { get; set; }

        #region Relations
        public List<Vam> Vams { get; set; }
        public List<Hamesh> Hameshes { get; set; }

        #endregion
    }
}

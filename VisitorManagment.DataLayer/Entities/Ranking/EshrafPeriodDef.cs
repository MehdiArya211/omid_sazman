using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.DataLayer.Entities.Ranking
{
    public class EshrafPeriodDef
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "شناسه بازه ی گزارش ")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Display(Name = " شروع بازه زمانی ")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public DateTime StartDate { get; set; }


        [Display(Name = " پایان بازه زمانی ")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public DateTime EndDate { get; set; }

        #region Relation
        public List<Point> Points { get; set; }
        #endregion


    }
}

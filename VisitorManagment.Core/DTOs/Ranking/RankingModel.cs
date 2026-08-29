using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs.Ranking
{
    public class PointViewModel
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentTitle { get; set; }

        [Display(Name = "شناسه بازه ی گزارش ")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public int EshrafPeriodDefId { get; set; }
        public string EshrafPeriodDefTitle { get; set; }

        [Display(Name = "کد یگان")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public int UnitCode { get; set; }

        [Display(Name = "عنوان یگان")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string UnitTitle { get; set; }

        [Display(Name = "کد قرارگاه")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public int? CodeGha { get; set; }

        [Display(Name = "عنوان قرارگاه")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string GhaTitle { get; set; }

        #region نمرات / رتبه
        public int? PointEghdam { get; set; }
        public int? PointReject { get; set; }
        public int? PointNezaja { get; set; }
        public int? FinalPoint { get; set; }
        public int? Rank { get; set; }
        #endregion


    }


    public class ListPointViewModel
    {
        public List<PointViewModel> Points { get; set; }

        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int skip { get; set; }
        public int count { get; set; }
    }
}

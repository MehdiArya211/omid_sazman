using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.DTOs
{
    public class ReportTestInfoViewModel
    {
        public string PersonalCode { get; set; }
       // public string MelliCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RankTitle { get; set; }
        public string BranchTitle { get; set; }
        public string UnitTitle { get; set; }
        public string UnitDutyTitle { get; set; }
        public string JobDes { get; set; }
        public string HameshGharagah { get; set; }
        public string HameshHeiatRaeiseh { get; set; }
        public List<HameshInfoViewModel> HameshMoavenat { get; set; }
       // public string HameshMoavenat { get; set; }

        #region HameshUnit
        public string HameshUnit { get; set; }
        //public string HameshUnitSemat { get; set; }
        //public string HameshUnitDaraje { get; set; }
        //public string HameshUnitFullName { get; set; }
        //public string HameshUnitRegDate { get; set; }
        #endregion

        public string HameshUnitDuty { get; set; }

        public string CodGhaTitle { get; set; }

        [Display(Name = " درصد جانبازی ارتش")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public decimal? DrsadJa { get; set; }

        [Display(Name = " درصد جانبازی بنیاد")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public decimal? DrsadJb { get; set; }

        //[Display(Name = "وضعیت ایثارگری")]
        //[MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        //public string IsarStatus { get; set; }

        [Display(Name = "مدت خدمت در منطقه عملیاتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string TotAml2 { get; set; }

        [Display(Name = " مدت خدمت در منطقه قبل قطعنامه")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string TotAml { get; set; }

        //[Display(Name = " نام فرمانده")]
        //[MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        //public string FarmandehPersonalName { get; set; }

        [Display(Name = "تلفن همراه")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Phone { get; set; }
        public string Address { get; set; }
        public string RegDate { get; set; }


        [Display(Name = "شرح درخواست")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(5000, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string RequestDescription { get; set; }
        public string ProblemDescription { get; set; }
        public string MarridTitle { get; set; }
        public string EmploymentDate { get; set; }
       // public string LocationJob { get; set; }
        public List<EnteghaInfolViewModel> LocationJob { get; set; }
        public int? TashvighatCount { get; set; }
        public int? TanbihatCount { get; set; }
        public int? NahastCount { get; set; }
        public int? FararCount { get; set; }
        public string EmployDate { get; set; }
        //Enteghalate

        #region job location
        public string FromOrgan { get; set; }
        public int? Mntprevious { get; set; }
        public string WentDate { get; set; }
        public string ToOrgan { get; set; }
        public int? Mntnew { get; set; }
        #endregion

        #region tashilate
        public string TashilatBelaavazRowBeginDate { get; set; }
        public string TashilatBelaavazMaliTitle { get; set; }
        public string TashilatBelaavazMablagheVam { get; set; }
        public string TashilatDaryafti { get; set; }
        public string TashilatDaryaftiMablagheVam { get; set; }
        public string TashilatDaryaftiRowBeginDate { get; set; }
        public string TashilatDaryaftiMaliTitle { get; set; }

        #endregion

        public string MeetingDate { get; set; }
        public string RolePersonLogin { get; set; }
        public string HameshKarshenasGharagahAnsarNezaja { get; set; }
        #region Fish Information
        [Display(Name = "حقوق کل")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public double TotalMoney { get; set; }

        [Display(Name = "میزان حقوق دریافتی")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public double ReciveMoney { get; set; }

        [Display(Name = "تعداد وام")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public int CountVam { get; set; }

        [Display(Name = "میزان حقوق دریافتی")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public double SumAghsatVamMahiyaneh { get; set; }
        #endregion

        //end Enteghalat

    }

    public class ReportTestInfoTestViewModel
    {
        public string PersonalCode { get; set; }
        public string RequestDescription { get; set; }
        //

    }

    public class ReportTestInfoViewModelV2
    {
        public string HameshUnit { get; set; }
        public string HameshGharagah { get; set; }
    }
}

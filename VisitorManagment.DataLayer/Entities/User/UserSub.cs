using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.DataLayer.Entities.User
{
    public class UserSub
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string UserName { get; set; }

        [Display(Name = "کدپرسنلی ف عمده")]
        [MaxLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string FPrsnNo { get; set; }

        [Display(Name = "کلمه عبور")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Password { get; set; }

        [Display(Name = "نام")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string LastName { get; set; }



        [Display(Name = "عنوان شغل")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string JobDes { get; set; }


        [Display(Name = "کد یگان خدمتی")]
        public int? UnitDutyCode { get; set; }

        [Display(Name = "کد یگان عمده")]
        public int? UnitCode { get; set; }

        [Display(Name = "نام یگان خدمتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string UnitDutyTitle { get; set; }

        [Display(Name = "نام یگان عمده")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string UnitTitle { get; set; }



        [Display(Name = "نام قرارگاه منطقه ای / ارشد نظامی ")]
        public int? CodGha { get; set; }

        [Display(Name = "کد قرارگاه منطقه ای / ارشد نظامی ")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string CodGhaTitle { get; set; }

        #region Relations
        public List<UserRole> UserRoles { get; set; }
        public List<Hamesh> Hameshes { get; set; }

        #endregion    
    }
}

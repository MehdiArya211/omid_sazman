using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.Core.DTOs
{
    public class UserForAdminViewModel
    {
        public List<ListUserViewModel> Users { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int skip { get; set; }
        public int count { get; set; }
    }

    public class CreateUserViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Password { get; set; }
        public IFormFile UserAvatar { get; set; }

        [Display(Name = "عنوان رسته")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string BranchTitle { get; set; }

        [Display(Name = "کد رسته")]
        public int? BranchCode { get; set; }

        [Display(Name = "عنوان شغل")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string JobDes { get; set; }
        [Display(Name = "درجه")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string RankTitle { get; set; }

        [Display(Name = " کد درجه")]
        public int? RankCode { get; set; }
        public int Id { get; set; }
        public string PersonalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Display(Name = "کد یگان خدمتی")]
        public int? UnitDutyCode { get; set; }

        [Display(Name = "کد یگان عمده")]
        public int UnitCode { get; set; }
        [Display(Name = "نام یگان خدمتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string UnitDutyTitle { get; set; }

        [Display(Name = "نام یگان عمده")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string UnitTitle { get; set; }

        [Display(Name = "کد قرارگاه منطقه ای / ارشد نظامی ")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string CodGhaTitle { get; set; }

        [Display(Name = "نام قرارگاه منطقه ای / ارشد نظامی ")]

        public int? CodGha { get; set; }
        public int AddUserId { get; set; }
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string SaveDate { get; set; }
        public int? EditUserId { get; set; }
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string EditDate { get; set; }

        public int UserRoles { get; set; }
        public string UserRolesTitle { get; set; }
        public string AvatarName { get; set; }
    }

    public class UserInfoViewModel : CreateUserViewModel
    {

    }

    public class EditUserViewModel : CreateUserViewModel
    {
        //public int UserId { get; set; }

        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
        public int UserRolesId { get; set; }

    }

    public class ListUserViewModel
    {
        public int Id { get; set; }
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
        public string RoleTitle { get; set; }

        [Display(Name = "کلمه عبور")]

        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IFormFile UserAvatar { get; set; }
        public List<int> UserRoles { get; set; }
        public string AvatarName { get; set; }
        //
        [Display(Name = "عنوان رسته")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string BranchTitle { get; set; }

        [Display(Name = "کد رسته")]
        public int? BranchCode { get; set; }

        [Display(Name = "عنوان شغل")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string JobDes { get; set; }

        [Display(Name = "کد فعال سازی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string ActiveCode { get; set; }

        [Display(Name = "وضعیت")]
        public bool IsActive { get; set; }

        [Display(Name = "درجه")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string RankTitle { get; set; }

        [Display(Name = " کد درجه")]
        public int? RankCode { get; set; }
        public bool IsDelete { get; set; }

        [Display(Name = "کد یگان خدمتی")]
        public int? UnitDutyCode { get; set; }

        [Display(Name = "کد یگان عمده")]
        public int UnitCode { get; set; }

        [Display(Name = "نام یگان خدمتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string UnitDutyTitle { get; set; }

        [Display(Name = "نام یگان عمده")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string UnitTitle { get; set; }

        [Display(Name = "کد قرارگاه منطقه ای / ارشد نظامی ")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string CodGhaTitle { get; set; }
        [Display(Name = "نام قرارگاه منطقه ای / ارشد نظامی ")]
        public int? CodGha { get; set; }

        public int RegUserId { get; set; }

        public string RegUserTitle { get; set; }


        [Display(Name = "تاریخ ثبت نام")]
        public DateTime RegDate { get; set; }
        public int? EditUserId { get; set; }
        public DateTime? EditDate { get; set; }

    }

    /// <summary>
    ///لیست کاربران برای ارسال به مراتب بالاتر
    /// </summary>
    public class ListUserForSendToCartable
    {
        public List<UserForSendToCartable> Users { get; set; }

        //public int ActionTypeId { get; set; }
        //public string UserDesc { get; set; }
        //public int FileId { get; set; }
    }

    public class ListUserForAoudatToCartable: ListUserForSendToCartable
    {

    }


    public class UserForSendToCartable
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        [Display(Name = "عنوان رسته")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string BranchTitle { get; set; }

        [Display(Name = "عنوان شغل")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string JobDes { get; set; }
        [Display(Name = "درجه")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string RankTitle { get; set; }
        public string PersonalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Display(Name = "کد یگان خدمتی")]
        public int? UnitDutyCode { get; set; }

        [Display(Name = "کد یگان عمده")]
        public int UnitCode { get; set; }
        [Display(Name = "نام یگان خدمتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string UnitDutyTitle { get; set; }

        [Display(Name = "نام یگان عمده")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string UnitTitle { get; set; }

        [Display(Name = "کد قرارگاه منطقه ای / ارشد نظامی ")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string CodGhaTitle { get; set; }

        [Display(Name = "نام قرارگاه منطقه ای / ارشد نظامی ")]

        public int? CodGha { get; set; }
    }


    public class SignUpViewModel
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public bool IsDelete{ get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string UserName { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Password { get; set; }

        [Display(Name = "تکرار کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string RePassword { get; set; }
    }

    #region ChangePasswordViewModel
    public class ForgetPasswordViewModel : SignUpViewModel
    {

    }
    #endregion

    public class checkingFPrsnNoViewModel
    {
        public string  PrsnNo { get; set; }
        public bool  Respond { get; set; }
    }

}

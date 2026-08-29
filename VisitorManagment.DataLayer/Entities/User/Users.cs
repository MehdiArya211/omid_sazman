using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using VisitorManagment.DataLayer.Entities.SystemChatRoom;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.DataLayer.Entities.User
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string UserName { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Password { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string LastName { get; set; }

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

        [Display(Name = "آواتار")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string UserAvatar { get; set; }

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

        [Display(Name = "تاریخ ثبت نام")]
        public DateTime RegDate { get; set; }
        public int? EditUserId { get; set; }
        public DateTime? EditDate { get; set; }
        #region Relations
        public List<UserRole> UserRoles { get; set; }
        public List<Hamesh> Hameshes { get; set; }
        public List<ChatRoom> ChatRooms { get; set; }

        #endregion    
    }
}

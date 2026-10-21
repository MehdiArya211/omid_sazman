using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
   public class Hamesh
    {
        [Key]
        public int Id { get; set; }
        public int FileId { get; set; }
        //کی نظر داده
        public int UserId { get; set; }
        //از چه کسی به اینجا رسیده
        public int? ParentId { get; set; }

        [Display(Name = "نوع اقدام")]
        public int ActionTypeId { get; set; }

        [Display(Name = "هامش(نظریه)")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public string UserDesc { get; set; }

        public int RoleTypeId { get; set; }

        [Display(Name = "عنوان نقش")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string RoleTypeTitle { get; set; }

        public int? RoleTypeFinalId { get; set; }

        [Display(Name = "عنوان نقش")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string RoleTypeFinalTitle { get; set; }
        public DateTime RegDate { get; set; }

        #region تصویر ثابت مشخصات اقدام‌کننده

        /// <summary>
        /// مشخصات اقدام‌کننده در لحظه ثبت هامش؛ تغییرات بعدی پروفایل این مقادیر را عوض نمی‌کند.
        /// </summary>
        [MaxLength(250)]
        public string ActorFullNameSnapshot { get; set; }

        [MaxLength(50)]
        public string ActorPersonalCodeSnapshot { get; set; }

        [MaxLength(100)]
        public string ActorRankTitleSnapshot { get; set; }

        [MaxLength(250)]
        public string ActorUnitTitleSnapshot { get; set; }

        #endregion

        /// <summary>
        /// مبلغ وام درخواستی 
        /// </summary>
        public double? MablaghVamDarkhasti { get; set; }
        /// <summary>
        /// مبلغ وام محقق شده
        /// </summary>
        public double? MablaghVamMohaghaghSode { get; set; }

        #region Relations
        public Files File { get; set; }
        public User.Users User { get; set; }
        //
        public UserSub UserSub { get; set; }
        //
        public ActionType ActionType { get; set; }

        [ForeignKey("ParentId")]
        public List<Hamesh> Hameshes { get; set; }

        #endregion
    }
}

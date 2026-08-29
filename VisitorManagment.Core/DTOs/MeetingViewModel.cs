using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.DTOs
{

    public class ListMeetingViewModel
    {
        public List<MeetingInfoViewModel> Meetings { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int skip { get; set; }
        public int count { get; set; }

    }

    public class ListMeetingForHameshFormViewModel: ListMeetingViewModel
    {
      
        public int FileId { get; set; }
        public int ActionTypeId { get; set; }
        public string UserDesc { get; set; }

    }
    public class MeetingInfoViewModel
    {
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Display(Name = "موضوع جلسه")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Caption { get; set; }

        [Display(Name = "اولویت")]
        public int? SortName { get; set; }

        [Display(Name = "وضعیت جلسه")]
        public string MeetingStatusTitle { get; set; }
        public int MeetingStatusId { get; set; }

        [Display(Name = "تاریخ برگزاری جلسه")]
        public string StartMeetingDate { get; set; }

        [Display(Name = "محل جلسه")]
        public string MeetingPlaceTitle { get; set; }

        [Display(Name = "آی دی رئیس جلسه")]
        public string BoseMeetingTitle { get; set; }

        [Display(Name = "آی دی منشی جلسه")]
        public string ClerkMeetingTitle { get; set; }

        public bool IsOkay { get; set; }
        public bool IsSend { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Description { get; set; }
        //
        [Display(Name = "تاریخ ثبت جلسه")]
        public DateTime RegDate { get; set; }
        public int regUserId { get; set; }
        public int? EditUserId { get; set; }
        public DateTime? EditDate { get; set; }

    }
    public class EditMeetingViewModel
    {
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Display(Name = "موضوع جلسه")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Caption { get; set; }

        [Display(Name = "اولویت")]
        public int? SortName { get; set; }

        [Display(Name = "وضعیت جلسه")]
        public string MeetingStatusTitle { get; set; }
        public int MeetingStatusId { get; set; }

        [Display(Name = "تاریخ برگزاری جلسه")]
        public string StartMeetingDate { get; set; }
        public string StartMeetingTime { get; set; }

        [Display(Name = "محل جلسه")]
        public string MeetingPlaceTitle { get; set; }
        public int MeetingPlaceId { get; set; }

        [Display(Name = "آی دی رئیس جلسه")]
        public string BoseMeetingTitle { get; set; }
        public int BoseMeetingId { get; set; }

        [Display(Name = "آی دی منشی جلسه")]
        public string ClerkMeetingTitle { get; set; }
        public int ClerkMeetingId { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Description { get; set; }
        //
        [Display(Name = "تاریخ ثبت جلسه")]
        public DateTime RegDate { get; set; }
        public int regUserId { get; set; }
        public int? EditUserId { get; set; }
        public DateTime? EditDate { get; set; }

    }
    public class DeleteMeetingViewModel
    {
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Display(Name = "موضوع جلسه")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Caption { get; set; }
        public bool IsDelete { get; set; }

    }

    #region Meeting Hold View Model
    public class ListMeetingHoldViewModel
    {
        public List<MeetingHoldInfoViewModel> MeetingHolds { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int skip { get; set; }
        public int count { get; set; }
    }
    public class MeetingHoldInfoViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RankTitle { get; set; }
        public int PersonalId { get; set; }
        public int RequestSubjectId { get; set; }
        public int PriorityId { get; set; }
        public int FileStatusId { get; set; }
        public string RequestDescription { get; set; }

        [Display(Name = "کد پرسنلی")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string PersonalCode { get; set; }

        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string MelliCode { get; set; }

        [Display(Name = " کد درجه")]
        public int? RankCode { get; set; }

        [Display(Name = "عنوان رسته")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string BranchTitle { get; set; }

        [Display(Name = "کد رسته")]
        public int? BranchCode { get; set; }

        [Display(Name = "عنوان شغل")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string JobDes { get; set; }

        [Display(Name = "تاریخ اعزام")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string EzamDate { get; set; }

        [Display(Name = "وضعیت خدمتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string StatusTitle { get; set; }

        [Display(Name = " درصد جانبازی ارتش")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string JanbaziArtesh { get; set; }

        [Display(Name = " درصد جانبازی بنیاد")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string JanbaziBonyad { get; set; }

        [Display(Name = "وضعیت ایثارگری")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string IsarStatus { get; set; }

        [Display(Name = "مدت خدمت در منطقه عملیاتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string AmaliatiKhedmate { get; set; }

        [Display(Name = " مدت خدمت در منطقه قبل قطعنامه")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string GhableGhatnameAmaliatiKhedmate { get; set; }

        [Display(Name = "کد یگان خدمتی")]
        public int? UnitDutyCode { get; set; }

        [Display(Name = "نام یگان خدمتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string UnitDutyTitle { get; set; }

        [Display(Name = "کد یگان عمده")]
        public int? UnitCode { get; set; }

        [Display(Name = "نام یگان عمده")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string UnitTitle { get; set; }

        [Display(Name = "کد قرارگاه منطقه ای / ارشد نظامی ")]
        public string CodGhaTitle { get; set; }
        [Display(Name = "نام قرارگاه منطقه ای / ارشد نظامی ")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public int? CodGha { get; set; }

        [Display(Name = "کد پرسنلی فرمانده")]
        public int? FarmandehPersonalCode { get; set; }

        [Display(Name = " نام فرمانده")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string FarmandehPersonalName { get; set; }

        [Display(Name = "تلفن همراه")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Phone { get; set; }
        public int FileId { get; set; }
        //کی نظر داده
        public int UserId { get; set; }
        //از چه کسی به اینجا رسیده
        public int? ParentId { get; set; }

        [Display(Name = "نوع اقدام")]
        public int ActionTypeId { get; set; }

        [Display(Name = "هامش(نظریه)")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string UserDesc { get; set; }
        public DateTime RegDate { get; set; }

        public bool IsMeetingHold { get; set; }
    }


    public class MeetingHoldViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RankTitle { get; set; }
        public int PersonalId { get; set; }
        public int RequestSubjectId { get; set; }
        public int PriorityId { get; set; }
        public int FileStatusId { get; set; }
        public int MeetingId { get; set; }
        public string RequestDescription { get; set; }
        public int FileId { get; set; }
        //کی نظر داده
        public int UserId { get; set; }
        //از چه کسی به اینجا رسیده
        public int? ParentId { get; set; }

        [Display(Name = "نوع اقدام")]
        public int ActionTypeId { get; set; }

        [Display(Name = "هامش(نظریه)")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string UserDesc { get; set; }
        public DateTime RegDate { get; set; }
    }
    #endregion


}

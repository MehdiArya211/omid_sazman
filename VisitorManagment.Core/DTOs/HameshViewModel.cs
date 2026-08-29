using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.DTOs
{
    public class ListHameshViewModel
    {
        public List<HameshInfoViewModel> hameshes { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int count { get; set; }
        public int skip { get; set; }
        //اینو برای دکمه بازگشت گزاشتم تا آیدی جلسه رو بده به دکمه
        public int? MeetingId { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class HameshInfoViewModel
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public int MeetingId { get; set; }

        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string RequestDescription { get; set; }
        //کی نظر داده
        public int UserId { get; set; }
        //از چه کسی به اینجا رسیده
        public int? ParentId { get; set; }

        [Display(Name = "نوع اقدام")]
        public int ActionTypeId { get; set; }

        [Display(Name = "هامش(نظریه)")]
        public string UserDesc { get; set; }
        public int RoleTypeId { get; set; }
        public string RoleTypeTitle { get; set; }

        public int? RoleTypeIdFinal { get; set; }
        public string RoleTypeTitleFinal { get; set; }


        public string AttacmentFileName { get; set; }
        public IFormFile VoiceRecord { get; set; }
        public string VoiceRecordName { get; set; }

        public List<IFormFile> AttachDastors { get; set; }
        public string AttachDastorName { get; set; }
        public DateTime RegDate { get; set; }

        //for meeting hold
        public string FirstName { get; set; }
        public string FirstNamePersonel { get; set; }
        public string LastNamePersonel { get; set; }
        public string FarmandehPersonalName { get; set; }
        public string LastName { get; set; }
        public string RankTitle { get; set; }
        public string RankTitlePersonal { get; set; }
        public string ActionTypeTitle { get; set; }
        public string SenderUserName { get; set; }
        public string RcvrUserName { get; set; }
        //اطلاعات نفر هامش زده قبلی
        public string ParentRcvrUserName { get; set; }

        public string PhoneSelseleMaratebYeganNafar { get; set; }
        public string ErrorMessage { get; set; }

    }
    public class HameshForRequestDescriptionInfoViewModel
    {
        public string RequestDescription { get; set; }
        public string PervHamesh { get; set; }
        public string hameshKarshenasGharagahAnsarNezaja { get; set; }
        public string hameshKarbarNezaja { get; set; }
        public string UserDesc { get; set; }
        public string ProblemUserDesc { get; set; }
        public int ActionTypeId { get; set; }
        public List<Hamesh> HameshMoavenat { get; set; }

        public string RequestSubjectTitle { get; set; }
    }

    /// <summary>
    /// کل جزئیات هامش برای یک درخواست ملاقات
    /// </summary>
    public class HameshFullInfoFileViewModel
    {

        public FactPersonalViewModel file { get; set; }
        public int FileId { get; set; }
        public int HameshId { get; set; }
        public int MeetingId { get; set; }
        public int RoleTypeId { get; set; }
        public string RoleTypeTitle { get; set; }
        public string RequestDescription { get; set; }
        public string ProblemUserDesc { get; set; }
        public int ActionTypeId { get; set; }
        /// <summary>
        ///اقدامی که نفز لاگین کرده برای نفر ثبت میکند
        /// </summary>
        public int? ActionTypeIdUserLogin { get; set; }
        public string RequestSubjectTitle { get; set; }

        /// <summary>
        /// اطلاعات مربوط به اس ام اس
        /// </summary>
        public SMSInfoViewModel smsInfoViewModel { get; set; }
        /// <summary>
        /// هامش نهایی که ثبت میشه  توسط هر فرمانده / قسمت
        /// </summary>
        public string UserDesc { get; set; }

        #region پیوست ها

        /// <summary>
        /// صدای ضبظ شده جلسه ملاقات
        /// </summary>
        public IFormFile VoiceRecord { get; set; }
        public string VoiceRecordName { get; set; }

        /// <summary>
        /// فایل پیوستی درخواست ملاقات نفر
        /// </summary>
        public List<string> attacmentFileName { get; set; }
        /// <summary>
        /// فیش پیوست شده
        /// </summary>
        public string fishAttacmentFileName { get; set; }
        /// <summary>
        /// فایل های پوست شده جلسه ملاقات که جلسه ی آن برگزار شده است
        /// </summary>
        public List<IFormFile> AttachDastors { get; set; }
        public List<string> AttachDastorName { get; set; }
        #endregion

        public List<FactPersonalViewModel> files { get; set; }
        /// <summary>
        /// لیست هامش های ثبت شده برای درخواست ملاقات
        /// </summary>
        public List<HameshInfoViewModel> hameshes { get; set; }
        /// <summary>
        /// هامش معاونت ها
        /// </summary>
        public List<HameshInfoViewModel> HameshMoavenats { get; set; }

        /// <summary>
        /// هامش کارشناس قرارگاه انصار
        /// </summary>
        public string hameshKarshenasGharagahAnsarNezaja { get; set; }

        /// <summary>
        /// هامش  هیئت رییسه
        /// </summary>
        public string hameshHeiatReeise { get; set; }


        /// <summary>
        /// تمام هامش هایی که یگان ثبت کرده اند
        /// </summary>
        public List<HameshInfoViewModel> hameshAllYegan { get; set; }

        /// <summary>
        /// آخرین هامش ثبت شده
        /// </summary>
        public string PervHamesh { get; set; }

        /// <summary>
        /// هامش کسی که لاگین کرده برای درخواست ملاقات نفر
        /// </summary>
        public string HameshUserLogin { get; set; }

        /// <summary>
        /// هامش  کاربر نزاجا
        /// </summary>
        public string HameshKarbarNezaja { get; set; }

        #region برای لیست ملاقات ها
        public string FirstName { get; set; }
        public string FirstNamePersonel { get; set; }
        public string LastNamePersonel { get; set; }
        public string FarmandehPersonalName { get; set; }
        public string LastName { get; set; }
        public string RankTitle { get; set; }
        public string RankTitlePersonal { get; set; }
        public string ActionTypeTitle { get; set; }
        public string RcvrUserName { get; set; }
        //اطلاعات نفر هامش زده قبلی
        public string ParentRcvrUserName { get; set; }
        #endregion


        /// <summary>
        /// مبلغ وام درخواستی 
        /// </summary>
        public double? SumMablaghVamDarkhasti { get; set; }
        /// <summary>
        /// مبلغ وام محقق شده
        /// </summary>
        public double? MablaghVamMohaghaghSode { get; set; }

        //لیست نفرات برای ارسال به کارتابل آنها
        public List<int> RcvrId { get; set; }

        #region وام

        public List<VamViewModel> ListVam { get; set; }
        //public List<string> VamTitle { get; set; }
        //public List<int> VamCodeId { get; set; }
        //public List<int> VamCode { get; set; }


        #endregion

    }



}

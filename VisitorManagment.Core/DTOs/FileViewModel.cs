using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VisitorManagment.Core.DTOs
{
    public class ListFileInfoViewModel
    {
        public List<FileInfoViewModel> files { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public string personalCode { get; set; }

    }
    public class CreateFileViewModel
    {
        public int PersonalId { get; set; }
        public int ReqSubId { get; set; }
        public int PriorityId { get; set; }
        public int FileStatusId { get; set; }
        [Display(Name = "کد پرسنلی فرمانده")]
        public int? FarmandehPersonNo { get; set; }

        [Display(Name = " نام فرمانده")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string FarmandehName { get; set; }

        [Display(Name = "شرح درخواست")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string RequestDescription { get; set; }

        [Display(Name = "شرح مشکل")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(5000, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string ProblemDescription { get; set; }

        [Display(Name = "تلفن همراه")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Phone { get; set; }

        [Display(Name = "آدرس")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Address { get; set; }

        [Display(Name = "پیوست")]
        public IFormFile Attachment { get; set; }

        [Display(Name = "نام پیوست")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string AttachmentFileName { get; set; }

        [Display(Name = "فایل صوتی")]
        public string VoiceRecord { get; set; }

        [Display(Name = "اسکن دستور جلسه")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public IFormFile AttachDastor { get; set; }

        [Display(Name = "اسکن دستور جلسه")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string AttachDastorName { get; set; }

        [Display(Name = "فیش حقوقی")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public IFormFile FishAttachment { get; set; }

        [Display(Name = "فیش حقوقی")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string FishAttachmentName { get; set; }



    }
    public class FileInfoViewModel
    {
        public int Id { get; set; }
        public string PersonalCode { get; set; }
        public string PersonalAvatar { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? RankCode { get; set; }
        public string RankTitle { get; set; }
        public int? BranchCode { get; set; }
        public string BranchTitle { get; set; }
        public string StatuseTitle { get; set; }
        public int UnitCode { get; set; }
        public string UnitTitle { get; set; }
        public int? UnitDutyCode { get; set; }
        public string UnitDutyTitle { get; set; }
        public string JobDes { get; set; }
        public int? CodGha { get; set; }
        public string CodGhaTitle { get; set; }
        public string MelliCode { get; set; }

        [Display(Name = "وضعیت خدمتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string StatusTitle { get; set; }

        [Display(Name = " درصد جانبازی ارتش")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public decimal? DRSAD_JA { get; set; }

        [Display(Name = " درصد جانبازی بنیاد")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public decimal? DRSAD_JB { get; set; }

        [Display(Name = "وضعیت ایثارگری")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string IsarStatus { get; set; }

        [Display(Name = "مدت خدمت در منطقه عملیاتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string TOT_AML { get; set; }

        [Display(Name = " مدت خدمت در منطقه قبل قطعنامه")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string TOT_AML2 { get; set; }

        [Display(Name = "کد پرسنلی فرمانده")]
        public int? FPersonalCode { get; set; }

        [Display(Name = " نام فرمانده")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string FPersonalName { get; set; }

        [Display(Name = "تلفن همراه")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Phone { get; set; }

        [Display(Name = "آدرس")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Addres { get; set; }

        [Display(Name = "شرح درخواست")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(5000, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string RequestDescription { get; set; }

        [Display(Name = "پیوست")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public IFormFile Attachment { get; set; }
        public string AttachmentFileName { get; set; }

        [Display(Name = "اسکن دستور جلسه")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public IFormFile AttachDastor { get; set; }

        [Display(Name = "اسکن دستور جلسه")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string AttachDastorName { get; set; }
        public int PersonalId { get; set; }
        public int RequestSubjectId { get; set; }
        public string ReqSubTitle { get; set; }
        public int PriorityId { get; set; }
        public string PriorityTitle { get; set; }
        public int FileStatusId { get; set; }
        public string FileStatusTitle { get; set; }
        public int AddUserId { get; set; }
        public DateTime RegDate { get; set; }
        public string RegDateFa { get; set; }
        public int? EditUserId { get; set; }
        public string EditDate { get; set; }
        //
        public string EmploymentDate { get; set; }
        public string EmploymentTitle { get; set; }
        public string BirthPlaceTitle { get; set; }
        public string BirthDate { get; set; }
        public string BloodTitle { get; set; }
        public string MarridTitle { get; set; }
        public string ReligoinTitle { get; set; }
        //
        public string LocationJob { get; set; }
        public int? TashvighatCount { get; set; }
        public int? TanbihatCount { get; set; }
        public int? NahastCount { get; set; }
        public int? FararCount { get; set; }
        //
        public string UserDesc { get; set; }
        //use in SpecificationPersonal.cshtml
        public string FinalHameshDesc { get; set; }


    }
    public class CreatFileDTO
    {
        public int Id { get; set; }
        public int PersonalId { get; set; }
        public int RequestSubjectId { get; set; }
        public int PriorityId { get; set; }
        public int FileStatusId { get; set; }
        public int? MeetingId { get; set; }

        [Display(Name = "شرح درخواست")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(5000, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string RequestDescription { get; set; }

        [Display(Name = "شرح مشکل")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(5000, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string ProblemDescription { get; set; }

        [Display(Name = "پیوست")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Attachment { get; set; }
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string AttachmentName { get; set; }

        [Display(Name = "پیوست فیش حقوقی")]
        [MaxLength(250, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string FishAttachment { get; set; }

        [MaxLength(250, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string FishAttachmentName { get; set; }

        [Display(Name = "فایل صوتی")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string VoiceRecord { get; set; }

        [Display(Name = "اسکن دستور جلسه")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string AttachDastor { get; set; }

        [Display(Name = "کد پرسنلی")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string PersonalCode { get; set; }

        [Display(Name = "کد ملی")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string MelliCode { get; set; }

        [Display(Name = "نام")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string LastName { get; set; }


        [Display(Name = "درجه")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string RankTitle { get; set; }

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
        public decimal? DRSAD_JA { get; set; }

        [Display(Name = " درصد جانبازی بنیاد")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public decimal? DRSAD_JB { get; set; }

        [Display(Name = "وضعیت ایثارگری")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string IsarStatus { get; set; }

        [Display(Name = "مدت خدمت در منطقه عملیاتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string TOT_AML2 { get; set; }

        [Display(Name = " مدت خدمت در منطقه قبل قطعنامه")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string TOT_AML { get; set; }

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
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
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

        [Display(Name = "آدرس")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Addres { get; set; }

        [Display(Name = "تاریخ استخدام")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string EmploymentDate { get; set; }

        /// <summary>
        /// جلسه مربوطه برگزار شده یا نه 
        /// </summary>
        public bool IsMeetingHold { get; set; }
        public bool IsArchived { get; set; }

        [Display(Name = "اتمام کار ")]
        public bool IsFinished { get; set; }
        [Display(Name = "معاونت پاسخ داده یا نه ")]
        public bool IsMoavenatAnswered { get; set; }

        /// <summary>
        /// مجموع مبلغ وام درخواستی 
        /// </summary>
        public long? SumMablaghVamDarkhasti { get; set; }
        /// <summary>
        /// مبلغ وام محقق شده
        /// </summary>
        public long? MablaghVamMohaghaghSode { get; set; }

        #region Fish Information
        [Display(Name = "حقوق کل")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public long TotalMoney { get; set; }

        [Display(Name = "میزان حقوق دریافتی")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public long ReciveMoney { get; set; }

        [Display(Name = "تعداد وام")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public int CountVam { get; set; }

        [Display(Name = "میزان حقوق دریافتی")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public long SumAghsatVamMahiyaneh { get; set; }
        #endregion

        #region 4FiledConstant
        public bool IsDelete { get; set; }
        public int RegUserId { get; set; }
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [DisplayFormat(DataFormatString = "{0 :dd MMM yy HH:mm tt}")]
        public DateTime RegDate { get; set; }
        public int? EditUserId { get; set; }
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public DateTime? EditDate { get; set; }
        #endregion
    }

}

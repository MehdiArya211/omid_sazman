using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
    public class Files
    {
        [Key]
        public int Id { get; set; }
        public int FileTypeId { get; set; }
        public int? ActionTypeId { get; set; }
        public int PersonalId { get; set; }
        /// <summary>
        /// موضوع درخواست
        /// </summary>
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

        [Display(Name = "پیوست فیش حقوقی")]
        [MaxLength(250, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string FishAttachment { get; set; }

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
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string MelliCode { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
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
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public int CodGha { get; set; }

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
        public int ArchivedRegUserId { get; set; }

        [Display(Name = "اتمام کار ")]
        public bool IsFinished { get; set; }
        [Display(Name = "معاونت پاسخ داده یا نه ")]
        public bool IsMoavenatAnswered { get; set; }

        /// <summary>
        /// مجموع مبلغ وام درخواستی 
        /// </summary>
        public double? SumMablaghVamDarkhasti { get; set; }
        /// <summary>
        /// مبلغ وام محقق شده
        /// </summary>
        public double? MablaghVamMohaghaghSode { get; set; }

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


        #region Relations
        public Personal Personal { get; set; }
        public FileType FileType { get; set; }
        public ActionType ActionType { get; set; }
        public RequestSubject RequestSubject { get; set; }
        public Priority Priority { get; set; }
        public FileStatus FileStatus { get; set; }
        public List<Cartable> Cartables { get; set; }
        public List<Hamesh> Hameshes { get; set; }
        public List<FileAttachment> FileAttachments { get; set; }
        public Meeting Meeting { get; set; }
        public List<Vam> Vam { get; set; }
        public List<MemberMeeting> MemberMeeting { get; set; }

        #endregion
    }
}

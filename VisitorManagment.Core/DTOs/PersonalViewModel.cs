using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.Core.DTOs
{
    #region List File View Model
    public class ListFileViewModel
    {
        public List<FactPersonalViewModel> files { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        //
        public int count { get; set; }
        public int skip { get; set; }

        public string attacmentFileName { get; set; }
        public string attachDastor { get; set; }
        public string ErrorMessage { get; set; }

        public int? MoavenatId { get; set; }


    }


    public class ListCountCartableMoavenat 
    {
        public int? CountKarshenasGharargahAnsarNezaja { get; set; }
        public int? CountKarbarNezaja { get; set; }
        public int? CountMNEnsani { get; set; }
        public int? CountMMohandesi { get; set; }
        public int? CountMTarhVaBarnameh { get; set; }
        public int? CountMAmozesh { get; set; }
        public int? CountMAmadVaPosh { get; set; }
        public int? CountMHoghoghiVaGhazayi { get; set; }
        public int? CountBazresiNezaja { get; set; }
        public int? CountFHavapeymayi { get; set; }
        public int? CountDarayi { get; set; }
        public int? CountIsargaran { get; set; }
        public int? CountBehdasht { get; set; }


    }


    #endregion
    #region PersonalNezami View Model
    public class FactPersonalViewModel0
    {
        public int Id { get; set; }
        public bool IsAnswerdMoavenat { get; set; }
        public bool IsArchived { get; set; }

        [Display(Name = "کد پرسنلی فرمانده")]
        [Required(ErrorMessage = "{0} راوارد کنید")]
        public string PersonalCode { get; set; }
        public string PersonalCodeUserLogined { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? RankCode { get; set; }
        public string RankTitle { get; set; }
        public int? BranchCode { get; set; }
        public string BranchTitle { get; set; }
        public string StatuseTitle { get; set; }
        public int? UnitCode { get; set; }
        public string UnitTitle { get; set; }
        public int? UnitDutyCode { get; set; }
        public string UnitDutyTitle { get; set; }
        public string JobDes { get; set; }

        [Display(Name = "قرارگاه")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public int? CodGha { get; set; }
        public string CodGhaTitle { get; set; }
        /// <summary>
        /// نوع درخواست سامانه امید / لبیک 24
        /// </summary>
		public int FileTypeId { get; set; }
		public string FileTypeTitle { get; set; }
		public string MelliCode { get; set; }

        [Display(Name = "وضعیت خدمتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string StatusTitle { get; set; }

        [Display(Name = " درصد جانبازی ارتش")]
        public decimal? DRSAD_JA { get; set; }

        [Display(Name = " درصد جانبازی بنیاد")]
        public decimal? DRSAD_JB { get; set; }

        [Display(Name = "وضعیت ایثارگری")]
        public string IsarStatus { get; set; }

        [Display(Name = "مدت خدمت در منطقه عملیاتی")]
        public string TOT_AML { get; set; }

        [Display(Name = " مدت خدمت در منطقه قبل قطعنامه")]
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
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Addres { get; set; }

        [Display(Name = "شرح درخواست")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(5000, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string RequestDescription { get; set; }

        [Display(Name = "شرح مشکل")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(5000, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string ProblemDescription { get; set; }

        [Display(Name = "آیدی گیرنده")]
        public int ReciverUserId { get; set; }

        [Display(Name = "نقش گیرنده")]
        [MaxLength(5000, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string ReciverUserRole { get; set; }

        [Display(Name = "پیوست")]
        public IFormFile Attachment { get; set; }
        public string AttachmentFileName { get; set; }

        [Display(Name = " پیوست فیش حقوقی ")]
        [Required(ErrorMessage = " {0}را وارد کنید ")]
        public IFormFile FishAttachment { get; set; }
        public string FishAttachmentFileName { get; set; }

        public string attachDastor { get; set; }
        public IFormFile PersonalAvatar { get; set; }
        public string PersonalAvatarName { get; set; }
        public int PersonalId { get; set; }
        public int RequestSubjectId { get; set; }
        public string ReqSubTitle { get; set; }
        public int PriorityId { get; set; }
        public string PriorityTitle { get; set; }
        public int FileStatusId { get; set; }
        public string FileStatusTitle { get; set; }
        public int AddUserId { get; set; }
        public DateTime RegDate { get; set; }
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

 
        public string TotAml { get; set; }
        public string TotAml2 { get; set; }
        public decimal? DrsadJb { get; set; }
        public decimal? DrsadJa { get; set; }
        //
        public string LocationJob { get; set; }
        public int? TashvighatCount { get; set; }
        public int? TanbihatCount { get; set; }
        public int? NahastCount { get; set; }
        public int? FararCount { get; set; }
        //

        public string Respond { get; set; }
        public string LastMoavenatHamesh { get; set; }
        public bool IsMeetingHold { get; set; }

        public string HomDat { get; set; }

        #region Fish Information
        [Display(Name = "حقوق کل")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public long TotalMoney { get; set; }
        public long? TotalMoneyHidden { get; set; }

        [Display(Name = "میزان حقوق دریافتی")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public long ReciveMoney { get; set; }
        public long? ReciveMoneyHidden { get; set; }

        [Display(Name = "تعداد وام")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public int CountVam { get; set; }
        public int? CountVamHidden { get; set; }

        [Display(Name = "میزان حقوق دریافتی")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public long SumAghsatVamMahiyaneh { get; set; }
        public long? SumAghsatVamMahiyanehHidden { get; set; }
        #endregion

    }
    public class FactPersonalViewModel
    {
        public int Id { get; set; }

        public bool IsAnswerdMoavenat { get; set; }

        public bool IsArchived { get; set; }

        [Display(Name = "کد پرسنلی")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
        [RegularExpression(@"^\d{1,9}$", ErrorMessage = "{0} باید فقط عدد و حداکثر ۹ رقم باشد.")]
        public string PersonalCode { get; set; }

        public string PersonalCodeUserLogined { get; set; }

        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        public int? RankCode { get; set; }

        [Display(Name = "درجه")]
        public string RankTitle { get; set; }

        public int? BranchCode { get; set; }

        [Display(Name = "رسته")]
        public string BranchTitle { get; set; }

        public string StatuseTitle { get; set; }

        public int? UnitCode { get; set; }

        [Display(Name = "یگان عمده")]
        public string UnitTitle { get; set; }

        public int? UnitDutyCode { get; set; }

        [Display(Name = "یگان خدمتی")]
        public string UnitDutyTitle { get; set; }

        public string JobDes { get; set; }

        [Display(Name = "قرارگاه")]
        [Required(ErrorMessage = "لطفاً {0} را انتخاب کنید.")]
        public int? CodGha { get; set; }

        public string CodGhaTitle { get; set; }

        /// <summary>
        /// نوع درخواست سامانه امید / لبیک 24
        /// </summary>
        [Display(Name = "نوع درخواست")]
        [Required(ErrorMessage = "لطفاً {0} را انتخاب کنید.")]
        public int? FileTypeId { get; set; }

        public string FileTypeTitle { get; set; }

        [Display(Name = "کد ملی")]
        public string MelliCode { get; set; }

        [Display(Name = "وضعیت خدمتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی‌تواند بیشتر از {1} کاراکتر باشد.")]
        public string StatusTitle { get; set; }

        [Display(Name = "درصد جانبازی ارتش")]
        public decimal? DRSAD_JA { get; set; }

        [Display(Name = "درصد جانبازی بنیاد")]
        public decimal? DRSAD_JB { get; set; }

        [Display(Name = "وضعیت ایثارگری")]
        public string IsarStatus { get; set; }

        [Display(Name = "مدت خدمت در منطقه عملیاتی")]
        public string TOT_AML { get; set; }

        [Display(Name = "مدت خدمت در منطقه عملیاتی قبل از قطعنامه")]
        public string TOT_AML2 { get; set; }

        [Display(Name = "کد پرسنلی فرمانده")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
        public int? FPersonalCode { get; set; }

        [Display(Name = "نام فرمانده")]
        [MaxLength(200, ErrorMessage = "{0} نمی‌تواند بیشتر از {1} کاراکتر باشد.")]
        public string FPersonalName { get; set; }

        [Display(Name = "تلفن همراه")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
        [RegularExpression(@"^09\d{9}$", ErrorMessage = "لطفاً {0} را به صورت صحیح وارد کنید. مثال: 09123456789")]
        [MaxLength(11, ErrorMessage = "{0} نمی‌تواند بیشتر از {1} رقم باشد.")]
        public string Phone { get; set; }

        [Display(Name = "آدرس")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
        [MaxLength(200, ErrorMessage = "{0} نمی‌تواند بیشتر از {1} کاراکتر باشد.")]
        public string Addres { get; set; }

        [Display(Name = "شرح درخواست")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
        [MaxLength(5000, ErrorMessage = "{0} نمی‌تواند بیشتر از {1} کاراکتر باشد.")]
        public string RequestDescription { get; set; }

        [Display(Name = "شرح مشکل")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
        [MaxLength(5000, ErrorMessage = "{0} نمی‌تواند بیشتر از {1} کاراکتر باشد.")]
        public string ProblemDescription { get; set; }

        [Display(Name = "آیدی گیرنده")]
        public int ReciverUserId { get; set; }

        [Display(Name = "نقش گیرنده")]
        [MaxLength(5000, ErrorMessage = "{0} نمی‌تواند بیشتر از {1} کاراکتر باشد.")]
        public string ReciverUserRole { get; set; }

        [Display(Name = "پیوست درخواست")]
        public IFormFile Attachment { get; set; }

        public string AttachmentFileName { get; set; }

        [Display(Name = "فیش حقوقی")]
        [Required(ErrorMessage = "لطفاً {0} را بارگذاری کنید.")]
        public IFormFile FishAttachment { get; set; }

        public string FishAttachmentFileName { get; set; }

        public string attachDastor { get; set; }

        [Display(Name = "تصویر پرسنلی")]
        public IFormFile PersonalAvatar { get; set; }

        public string PersonalAvatarName { get; set; }

        public int PersonalId { get; set; }

        [Display(Name = "موضوع درخواست")]
        [Required(ErrorMessage = "لطفاً {0} را انتخاب کنید.")]
        public int? RequestSubjectId { get; set; }

        public string ReqSubTitle { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = "لطفاً {0} را انتخاب کنید.")]
        public int? PriorityId { get; set; }

        public string PriorityTitle { get; set; }

        [Display(Name = "وضعیت درخواست")]
        [Required(ErrorMessage = "لطفاً {0} را انتخاب کنید.")]
        public int? FileStatusId { get; set; }

        public string FileStatusTitle { get; set; }

        public int AddUserId { get; set; }

        public DateTime RegDate { get; set; }

        public int? EditUserId { get; set; }

        public string EditDate { get; set; }

        public string EmploymentDate { get; set; }

        public string EmploymentTitle { get; set; }

        public string BirthPlaceTitle { get; set; }

        public string BirthDate { get; set; }

        public string BloodTitle { get; set; }

        public string MarridTitle { get; set; }

        public string ReligoinTitle { get; set; }

        public string TotAml { get; set; }

        public string TotAml2 { get; set; }

        public decimal? DrsadJb { get; set; }

        public decimal? DrsadJa { get; set; }

        public string LocationJob { get; set; }

        public int? TashvighatCount { get; set; }

        public int? TanbihatCount { get; set; }

        public int? NahastCount { get; set; }

        public int? FararCount { get; set; }

        public string Respond { get; set; }

        public string LastMoavenatHamesh { get; set; }

        public bool IsMeetingHold { get; set; }

        public string HomDat { get; set; }

        #region Fish Information

        [Display(Name = "حقوق کل")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
        [Range(1, long.MaxValue, ErrorMessage = "{0} باید بیشتر از صفر باشد.")]
        public long? TotalMoney { get; set; }

        public long? TotalMoneyHidden { get; set; }

        [Display(Name = "میزان دریافتی")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
        [Range(1, long.MaxValue, ErrorMessage = "{0} باید بیشتر از صفر باشد.")]
        public long? ReciveMoney { get; set; }

        public long? ReciveMoneyHidden { get; set; }

        [Display(Name = "تعداد وام")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} نمی‌تواند منفی باشد.")]
        public int? CountVam { get; set; }

        public int? CountVamHidden { get; set; }

        [Display(Name = "مجموع اقساط ماهیانه")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
        [Range(0, long.MaxValue, ErrorMessage = "{0} نمی‌تواند منفی باشد.")]
        public long? SumAghsatVamMahiyaneh { get; set; }

        public long? SumAghsatVamMahiyanehHidden { get; set; }

        #endregion
    }

    public class EditFactPersonalViewModel
    {
        public int Id { get; set; }
        public int FileTypeId { get; set; }
        public string FileTypeTitle { get; set; }
        public bool IsAnswerdMoavenat { get; set; }
        public bool IsArchived { get; set; }

        [Display(Name = "کد پرسنلی")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public string PersonalCode { get; set; }
        public IFormFile PersonalAvatar { get; set; }

        [Display(Name = "فیش حقوقی")]
        [Required(ErrorMessage = "{0}راوارد کنید ")]
        public IFormFile FishAttachmnet { get; set; }
        public string PersonalAvatarName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? RankCode { get; set; }
        public string RankTitle { get; set; }
        public int? BranchCode { get; set; }
        public string BranchTitle { get; set; }
        public string StatuseTitle { get; set; }
        public int? UnitCode { get; set; }
        public string UnitTitle { get; set; }
        public int? UnitDutyCode { get; set; }
        public string UnitDutyTitle { get; set; }
        public string JobDes { get; set; }
        public int? CodGha { get; set; }
        public string CodGhaTitle { get; set; }
        public string MelliCode { get; set; }

        [Display(Name = "تاریخ اعزام")]
        public string EzamDate { get; set; }

        [Display(Name = "وضعیت خدمتی")]
        public string StatusTitle { get; set; }

        [Display(Name = " درصد جانبازی ارتش")]
        public decimal? DRSAD_JA { get; set; }

        [Display(Name = " درصد جانبازی بنیاد")]
        public decimal? DRSAD_JB { get; set; }

        [Display(Name = "وضعیت ایثارگری")]
        public string IsarStatus { get; set; }

        [Display(Name = "مدت خدمت در منطقه عملیاتی")]
        public string TOT_AML2 { get; set; }

        [Display(Name = " مدت خدمت در منطقه قبل قطعنامه")]
        public string TOT_AML { get; set; }

        [Display(Name = "کد پرسنلی فرمانده")]
        public int? FPersonalCode { get; set; }

        [Display(Name = " نام فرمانده")]
        public string FPersonalName { get; set; }

        [Display(Name = "تلفن همراه")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public string Phone { get; set; }

        [Display(Name = "آدرس")]
        public string Addres { get; set; }

        [Display(Name = "شرح درخواست")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public string RequestDescription { get; set; }

        [Display(Name = "شرح مشکل")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        public string ProblemDescription { get; set; }

        [Display(Name = "پیوست")]
        public IFormFile Attachment { get; set; }
        public string AttachmentFileName { get; set; }
        public string AttachDastor { get; set; }
        public int PersonalId { get; set; }
        public int RequestSubjectId { get; set; }
        public string ReqSubTitle { get; set; }
        public int PriorityId { get; set; }
        public string PriorityTitle { get; set; }
        public int FileStatusId { get; set; }
        public string FileStatusTitle { get; set; }

        public int AddUserId { get; set; }
        public string SaveDate { get; set; }
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

        public string AttacmentFileName { get; set; }
        public string FishAttacmentFileName { get; set; }

        #region Fish Information
        [Display(Name = "حقوق کل")]
        public double TotalMoney { get; set; }

        [Display(Name = "میزان حقوق دریافتی")]
        public double ReciveMoney { get; set; }

        [Display(Name = "تعداد وام")]
        public int CountVam { get; set; }

        [Display(Name = "میزان حقوق دریافتی")]
        public double SumAghsatVamMahiyaneh { get; set; }
        #endregion

    }
    public class DeleteFactPersonalViewModel
    {
        public int Id { get; set; }
        public string PersonalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? RankCode { get; set; }
        public string RankTitle { get; set; }
        public int? BranchCode { get; set; }
        public string BranchTitle { get; set; }
        public string StatuseTitle { get; set; }
        public int? UnitCode { get; set; }
        public string UnitTitle { get; set; }
        public int? UnitDutyCode { get; set; }
        public string UnitDutyTitle { get; set; }
        public string JobDes { get; set; }
        public int? CodGha { get; set; }
        public string CodGhaTitle { get; set; }
        public string MelliCode { get; set; }

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
        public IFormFile Attachment { get; set; }
        public string AttachmentFileName { get; set; }
        public int PersonalId { get; set; }
        public int RequestSubjectId { get; set; }
        public string ReqSubTitle { get; set; }
        public int PriorityId { get; set; }
        public string PriorityTitle { get; set; }
        public int FileStatusId { get; set; }
        public string FileStatusTitle { get; set; }

        public int AddUserId { get; set; }
        public string SaveDate { get; set; }
        public int? EditUserId { get; set; }
        public string EditDate { get; set; }

    }
    #endregion
    #region Show Count File In Index
    public class ShowCountFileViewModel
    {
        public int SabteAvaliye { get; set; }
        public int TayidVaErsalGharargah { get; set; }
        public int TayidVaErsalNezaja { get; set; }
        public int ErsalJahatNazarie { get; set; }
        public int SabtDarListMolaghat { get; set; }
    }
    #endregion

}

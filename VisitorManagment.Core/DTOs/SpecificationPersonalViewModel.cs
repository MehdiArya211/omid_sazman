using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VisitorManagment.Core.DTOs
{
    #region Collection View Model
    public class CollectionSpecificationPersonalViewModel
    {
        public HameshFullInfoFileViewModel hameshFullInfoViewModel { get; set; }
        //public class FactPersonal : FactSpecificationPersonalViewModel { };
        public FactSpecificationPersonalViewModel FactPersonal { get; set; }
        //public List<TashvighatInfoViewModel> Tashvighat { get; set; }
        public ApiResultTashvighatDto Tashvighat { get; set; }
       // public List<TanbihatInfoViewModel> Tanbihat { get; set; }
        public ApiResultTanbihatDto Tanbihat { get; set; }
        public ApiResultEnteghalDto Enteghal { get; set; }
        public ApiResultPersonFamilyDto PersonFamily { get; set; }
        public ApiResulMaskanDto TashilatMaskan { get; set; }
        public ApiResulDabirKhanehDto TashilatDabirkhaneh { get; set; }
        public List<ItoLogInfoViewModel> ItoLog { get; set; }
        public ApiResulDastorDto TashilatDastor { get; set; }
        public ApiResultOtherDto TashilatOther { get; set; }
        public ApiResultBelaavazDto TashilatBelaavaz { get; set; }
        public ApiResulExamDto Exam{ get; set; }
        public ApiResultFishDto Fish{ get; set; }
        public ApiResultMoeeserDto Moeeser{ get; set; }
        public ListFileInfoViewModel ListFile { get; set; }

    }
    #endregion

    #region personal
    public class FactSpecificationPersonalViewModel
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string PersonalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmploymentDate { get; set; }
        public string BirthPlaceTitle { get; set; }
        public string BirthDate { get; set; }
        public string EmploymentTitle { get; set; }
        public string BloodTitle { get; set; }
        public string ReligoinTitle { get; set; }
        public string MarridTitle { get; set; }
        public string StatuseTitle { get; set; }
        public string JOB_DES { get; set; }
        public int? RankCode { get; set; }
        public string RankTitle { get; set; }
        public int? BranchCode { get; set; }
        public string BranchTitle { get; set; }
        public int? UnitCode { get; set; }
        public string UnitTitle { get; set; }
        public int? UnitDutyCode { get; set; }
        public string UnitDutyTitle { get; set; }
        public string JobDes { get; set; }
        public int? CodGha { get; set; }
        public string CodGhaTitle { get; set; }
        public string MelliCode { get; set; }

        [Display(Name = "وضعیت خدمتی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string PersonalAvatar { get; set; }

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
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string RequestDescription { get; set; }
        //
        #region منازل سازمانی
        [Display(Name = "تاریخ تحویل خانه سازمانی")]
        public string HomDat { get; set; }
       #endregion


    }
    #endregion

    #region tashvighat
    public class TashvighatInfoViewModel
    {
        public int Id { get; set; }
        public string PersonalCode { get; set; }
        public string RowBeginDate { get; set; }
        public int? EncourageTypeId { get; set; }
        public string EncourageTypeTitle { get; set; }
        public int? EncourageReasonId { get; set; }
        public string EncourageReasonTitle { get; set; }
        

    }
    #endregion

    #region tenbihat
    public class TanbihatInfoViewModel
    {
        public int Id { get; set; }
        public string PersonalCode { get; set; }
        public string RowBeginDate { get; set; }
        public int? ConvictionReasonId { get; set; }
        public string ConvictionReasonTitle { get; set; }
        public int? ConvictionTypeId { get; set; }
        public string ConvictionTypeTitle { get; set; }
    }
    #endregion

    #region enteghal
    public class EnteghaInfolViewModel
    {
        public int? Counter { get; set; }
        public int Id { get; set; }
        public string PrsnNo { get; set; }
        public string FromOrgan { get; set; }
        public int? Mntprevious { get; set; }
        public string WentDate { get; set; }
        public string ToOrgan { get; set; }
        public int? Mntnew { get; set; }
    }
    #endregion

    #region Tashilat Maskan
    public class TashilatMaskanInfoViewModel
    {
        public int VagozariId { get; set; }
        public int ProjectCd { get; set; }
        public int VagozariTypeId { get; set; }
        public int VagozariStatId { get; set; }
        public string PrsnNo { get; set; }
        public long? Mablaq { get; set; }
        public string MablaqTitle { get; set; }
        public double? Metraz { get; set; }
        public string RequestLtrNo { get; set; }
        public string VagozariDesc { get; set; }
        public string RegDate { get; set; }
        public int RegUserCd { get; set; }
        public int? EditUserCd { get; set; }
        public string EditDate { get; set; }
        public string CancelDate { get; set; }
        public string CancelLtrNo { get; set; }
        public int? TashilatParnCd { get; set; }
        public string CancelDesc { get; set; }
        public int? ProjectBlockUnitInfoCd { get; set; }
        public string ProjectName { get; set; }
        public int TavoniCd { get; set; }
        public string TavoniName { get; set; }
        public string YeganDesc { get; set; }
        public string VagozariTypeDesc { get; set; }
        public string VagozariStatDesc { get; set; }
        public string UnitNo { get; set; }
        public decimal? UnitTotalArea { get; set; }
        public int? ProjectAazaCd { get; set; }
        public int? TashilatTypeId { get; set; }
        public int? ProjectTypeCd { get; set; }
        public string TashilatTypeDesc { get; set; }
        public string ProjectTypeDesc { get; set; }
        public string VagozariDate { get; set; }
        public int? ProjectBlockInfoCd { get; set; }
        public bool? IsTashilatVam { get; set; }
        public string FrstName { get; set; }
        public string LstName { get; set; }
        public string SoclScrtyNo { get; set; }
        public string BlockName { get; set; }
        public string BlockNo { get; set; }
        public short? FloorNoCd { get; set; }
        public string VamDate { get; set; }
        public long? MablaqYaraneh { get; set; }
        public string YaranehDate { get; set; }
        public int? SystemTypeCd { get; set; }
        public int? MaijerUnit { get; set; }
        public int? UnitDutyOrgId { get; set; }

        #region Relations

        public TashilatType TashilatTypes { get; set; }
        public VagozariStat VagozariStats { get; set; }
        public VagozariType VagozariTypes { get; set; }



        #endregion

        public class TashilatType
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Code { get; set; }

            #region Relations

            public ICollection<TashilatMaskanInfoViewModel> TashilatMaskans { get; set; }

            #endregion
        }
        public class VagozariStat
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Code { get; set; }

            #region Relations

            public ICollection<TashilatMaskanInfoViewModel> TashilatMaskans { get; set; }

            #endregion

        }
        public class VagozariType
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Code { get; set; }

            #region Relations

            public ICollection<TashilatMaskanInfoViewModel> TashilatMaskans { get; set; }

            #endregion
        }
    }
    #endregion

    #region TashilatDabirKhane
    public class TashilatDabirkhanehInfoViewModel
    {
        public long RequestVamDabirId { get; set; }
        public int ProjectCd { get; set; }
        public string PrsnNo { get; set; }
        public int? RequestOrgCd { get; set; }
        public string RequestDate { get; set; }
        public string RequestLtrNo { get; set; }
        public int TashilatTypeId { get; set; }
        public string TashilatTypeTitle { get; set; }
        public long? RequestMablaqVam { get; set; }
        public string AccountNoHekmat { get; set; }
        public string AccountNoSepah { get; set; }
        public string NationalId { get; set; }
        public string RequestTelNo { get; set; }
        public string VagozariDesc { get; set; }
        public int? ConfirmApplyCd { get; set; }
        public int? ConfirmTashilatTypeCd { get; set; }
        public long? ConfirmMablaqVam { get; set; }
        public string ConfirmMablaqVamTitle { get; set; }
        public string ConfirmOrderDate { get; set; }
        public string ConfirmDesc { get; set; }
        public int? LoanOrder { get; set; }
        public int VagozariVamStatId { get; set; }
        public string VagozariVamStatTitle { get; set; }
        public string RegDate { get; set; }
        public int RegUserCd { get; set; }
        public int? EditUserCd { get; set; }
        public string EditDate { get; set; }
        public bool? IsPersonalReport { get; set; }
        public int? MissingDocReason { get; set; }
        public string OtherFrstName { get; set; }
        public string OtherLstName { get; set; }
        public string OtherNationalNo { get; set; }
        public string OtherDescription { get; set; }
        public int? AnunceForVamCd { get; set; }
        public string TashilatTypeDesc { get; set; }
        public string VagozariVamStatDesc { get; set; }
        public string ConfirmApplyDesc { get; set; }
        public string ConfirmTashilatTypeDesc { get; set; }
        public string OrgFarsiName { get; set; }
        public string FrstName { get; set; }
        public string LstName { get; set; }
        public string RankTitle { get; set; }
        public string FullName { get; set; }
        public string VagozariVamStatCode { get; set; }
        public string UnConfirmBankDesc { get; set; }
        public string MissingDocReasonDesc { get; set; }
    }
    #endregion

    #region Person Family
    public class PersonalFamilyViewModel
    {
        public int Id { get; set; }
        public string PrsnNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RelativeId { get; set; }
        public string RelativeTitle { get; set; }
        public string NationalNoFamily { get; set; }
        public string CertificateNumber { get; set; }
    }
    #endregion

    #region Fajr
    public class ItoLogInfoViewModel
    {
        public long Id { get; set; }
        public DateTime? Time { get; set; }
        public string SoftwareVersion { get; set; }
        public string SoftwareId { get; set; }
        public string Url { get; set; }
        public string SoftwareName { get; set; }
        public string ServerIp { get; set; }
        public string PortNumber { get; set; }
        public string ServerHostname { get; set; }
        public string ClientHostname { get; set; }
        public string ClientIp { get; set; }
        public string PageTitle { get; set; }
        public string UserUniqueId { get; set; }
        public string Username { get; set; }
        public string Sensitivity { get; set; }
        public string Importance { get; set; }
        public string ActionType { get; set; }
        public string SubType { get; set; }
        public string SubTypeDescription { get; set; }
        public string Flag { get; set; }
        public string SendDate { get; set; }
        public byte? SendStat { get; set; }
        public short? SendCount { get; set; }
    }
    #endregion

    #region TashilateDastor
    public class TashilatDastorInfoViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RankTitle { get; set; }
        public string BranchTitle { get; set; }
        public string UnitDutyTitle { get; set; }
        public long? MablagheVam { get; set; }
        public int? RankCode { get; set; }
        public int UnitCode { get; set; }
        public string PersonalCode { get; set; }
        public string RowBeginDate { get; set; }
        public string UnitTitle { get; set; }
        public string MaliTitle { get; set; }
        public string RefahiTitle { get; set; }
        public string KhadamatiTitle { get; set; }
        public string VarzeshiTitle { get; set; }
        public string TashilatTypeTitle { get; set; }
        public string TashilatNesbatTitle { get; set; }
        public int? Tashilat { get; set; }
        public int? CodeVam { get; set; }
        public int? CodeRefahi { get; set; }
        public int? CodeKhadamati { get; set; }
        public int? CodeVarzeshi { get; set; }
        public int? Nesbat { get; set; }
    }
    #endregion

    #region TashilatOther
    public class TashilatOtherInfoViewModel
    {
        public int Id { get; set; }
        public int MosaedatResurceCd { get; set; }
        public string PrsnNo { get; set; }
        public int? OmdOrgCd { get; set; }
        public int? MosaedatTypeCd { get; set; }
        public string MosaedatDate { get; set; }
        public int? Price { get; set; }
        public string MosaedatDesc { get; set; }
        public int RowStatusCd { get; set; }
        public string RegDate { get; set; }
        public int RegUserCd { get; set; }
        public int? EditUserCd { get; set; }
        public string EditDate { get; set; }
        public int IsActive { get; set; }
        public string MosaedatTypeDesc { get; set; }
        public string MosaedatResourceDesc { get; set; }
        public string UnitTitle { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    #endregion

    #region تسهیلات بلاعوض
    public class TashilatFacilityBAViewModel
    {
        public int PaidId { get; set; }
        public int? FbasanadCd { get; set; }
        public long? Mablaq { get; set; }
        public string MablaqTitle { get; set; }
        public string AccountNo { get; set; }
        public string SiknessDesc { get; set; }
        public int GroupTypeId { get; set; }
        public long FacilityBelaAvazDetId { get; set; }
        public int RelationTypeId { get; set; }
        public string NationalNo { get; set; }
        public string FatherName { get; set; }
        public string VagozariDesc { get; set; }
        public string RegDate { get; set; }
        public string RelationTypeDesc { get; set; }
        public string TashilatTypeDesc { get; set; }
        public int TashilatTypeCd { get; set; }
        public string SeekFullName { get; set; }
        public int DeActiveTypeId { get; set; }
        public string DeActiveDate { get; set; }
        public string DeActiveDesc { get; set; }
        public string GroupTypeDesc { get; set; }
        public string DeactiveTypeDesc { get; set; }
        public int? ProjectCd { get; set; }
        public string PrsnNo { get; set; }
        public string LstName { get; set; }
        public string FrstName { get; set; }
        public int? OrgId { get; set; }
        public string RankTitle { get; set; }
        public string OrganTitle { get; set; }
        public string RequestTelNo { get; set; }
        public string SanadDate { get; set; }
        public string SandNo { get; set; }
        public string SanadDesc { get; set; }
        public int? DutyStatCd { get; set; }
        public int? DoctorConfirmUserCd { get; set; }
        public string DoctorConfirmDate { get; set; }
        public int? FinalConfirmUserCd { get; set; }
        public string FinalConfirmDate { get; set; }
        public int? FirstStepConfirmUserCd { get; set; }
        public string FirstStepConfirmDate { get; set; }
        public int? HospitalCd { get; set; }
        public string HospitalDesc { get; set; }
        public string ParvandehNo { get; set; }
        public string FacilityStatDesc { get; set; }
        public int? FacilitySeeknessId { get; set; }
        public string SeekDesc { get; set; }
        public int? FacilityStatId { get; set; }
        public string DurationTypeDesc { get; set; }
    }
    #endregion

    #region Nomarat Arzyabi
    public class ExamInfoViewModel
    {
        public long Id { get; set; }
        public decimal? Mark { get; set; }
        public int? TotalMark { get; set; }
        public int? ExamTypeId { get; set; }
        public string ExamTypeTitle { get; set; }
        public string PersonalCode { get; set; }
        public int? CommandRowId { get; set; }
        public string RowBeginDate { get; set; }
        public string RowEndDate { get; set; }
        public int? CommandCode { get; set; }
        public long? ChangeSerialRowId { get; set; }
        public int? SufixType { get; set; }
        public string DocumentOwnerUnitCode { get; set; }
        public int? DocumentType { get; set; }
        public string Document { get; set; }
        public byte? FinalFlag { get; set; }
        public int? LawId { get; set; }
        public string RegisterDate { get; set; }
        public string RegisterPersonalCode { get; set; }
        public byte? PublishedFlag { get; set; }
        public byte? ActiveFlag { get; set; }
        public byte? PrintFlag { get; set; }
        public int? IsNew { get; set; }
        public int? OldCommandCode { get; set; }
        public int? ChangeUnitCode { get; set; }
        public byte? HasChanged { get; set; }
        public string LastChangeTime { get; set; }
        public int? IsSalary { get; set; }
        public int? NiroCommandCode { get; set; }
        public string LastUserName { get; set; }
        public string Reasonchange { get; set; }
        public string ChangeDateRecorde { get; set; }
        public string MelliCode { get; set; }
        public string SmajaExportDate { get; set; }
        public string ActiveFlagStr { get; set; }
        public string PublishedFlagStr { get; set; }
        public string FinalFlagStr { get; set; }
        public string SufixTypeTitle { get; set; }
        public string LawTitle { get; set; }
        public string DocumentTypeTitle { get; set; }
        public long SerialRowId { get; set; }
        public string CommandRowTitle { get; set; }
        public string OcxfileName { get; set; }
    }
    #endregion

    #region Count Tashvighat- Tanbihat - Farar - Nahast

    public class CountTashvighatTanbihatNastFarar
    {
        public int TashvighatCount { get; set; }
        public int TanbihatCount { get; set; }
        public int NahastCount { get; set; }
        public int FararCount { get; set; }
    }

    #endregion

    #region فیش حقوقی
    public class FishViewModel
    {

        public string PrsnNo { get; set; }
        public string DeductionDate { get; set; }
        public string DeductionType { get; set; }
        public string DeductionCode { get; set; }
        public long? TotalDebt { get; set; }

        public string TotalDebtFormat { get; set; }
        public long Id { get; set; }
    }
    #endregion

    #region Moeeser
    public class MoeeserViewModel
    {

        public long Id { get; set; }
        public int OrganId { get; set; }
        public string PrsnNo { get; set; }
        public string AccountNo { get; set; }
        public string TelNo { get; set; }
        public int? AelehQty { get; set; }
        public string AccountNoWife { get; set; }
        public int? MoeeserTypeCd { get; set; }
        public string MoeeserTypeDesc { get; set; }
        public string MoeeserReason { get; set; }
        public string MoeeserConfirm { get; set; }
        public string UnitHelpToMoeeser { get; set; }
        public int? MonthPriceSuggest { get; set; }
        public string MoeeserDesc { get; set; }
        public string IssuedOrders { get; set; }
        public string RegDate { get; set; }
        public int RegUserCd { get; set; }
        public string LastUpdDate { get; set; }
        public int? LastUpdUserCd { get; set; }
        public string NezajaHelpToMoeeser { get; set; }
        public int? StatCd { get; set; }
        public string MoeeserPeriodFrom { get; set; }
        public string MoeeserPeriodTo { get; set; }
        public string FrstName { get; set; }
        public string LstName { get; set; }
        public string SoclScrtyNo { get; set; }
        public string RankTitle { get; set; }
        public int? RankCode { get; set; }
        public int? DayDuration { get; set; }
        public string JobDes { get; set; }
        public int? OmdUnitCode { get; set; }
        public string OmdUnitName { get; set; }
        public int? UnitDutyCode { get; set; }
        public string UnitDutyName { get; set; }
        public int? UserMajierUnitDastoor { get; set; }
        public string UserOrgFarsiName { get; set; }
    }
    #endregion

}

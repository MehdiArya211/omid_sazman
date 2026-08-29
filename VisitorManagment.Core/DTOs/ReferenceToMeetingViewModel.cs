using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VisitorManagment.Core.DTOs
{
    public class ListFileReferenceViewModel
    {
        public List<FactPersonalReferencViewModel> files { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int MeetingId { get; set; }
        public string MeetingTitle { get; set; }
        public string CaptionMeeting { get; set; }
        public bool IsOkayMeeting { get; set; }
        public DateTime IsOkayDate { get; set; }
        public int? IsOkayRegUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }


    }
    public class FactPersonalReferencViewModel
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public int? MeetingId { get; set; }
        public int CountMembersMeeting { get; set; }
        
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
        public string BloodTitle { get; set; }
        public string MarridTitle { get; set; }
        public string ReligoinTitle { get; set; }
        public string EmploymentDate { get; set; }

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
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string RequestDescription { get; set; }

        [Display(Name = "پیوست")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
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
        public DateTime RegDate { get; set; }
        public int? EditUserId { get; set; }
        public string EditDate { get; set; }

    }

}

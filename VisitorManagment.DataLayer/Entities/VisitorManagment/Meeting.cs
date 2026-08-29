using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
    public class Meeting
    {
        [Key]
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
        public int MeetingStatusId { get; set; }

        [Display(Name = "تاریخ برگزاری جلسه")]
        public string StartMeetingDate { get; set; }
        [Display(Name = "ساعت برگزاری جلسه")]
        public string StartMeetingTime { get; set; }

        [Display(Name = "محل جلسه")]
        public int MeetingPlaceId { get; set; }

        [Display(Name = "آی دی رئیس جلسه")]
        public int BoseMeetingId { get; set; }

        [Display(Name = "آی دی منشی جلسه")]
        public int ClerkMeetingId { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Description { get; set; }
        public bool IsDelete { get; set; }

        //zamani ke sherkat konnandeh be jalase erja shode true beshe
        public bool IsSend { get; set; }
        public bool IsOkay { get; set; }
        public DateTime IsOkayDate { get; set; }
        public int? IsOkayRegUserId { get; set; }
        public bool IsFinished { get; set; }

        //
        [Display(Name = "تاریخ ثبت جلسه")]
        public DateTime RegDate { get; set; }
        public int regUserId { get; set; }
        public int? EditUserId { get; set; }
        public DateTime? EditDate { get; set; }

        #region Relations
        public MeetingStatus MeetingStaus { get; set; }
        public BoseMeeting BoseMeeting { get; set; }
        public ClerkMeeting ClerkMeeting { get; set; }
        public MeetingPlace MeetingPlace { get; set; }

        public List<Files> Files { get; set; }
        public List<MemberMeeting> MemberMeeting { get; set; }

        #endregion
    }
}

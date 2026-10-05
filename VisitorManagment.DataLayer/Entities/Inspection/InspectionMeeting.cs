using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.DataLayer.Entities.Inspection
{
    public enum InspectionMeetingStatus { Draft = 1, Published = 2, InProgress = 3, Finished = 4, Cancelled = 5 }

    /// <summary>جلسه مستقل ماژول مدیریت بازرسی.</summary>
    public class InspectionMeeting
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(200)] public string Title { get; set; }
        [MaxLength(1000)] public string Description { get; set; }
        [Required, MaxLength(10)] public string MeetingDate { get; set; }
        [Required, MaxLength(5)] public string StartTime { get; set; }
        public InspectionMeetingStatus Status { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int RegisteredByUserId { get; set; }
        public ICollection<InspectionMeetingUnit> Units { get; set; } = new List<InspectionMeetingUnit>();
    }
}

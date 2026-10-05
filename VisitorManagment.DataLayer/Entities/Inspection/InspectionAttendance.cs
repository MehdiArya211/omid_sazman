using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorManagment.DataLayer.Entities.Inspection
{
    /// <summary>یک نوبت اتصال واقعی کاربر به جلسه بازرسی را نگهداری می‌کند.</summary>
    public class InspectionAttendance
    {
        [Key] public int Id { get; set; }
        public int InspectionMeetingId { get; set; }
        [Required, MaxLength(10)] public string PersonalCode { get; set; }
        [Required, MaxLength(250)] public string FullName { get; set; }
        [MaxLength(100)] public string RankTitle { get; set; }
        public int UnitCode { get; set; }
        [MaxLength(200)] public string UnitTitle { get; set; }
        [Required, MaxLength(100)] public string ConnectionId { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public int DurationSeconds { get; set; }
        [ForeignKey(nameof(InspectionMeetingId))] public InspectionMeeting Meeting { get; set; }
    }
}

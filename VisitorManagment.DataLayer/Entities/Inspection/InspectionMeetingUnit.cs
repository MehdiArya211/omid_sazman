using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorManagment.DataLayer.Entities.Inspection
{
    /// <summary>یگان دعوت‌شده به یک جلسه بازرسی.</summary>
    public class InspectionMeetingUnit
    {
        [Key] public int Id { get; set; }
        public int InspectionMeetingId { get; set; }
        public int UnitCode { get; set; }
        [Required, MaxLength(200)] public string UnitTitle { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int RegisteredByUserId { get; set; }
        [ForeignKey(nameof(InspectionMeetingId))] public InspectionMeeting Meeting { get; set; }
    }
}

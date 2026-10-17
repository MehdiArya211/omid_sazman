using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorManagment.DataLayer.Entities.Inspection
{
    /// <summary>ثبت یک فرد در یک جلسه بازرسی؛ مبنای گزارش تعداد دفعات شرکت است.</summary>
    public class InspectionMeetingPerson
    {
        [Key] public int Id { get; set; }
        public int InspectionMeetingId { get; set; }
        public int InspectionPersonId { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int RegisteredByUserId { get; set; }
        [ForeignKey(nameof(InspectionMeetingId))] public InspectionMeeting Meeting { get; set; }
        [ForeignKey(nameof(InspectionPersonId))] public InspectionPerson Person { get; set; }
    }
}

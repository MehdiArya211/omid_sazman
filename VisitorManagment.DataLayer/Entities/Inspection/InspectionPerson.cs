using System;
using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.DataLayer.Entities.Inspection
{
    /// <summary>تصویر ثابت اطلاعات فرد ثبت‌شده برای جلسات بازرسی.</summary>
    public class InspectionPerson
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(10)] public string PersonalCode { get; set; }
        [Required, MaxLength(100)] public string FirstName { get; set; }
        [Required, MaxLength(150)] public string LastName { get; set; }
        public int? RankCode { get; set; }
        [MaxLength(100)] public string RankTitle { get; set; }
        public int? BranchCode { get; set; }
        [MaxLength(150)] public string BranchTitle { get; set; }
        public int? UnitCode { get; set; }
        [MaxLength(200)] public string UnitTitle { get; set; }
        public int? UnitDutyCode { get; set; }
        [MaxLength(200)] public string UnitDutyTitle { get; set; }
        [MaxLength(250)] public string JobDescription { get; set; }
        public int? RegionalCommandCode { get; set; }
        [MaxLength(200)] public string RegionalCommandTitle { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int RegisteredByUserId { get; set; }
    }
}

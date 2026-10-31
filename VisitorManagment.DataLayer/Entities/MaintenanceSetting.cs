using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorManagment.DataLayer.Entities
{
    /// <summary>تنظیم ماندگار و تک‌رکوردی توقف موقت سامانه.</summary>
    public class MaintenanceSetting
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; } = 1;
        public bool IsEnabled { get; set; }
        [Required, MaxLength(1000)]
        public string Message { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedByUserId { get; set; }
        [ConcurrencyCheck]
        public int Revision { get; set; }
    }
}

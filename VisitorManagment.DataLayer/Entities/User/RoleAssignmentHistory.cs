using System;
using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.DataLayer.Entities.User
{
    /// <summary>
    /// سابقه تصدی جایگاه را نگهداری می‌کند تا مشخص باشد هر نقش در هر بازه در اختیار چه کسی بوده است.
    /// </summary>
    public class RoleAssignmentHistory
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ChangedByUserId { get; set; }

        [MaxLength(500)]
        public string ChangeReason { get; set; }
    }
}

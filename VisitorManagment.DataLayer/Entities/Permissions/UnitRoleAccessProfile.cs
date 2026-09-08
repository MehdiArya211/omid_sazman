using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.DataLayer.Entities.Permissions
{
    /// <summary>
    /// پروفایل دسترسی اختصاصی یک نقش در یک یگان.
    /// نبودن این رکورد به معنی استفاده از دسترسی پایه نقش است.
    /// </summary>
    public class UnitRoleAccessProfile
    {
        [Key]
        public int Id { get; set; }
        public int UnitCode { get; set; }

        [Required, MaxLength(200)]
        public string UnitTitle { get; set; }

        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }

        public List<UnitRolePermission> Permissions { get; set; }
    }
}

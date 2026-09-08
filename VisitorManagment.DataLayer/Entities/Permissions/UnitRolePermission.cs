using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorManagment.DataLayer.Entities.Permissions
{
    /// <summary>
    /// منوی فعال در پروفایل دسترسی اختصاصی یگان و نقش.
    /// </summary>
    public class UnitRolePermission
    {
        [Key]
        public int Id { get; set; }
        public int UnitRoleAccessProfileId { get; set; }
        public int PermissionId { get; set; }

        [ForeignKey(nameof(UnitRoleAccessProfileId))]
        public UnitRoleAccessProfile Profile { get; set; }

        [ForeignKey(nameof(PermissionId))]
        public Permission Permission { get; set; }
    }
}

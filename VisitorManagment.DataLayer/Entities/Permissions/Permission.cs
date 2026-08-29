using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace VisitorManagment.DataLayer.Entities.Permissions
{
    public class Permission
    {
        [Key]
        public int PermissionId { get; set; }

        [Display(Name = "عنوان نقش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string PermissionTitle { get; set; }
        public int? ParentID { get; set; }

        [MaxLength(200)]
        public string ParentUrl { get; set; }
        [MaxLength(200)]
        public string SubUrl { get; set; }
        public int Order { get; set; }
        public bool ShowAll { get; set; }
        public bool IsActive { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string IconName { get; set; }

        [MaxLength (150)]
        public string MenuUrl { get; set; }

        [ForeignKey("ParentID")]
        public List<Permission> Permissions { get; set; }
        public List<RolePermission> RolePermissions { get; set; }


    }
}

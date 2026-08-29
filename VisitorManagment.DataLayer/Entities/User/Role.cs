using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.DataLayer.Entities.User
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Display(Name = "نام نقش")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public int Code { get; set; }
        public int? SortNum { get; set; }
        public int? SortNumMoavenat { get; set; }
        public int RoleType { get; set; }
        public int? RoleTypeFinalId { get; set; }
        public bool IsDelete { get; set; }

        #region Relations
        public List<UserRole> UserRoles { get; set; }
        public List<WorkFlow> WorkFlows { get; set; }
        public RoleTypeFinal RoleTypeFinal { get; set; }

        #endregion
    }
}

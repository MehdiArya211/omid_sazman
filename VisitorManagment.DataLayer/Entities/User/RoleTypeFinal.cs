using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.DataLayer.Entities.User
{
    public class RoleTypeFinal
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "نام نقش")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public int Code { get; set; }
        public int? SortNum { get; set; }
        public bool IsDelete { get; set; }

        #region Relation
        public List<Role> Roles { get; set; }

        #endregion
    }
}

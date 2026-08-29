using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.DataLayer.Entities.Ranking
{
    public class TblDepartmentType
    {
        [Key]
        public int Id { get; set; }
        public int Code { get; set; }
        [Display(Name = "نام قسمت")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string DepartmentName { get; set; }
        public int SortNum { get; set; }
        public bool IsActive { get; set; }

        #region Relation
        public List<TblDepartment> TblDepartments { get; set; }
        #endregion


    }
}

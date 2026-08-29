using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.DataLayer.Entities.Ranking
{
    public class TblDepartment
    {
        [Key]
        public int Id { get; set; }
        public int DepartmentCode { get; set; }
        public int DepartmentFatherCode { get; set; }

        [Display(Name = "نام قسمت")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string DepartmentName { get; set; }
        public int Priority { get; set; }
        public bool IsMostaghel { get; set; }
        public int UnitCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsVip { get; set; }
        public int DepartmentTypeId { get; set; }

        [Display(Name = "عنوان قسمت")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string DepartmentTitle { get; set; }

        #region Relation
        public TblDepartmentType TblDepartmentType { get; set; }
        public List<Point> Points { get; set; }
        #endregion
    }
}

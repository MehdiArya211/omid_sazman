using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
    public class ActionType
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "اقدام")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }
        public int Code { get; set; }

        #region Relations
        public List<WorkFlow> WorkFlows { get; set; }
        public List<Hamesh> Hameshes { get; set; }

        #endregion
    }
}

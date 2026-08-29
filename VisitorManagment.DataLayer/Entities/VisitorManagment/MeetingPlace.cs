using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
    public class MeetingPlace
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "نام محل")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Display(Name = "اولویت")]
        public int? SortName { get; set; }

        [Display(Name = "وضعیت")]
        public bool IsActive { get; set; }
        public int Code { get; set; }

        #region Relations

        #endregion
    }
}
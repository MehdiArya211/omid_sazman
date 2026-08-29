using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
    public class MemberMeeting
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "شناسه درخواست ملاقات")]
        public int FileId { get; set; }

        [Display(Name = "شناسه جلسه ملاقات")]
        public int? MeetingId { get; set; }

        [Display(Name = "کد یگان ")]
        public int UnitCode { get; set; }

        [Display(Name = "عنوان یگان")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string UnitTitle { get; set; }

        public int? SortNum { get; set; }

        //ممکنه که نفر در جلسه حضور نداشته باشه و بخوایم به جلسه دیگه ای ارجاعش بدیم
        [Display(Name = "آیا جلسه برگزار شده یا خیر؟")]
        public bool IsMeetingHold { get; set; }

        [Display(Name = "فعال یا خیر فعال بودن برای برگزاری جلسه نفر")]
        public bool IsActive { get; set; }

        public DateTime RegDate { get; set; }

        public bool IsDelete { get; set; }

        #region Relation
        public VisitorManagment.Files File { get; set; }
        public Meeting Meeting { get; set; }
        #endregion
    }
}

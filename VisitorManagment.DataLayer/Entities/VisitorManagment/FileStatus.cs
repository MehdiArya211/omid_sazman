using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
    public class FileStatus
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "{0}راوارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} باشد")]
        public string Title { get; set; }
        public int Code { get; set; }
        #region Relations
        public List<Files> Files { get; set; }

        #endregion
    }
}

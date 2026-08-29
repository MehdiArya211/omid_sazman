using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
    public class FileType
    {
        public int Id { get; set; }
        [Display(Name = "عنوان")]
        [MaxLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Title { get; set; }
        public int Code { get; set; }
        public bool IsDelete { get; set; }

        #region Relation
        public List<Files> File { get; set; }
        #endregion
    }
}

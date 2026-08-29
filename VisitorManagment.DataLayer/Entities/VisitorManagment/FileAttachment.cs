using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
    public class FileAttachment
    {
        [Key]
        public int Id { get; set; }
        public int FileId { get; set; }

        [Display(Name = "فایل آپلودی")]
        public string FileUplodeAttacmentDastor { get; set; }



        #region Relation
        public Files File { get; set; }
        #endregion
    }
}

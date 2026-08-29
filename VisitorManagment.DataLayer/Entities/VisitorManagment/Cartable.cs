using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
    public class Cartable
    {
        [Key]
        public int Id { get; set; }
        public int RcvrUserId { get; set; }
        public int SndrUserId { get; set; }
        public int FileId { get; set; }
        public int StateCd { get; set; }
        public bool IsView { get; set; }
        public bool IsDone { get; set; }
        public DateTime RegDate { get; set; }

        #region Relation
        public Files File { get; set; }
        #endregion
    }
}

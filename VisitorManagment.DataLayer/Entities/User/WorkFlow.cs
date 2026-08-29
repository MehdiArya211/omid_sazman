using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.DataLayer.Entities.VisitorManagment
{
   public class WorkFlow
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public int SndrRoleId { get; set; }

        [Required(ErrorMessage = "{0} را وارد کنید")]
        //آی دی کسی که بهش دسترسی داره
        //یا همون آی دی ِوزری که بهش دسترسی داره
        public int RcvrRoleId { get; set; }

        [Required(ErrorMessage = "{0} را وارد کنید")]
        public int RegUserId { get; set; }
        public int? EditUserId { get; set; }

        [Required(ErrorMessage = "{0} را وارد کنید")]
        public DateTime RegDate { get; set; }
        public DateTime? EditDate { get; set; }


        #region Realtion


        [ForeignKey("RcvrRoleId")]
        public Role Role { get; set; }
        #endregion

    }
}

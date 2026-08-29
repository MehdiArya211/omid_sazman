using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VisitorManagment.DataLayer.Entities.User
{
    public class UserRole
    {
        [Key]
        public int UR_Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        #region Relations
        public Users User { get; set; }
        //
        public UserSub UserSub { get; set; }
        //
        public Role Role { get; set; }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using VisitorManagment.DataLayer.Entities.User;
namespace VisitorManagment.Core.DTOs
{
    public class CreateUserAccessViewModel
    {
        public int RoleId { get; set; }
        public List<UserRole> AccessUserId { get; set; }
        public int AddUserId { get; set; }
        public DateTime SaveDate { get; set; }
        public int? EditUserId { get; set; }
        public DateTime? EditDate { get; set; }

    }

    public class WorkFlowViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string RankTitle { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string PersonalCode { get; set; }

        public int SenderRoleId { get; set; }
        public int ReciverRoleId { get; set; }


    }
}

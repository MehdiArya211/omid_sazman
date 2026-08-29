using System;
using System.Collections.Generic;
using System.Text;
using VisitorManagment.Core.DTOs;
using VisitorManagment.DataLayer.Entities.User;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface IWorkFlowService
    {
        List<Role> GetrcvrList(int roleId);
        /// <summary>
        /// لیست کاربرانی که یک نقش به آنها دسترسی دارد
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<WorkFlowViewModel> GetReciverList(int roleId);
        /// <summary>
        /// لیست کاربرانی که یک نقش به آنها دسترسی ندارد
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<WorkFlowViewModel> GetUnAccessList(int roleId);

        List<Users> GetRecieverUserListBySndrRoleId(int roleId, int unitDutyCode , int unitCode , int codeGha , int fileId , int roleTypeId);
        List<Users> GetRecieverFarmandehiNezajaList(int roleId);
        List<Users> GetRecieverUserListByFileId(int fileId , int userId);
        ListUserForAoudatToCartable GetRecieverUserListByFileIdDto(int fileId);
        Users GetRecieverUserByFileId(int fileId);

        /// <summary>
        /// اعطای دسترسی هر نقش به چه نقش هایی دسترسی دارد
        /// </summary>
        /// <param name="reciverRoleList"></param>
        /// <param name="roleId"></param>
        /// <param name="addUserId"></param>
        void AddAccessToRole(List<int> reciverRoleList, int roleId, int addUserId);
        /// <summary>
        /// حذف دسترسی هر نقش به چه نقش هایی دسترسی دارد
        /// </summary>
        /// <param name="reciverRoleList"></param>
        /// <param name="roleId"></param>
        /// <param name="addUserId"></param>
        void RemoveAccessToRole(List<int> reciverRoleList, int roleId, int addUserId);

    }
}

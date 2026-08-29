using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManagment.Core.Services.Interfaces.RolePermissions
{
    public interface IRolePermissionService
    {
        /// <summary>
        /// ثبت دسترسی هر نقش به منوها
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionId"></param>
        void AddPermissionToRole(int roleId , List<int> permissionId);
        /// <summary>
        /// حذف دسترسی هر نقش به منوها
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionId"></param>
        void RemovePermissionToRole(int roleId , List<int> permissionId);
    }
}

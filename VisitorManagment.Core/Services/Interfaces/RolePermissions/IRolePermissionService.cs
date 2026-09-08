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
        int AddPermissionToRole(int roleId , List<int> permissionId);
        int AddPermissionToRole(int roleId, List<int> permissionId, int? unitCode);
        /// <summary>
        /// حذف دسترسی هر نقش به منوها
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionId"></param>
        int RemovePermissionToRole(int roleId , List<int> permissionId);
        int RemovePermissionToRole(int roleId, List<int> permissionId, int? unitCode);
        bool ResetUnitPermissionToRole(int roleId, int unitCode);
    }
}

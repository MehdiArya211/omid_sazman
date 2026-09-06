using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorManagment.Core.Services.Interfaces.RolePermissions;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.Permissions;

namespace VisitorManagment.Core.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly VisitorManagmentContext _context;
        public RolePermissionService(VisitorManagmentContext context)
        {
            _context = context;
        }
        #region اعضا و متدهای کلاس

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public void AddPermissionToRole(int roleId, List<int> permissionId)
        {
            if (roleId <= 0 || permissionId == null || permissionId.Count == 0)
            {
                return;
            }

            var requestedPermissionIds = permissionId.Where(id => id > 0).Distinct().ToList();
            var existingPermissionIds = _context.RolePermission
                .Where(item => item.RoleId == roleId && requestedPermissionIds.Contains(item.PermissionId))
                .Select(item => item.PermissionId)
                .ToList();

            var newRolePermissions = requestedPermissionIds
                .Where(id => !existingPermissionIds.Contains(id))
                .Select(id => new RolePermission { RoleId = roleId, PermissionId = id })
                .ToList();

            if (newRolePermissions.Count == 0)
            {
                return;
            }

            _context.RolePermission.AddRange(newRolePermissions);
            _context.SaveChanges();
        }

        /// <summary>
        /// اطلاعات مشخص‌شده را حذف می‌کند.
        /// </summary>
        public void RemovePermissionToRole(int roleId, List<int> permissionId)
        {
            if (roleId <= 0 || permissionId == null || permissionId.Count == 0)
            {
                return;
            }

            var requestedPermissionIds = permissionId.Where(id => id > 0).Distinct().ToList();
            var rolePermissions = _context.RolePermission
                .Where(item => item.RoleId == roleId && requestedPermissionIds.Contains(item.PermissionId))
                .ToList();

            if (rolePermissions.Count == 0)
            {
                return;
            }

            _context.RolePermission.RemoveRange(rolePermissions);
            _context.SaveChanges();
        }
        #endregion
    }
}

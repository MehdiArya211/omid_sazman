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
        public int AddPermissionToRole(int roleId, List<int> permissionId)
        {
            return AddPermissionToRole(roleId, permissionId, null);
        }

        public int AddPermissionToRole(int roleId, List<int> permissionId, int? unitCode)
        {
            if (roleId <= 0 || permissionId == null || permissionId.Count == 0)
            {
                return 0;
            }

            var requestedPermissionIds = permissionId.Where(id => id > 0).Distinct().ToList();
            if (!_context.Roles.Any(role => role.RoleId == roleId && !role.IsDelete)) return 0;
            requestedPermissionIds = _context.Permission.Where(permission => requestedPermissionIds.Contains(permission.PermissionId)).Select(permission => permission.PermissionId).ToList();
            var parentIds = _context.Permission
                .Where(permission => requestedPermissionIds.Contains(permission.PermissionId) && permission.ParentID.HasValue)
                .Select(permission => permission.ParentID.Value)
                .ToList();
            requestedPermissionIds = requestedPermissionIds.Concat(parentIds).Distinct().ToList();
            if (unitCode.HasValue)
            {
                var profile = GetOrCreateUnitProfile(roleId, unitCode.Value);
                if (profile == null) return 0;
                var existingUnitPermissionIds = _context.UnitRolePermissions
                    .Where(item => item.UnitRoleAccessProfileId == profile.Id && requestedPermissionIds.Contains(item.PermissionId))
                    .Select(item => item.PermissionId)
                    .ToList();
                var unitPermissions = requestedPermissionIds
                    .Where(id => !existingUnitPermissionIds.Contains(id))
                    .Select(id => new UnitRolePermission { UnitRoleAccessProfileId = profile.Id, PermissionId = id })
                    .ToList();
                if (unitPermissions.Count == 0) return 0;
                _context.UnitRolePermissions.AddRange(unitPermissions);
                _context.SaveChanges();
                return unitPermissions.Count;
            }
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
                return 0;
            }

            _context.RolePermission.AddRange(newRolePermissions);
            _context.SaveChanges();
            return newRolePermissions.Count;
        }

        /// <summary>
        /// اطلاعات مشخص‌شده را حذف می‌کند.
        /// </summary>
        public int RemovePermissionToRole(int roleId, List<int> permissionId)
        {
            return RemovePermissionToRole(roleId, permissionId, null);
        }

        public int RemovePermissionToRole(int roleId, List<int> permissionId, int? unitCode)
        {
            if (roleId <= 0 || permissionId == null || permissionId.Count == 0)
            {
                return 0;
            }

            var requestedPermissionIds = permissionId.Where(id => id > 0).Distinct().ToList();
            var childIds = _context.Permission
                .Where(permission => permission.ParentID.HasValue && requestedPermissionIds.Contains(permission.ParentID.Value))
                .Select(permission => permission.PermissionId)
                .ToList();
            requestedPermissionIds = requestedPermissionIds.Concat(childIds).Distinct().ToList();
            if (unitCode.HasValue)
            {
                var profile = GetOrCreateUnitProfile(roleId, unitCode.Value);
                if (profile == null) return 0;
                var unitPermissions = _context.UnitRolePermissions
                    .Where(item => item.UnitRoleAccessProfileId == profile.Id && requestedPermissionIds.Contains(item.PermissionId))
                    .ToList();
                if (unitPermissions.Count == 0) return 0;
                _context.UnitRolePermissions.RemoveRange(unitPermissions);
                _context.SaveChanges();
                return unitPermissions.Count;
            }
            var rolePermissions = _context.RolePermission
                .Where(item => item.RoleId == roleId && requestedPermissionIds.Contains(item.PermissionId))
                .ToList();

            if (rolePermissions.Count == 0)
            {
                return 0;
            }

            _context.RolePermission.RemoveRange(rolePermissions);
            _context.SaveChanges();
            return rolePermissions.Count;
        }

        public bool ResetUnitPermissionToRole(int roleId, int unitCode)
        {
            var profile = _context.UnitRoleAccessProfiles.SingleOrDefault(item => item.RoleId == roleId && item.UnitCode == unitCode);
            if (profile == null) return false;
            var permissions = _context.UnitRolePermissions.Where(item => item.UnitRoleAccessProfileId == profile.Id).ToList();
            _context.UnitRolePermissions.RemoveRange(permissions);
            _context.UnitRoleAccessProfiles.Remove(profile);
            _context.SaveChanges();
            return true;
        }

        private UnitRoleAccessProfile GetOrCreateUnitProfile(int roleId, int unitCode)
        {
            var profile = _context.UnitRoleAccessProfiles.SingleOrDefault(item => item.RoleId == roleId && item.UnitCode == unitCode);
            if (profile != null) return profile;

            var unitTitle = _context.Users
                .Where(user => user.UnitCode == unitCode)
                .Select(user => user.UnitTitle)
                .FirstOrDefault();
            if (string.IsNullOrWhiteSpace(unitTitle)) return null;

            profile = new UnitRoleAccessProfile { RoleId = roleId, UnitCode = unitCode, UnitTitle = unitTitle };
            _context.UnitRoleAccessProfiles.Add(profile);
            _context.SaveChanges();

            var basePermissionIds = _context.RolePermission
                .Where(item => item.RoleId == roleId)
                .Select(item => item.PermissionId)
                .Distinct()
                .ToList();
            if (basePermissionIds.Count > 0)
            {
                _context.UnitRolePermissions.AddRange(basePermissionIds.Select(permissionId => new UnitRolePermission
                {
                    UnitRoleAccessProfileId = profile.Id,
                    PermissionId = permissionId
                }));
                _context.SaveChanges();
            }
            return profile;
        }
        #endregion
    }
}

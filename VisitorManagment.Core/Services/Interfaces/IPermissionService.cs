using VisitorManagment.DataLayer.Entities.Permissions;
using VisitorManagment.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using VisitorManagment.Core.DTOs;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        #region Roles

        List<Role> GetRoles(string roleTypeId);
        /// <summary>
        /// تمام نقش های موجود در سیستم
        /// </summary>
        /// <returns></returns>
        List<Role> GetAllRoles();
        int AddRole(Role role);
        Role GetRoleById(int roleId);
        void UpdateRole(Role role);
        void DeleteRole(Role role);
        bool RoleTitleOrCodeExists(string title, int code, int excludeRoleId = 0);
        bool IsRoleInUse(int roleId);
        void AddRolesToUser(int roleId, int userId);
        void EditRolesToUser(int roleId, int userId);
        #endregion
        #region province&&City
        List<Permission> GetPermissionsForUser(int userId);
        List<UnitAccessOptionViewModel> GetUnitsForAccessManagement();
        bool HasUnitAccessProfile(int roleId, int unitCode);
        #endregion

        /// <summary>
        /// تمامی دسترسی های منو
        /// </summary>
        /// <returns></returns>
        List<Permission> GetAllPermission();
        Permission GetPermissionById(int permissionId);
        int AddPermission(Permission permission);
        void UpdatePermission(Permission permission);
        bool IsPermissionInUse(int permissionId);
        void DeletePermission(int permissionId);
        bool UpdatePermissionOrder(List<PermissionOrderViewModel> items);

        /// <summary>
        /// دادن دسترسی نقش ها به منو ها
        /// </summary>
        /// <param name="rolesId"></param>
        /// <param name="permissionsId"></param>
        void AddRolesToRolePermission(List<int> rolesId , List<int> permissionsId);

        /// <summary>
        /// لیست هر نقشی به چه منو هایی دسترسی دارد
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<PermissionViewModel> GetAccessReciverMenuList(int roleId);
        List<PermissionViewModel> GetAccessReciverMenuList(int roleId, int? unitCode);

        /// <summary>
        /// لیست هر نقشی به چه منو هایی دسترسی ندارد
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<PermissionViewModel> GetUnAccessReciverMenuList(int roleId);
        List<PermissionViewModel> GetUnAccessReciverMenuList(int roleId, int? unitCode);
    }
}

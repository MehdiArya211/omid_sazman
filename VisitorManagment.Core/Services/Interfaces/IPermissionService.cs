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
        void AddRolesToUser(int roleId, int userId);
        void EditRolesToUser(int roleId, int userId);
        #endregion
        #region province&&City
        List<Permission> GetPermissionsForUser(int userId);
        #endregion

        /// <summary>
        /// تمامی دسترسی های منو
        /// </summary>
        /// <returns></returns>
        List<Permission> GetAllPermission();

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

        /// <summary>
        /// لیست هر نقشی به چه منو هایی دسترسی ندارد
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<PermissionViewModel> GetUnAccessReciverMenuList(int roleId);
    }
}

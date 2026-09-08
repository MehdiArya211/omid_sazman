using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.Permissions;
using VisitorManagment.DataLayer.Entities.User;
using System.Collections.Generic;
using System.Linq;
using VisitorManagment.Core.DTOs;
using Microsoft.EntityFrameworkCore;

namespace VisitorManagment.Core.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly VisitorManagmentContext _context;
        public PermissionService(VisitorManagmentContext context)
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
        public int AddRole(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return role.RoleId;
        }

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public void AddRolesToUser(int roleId, int userId)
        {


            _context.UserRoles.Add(new UserRole()
            {
                RoleId = roleId,
                UserId = userId
            });


            _context.SaveChanges();
        }

        /// <summary>
        /// اطلاعات مشخص‌شده را حذف می‌کند.
        /// </summary>
        public void DeleteRole(Role role)
        {
            role.IsDelete = true;
            UpdateRole(role);
        }

        public bool RoleTitleOrCodeExists(string title, int code, int excludeRoleId = 0)
        {
            var normalizedTitle = (title ?? string.Empty).Trim();
            return _context.Roles.Any(role => role.RoleId != excludeRoleId && !role.IsDelete &&
                (role.Title == normalizedTitle || role.Code == code));
        }

        public bool IsRoleInUse(int roleId)
        {
            return _context.UserRoles.Any(item => item.RoleId == roleId) ||
                   _context.RolePermission.Any(item => item.RoleId == roleId);
        }

        /// <summary>
        /// اطلاعات موجود را بررسی و به‌روزرسانی می‌کند.
        /// </summary>
        public void EditRolesToUser(int roleId, int userId)
        {
            //=== Delete All Roles

            _context.UserRoles.Where(u => u.UserId == userId).ToList().ForEach(r => _context.UserRoles.Remove(r));

            AddRolesToUser(roleId, userId);
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public Role GetRoleById(int roleId)
        {
            return _context.Roles.Find(roleId);
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<Role> GetRoles(string roleTypeId)
        {
            List<Role> role = _context.Roles.ToList();

            //switch (roleTypeId)
            //{
            //    case "101":
            //        roles = _context.Roles.Where(x=>x.RoleType==1 && x.RoleType==2 && x.RoleType==3).OrderBy(x => x.SortNum).ToList();
            //        break;
            //}
            // return _context.Roles.OrderBy(x=>x.SortNum).ToList();
            //return roles;
            //*****************************

            //اگر کاربر ادمین قرارگاه بود فقط بتونه نقش های کاربر عادی ، ف یگان مستقیم ، ف یگان عمده ، ادمین یگان سامانه رو بتونه بده 

            if (roleTypeId == "100")
            {
                role = role.OrderBy(x => x.SortNum).ToList();

            }
            else
            {
                if (roleTypeId == "102")
                {
                    role = role.Where(x => x.RoleType == 1 || x.RoleType == 2 || x.RoleType == 3).OrderBy(x => x.SortNum).ToList();
                }
                else
                {
                    role = role.Where(x => x.RoleType == 1 || x.RoleType == 2 || x.RoleType == 3 || x.RoleType == 102).OrderBy(x => x.SortNum).ToList();
                }

            }
            return role;

        }

        /// <summary>
        /// اطلاعات موجود را بررسی و به‌روزرسانی می‌کند.
        /// </summary>
        public void UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            _context.SaveChanges();
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<Permission> GetPermissionsForUser(int userId)
        {
            var query = from p in _context.Permission
                        join rp in _context.RolePermission on p.PermissionId equals rp.PermissionId
                        join r in _context.Roles on rp.RoleId equals r.RoleId
                        join ur in _context.UserRoles on r.RoleId equals ur.RoleId
                        where ur.UserId == userId
                        select p;

            return query.Distinct().OrderBy(p => p.Order).ToList();
        }
        /// <summary>
        /// تمام نقش های موجود در سیستم
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<Role> GetAllRoles()
        {
            var res = _context.Roles.ToList();

            return res;
        }

        /// <summary>
        /// تمامی دسترسی های منو
        /// </summary>
        /// <returns></returns>
        public List<Permission> GetAllPermission()
        {
            var result = _context.Permission.ToList();

            return result;
        }

        public Permission GetPermissionById(int permissionId)
        {
            return _context.Permission.SingleOrDefault(permission => permission.PermissionId == permissionId);
        }

        public int AddPermission(Permission permission)
        {
            _context.Permission.Add(permission);
            _context.SaveChanges();
            return permission.PermissionId;
        }

        public void UpdatePermission(Permission permission)
        {
            _context.Permission.Update(permission);
            _context.SaveChanges();
        }

        public bool IsPermissionInUse(int permissionId)
        {
            return _context.Permission.Any(permission => permission.ParentID == permissionId) ||
                   _context.RolePermission.Any(item => item.PermissionId == permissionId);
        }

        public void DeletePermission(int permissionId)
        {
            var permission = GetPermissionById(permissionId);
            if (permission == null || IsPermissionInUse(permissionId)) return;
            _context.Permission.Remove(permission);
            _context.SaveChanges();
        }

        public bool UpdatePermissionOrder(List<PermissionOrderViewModel> items)
        {
            if (items == null || items.Count == 0) return false;
            if (items.Any(item => item.PermissionId <= 0) || items.GroupBy(item => item.PermissionId).Any(group => group.Count() > 1)) return false;
            var ids = items.Select(item => item.PermissionId).Distinct().ToList();
            var allPermissions = _context.Permission.ToList();
            if (ids.Count != allPermissions.Count || allPermissions.Any(permission => !ids.Contains(permission.PermissionId))) return false;
            var requested = items.ToDictionary(item => item.PermissionId);
            foreach (var item in items)
            {
                if (!item.ParentId.HasValue) continue;
                if (item.ParentId.Value == item.PermissionId || !requested.ContainsKey(item.ParentId.Value) || requested[item.ParentId.Value].ParentId.HasValue) return false;
            }
            var permissions = allPermissions;
            foreach (var permission in permissions)
            {
                var item = items.First(value => value.PermissionId == permission.PermissionId);
                permission.ParentID = item.ParentId == permission.PermissionId ? null : item.ParentId;
                permission.Order = item.Order;
            }
            _context.SaveChanges();
            return true;
        }


        /// <summary>
        /// دادن دسترسی نقش ها به منو ها
        /// </summary>
        /// <param name="rolesId"></param>
        /// <param name="permissionsId"></param>
        public void AddRolesToRolePermission(List<int> rolesId, List<int> permissionsId)
        {
            var rolePermissionList = new List<RolePermission>();

            foreach (var item in rolesId)
            {
                foreach (var item1 in permissionsId)
                {
                    var rolePermission = new RolePermission();
                    rolePermission.RoleId = item;
                    rolePermission.PermissionId = item1;
                    rolePermissionList.Add(rolePermission);
                }
            }

            foreach (var item in rolePermissionList)
            {
                _context.RolePermission.Add(item);
            }
            // _context.Cartables.Add(cartable);
            _context.SaveChanges();
        }


        /// <summary>
        /// لیست هر نقشی به چه منو هایی دسترسی دارد
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<PermissionViewModel> GetAccessReciverMenuList(int roleId)
        {
            var rcvrList = _context.RolePermission
                .Include(x => x.Role)
                .Include(x => x.Permission)
                .Where(c => c.RoleId == roleId)
                .ToList();

            var permission = new List<PermissionViewModel>();

            permission = rcvrList.Select(x => new PermissionViewModel
            {
                RoleId = x.RoleId,
                PermissionId = x.PermissionId,
                RoleTitle = x.Role.Title,
                PermossionTitle = x.Permission.PermissionTitle,

            }).ToList();


            return permission;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<PermissionViewModel> GetUnAccessReciverMenuList(int roleId)
        {
            var reciverList = _context.RolePermission.Where(x => x.RoleId == roleId).Select(x => x.PermissionId).ToList();

            var rcvrList = _context.Permission
                .Include(x => x.RolePermissions)
                .ThenInclude(x => x.Role)
                .Where(c => (!reciverList.Contains(c.PermissionId)))
                .ToList();

            var permission = new List<PermissionViewModel>();

            permission = rcvrList.Select(x => new PermissionViewModel
            {
                Id = x.PermissionId,
                PermossionTitle = x.PermissionTitle,
                RoleTitle = "",
            }).ToList();


            return permission;
        }
        #endregion
    }
}

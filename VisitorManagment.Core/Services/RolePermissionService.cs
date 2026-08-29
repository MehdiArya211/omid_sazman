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


            foreach (var item in permissionId)
            {
                RolePermission rolePermission = new RolePermission();
                rolePermission.RoleId = roleId;
                rolePermission.PermissionId = item;
                _context.RolePermission.Add(rolePermission);
                _context.SaveChanges();
            }


        }

        /// <summary>
        /// اطلاعات مشخص‌شده را حذف می‌کند.
        /// </summary>
        public void RemovePermissionToRole(int roleId, List<int> permissionId)
        {
            foreach (int item in permissionId)
            {
                var rolePermission = _context.RolePermission.Where(x => x.RoleId == roleId && x.PermissionId == item).FirstOrDefault();

                _context.Remove(rolePermission);
                _context.SaveChanges();
            }
        }
        #endregion
    }
}

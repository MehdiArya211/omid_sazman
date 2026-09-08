using System.Linq;
using Microsoft.EntityFrameworkCore;
using VisitorManagment.Core.Constants;
using VisitorManagment.Core.DTOs.Access;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.Core.Services
{
    /// <summary>
    /// پیاده‌سازی متمرکز تشخیص سمت و محدوده سازمانی کاربران.
    /// </summary>
    public class UserAccessContextService : IUserAccessContextService
    {
        #region Fields and constructor

        private readonly VisitorManagmentContext _context;

        public UserAccessContextService(VisitorManagmentContext context)
        {
            _context = context;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public UserOrganizationalContext GetUserContext(int userId)
        {
            if (userId <= 0) return null;

            var user = _context.Users.AsNoTracking()
                .Where(item => item.Id == userId && item.IsActive)
                .Select(item => new
                {
                    item.Id,
                    item.UnitCode,
                    item.UnitDutyCode,
                    item.CodGha
                })
                .SingleOrDefault();

            if (user == null) return null;

            var roles = _context.UserRoles.AsNoTracking()
                .Where(item => item.UserId == userId && !item.Role.IsDelete)
                .Select(item => new { item.RoleId, item.Role.RoleType })
                .ToList();

            var roleTypes = roles.Select(item => item.RoleType).Distinct().ToList();
            return new UserOrganizationalContext
            {
                UserId = user.Id,
                UnitCode = user.UnitCode,
                UnitDutyCode = user.UnitDutyCode,
                GharargahCode = user.CodGha,
                RoleIds = roles.Select(item => item.RoleId).Distinct().ToList(),
                RoleTypes = roleTypes,
                IsSystemAdministrator = roleTypes.Contains(SystemRoleTypes.SystemAdministrator)
            };
        }

        /// <inheritdoc />
        public bool CanAccessUnit(int userId, int unitCode)
        {
            if (unitCode <= 0) return false;
            var context = GetUserContext(userId);
            return context != null && (context.IsSystemAdministrator || context.UnitCode == unitCode);
        }

        /// <inheritdoc />
        public bool CanAccessGharargah(int userId, int gharargahCode)
        {
            if (gharargahCode <= 0) return false;
            var context = GetUserContext(userId);
            return context != null && (context.IsSystemAdministrator || context.GharargahCode == gharargahCode);
        }

        #endregion
    }
}

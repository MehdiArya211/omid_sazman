using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.Permissions;
using VisitorManagment.DataLayer.Entities.User;
using System.Collections.Generic;
using System.Linq;
using VisitorManagment.Core.DTOs;
using Microsoft.EntityFrameworkCore;
using VisitorManagment.Core.Constants;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.DataLayer.Entities.VisitorManagment;
using System;

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

        /// <inheritdoc />
        public BaseResult ReplaceRoleHolder(int roleId, int newUserId, int administratorUserId)
        {
            if (roleId <= 0 || newUserId <= 0 || administratorUserId <= 0)
                return new BaseResult(false, "اطلاعات جایگزینی نقش کامل نیست.");

            var role = _context.Roles.FirstOrDefault(item => item.RoleId == roleId && !item.IsDelete);
            var newUser = _context.Users.FirstOrDefault(item => item.Id == newUserId && item.IsActive && !item.IsDelete);
            var administrator = _context.Users.Include(item => item.UserRoles).ThenInclude(item => item.Role)
                .FirstOrDefault(item => item.Id == administratorUserId && item.IsActive && !item.IsDelete);
            if (role == null) return new BaseResult(false, "نقش انتخابی معتبر نیست.");
            if (newUser == null) return new BaseResult(false, "کاربر جدید فعال و معتبر نیست.");
            if (administrator == null) return new BaseResult(false, "هویت مدیر انجام‌دهنده معتبر نیست.");

            // سطح Serializable مانع می‌شود دو مدیر هم‌زمان دو متصدی متفاوت برای یک نقش ثبت کنند.
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    var changeDate = DateTime.Now;
                    var formerAssignments = _context.UserRoles
                        .Where(item => item.RoleId == roleId && item.UserId != newUserId)
                        .ToList();
                    var formerUserIds = formerAssignments.Select(item => item.UserId).Distinct().ToList();

                    // نقش‌های قبلی نفر جدید باید خالی شوند؛ کارتابل آن جایگاه‌ها نباید به جایگاه مقصد منتقل شود.
                    var previousAssignments = _context.UserRoles
                        .Where(item => item.UserId == newUserId && item.RoleId != roleId)
                        .ToList();
                    var previousRoleIds = previousAssignments.Select(item => item.RoleId).Distinct().ToList();

                    BackfillOpenCartableRoles(formerUserIds.Concat(new[] { newUserId }).Distinct().ToList());

                    var openRows = _context.Cartables
                        .Where(item => !item.IsDone &&
                            (item.RcvrRoleId == roleId ||
                             (!item.RcvrRoleId.HasValue && formerUserIds.Contains(item.RcvrUserId))))
                        .OrderBy(item => item.RegDate).ThenBy(item => item.Id)
                        .ToList();
                    var fileIds = openRows.Select(item => item.FileId).Distinct().ToList();

                    foreach (var row in openRows) row.IsDone = true;

                    foreach (var fileId in fileIds)
                    {
                        _context.Cartables.Add(new Cartable
                        {
                            FileId = fileId,
                            SndrUserId = administratorUserId,
                            RcvrUserId = newUserId,
                            RcvrRoleId = roleId,
                            StateCd = 0,
                            IsView = false,
                            IsDone = false,
                            RegDate = DateTime.Now
                        });
                    }

                    // درخواست‌های نقش قبلی احمدی در همان جایگاه باقی می‌مانند، ولی تا تعیین مسئول جدید بدون کاربر هستند.
                    var previousPositionRows = _context.Cartables
                        .Where(item => !item.IsDone && item.RcvrUserId == newUserId &&
                            item.RcvrRoleId.HasValue && previousRoleIds.Contains(item.RcvrRoleId.Value))
                        .ToList();
                    foreach (var row in previousPositionRows)
                    {
                        row.IsDone = true;
                        _context.Cartables.Add(new Cartable
                        {
                            FileId = row.FileId,
                            SndrUserId = administratorUserId,
                            RcvrUserId = 0,
                            RcvrRoleId = row.RcvrRoleId,
                            StateCd = row.StateCd,
                            IsView = false,
                            IsDone = false,
                            RegDate = changeDate
                        });
                    }

                    var adminRole = administrator.UserRoles.Select(item => item.Role).FirstOrDefault();
                    var newHolderTitle = (newUser.RankTitle + " " + newUser.FirstName + " " + newUser.LastName).Trim();
                    foreach (var fileId in fileIds)
                    {
                        var parentId = _context.Hameshes
                            .Where(item => item.FileId == fileId && item.UserDesc != null && item.UserDesc.Trim() != "")
                            .OrderByDescending(item => item.RegDate).ThenByDescending(item => item.Id)
                            .Select(item => (int?)item.Id).FirstOrDefault();
                        _context.Hameshes.Add(new Hamesh
                        {
                            FileId = fileId,
                            UserId = administratorUserId,
                            ParentId = parentId,
                            ActionTypeId = 1002,
                            UserDesc = "جایگزینی مسئول نقش «" + role.Title + "» و انتقال کارتابل باز به " + newHolderTitle + ".",
                            RoleTypeId = adminRole == null ? 0 : adminRole.RoleType,
                            RoleTypeTitle = adminRole == null ? "مدیر سامانه" : adminRole.Title,
                            RoleTypeFinalId = adminRole == null ? 0 : adminRole.RoleTypeFinalId,
                            RoleTypeFinalTitle = adminRole == null ? "مدیر سامانه" : adminRole.Title,
                            RegDate = changeDate
                        });
                    }

                    EnsureAssignmentHistory(formerAssignments.Concat(previousAssignments)
                        .Concat(_context.UserRoles.Where(item => item.UserId == newUserId && item.RoleId == roleId).ToList())
                        .ToList(), administratorUserId, changeDate);
                    CloseActiveRoleHistory(formerUserIds.Concat(new[] { newUserId }).Distinct().ToList(),
                        previousRoleIds.Concat(new[] { roleId }).Distinct().ToList(), changeDate);
                    _context.RoleAssignmentHistories.Add(new RoleAssignmentHistory
                    {
                        RoleId = roleId,
                        UserId = newUserId,
                        StartDate = changeDate,
                        ChangedByUserId = administratorUserId,
                        ChangeReason = "تغییر مسئول جایگاه از طریق مدیریت کاربران"
                    });

                    _context.UserRoles.RemoveRange(formerAssignments);
                    _context.UserRoles.RemoveRange(previousAssignments);
                    var duplicateDestinationAssignments = _context.UserRoles
                        .Where(item => item.UserId == newUserId && item.RoleId == roleId).ToList();
                    _context.UserRoles.RemoveRange(duplicateDestinationAssignments);
                    _context.UserRoles.Add(new UserRole { RoleId = roleId, UserId = newUserId });

                    _context.SaveChanges();
                    transaction.Commit();
                    return new BaseResult(true,
                        $"مسئول جایگاه تغییر کرد؛ {fileIds.Count} درخواست جایگاه مقصد منتقل و " +
                        $"{previousPositionRows.Count} درخواست جایگاه قبلی در کارتابل خالی آن جایگاه محفوظ ماند.");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return new BaseResult(false, "جایگزینی انجام نشد؛ نقش و کارتابل قبلی بدون تغییر باقی ماند.");
                }
            }
        }

        /// <summary>
        /// برای داده‌های قدیمی، نقش مقصد کارتابل را از نقش فعلی گیرنده تکمیل می‌کند.
        /// این کار قبل از جابه‌جایی انجام می‌شود تا درخواست‌های دو جایگاه با هم مخلوط نشوند.
        /// </summary>
        private void BackfillOpenCartableRoles(List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0) return;

            var roleByUser = _context.UserRoles.AsNoTracking()
                .Where(item => userIds.Contains(item.UserId))
                .OrderBy(item => item.UR_Id)
                .ToList()
                .GroupBy(item => item.UserId)
                .ToDictionary(group => group.Key, group => group.Select(item => item.RoleId).FirstOrDefault());
            var rows = _context.Cartables
                .Where(item => !item.IsDone && !item.RcvrRoleId.HasValue && userIds.Contains(item.RcvrUserId))
                .ToList();
            foreach (var row in rows)
            {
                if (roleByUser.TryGetValue(row.RcvrUserId, out var assignedRoleId))
                    row.RcvrRoleId = assignedRoleId;
            }
        }

        /// <summary>
        /// تصدی‌های فعال قبلی را خاتمه می‌دهد؛ رکوردها حذف نمی‌شوند تا سابقه مدیریتی قابل گزارش باشد.
        /// </summary>
        private void CloseActiveRoleHistory(List<int> userIds, List<int> roleIds, DateTime endDate)
        {
            var histories = _context.RoleAssignmentHistories
                .Where(item => !item.EndDate.HasValue &&
                    (userIds.Contains(item.UserId) || roleIds.Contains(item.RoleId)))
                .ToList();
            foreach (var history in histories) history.EndDate = endDate;
        }

        /// <summary>
        /// برای نقش‌های موجود پیش از راه‌اندازی تاریخچه، یک رکورد پایه می‌سازد تا نام متصدی قبلی از دست نرود.
        /// زمان شروع این رکوردها قابل بازیابی نیست و علت آن صریحاً ثبت می‌شود.
        /// </summary>
        private void EnsureAssignmentHistory(List<UserRole> assignments, int administratorUserId, DateTime endDate)
        {
            foreach (var assignment in assignments
                .GroupBy(item => new { item.RoleId, item.UserId })
                .Select(group => group.First()))
            {
                var exists = _context.RoleAssignmentHistories.Any(item =>
                    item.RoleId == assignment.RoleId && item.UserId == assignment.UserId);
                if (exists) continue;

                _context.RoleAssignmentHistories.Add(new RoleAssignmentHistory
                {
                    RoleId = assignment.RoleId,
                    UserId = assignment.UserId,
                    StartDate = endDate,
                    EndDate = endDate,
                    ChangedByUserId = administratorUserId,
                    ChangeReason = "ثبت سابقه اولیه هنگام نخستین جابه‌جایی؛ تاریخ شروع قدیمی در دسترس نیست"
                });
            }
        }

        /// <summary>
        /// نقش را با شناسه دیتابیسی آن دریافت می‌کند.
        /// </summary>
        public Role GetRoleById(int roleId)
        {
            return _context.Roles.Find(roleId);
        }

        /// <summary>
        /// نقش‌هایی را برمی‌گرداند که مدیر جاری مجاز است به یک کاربر اختصاص دهد.
        /// مدیر سامانه همه نقش‌ها، مدیر قرارگاه نقش‌های زیرمجموعه قرارگاه و مدیر یگان
        /// فقط نقش‌های عملیاتی همان سطح را مشاهده می‌کند.
        /// </summary>
        public List<Role> GetRoles(string roleTypeId)
        {
            if (!int.TryParse(roleTypeId, out var currentRoleType))
                return new List<Role>();

            var roles = _context.Roles.AsNoTracking().Where(role => !role.IsDelete);
            if (currentRoleType == SystemRoleTypes.SystemAdministrator)
                return roles.OrderBy(role => role.SortNum).ThenBy(role => role.Title).ToList();

            var assignableTypes = currentRoleType == SystemRoleTypes.HeadquartersAdministrator
                ? SystemRoleTypes.GetHeadquartersAdministratorAssignableTypes().ToArray()
                : SystemRoleTypes.GetUnitAdministratorAssignableTypes().ToArray();

            return roles
                .Where(role => assignableTypes.Contains(role.RoleType))
                .OrderBy(role => role.SortNum)
                .ThenBy(role => role.Title)
                .ToList();

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
            // مدیر سامانه باید بتواند همه منوهای فعال را مدیریت کند؛ نمایش منوی مدیریتی
            // او نباید به وجود یک رکورد تکراری در RolePermission وابسته باشد.
            var isSystemAdministrator = _context.UserRoles
                .Any(userRole => userRole.UserId == userId &&
                                 !userRole.Role.IsDelete &&
                                 userRole.Role.RoleType == SystemRoleTypes.SystemAdministrator);

            if (isSystemAdministrator)
            {
                return _context.Permission
                    .AsNoTracking()
                    .Where(permission => permission.IsActive)
                    .OrderBy(permission => permission.Order)
                    .ThenBy(permission => permission.PermissionTitle)
                    .ToList();
            }

            var query = from p in _context.Permission
                        join rp in _context.RolePermission on p.PermissionId equals rp.PermissionId
                        join r in _context.Roles on rp.RoleId equals r.RoleId
                        join ur in _context.UserRoles on r.RoleId equals ur.RoleId
                        where ur.UserId == userId && p.IsActive && !r.IsDelete
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

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.User;
using VisitorManagment.DataLayer.Entities.VisitorManagment;

namespace VisitorManagment.Core.Services
{
    public class WorkFlowService : IWorkFlowService
    {

        private readonly VisitorManagmentContext _context;
        public WorkFlowService(VisitorManagmentContext context)
        {
            _context = context;

        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<Role> GetrcvrList(int roleId)
        {
            var rcvrList = _context.WorkFlows.Where(c => c.SndrRoleId == roleId).Select(c => c.RcvrRoleId).ToList();
            return _context.UserRoles.Include(u => u.Role).Where(u => rcvrList.Contains(u.RoleId))
                .Select(u => new Role()
                {
                    Title = u.Role.Title,
                }).ToList();
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<Users> GetRecieverUserListByFileId(int fileId, int userId)
        {
            var users = _context.Hameshes.Include(h => h.User)
                .Where(h => h.FileId == fileId)
                .Select(h => h.User)
                .OrderBy(h => h.Id).Distinct().ToList();

            #region تغییرات برای درکتر صدر ف ابهاد
            //1617 => ر ابهاد
            if (userId == 1617)
            {
                if (users.AsQueryable().Select(x => x.Id).Contains(575))
                {

                }
                else
                {
                    var karshenash = _context.Users.Where(x => x.Id == 575).FirstOrDefault();
                    users.Add(karshenash);
                }

            }
            #endregion


            return users;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public Users GetRecieverUserByFileId(int fileId)
        {

            var users = _context.Hameshes.Include(h => h.User).Where(h => h.FileId == fileId).Select(h => h.User).Distinct().SingleOrDefault();
            return users;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<Users> GetRecieverUserListBySndrRoleId(int roleId, int unitDutyCode, int unitCode, int codeGha, int fileId, int roleTypeId)
        {
            var FileId = fileId;

            #region از جدول دسترسی ها بررسی میکنیم کدام نقش به کدام نقش ها دسترسی داره

            var rcvrList = _context.WorkFlows
                                .Where(w => w.SndrRoleId == roleId);


            var rcvrRoleIdList = rcvrList.Select(c => c.RcvrRoleId)
                .ToList();


            return _context.UserRoles.Include(u => u.User).Include(x=>x.Role)
    .Where(u => rcvrRoleIdList.Contains(u.RoleId) && u.Role.IsDelete==false )
    .OrderBy(x => x.Role.SortNumMoavenat)
     .Select(u => new Users()
     {
         ActiveCode = u.User.ActiveCode,
         LastName = u.User.LastName,
         FirstName = u.User.FirstName,
         RankTitle = u.User.RankTitle,
         BranchTitle = u.User.BranchTitle,
         UserName = u.User.UserName,
         UnitDutyTitle = u.User.UnitDutyTitle,
         JobDes = u.Role.Title,
         Id = u.UserId
     }).ToList();
            #endregion

        }



        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public List<Users> GetRecieverFarmandehiNezajaList(int roleId)
        {

            var rcvrList = _context.WorkFlows.Where(w => w.SndrRoleId == roleId).Select(c => c.RcvrRoleId).ToList();

            var res = _context.UserRoles.Include(u => u.User).Where(u => rcvrList.Contains(u.RoleId))
              .Select(u => new Users()
              {
                  ActiveCode = u.User.ActiveCode,
                  LastName = u.User.LastName,
                  FirstName = u.User.FirstName,
                  RankTitle = u.User.RankTitle,
                  BranchTitle = u.User.BranchTitle,
                  UserName = u.User.UserName,
                  UnitDutyTitle = u.User.UnitDutyTitle,
                  JobDes = u.User.JobDes,
                  Id = u.UserId
              }).OrderBy(u => u.RankTitle).ToList();

            return res;
        }

        /// <summary>
        /// اطلاعات موردنیاز را دریافت می‌کند.
        /// </summary>
        public ListUserForAoudatToCartable GetRecieverUserListByFileIdDto(int fileId)
        {
            var users = _context.Hameshes.Include(h => h.User)
                .Where(h => h.FileId == fileId).Select(h => h.User).OrderBy(h => h.Id).Distinct().ToList();

            var list = new ListUserForAoudatToCartable();

            list.Users = users.Select(x => new UserForSendToCartable
            {
                Id = x.Id,
                PersonalCode = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                BranchTitle = x.BranchTitle,
                JobDes = x.JobDes,
                RankTitle = x.RankTitle,
                UnitDutyCode = x.UnitDutyCode,
                UnitDutyTitle = x.UnitDutyTitle,
                UnitTitle = x.UnitTitle,
                CodGhaTitle = x.CodGhaTitle,
                CodGha = x.CodGha,
            }).ToList();


            return list;
        }

        /// <summary>
        /// لیست کاربرانی که یک نقش به آنها دسترسی دارد
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<WorkFlowViewModel> GetReciverList(int roleId)
        {
            var rcvrList = _context.WorkFlows
                .Include(x => x.Role)
                .ThenInclude(x => x.UserRoles)
                .ThenInclude(x => x.User)
                .Where(c => c.SndrRoleId == roleId)
                .ToList();

            var workFlow = new List<WorkFlowViewModel>();

            workFlow = rcvrList.Select(x => new WorkFlowViewModel
            {
                Id = x.Role.RoleId,
                Title = x.Role.Title,
                RankTitle = x.Role.UserRoles.Select(x => x.User.RankTitle).FirstOrDefault(),
                Fname = x.Role.UserRoles.Select(x => x.User.FirstName).FirstOrDefault(),
                Lname = x.Role.UserRoles.Select(x => x.User.LastName).FirstOrDefault(),
                SenderRoleId = x.SndrRoleId,
                ReciverRoleId = x.RcvrRoleId,
                PersonalCode = x.Role.UserRoles.Select(x => x.User.UserName).FirstOrDefault(),

            }).ToList();


            return workFlow;
        }

        /// <summary>
        /// لیست کاربرانی که یک نقش به آنها دسترسی ندارد
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<WorkFlowViewModel> GetUnAccessList(int roleId)
        {
            var reciverList = _context.WorkFlows.Where(x => x.SndrRoleId == roleId).Select(x => x.RcvrRoleId).ToList();

            var rcvrList = _context.Roles
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.User)
                .Where(c => (!reciverList.Contains(c.RoleId)))
                .ToList();

            var workFlow = new List<WorkFlowViewModel>();

            workFlow = rcvrList.Select(x => new WorkFlowViewModel
            {
                Id = x.RoleId,
                Title = x.Title,
                RankTitle = x.UserRoles.Select(x => x.User.RankTitle).FirstOrDefault(),
                Fname = x.UserRoles.Select(x => x.User.FirstName).FirstOrDefault(),
                Lname = x.UserRoles.Select(x => x.User.LastName).FirstOrDefault(),
                SenderRoleId = 0,
                ReciverRoleId = 0,
                PersonalCode = x.UserRoles.Select(x => x.User.UserName).FirstOrDefault(),

            }).ToList();


            return workFlow;
        }

        /// <summary>
        /// اعطای دسترسی هر نقش به چه نقش هایی دسترسی دارد
        /// </summary>
        /// <param name="reciverRoleList"></param>
        /// <param name="roleId"></param>
        /// <param name="addUserId"></param>
        public void AddAccessToRole(List<int> reciverRoleList, int roleId, int addUserId)
        {
            if (roleId <= 0 || reciverRoleList == null || reciverRoleList.Count == 0)
            {
                return;
            }

            var requestedRoleIds = reciverRoleList.Where(id => id > 0 && id != roleId).Distinct().ToList();
            var existingRoleIds = _context.WorkFlows
                .Where(item => item.SndrRoleId == roleId && requestedRoleIds.Contains(item.RcvrRoleId))
                .Select(item => item.RcvrRoleId)
                .ToList();

            var workFlows = requestedRoleIds
                .Where(id => !existingRoleIds.Contains(id))
                .Select(id => new WorkFlow
                {
                    RcvrRoleId = id,
                    SndrRoleId = roleId,
                    RegUserId = addUserId,
                    RegDate = DateTime.Now
                })
                .ToList();

            if (workFlows.Count == 0)
            {
                return;
            }

            _context.WorkFlows.AddRange(workFlows);
            _context.SaveChanges();
        }

        /// <summary>
        /// اطلاعات مشخص‌شده را حذف می‌کند.
        /// </summary>
        public void RemoveAccessToRole(List<int> reciverRoleList, int roleId, int addUserId)
        {
            if (roleId <= 0 || reciverRoleList == null || reciverRoleList.Count == 0)
            {
                return;
            }

            var requestedRoleIds = reciverRoleList.Where(id => id > 0).Distinct().ToList();
            var workFlows = _context.WorkFlows
                .Where(item => item.SndrRoleId == roleId && requestedRoleIds.Contains(item.RcvrRoleId))
                .ToList();

            if (workFlows.Count == 0)
            {
                return;
            }

            _context.WorkFlows.RemoveRange(workFlows);
            _context.SaveChanges();
        }
    }
}

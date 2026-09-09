using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.Base;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.VisitorManagment;
using System.Collections.Generic;

namespace VisitorManagment.Core.Services
{
    /// <summary>
    /// درخواست‌ها و گردش کامل هامش‌ها را برای جست‌وجوی مدیریتی آماده می‌کند.
    /// Query درخواست و هامش جدا اجرا می‌شود تا Join باعث تکرار درخواست‌ها نشود.
    /// </summary>
    public class AdminVisitorRequestService : IAdminVisitorRequestService
    {
        private readonly VisitorManagmentContext _context;

        public AdminVisitorRequestService(VisitorManagmentContext context)
        {
            _context = context;
        }

        public AdminVisitorRequestSearchViewModel GetRequestsByPersonalCode(string personalCode)
        {
            if (string.IsNullOrWhiteSpace(personalCode)) return null;
            var normalizedCode = personalCode.Trim();

            var person = _context.Personals.AsNoTracking()
                .Where(item => item.PersonalCode == normalizedCode)
                .Select(item => new AdminVisitorRequestSearchViewModel
                {
                    PersonalCode = item.PersonalCode,
                    FullName = (item.RankTitle ?? "") + " " + (item.FirstName ?? "") + " " + (item.LastName ?? ""),
                    RankTitle = item.RankTitle,
                    UnitTitle = item.UnitTitle,
                    UnitDutyTitle = item.UnitDutyTitle,
                    Phone = item.Phone
                })
                .FirstOrDefault();

            var requests = _context.Files.AsNoTracking()
                .Where(file => !file.IsDelete &&
                               (file.PersonalCode == normalizedCode || file.Personal.PersonalCode == normalizedCode))
                .OrderByDescending(file => file.RegDate)
                .Select(file => new AdminVisitorRequestItemViewModel
                {
                    Id = file.Id,
                    RequestSubjectTitle = file.RequestSubject == null ? "—" : file.RequestSubject.Title,
                    FileTypeTitle = file.FileType == null ? "—" : file.FileType.Title,
                    PriorityTitle = file.Priority == null ? "—" : file.Priority.Title,
                    FileStatusTitle = file.FileStatus == null ? "—" : file.FileStatus.Title,
                    RequestDescription = file.RequestDescription,
                    ProblemDescription = file.ProblemDescription,
                    IsArchived = file.IsArchived,
                    IsFinished = file.IsFinished,
                    RegDate = file.RegDate
                })
                .ToList();

            if (person == null && requests.Count == 0) return null;

            if (person == null)
            {
                var firstFile = _context.Files.AsNoTracking()
                    .Where(file => !file.IsDelete && file.PersonalCode == normalizedCode)
                    .OrderByDescending(file => file.RegDate)
                    .Select(file => new
                    {
                        file.PersonalCode,
                        file.RankTitle,
                        file.FirstName,
                        file.LastName,
                        file.UnitTitle,
                        file.UnitDutyTitle,
                        file.Phone
                    }).FirstOrDefault();

                person = new AdminVisitorRequestSearchViewModel
                {
                    PersonalCode = normalizedCode,
                    FullName = firstFile == null ? normalizedCode :
                        ((firstFile.RankTitle ?? "") + " " + (firstFile.FirstName ?? "") + " " + (firstFile.LastName ?? "")),
                    RankTitle = firstFile == null ? null : firstFile.RankTitle,
                    UnitTitle = firstFile == null ? null : firstFile.UnitTitle,
                    UnitDutyTitle = firstFile == null ? null : firstFile.UnitDutyTitle,
                    Phone = firstFile == null ? null : firstFile.Phone
                };
            }

            var requestIds = requests.Select(item => item.Id).ToArray();
            // اتصال هامش‌ها بر اساس شناسه درخواست، بدون ایجاد ردیف تکراری برای درخواست.
            if (requestIds.Length > 0)
            {
                var emptyCounts = _context.Hameshes.AsNoTracking()
                    .Where(item => requestIds.Contains(item.FileId) &&
                                   (item.UserDesc == null || item.UserDesc.Trim() == ""))
                    .GroupBy(item => item.FileId)
                    .Select(group => new { FileId = group.Key, Count = group.Count() })
                    .ToDictionary(item => item.FileId, item => item.Count);

                var hameshRows = _context.Hameshes.AsNoTracking()
                    .Where(item => requestIds.Contains(item.FileId) &&
                                   item.UserDesc != null && item.UserDesc.Trim() != "")
                    .OrderBy(item => item.RegDate)
                    .Select(item => new
                    {
                        item.FileId,
                        Item = new AdminVisitorHameshItemViewModel
                        {
                            Id = item.Id,
                            RegistrarFullName = item.User == null ? "—" :
                                ((item.User.RankTitle ?? "") + " " + (item.User.FirstName ?? "") + " " + (item.User.LastName ?? "")),
                            RegistrarPersonalCode = item.User == null ? "—" : item.User.UserName,
                            RoleTitle = item.RoleTypeTitle,
                            ActionTitle = item.ActionType == null ? "—" : item.ActionType.Title,
                            Description = item.UserDesc,
                            RegDate = item.RegDate
                        }
                    }).ToList();

                var lookup = hameshRows.ToLookup(item => item.FileId, item => item.Item);
                var currentOwners = (from cartable in _context.Cartables.AsNoTracking()
                                     join user in _context.Users.AsNoTracking() on cartable.RcvrUserId equals user.Id
                                     where requestIds.Contains(cartable.FileId) && !cartable.IsDone
                                     select new
                                     {
                                         cartable.FileId,
                                         Title = (user.RankTitle ?? "") + " " + (user.FirstName ?? "") + " " +
                                                 (user.LastName ?? "") + " (" + (user.UserName ?? "") + ")"
                                     }).ToList().ToLookup(item => item.FileId, item => item.Title.Trim());

                foreach (var request in requests)
                {
                    request.Hameshes = lookup[request.Id].ToList();
                    request.CurrentOwners = currentOwners[request.Id].Distinct().ToList();
                    request.EmptyHameshCount = emptyCounts.ContainsKey(request.Id) ? emptyCounts[request.Id] : 0;
                }
            }

            person.Requests = requests;
            return person;
        }

        /// <summary>
        /// فهرست کاربران فعال را برای انتخاب مقصد اصلاح مسیر برمی‌گرداند.
        /// </summary>
        public List<WorkflowReceiverViewModel> GetActiveReceivers()
        {
            return _context.Users.AsNoTracking()
                .Where(user => user.IsActive && !user.IsDelete)
                .OrderBy(user => user.FirstName).ThenBy(user => user.LastName)
                .Select(user => new WorkflowReceiverViewModel
                {
                    Id = user.Id,
                    DisplayTitle = (user.RankTitle ?? "") + " " + user.FirstName + " " + user.LastName +
                                   " | " + user.UserName + " | " + (user.UnitDutyTitle ?? user.UnitTitle)
                }).ToList();
        }

        /// <summary>
        /// درخواست گیرکرده را اتمیک به کاربر جدید منتقل می‌کند و علت اصلاح را در تاریخچه هامش ثبت می‌کند.
        /// </summary>
        public BaseResult TransferRequest(int fileId, int receiverUserId, int administratorUserId, string reason)
        {
            if (fileId <= 0 || receiverUserId <= 0 || administratorUserId <= 0)
                return new BaseResult(false, "اطلاعات انتقال کامل نیست.");
            if (string.IsNullOrWhiteSpace(reason))
                return new BaseResult(false, "ثبت علت اصلاح مسیر الزامی است.");

            var fileExists = _context.Files.Any(file => file.Id == fileId && !file.IsDelete);
            var receiver = _context.Users.Include(user => user.UserRoles).ThenInclude(userRole => userRole.Role)
                .FirstOrDefault(user => user.Id == receiverUserId && user.IsActive && !user.IsDelete);
            var administrator = _context.Users.Include(user => user.UserRoles).ThenInclude(userRole => userRole.Role)
                .FirstOrDefault(user => user.Id == administratorUserId && user.IsActive && !user.IsDelete);
            if (!fileExists) return new BaseResult(false, "درخواست موردنظر یافت نشد.");
            if (receiver == null) return new BaseResult(false, "گیرنده فعال و معتبری انتخاب نشده است.");
            if (administrator == null) return new BaseResult(false, "هویت مدیر سامانه معتبر نیست.");

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var row in _context.Cartables.Where(item => item.FileId == fileId && !item.IsDone).ToList())
                        row.IsDone = true;

                    _context.Cartables.Add(new Cartable
                    {
                        FileId = fileId,
                        SndrUserId = administratorUserId,
                        RcvrUserId = receiverUserId,
                        StateCd = 0,
                        IsView = false,
                        IsDone = false,
                        RegDate = DateTime.Now
                    });

                    var adminRole = administrator.UserRoles.Select(item => item.Role).FirstOrDefault();
                    var receiverTitle = (receiver.RankTitle + " " + receiver.FirstName + " " + receiver.LastName).Trim();
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
                        UserDesc = "اصلاح مسیر توسط مدیر سامانه؛ انتقال به " + receiverTitle + ". علت: " + reason.Trim(),
                        RoleTypeId = adminRole == null ? 0 : adminRole.RoleType,
                        RoleTypeTitle = adminRole == null ? "مدیر سامانه" : adminRole.Title,
                        RoleTypeFinalId = adminRole == null ? 0 : adminRole.RoleTypeFinalId,
                        RoleTypeFinalTitle = adminRole == null ? "مدیر سامانه" : adminRole.Title,
                        RegDate = DateTime.Now
                    });

                    _context.SaveChanges();
                    transaction.Commit();
                    return new BaseResult(true, "درخواست با موفقیت به کارتابل مقصد منتقل شد.");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return new BaseResult(false, "انتقال انجام نشد؛ اطلاعات قبلی بدون تغییر باقی ماند.");
                }
            }
        }
    }
}

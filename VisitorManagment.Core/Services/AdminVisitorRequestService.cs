using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;

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
                var hameshRows = _context.Hameshes.AsNoTracking()
                    .Where(item => requestIds.Contains(item.FileId))
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
                foreach (var request in requests)
                    request.Hameshes = lookup[request.Id].ToList();
            }

            person.Requests = requests;
            return person;
        }
    }
}

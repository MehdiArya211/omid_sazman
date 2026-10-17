using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VisitorManagment.Core.DTOs.Inspection;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.Inspection;

namespace VisitorManagment.Core.Services
{
    /// <summary>
    /// تمام قواعد ماژول بازرسی را در یک نقطه نگه می‌دارد تا صفحات وب مستقیماً با دیتابیس یا سرویس پرسنلی کار نکنند.
    /// </summary>
    public class InspectionService : IInspectionService
    {
        private readonly VisitorManagmentContext _context;
        private readonly IWebApiService _webApiService;

        public InspectionService(VisitorManagmentContext context, IWebApiService webApiService)
        {
            _context = context;
            _webApiService = webApiService;
        }

        public InspectionPersonLookupViewModel LookupPerson(string personalCode)
        {
            var result = _webApiService.GetPersonalByPersonalNo(NormalizeDigits(personalCode));
            if (result?.IsSuccess != true || result.Data == null)
                return new InspectionPersonLookupViewModel { Message = result?.Message ?? "اطلاعات فرد از سرویس پرسنلی دریافت نشد." };

            var person = result.Data;
            return new InspectionPersonLookupViewModel
            {
                IsSuccess = true,
                Message = "اطلاعات فرد دریافت شد.",
                PersonalCode = NormalizeDigits(person.PersonalCode),
                FullName = $"{person.FirstName} {person.LastName}".Trim(),
                RankTitle = person.RankTitle,
                UnitTitle = person.UnitTitle,
                UnitDutyTitle = person.UnitDutyTitle,
                JobDescription = person.JobDes
            };
        }

        public InspectionOperationResult RegisterPerson(string personalCode, int userId)
        {
            personalCode = NormalizeDigits(personalCode);
            if (string.IsNullOrWhiteSpace(personalCode)) return InspectionOperationResult.Failure("کد پرسنلی معتبر نیست.");
            if (_context.InspectionPeople.Any(x => x.PersonalCode == personalCode && !x.IsDeleted))
                return InspectionOperationResult.Failure("این فرد قبلاً در مدیریت بازرسی ثبت شده است.");

            var apiResult = _webApiService.GetPersonalByPersonalNo(personalCode);
            if (apiResult?.IsSuccess != true || apiResult.Data == null)
                return InspectionOperationResult.Failure(apiResult?.Message ?? "اطلاعات فرد از سرویس پرسنلی دریافت نشد.");

            var source = apiResult.Data;
            if (string.IsNullOrWhiteSpace(source.FirstName) || string.IsNullOrWhiteSpace(source.LastName))
                return InspectionOperationResult.Failure("نام و نام خانوادگی فرد در پاسخ سرویس پرسنلی کامل نیست.");
            if (!source.UnitCode.HasValue)
                return InspectionOperationResult.Failure("یگان این فرد در سرویس پرسنلی مشخص نشده است.");

            _context.InspectionPeople.Add(new InspectionPerson
            {
                PersonalCode = personalCode, FirstName = source.FirstName?.Trim(), LastName = source.LastName?.Trim(),
                RankCode = source.RankCode, RankTitle = source.RankTitle?.Trim(), BranchCode = source.BranchCode,
                BranchTitle = source.BranchTitle?.Trim(), UnitCode = source.UnitCode, UnitTitle = source.UnitTitle?.Trim(),
                UnitDutyCode = source.UnitDutyCode, UnitDutyTitle = source.UnitDutyTitle?.Trim(), JobDescription = source.JobDes?.Trim(),
                RegionalCommandCode = source.CodGha, RegionalCommandTitle = source.CodGhaTitle?.Trim(),
                RegisteredAt = DateTime.Now, RegisteredByUserId = userId, IsActive = true
            });
            try { _context.SaveChanges(); }
            catch (DbUpdateException)
            {
                return InspectionOperationResult.Failure("ثبت انجام نشد؛ احتمالاً این کد پرسنلی قبلاً ثبت شده است.");
            }
            return InspectionOperationResult.Success("فرد با موفقیت ثبت شد.");
        }

        public IReadOnlyList<InspectionPerson> GetPeople() => _context.InspectionPeople.AsNoTracking()
            .Where(x => !x.IsDeleted).OrderBy(x => x.UnitTitle).ThenBy(x => x.LastName).ToList();

        public IReadOnlyList<InspectionUnitViewModel> GetRegisteredUnits() => _context.InspectionPeople.AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive && x.UnitCode.HasValue)
            .GroupBy(x => new { UnitCode = x.UnitCode.Value, x.UnitTitle })
            .Select(group => new InspectionUnitViewModel { UnitCode = group.Key.UnitCode, UnitTitle = group.Key.UnitTitle, PeopleCount = group.Count() })
            .OrderBy(x => x.UnitTitle).ToList();

        public InspectionOperationResult CreateMeeting(InspectionMeetingCreateViewModel model, int userId)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Title)) return InspectionOperationResult.Failure("اطلاعات جلسه کامل نیست.");
            var selectedCodes = (model.UnitCodes ?? new List<int>()).Where(code => code > 0).Distinct().ToList();
            if (!selectedCodes.Any()) return InspectionOperationResult.Failure("حداقل یک یگان را انتخاب کنید.");
            var units = GetRegisteredUnits().Where(unit => selectedCodes.Contains(unit.UnitCode)).ToList();
            if (units.Count != selectedCodes.Count) return InspectionOperationResult.Failure("یک یا چند یگان انتخاب‌شده معتبر نیست.");
            var selectedPersonIds = (model.PersonIds ?? new List<int>()).Where(id => id > 0).Distinct().ToList();
            if (!selectedPersonIds.Any()) return InspectionOperationResult.Failure("حداقل یک نفر را برای جلسه انتخاب کنید.");
            var people = _context.InspectionPeople.Where(person => selectedPersonIds.Contains(person.Id) && person.IsActive && !person.IsDeleted).ToList();
            if (people.Count != selectedPersonIds.Count || people.Any(person => !person.UnitCode.HasValue || !selectedCodes.Contains(person.UnitCode.Value)))
                return InspectionOperationResult.Failure("افراد انتخاب‌شده باید فعال و متعلق به یگان‌های انتخاب‌شده باشند.");

            using var transaction = _context.Database.BeginTransaction();
            var meeting = new InspectionMeeting
            {
                Title = model.Title.Trim(), Description = model.Description?.Trim(), MeetingDate = NormalizeDigits(model.MeetingDate),
                StartTime = NormalizeDigits(model.StartTime), Status = model.Publish ? InspectionMeetingStatus.Published : InspectionMeetingStatus.Draft,
                RegisteredAt = DateTime.Now, RegisteredByUserId = userId, IsActive = true
            };
            _context.InspectionMeetings.Add(meeting);
            _context.SaveChanges();
            _context.InspectionMeetingUnits.AddRange(units.Select(unit => new InspectionMeetingUnit
            {
                InspectionMeetingId = meeting.Id, UnitCode = unit.UnitCode, UnitTitle = unit.UnitTitle,
                RegisteredAt = DateTime.Now, RegisteredByUserId = userId
            }));
            _context.InspectionMeetingPeople.AddRange(people.Select(person => new InspectionMeetingPerson
            {
                InspectionMeetingId = meeting.Id, InspectionPersonId = person.Id,
                RegisteredAt = DateTime.Now, RegisteredByUserId = userId
            }));
            _context.SaveChanges();
            transaction.Commit();
            return InspectionOperationResult.Success("جلسه و یگان‌های دعوت‌شده با موفقیت ثبت شدند.");
        }

        public IReadOnlyList<InspectionMeeting> GetMeetings() => _context.InspectionMeetings.AsNoTracking().Include(x => x.Units).Include(x => x.People)
            .Where(x => !x.IsDeleted).OrderByDescending(x => x.Id).ToList();

        public IReadOnlyList<InspectionMeeting> GetAvailableMeetings(int unitCode) => _context.InspectionMeetings.AsNoTracking().Include(x => x.Units)
            .Where(x => !x.IsDeleted && x.IsActive && (x.Status == InspectionMeetingStatus.Published || x.Status == InspectionMeetingStatus.InProgress)
                && x.Units.Any(unit => unit.UnitCode == unitCode))
            .OrderBy(x => x.MeetingDate).ThenBy(x => x.StartTime).ToList();

        public bool CanUnitEnterMeeting(int meetingId, int unitCode) => _context.InspectionMeetings.AsNoTracking()
            .Any(x => x.Id == meetingId && !x.IsDeleted && x.IsActive
                && (x.Status == InspectionMeetingStatus.Published || x.Status == InspectionMeetingStatus.InProgress)
                && x.Units.Any(unit => unit.UnitCode == unitCode));

        public IReadOnlyList<InspectionParticipationSummaryViewModel> GetParticipationHistory(int? meetingId = null, string search = null)
        {
            var query = _context.InspectionMeetingPeople.AsNoTracking().Include(x => x.Meeting).Include(x => x.Person).AsQueryable();
            if (meetingId.HasValue) query = query.Where(x => x.InspectionMeetingId == meetingId.Value);
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(x => x.Person.PersonalCode.Contains(search) || (x.Person.FirstName + " " + x.Person.LastName).Contains(search) || x.Person.UnitTitle.Contains(search));
            }
            var rows = query.OrderByDescending(x => x.Meeting.MeetingDate).ToList();
            return rows.GroupBy(x => new { x.Person.PersonalCode, x.Person.FirstName, x.Person.LastName, x.Person.RankTitle, x.Person.UnitCode, x.Person.UnitTitle })
                .Select(group => new InspectionParticipationSummaryViewModel
                {
                    PersonalCode = group.Key.PersonalCode, FullName = (group.Key.FirstName + " " + group.Key.LastName).Trim(), RankTitle = group.Key.RankTitle,
                    UnitCode = group.Key.UnitCode ?? 0, UnitTitle = group.Key.UnitTitle,
                    MeetingsCount = group.Select(x => x.InspectionMeetingId).Distinct().Count(),
                    Details = group.Select(x => new InspectionParticipationDetailViewModel { MeetingId = x.InspectionMeetingId, MeetingTitle = x.Meeting.Title, MeetingDate = x.Meeting.MeetingDate, StartTime = x.Meeting.StartTime, RegisteredAt = x.RegisteredAt }).ToList()
                }).OrderByDescending(x => x.MeetingsCount).ThenBy(x => x.FullName).ToList();
        }

        private static string NormalizeDigits(string value) => value?.Trim()
            .Replace('۰', '0').Replace('۱', '1').Replace('۲', '2').Replace('۳', '3').Replace('۴', '4')
            .Replace('۵', '5').Replace('۶', '6').Replace('۷', '7').Replace('۸', '8').Replace('۹', '9')
            .Replace('٠', '0').Replace('١', '1').Replace('٢', '2').Replace('٣', '3').Replace('٤', '4')
            .Replace('٥', '5').Replace('٦', '6').Replace('٧', '7').Replace('٨', '8').Replace('٩', '9');
    }
}

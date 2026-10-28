using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.Web.Pages.Admin.Users
{
    [Authorize]
    public class BulkImportModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IWebApiService _webApiService;
        private readonly VisitorManagmentContext _context;

        public BulkImportModel(IUserService userService, IPermissionService permissionService,
            IWebApiService webApiService, VisitorManagmentContext context)
        {
            _userService = userService;
            _permissionService = permissionService;
            _webApiService = webApiService;
            _context = context;
        }

        [BindProperty]
        public List<BulkUserImportRowViewModel> Rows { get; set; } = new List<BulkUserImportRowViewModel>();

        [BindProperty]
        public int DefaultRoleId { get; set; }

        public BulkUserImportResultViewModel Result { get; set; }
        public List<OrganViewModelDto> Units { get; private set; } = new List<OrganViewModelDto>();
        public bool PreviewAttempted { get; private set; }
        public string PreviewError { get; private set; }
        public string PreviewFileName { get; private set; }

        public IActionResult OnGet()
        {
            if (!TryPrepareLookups()) return Forbid();
            return Page();
        }

        /// <summary>
        /// فایل را فقط تحلیل می‌کند و نتیجه را برای اصلاح و تأیید مدیر نمایش می‌دهد.
        /// </summary>
        public IActionResult OnPostPreview(IFormFile importFile)
        {
            if (!TryPrepareLookups()) return Forbid();

            PreviewAttempted = true;
            PreviewFileName = importFile?.FileName;

            try
            {
                Rows = BulkUserImportParser.Parse(importFile);
                EnrichAndValidatePreview(Rows);
                foreach (var row in Rows.Where(row => row.RoleId <= 0)) row.RoleId = DefaultRoleId;

                if (!Rows.Any())
                    PreviewError = "هیچ ردیف قابل استفاده‌ای در فایل شناسایی نشد.";
            }
            catch (Exception exception)
            {
                PreviewError = GetSafeErrorMessage(exception);
                Rows = new List<BulkUserImportRowViewModel>();
            }

            return Page();
        }

        /// <summary>
        /// ردیف‌های تأییدشده را مستقل از یکدیگر ثبت می‌کند تا خطای یک نفر مانع ثبت سایر افراد نشود.
        /// </summary>
        public IActionResult OnPostImport()
        {
            if (!TryPrepareLookups()) return Forbid();

            PreviewAttempted = true;
            Result = new BulkUserImportResultViewModel();
            var allowedRoleIds = GetAllowedRoles().Select(role => role.RoleId).ToHashSet();
            var duplicateCodes = Rows.Where(row => row.Selected)
                .GroupBy(row => BulkUserImportParser.NormalizeDigits(row.PersonalCode))
                .Where(group => !string.IsNullOrWhiteSpace(group.Key) && group.Count() > 1)
                .Select(group => group.Key).ToHashSet();

            foreach (var row in Rows)
            {
                row.PersonalCode = BulkUserImportParser.NormalizeDigits(row.PersonalCode);
                row.FirstName = BulkUserImportParser.NormalizeText(row.FirstName);
                row.LastName = BulkUserImportParser.NormalizeText(row.LastName);
                row.UnitTitle = BulkUserImportParser.NormalizeText(row.UnitTitle);

                if (!row.Selected)
                {
                    row.Status = "صرف‌نظر شد";
                    Result.SkippedCount++;
                    Result.Rows.Add(row);
                    continue;
                }

                var error = ValidateImportRow(row, allowedRoleIds, duplicateCodes);
                if (!string.IsNullOrEmpty(error))
                {
                    MarkFailed(row, error);
                    continue;
                }

                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    var model = new CreateUserViewModel
                    {
                        PersonalCode = row.PersonalCode,
                        Password = row.PersonalCode,
                        FirstName = row.FirstName,
                        LastName = row.LastName,
                        RankTitle = row.RankTitle,
                        UnitCode = row.UnitCode.Value,
                        UnitTitle = row.UnitTitle,
                        UnitDutyCode = row.UnitDutyCode,
                        UnitDutyTitle = row.UnitDutyTitle,
                        CodGha = row.CodGha,
                        CodGhaTitle = row.CodGhaTitle,
                        AddUserId = int.Parse(User.FindFirst("Id").Value)
                    };

                    var userId = _userService.AddUserFromAdmin(model);
                    _permissionService.AddRolesToUser(row.RoleId, userId);
                    _userService.ResetPasswordToPersonnelCode(userId);
                    transaction.Commit();

                    row.Imported = true;
                    row.Selected = false;
                    row.Status = "ثبت شد";
                    row.ErrorMessage = string.Empty;
                    Result.ImportedCount++;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    MarkFailed(row, "خطا هنگام ثبت: " + GetSafeErrorMessage(exception));
                    continue;
                }

                Result.Rows.Add(row);
            }

            Rows = Result.Rows;
            // TagHelperها در پاسخ POST ابتدا ModelState را می‌خوانند؛ پاک‌سازی باعث نمایش وضعیت واقعی ثبت می‌شود.
            ModelState.Clear();
            return Page();
        }

        private void EnrichAndValidatePreview(IEnumerable<BulkUserImportRowViewModel> rows)
        {
            var existingUsers = _context.Users.AsNoTracking().Select(user => user.UserName).ToHashSet();
            foreach (var row in rows)
            {
                row.PersonalCode = BulkUserImportParser.NormalizeDigits(row.PersonalCode);
                if (string.IsNullOrWhiteSpace(row.PersonalCode))
                {
                    row.Status = "ناقص";
                    row.ErrorMessage = "کد پرسنلی تشخیص داده نشد؛ آن را دستی وارد کنید.";
                    continue;
                }

                if (existingUsers.Contains(row.PersonalCode))
                {
                    row.Selected = false;
                    row.Status = "تکراری";
                    row.ErrorMessage = "این کد پرسنلی قبلاً در سامانه ثبت شده است.";
                    continue;
                }

                TryFillFromPersonnelService(row);
                if (!row.UnitMatched) TryMatchUnitByTitle(row);

                if (!row.UnitMatched)
                {
                    row.Status = "یگان نامشخص";
                    row.ErrorMessage = "یگان تشخیص داده نشد؛ یگان صحیح را از فهرست انتخاب کنید.";
                }
                else if (string.IsNullOrWhiteSpace(row.FirstName) || string.IsNullOrWhiteSpace(row.LastName))
                {
                    row.Status = "نیازمند اصلاح";
                    row.ErrorMessage = "نام یا نام خانوادگی ناقص است و باید دستی تکمیل شود.";
                }
                else
                {
                    row.Status = "آماده ثبت";
                    row.ErrorMessage = string.Empty;
                }
            }
        }

        private void TryFillFromPersonnelService(BulkUserImportRowViewModel row)
        {
            try
            {
                var response = _webApiService.GetPersonalByPersonalNo(row.PersonalCode);
                var person = response?.Data;
                if (person == null || string.Equals(person.Respond, "ERROR Service", StringComparison.OrdinalIgnoreCase)) return;

                row.FirstName = person.FirstName;
                row.LastName = person.LastName;
                row.RankTitle = person.RankTitle;
                row.UnitCode = person.UnitCode;
                row.UnitTitle = person.UnitTitle;
                row.UnitDutyCode = person.UnitDutyCode;
                row.UnitDutyTitle = person.UnitDutyTitle;
                row.CodGha = person.CodGha;
                row.CodGhaTitle = person.CodGhaTitle;
                row.PersonnelVerified = true;
                row.UnitMatched = person.UnitCode.HasValue && person.UnitCode.Value > 0;
            }
            catch
            {
                // قطع سرویس پرسنلی مانع نمایش فایل نمی‌شود؛ مدیر می‌تواند ردیف را دستی تکمیل کند.
            }
        }

        private void TryMatchUnitByTitle(BulkUserImportRowViewModel row)
        {
            var title = BulkUserImportParser.NormalizeText(row.UnitTitle);
            if (string.IsNullOrEmpty(title)) return;

            var exact = Units.Where(unit => BulkUserImportParser.NormalizeText(unit.Title) == title).ToList();
            var matches = exact.Any() ? exact : Units.Where(unit =>
                BulkUserImportParser.NormalizeText(unit.Title).Contains(title) ||
                title.Contains(BulkUserImportParser.NormalizeText(unit.Title))).ToList();

            if (matches.Count != 1) return;
            row.UnitCode = matches[0].UnitCode > 0 ? matches[0].UnitCode : matches[0].Id;
            row.UnitTitle = matches[0].Title;
            row.UnitMatched = row.UnitCode > 0;
        }

        private string ValidateImportRow(BulkUserImportRowViewModel row, HashSet<int> allowedRoleIds,
            HashSet<string> duplicateCodes)
        {
            if (string.IsNullOrWhiteSpace(row.PersonalCode) || !row.PersonalCode.All(char.IsDigit) || row.PersonalCode.Length > 10)
                return "کد پرسنلی معتبر نیست.";
            if (duplicateCodes.Contains(row.PersonalCode)) return "کد پرسنلی در فایل تکرار شده است.";
            if (_userService.IsExistUserName(row.PersonalCode)) return "این کاربر قبلاً در سامانه ثبت شده است.";
            if (string.IsNullOrWhiteSpace(row.FirstName) || string.IsNullOrWhiteSpace(row.LastName)) return "نام و نام خانوادگی الزامی است.";
            if (!row.UnitCode.HasValue || row.UnitCode <= 0 || string.IsNullOrWhiteSpace(row.UnitTitle)) return "یگان تشخیص داده نشده است.";
            if (!allowedRoleIds.Contains(row.RoleId)) return "نقش انتخاب‌شده معتبر یا قابل تخصیص نیست.";

            var currentHolder = _context.UserRoles.AsNoTracking()
                .Where(item => item.RoleId == row.RoleId && !item.User.IsDelete && item.User.IsActive)
                .Select(item => item.User.FirstName + " " + item.User.LastName + " (" + item.User.UserName + ")")
                .FirstOrDefault();
            return currentHolder == null ? null : "این نقش در اختیار «" + currentHolder + "» است؛ ابتدا جایگاه را آزاد یا نقش دیگری انتخاب کنید.";
        }

        private bool TryPrepareLookups()
        {
            var roleTypeId = User.FindFirst("RoleTypeId")?.Value;
            if (string.IsNullOrWhiteSpace(roleTypeId)) return false;

            var roles = GetAllowedRoles();
            ViewData["Roles"] = new SelectList(roles, "RoleId", "Title");
            try
            {
                Units = _webApiService.GetAllOrgan()?.Data ?? new List<OrganViewModelDto>();
            }
            catch
            {
                Units = new List<OrganViewModelDto>();
            }
            return true;
        }

        private List<VisitorManagment.DataLayer.Entities.User.Role> GetAllowedRoles() =>
            _permissionService.GetRoles(User.FindFirst("RoleTypeId")?.Value ?? string.Empty) ??
            new List<VisitorManagment.DataLayer.Entities.User.Role>();

        private void MarkFailed(BulkUserImportRowViewModel row, string message)
        {
            row.Imported = false;
            row.Status = "ثبت نشد";
            row.ErrorMessage = message;
            Result.FailedCount++;
            Result.Rows.Add(row);
        }

        private static string GetSafeErrorMessage(Exception exception) =>
            exception.InnerException?.Message ?? exception.Message ?? "خطای نامشخص";
    }
}

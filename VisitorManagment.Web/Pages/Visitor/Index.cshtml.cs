using System.Collections.Generic;
using ITOWebApiClient;
using System;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.Constants;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.ReportsAdmin;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Core.Services.Interfaces.Ranking;
using VisitorManagment.Core.Services.Interfaces.Reports;

namespace VisitorManagment.Web.Pages.Visitor
{
    [Authorize]
    public class IndexModel : PageModel 
    {

        private readonly IRankingService _rankingService;
        private readonly IFileService _fileService;
        private readonly IWebApiService _webApiService;
        private readonly ICartableService _cartableService;
        private readonly IUserService _userService;
        private readonly IHameshService _hameshService;
        private readonly ApiTokenCacheClient _apiTokenClient;
        private readonly IRequestGhaReportService _requestGhaReportService;
        private readonly IPermissionService _permissionService;

        public IndexModel(IFileService fileService, IWebApiService webApiService,
            ICartableService cartableService, IUserService userService, 
            IHameshService hameshService, ApiTokenCacheClient apiTokenClient , IRequestGhaReportService requestGhaReportService
            ,IRankingService rankingService, IPermissionService permissionService)
        {
            _fileService = fileService;
            _webApiService = webApiService;
            _cartableService = cartableService;
            _userService = userService;
            _hameshService = hameshService;
            _apiTokenClient = apiTokenClient;
            _requestGhaReportService = requestGhaReportService;
            _rankingService = rankingService;
            _permissionService = permissionService;
        }

        public List<ProblemReportViewModel> lstChartModel { get; set; }
        public ChartNomrehArzyabiGha chartDto { get; set; }
        public SearchPageAllUnitCodeForGhaReportViewModel searchPageUnitCodeReportViewModel { get; set; }
        public DashboardRequestStatistics Statistics { get; private set; } = new DashboardRequestStatistics();
        public DashboardDemandStatistics DemandStatistics { get; private set; } = new DashboardDemandStatistics();
        public string FromDate { get; private set; }
        public string ToDate { get; private set; }
        public string DateFilterError { get; private set; }
        public string UserDisplayName { get; private set; }
        public string UserUnitTitle { get; private set; }
        public string UserIpAddress { get; private set; }
        public string LoginDateTime { get; private set; }
        public int SelectedPeriod { get; private set; }
        public int? SelectedUnitCode { get; private set; }
        public bool CanViewManagementDashboard { get; private set; }
        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet(int period = 0, int? selectedUnitCode = null, string fromDate = null, string toDate = null)
        {
            SelectedPeriod = new[] { 0, 3, 6, 12 }.Contains(period) ? period : 0;
            SelectedUnitCode = selectedUnitCode;
            FromDate = NormalizePersianDate(fromDate);
            ToDate = NormalizePersianDate(toDate);
            var parsedFromDate = ParsePersianDate(FromDate);
            var parsedToDate = ParsePersianDate(ToDate);
            if ((!string.IsNullOrWhiteSpace(FromDate) && !parsedFromDate.HasValue) ||
                (!string.IsNullOrWhiteSpace(ToDate) && !parsedToDate.HasValue))
            {
                DateFilterError = "تاریخ را به شکل سال/ماه/روز وارد کنید؛ برای نمونه ۱۴۰۵/۰۱/۰۱.";
                parsedFromDate = null;
                parsedToDate = null;
            }
            else if (parsedFromDate.HasValue && parsedToDate.HasValue && parsedFromDate.Value > parsedToDate.Value)
            {
                DateFilterError = "تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد.";
                parsedFromDate = null;
                parsedToDate = null;
            }
            var endDateExclusive = parsedToDate.HasValue ? parsedToDate.Value.AddDays(1) : (DateTime?)null;

            #region مودال اطلاعات سیستمی نفر لاگین کرده
            var userName = User.FindFirst("UserName")?.Value ?? string.Empty;
            UserDisplayName = User.FindFirst("FullName")?.Value ?? "کاربر سامانه";
            UserUnitTitle = User.FindFirst("UnitCodeTitle")?.Value ?? "یگان سازمانی";
            UserIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "نامشخص";
            LoginDateTime = DateTime.UtcNow.AddHours(3.5).ToString("yyyy/MM/dd HH:mm");

            if (int.TryParse(userName, out var personalNumber))
            {
                ViewData["FullName"] = _userService.GetInfoUserLoginHistory(personalNumber);
            }
            #endregion
            //
            #region بدست آوردن رتبه یگان 
            var unitCode = User.FindFirst("UnitCode")?.Value ?? "300000";
            var codeGha = User.FindFirst("CodGha")?.Value ?? "300000";
            //var departmentTypeId = User.FindFirst("DepartmentTypeId").Value;
            //var listAllUnitHamvand = _rankingService.GetListUnitWithDepartmentTypeId(int.Parse(departmentTypeId));
            //var index = "";
            //int j = 0;
            //Dictionary<int, string> dictionery = new Dictionary<int, string>();

            //var pointAllUnit = _rankingService.pointListHamvandUnit(listAllUnitHamvand).ToList();


            //foreach (var item in pointAllUnit)
            //{
            //    j++;

            //    if (item.UnitCode == int.Parse(unitCode))
            //    {
            //        break;
            //    }

            //}
            //ViewData["rankPoint"] = j;

            //var finlPoint = j;

            #endregion



            var unitDutyCode = User.FindFirst("UnitDutyCode")?.Value ?? string.Empty;
            var personalCode = User.FindFirst("PersonalCode")?.Value ?? string.Empty;
            var roleTypeId = User.FindFirst("RoleTypeId")?.Value ?? string.Empty;
            var roleTitle = User.FindFirst("RoleTypeTitle")?.Value ?? string.Empty;
            var userId = int.TryParse(User.FindFirst("Id")?.Value, out var parsedUserId) ? parsedUserId : 0;
            var hasUserAccess = userId > 0 && (_userService.GetUserByUserId(userId)?.HasDashboardAccess ?? false);
            var hasRoleAccess = userId > 0 && _permissionService.GetPermissionsForUser(userId)
                .Any(permission => permission.MenuUrl == "#management-dashboard");
            var isAnsarCommander = roleTitle.Contains("انصار") &&
                (roleTitle.Contains("ف ق") || roleTitle.Contains("فرمانده قرارگاه"));
            CanViewManagementDashboard = roleTypeId == SystemRoleTypes.SystemAdministrator.ToString() || isAnsarCommander || hasUserAccess || hasRoleAccess;

            Statistics = _fileService.GetDashboardRequestStatistics(
                unitDutyCode,
                unitCode,
                codeGha,
                roleTypeId,
                personalCode,
                SelectedPeriod,
                SelectedUnitCode,
                parsedFromDate,
                endDateExclusive);

            DemandStatistics = _fileService.GetDashboardDemandStatistics(
                unitDutyCode, unitCode, codeGha, roleTypeId, personalCode,
                parsedFromDate, endDateExclusive, SelectedUnitCode);

            #region رتبه بندی

           // var point = _rankingService.GetRanking(int.Parse(unitCode));
            //ViewData["rankPoint"] = point;

            #endregion





        }

        private static string NormalizePersianDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            var chars = value.Trim().ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (chars[i] >= '\u06F0' && chars[i] <= '\u06F9')
                    chars[i] = (char)('0' + chars[i] - '\u06F0');
                else if (chars[i] >= '\u0660' && chars[i] <= '\u0669')
                    chars[i] = (char)('0' + chars[i] - '\u0660');
            }
            return new string(chars).Replace('-', '/').Replace('.', '/');
        }

        private static DateTime? ParsePersianDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var parts = value.Split('/');
            if (parts.Length != 3 ||
                !int.TryParse(parts[0], out var year) ||
                !int.TryParse(parts[1], out var month) ||
                !int.TryParse(parts[2], out var day))
                return null;

            try
            {
                return new PersianCalendar().ToDateTime(year, month, day, 0, 0, 0, 0);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }
    }
}

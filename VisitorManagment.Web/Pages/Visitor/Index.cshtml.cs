using System.Collections.Generic;
using ITOWebApiClient;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public IndexModel(IFileService fileService, IWebApiService webApiService,
            ICartableService cartableService, IUserService userService, 
            IHameshService hameshService, ApiTokenCacheClient apiTokenClient , IRequestGhaReportService requestGhaReportService
            ,IRankingService rankingService)
        {
            _fileService = fileService;
            _webApiService = webApiService;
            _cartableService = cartableService;
            _userService = userService;
            _hameshService = hameshService;
            _apiTokenClient = apiTokenClient;
            _requestGhaReportService = requestGhaReportService;
            _rankingService = rankingService;
        }

        public List<ProblemReportViewModel> lstChartModel { get; set; }
        public ChartNomrehArzyabiGha chartDto { get; set; }
        public SearchPageAllUnitCodeForGhaReportViewModel searchPageUnitCodeReportViewModel { get; set; }
        public DashboardRequestStatistics Statistics { get; private set; } = new DashboardRequestStatistics();
        public string UserDisplayName { get; private set; }
        public string UserUnitTitle { get; private set; }
        public string UserIpAddress { get; private set; }
        public string LoginDateTime { get; private set; }
        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet()
        {

            #region مودال اطلاعات سیستمی نفر لاگین کرده
            var userName = User.FindFirst("UserName")?.Value ?? string.Empty;
            UserDisplayName = User.FindFirst("FullName")?.Value ?? "کاربر سامانه";
            UserUnitTitle = User.FindFirst("UnitCodeTitle")?.Value ?? "یگان سازمانی";
            UserIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "نامشخص";
            LoginDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");

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

            Statistics = _fileService.GetDashboardRequestStatistics(
                unitDutyCode,
                unitCode,
                codeGha,
                roleTypeId,
                personalCode);

            #region رتبه بندی

           // var point = _rankingService.GetRanking(int.Parse(unitCode));
            //ViewData["rankPoint"] = point;

            #endregion





        }
    }
}

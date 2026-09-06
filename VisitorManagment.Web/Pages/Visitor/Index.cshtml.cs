using System.Collections.Generic;
using ITOWebApiClient;
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
        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet()
        {

            #region مودال اطلاعات سیستمی نفر لاگین کرده
            var userId = User.FindFirst("Id").Value;
            var userName = User.FindFirst("UserName").Value;
            ViewData["FullName"] = _userService.GetInfoUserLoginHistory(int.Parse(userName));
            #endregion
            //
            #region بدست آوردن رتبه یگان 
            var unitCode = User.FindFirst("UnitCode").Value ?? 300000.ToString();
            var codeGha = User.FindFirst("CodGha").Value ?? 300000.ToString();
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



            var unitDutyCode = User.FindFirst("UnitDutyCode").Value;
            var unitCodeTitle = User.FindFirst("UnitCodeTitle").Value;
            var codeGhaTitle = User.FindFirst("CodGhaTitle").Value;
            var personalCode = User.FindFirst("PersonalCode").Value;
           
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);
            //
            ViewData["AllFileCount"] = _fileService.GetFileCount(unitDutyCode, unitCode, codeGha, roleTypeId.ToString(), personalCode);
            ViewData["EghdamShode"] = _fileService.GetFileCountEghdamShode(unitDutyCode, unitCode, codeGha, roleTypeId.ToString(), personalCode);
            ViewData["SabteNazariye"] = _fileService.GetFileCountSabteNazariye(unitDutyCode, unitCode, codeGha, roleTypeId.ToString(), personalCode);
            ViewData["RadeDarkhastVaAodat"] = _fileService.GetFileCountRadeDarkhastVaAodat(unitDutyCode, unitCode, codeGha, roleTypeId.ToString(), personalCode);

            #region رتبه بندی

           // var point = _rankingService.GetRanking(int.Parse(unitCode));
            //ViewData["rankPoint"] = point;

            #endregion





        }
    }
}

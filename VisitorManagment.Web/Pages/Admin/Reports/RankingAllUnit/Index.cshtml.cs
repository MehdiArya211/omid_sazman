using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VisitorManagment.Core.DTOs.ReportsAdmin;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Core.Services.Interfaces.Reports;

namespace VisitorManagment.Web.Pages.Admin.Reports.RankingAllUnit
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IWebApiService _webApiService;
        private readonly IPersonService _personService;
        private readonly IChartService _chartService;
        private readonly IProblemNezReportService _ProblemNez;
        private readonly IHamishReportService _HamishReportService;
        private readonly IRequestGhaReportService _requestGhaReportService;
        private readonly IHameshService _hameshService;


        public IndexModel(IWebApiService webApiService, IPersonService personService,
            IChartService chartService, IProblemNezReportService ProblemNez, IHamishReportService hamishReportService
            , IRequestGhaReportService requestGhaReportService, IHameshService hameshService
            )
        {
            _webApiService = webApiService;
            _personService = personService;
            _chartService = chartService;
            _ProblemNez = ProblemNez;
            _HamishReportService = hamishReportService;
            _requestGhaReportService = requestGhaReportService;
            _hameshService = hameshService;
        }

        [BindProperty]
        //مدل نمودار میله ای گزارش فراوانی مشکلات
        public List<ProblemReportViewModel> lstChartModel { get; set; }
        public ChartNomrehArzyabiGha chartDto { get; set; }
        public SearchPageRankingAllUnitReportViewModel searchPageRankingAllUnitReportViewModel { get; set; }

        public IActionResult OnGet(int filterGharargah = 0)
        {
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["AllListUnit"] = new SelectList(_webApiService.GetAllOrgan().Data, "Id", "Title");
            ViewData["ActionType"] = new SelectList(_hameshService.GetActionType(), "Id", "Title");

            lstChartModel = new List<ProblemReportViewModel>();
            chartDto = new ChartNomrehArzyabiGha();
            searchPageRankingAllUnitReportViewModel = new SearchPageRankingAllUnitReportViewModel();

            return Page();

        }

        public IActionResult OnPost(int codeGha, List<int> unitCode, int actionTypeId, string startDateSearch = "", string endDateSearch = "")
        {
            ViewData["AllListGha"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["AllListUnit"] = new SelectList(_webApiService.GetAllOrgan().Data, "Id", "Title");
            ViewData["ActionType"] = new SelectList(_hameshService.GetActionType(), "Id", "Title");

            #region Initial
            searchPageRankingAllUnitReportViewModel = new SearchPageRankingAllUnitReportViewModel()
            {
                StartDate = startDateSearch,
                EndDate = endDateSearch,
                ActionTypeId = actionTypeId,
                CodeGha = codeGha,
                UnitCode = unitCode,
            };


            lstChartModel = new List<ProblemReportViewModel>();
            chartDto = new ChartNomrehArzyabiGha();
            #endregion

            #region تبدیل تاریخ تقویم
            DateTime? startDateEnglish = null;
            DateTime? endDateEnglish = null;

            if ((startDateSearch != "" && startDateSearch != null) || (endDateSearch != "" && endDateSearch != null))
            {
                if (startDateSearch != "" && startDateSearch != null)
                {
                    //زمان شروع رو با / جدا میکنه
                    var startDate = startDateSearch.Split('/');
                    //سال و ماه و روز تاریخ شروع رو به اینت تبدیل میکنیم
                    var yearstart = Int32.Parse(string.Join("", startDate[0].Select(c => char.GetNumericValue(c))));
                    var monthstart = Int32.Parse(string.Join("", startDate[1].Select(c => char.GetNumericValue(c))));
                    var daystart = Int32.Parse(string.Join("", startDate[2].Select(c => char.GetNumericValue(c))));
                    //زمان شروع شمسی رو به میلادی تبدیل میکنیم
                    startDateEnglish = new DateTime(yearstart, monthstart, daystart, new PersianCalendar());
                }
                //********************************************************************************************************************//
                if ((endDateSearch != "" && endDateSearch != null))
                {
                    var endDate = endDateSearch.Split('/');
                    var yearend = Int32.Parse(string.Join("", endDate[0].Select(c => char.GetNumericValue(c))));
                    var monthend = Int32.Parse(string.Join("", endDate[1].Select(c => char.GetNumericValue(c))));
                    var dayend = Int32.Parse(string.Join("", endDate[2].Select(c => char.GetNumericValue(c))));
                    endDateEnglish = new DateTime(yearend, monthend, dayend, new PersianCalendar());
                }

            }


            #endregion

            #region درخواست قرارگاه و فراوانی مشکلات  

            var ListresultRequest = _requestGhaReportService
                .GetNomrehArzyabiGharargah(actionTypeId, startDateEnglish, endDateEnglish)
                .OrderByDescending(x => x.TCount);


            if (ListresultRequest.Count() == 0)
            {

                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "موردی یافت نشد",
                    Quantity = 0
                });
                
            }

            else
            {

                foreach (var item in unitCode)
                {
                    chartDto.MainCount = 0;

                    chartDto.Title = _webApiService.GetOmdOrgan(item).Data.UnitTitle;

                    foreach (var resultRequest in ListresultRequest)
                    {
                        if (item == resultRequest.UnitCode)
                        {
                            chartDto.MainCount = chartDto.MainCount + resultRequest.TCount;
                        }
                    }


                    lstChartModel.Add(new ProblemReportViewModel
                    {
                        DimensionOne = chartDto.Title,
                        Quantity = chartDto.MainCount,


                    });


                }
            }

            var res = lstChartModel.OrderBy(x => x.Quantity);
            #endregion

            return Page();

        }



        public JsonResult OnGetYegan(int id)
        {
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["AllListUnit"] = new SelectList(_webApiService.GetAllOrgan().Data, "Id", "Title");
            ViewData["ActionType"] = new SelectList(_hameshService.GetActionType(), "Id", "Title");

            var result = _webApiService.GetOrganByGharargahId(id).Data;

            return new JsonResult(result);
        }
    }

}

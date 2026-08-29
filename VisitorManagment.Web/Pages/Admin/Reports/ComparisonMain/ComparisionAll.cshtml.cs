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

namespace VisitorManagment.Web.Pages.Admin.Reports.ComparisonAll
{
    [Authorize]

    public class ComparisionAllModel : PageModel
    {
        private readonly IWebApiService _webApiService;
        private readonly IPersonService _personService;
        private readonly IChartService _chartService;
        private readonly IProblemNezReportService _ProblemNez;
        private readonly IHamishReportService _HamishReportService;
        private readonly IRequestGhaReportService _requestGhaReportService;
        private readonly IHameshService _hameshService;


        public ComparisionAllModel(IWebApiService webApiService, IPersonService personService,
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
        public SearchPageUnitCodeReportViewModel searchPageUnitCodeReportViewModel1 { get; set; }
        public SearchPageUnitCodeReportViewModel searchPageUnitCodeReportViewModel2 { get; set; }

        public IActionResult OnGet(int filterGharargah = 0)
        {
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["Yegan1"] = new SelectList(_webApiService.GetOrganByGharargahId(filterGharargah).Data, "Id", "Title");
            ViewData["Yegan2"] = new SelectList(_webApiService.GetOrganByGharargahId(filterGharargah).Data, "Id", "Title");
            ViewData["ActionType"] = new SelectList(_hameshService.GetActionType(), "Id", "Title");

            lstChartModel = new List<ProblemReportViewModel>();
            chartDto = new ChartNomrehArzyabiGha();
            searchPageUnitCodeReportViewModel1 = new SearchPageUnitCodeReportViewModel();
            searchPageUnitCodeReportViewModel2 = new SearchPageUnitCodeReportViewModel();

            #region ارزیابی کلی قرارگاه ها
            var ListresultRequest = _requestGhaReportService.GetNomrehArzyabiGharargah(1, null, null);

            foreach (var resultRequest in ListresultRequest)
            {

                #region فراوانی مشکلات همون موضوع درخواست های ملاقات
                if (resultRequest.CodGha == 300000)//==نزاجا
                {
                    chartDto.CountNezaja = chartDto.CountNezaja + resultRequest.TCount;
                }

                if (resultRequest.CodGha == 300095)//==مراتو
                {
                    chartDto.CountMarato = chartDto.CountMarato + resultRequest.TCount;
                }

                if (resultRequest.CodGha == 300940)//==علوم فنون مکانیزه
                {
                    chartDto.CountOlomFononMekanizeh = chartDto.CountOlomFononMekanizeh + resultRequest.TCount;
                }

                if (resultRequest.CodGha == 300594)//==غرب
                {
                    chartDto.CountGhaGharb = chartDto.CountGhaGharb + resultRequest.TCount;
                }


                if (resultRequest.CodGha == 300614)//==جنوب غرب
                {
                    chartDto.CountGhaJonobGharb = chartDto.CountGhaJonobGharb + resultRequest.TCount;
                }


                if (resultRequest.CodGha == 303343)//==شمال غرب
                {
                    chartDto.CountGhaShomalGharb = chartDto.CountGhaShomalGharb + resultRequest.TCount;
                }

                if (resultRequest.CodGha == 303532)//==شمال شرق
                {
                    chartDto.CountGhaShomalShargh = chartDto.CountGhaShomalShargh + resultRequest.TCount;
                }

                if (resultRequest.CodGha == 304281)//==جنوب شرق
                {
                    chartDto.CountGhaJonobShargh = chartDto.CountGhaJonobShargh + resultRequest.TCount;
                }
                #endregion
            }


            return Page();
            #endregion
        }

        public IActionResult OnPost(int codeGha1 , int codeGha2 ,int unitCode1, int unitCode2, int actionTypeId, string startDateSearch = "", string endDateSearch = "")
        {
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["Yegan1"] = new SelectList(_webApiService.GetOrganByGharargahId(codeGha1).Data, "Id", "Title");
            ViewData["Yegan2"] = new SelectList(_webApiService.GetOrganByGharargahId(codeGha2).Data, "Id", "Title");

            ViewData["AllListGha"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["ActionType"] = new SelectList(_hameshService.GetActionType(), "Id", "Title");
            #region Initial
            searchPageUnitCodeReportViewModel1 = new SearchPageUnitCodeReportViewModel()
            {
                StartDate = startDateSearch,
                EndDate = endDateSearch,
                ActionTypeId = actionTypeId,
                CodeGha = codeGha1,
                UnitCode = unitCode1,
            };

            searchPageUnitCodeReportViewModel2 = new SearchPageUnitCodeReportViewModel()
            {
                StartDate = startDateSearch,
                EndDate = endDateSearch,
                ActionTypeId = actionTypeId,
                CodeGha = codeGha2,
                UnitCode = unitCode2,
            };

            lstChartModel = new List<ProblemReportViewModel>();
            chartDto = new ChartNomrehArzyabiGha();
            #endregion

            List<int> listUnitCode = new List<int> { unitCode1, unitCode2 };
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

                #region
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "موردی یافت نشد",
                    Quantity = 1
                });

                chartDto = new ChartNomrehArzyabiGha()
                {
                    CountGhaShomal = 0,
                    CountGhaShomalGharb = 0,
                    CountGhaShomalShargh = 0,
                    CountGhaGharb = 0,
                    CountGhaShargh = 0,
                    CountGhaJonob = 0,
                    CountGhaJonobGharb = 0,
                    CountGhaJonobShargh = 0,
                    CountAll = 0,

                };
                #endregion
            }

            else
            {

                foreach (var item in listUnitCode)
                {
                    chartDto.MainCount = 0;
                    chartDto.Title = "";

                    chartDto.Title = ListresultRequest
                        .Where(x => x.UnitCode == item && x.UnitCodeTitle != "" && x.UnitCodeTitle != null)
                        .Select(x => x.UnitCodeTitle).FirstOrDefault();

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

            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            return Page();

        }



        public JsonResult OnGetYegan(int id)
        {
            var result = _webApiService.GetOrganByGharargahId(id).Data;

            return new JsonResult(result);
        }
    }
}

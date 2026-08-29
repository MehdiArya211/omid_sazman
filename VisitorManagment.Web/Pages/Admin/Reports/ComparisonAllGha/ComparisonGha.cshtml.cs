using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.ReportsAdmin;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Core.Services.Interfaces.Reports;

namespace VisitorManagment.Web.Pages.Admin.Reports.ComparisonAllGha
{
    [Authorize]

    public class ComparisonGhaModel : PageModel
    {
        private readonly IWebApiService _webApiService;
        private readonly IPersonService _personService;
        private readonly IChartService _chartService;
        private readonly IProblemNezReportService _ProblemNez;
        private readonly IHamishReportService _HamishReportService;
        private readonly IRequestGhaReportService _requestGhaReportService;
        private readonly IHameshService _hameshService;


        public ComparisonGhaModel(IWebApiService webApiService, IPersonService personService,
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
        public SearchPageReportViewModel searchPageReportViewModel { get; set; }

        public IActionResult OnGet()
        {
            ViewData["AllListGha"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["ActionType"] = new SelectList(_hameshService.GetActionType(), "Id", "Title");

            lstChartModel = new List<ProblemReportViewModel>();
            chartDto = new ChartNomrehArzyabiGha();
            searchPageReportViewModel = new SearchPageReportViewModel();

            #region ارزیابی کلی قرارگاه ها
            var ListresultRequest = _requestGhaReportService.GetNomrehArzyabiGharargah(1 ,null, null);

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

        public IActionResult OnPost(List<int> listGha,int actionTypeId, string startDateSearch = "", string endDateSearch = "")
        {
            ViewData["AllListGha"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["ActionType"] = new SelectList(_hameshService.GetActionType(), "Id", "Title");

            lstChartModel = new List<ProblemReportViewModel>();
            chartDto = new ChartNomrehArzyabiGha();

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

            searchPageReportViewModel = new SearchPageReportViewModel()
            {
                StartDate = startDateSearch,
                EndDate = endDateSearch,
                ActionTypeId = actionTypeId,
                CodeGha = listGha,
            };

            #region درخواست قرارگاه و فراوانی مشکلات  

            var ListresultRequest = _requestGhaReportService
                .GetNomrehArzyabiGharargah(actionTypeId, startDateEnglish, endDateEnglish)
                .OrderByDescending(x=>x.TCount);


            if (ListresultRequest.Count()==0)
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
                foreach (var item in listGha)
                {
                    chartDto.MainCount = 0;
                    chartDto.Title = "";

                    chartDto.Title = ListresultRequest
                        .Where(x => x.CodGha == item && x.CodGhaTitle != "" && x.CodGhaTitle != null)
                        .Select(x => x.CodGhaTitle).FirstOrDefault();

                    foreach (var resultRequest in ListresultRequest)
                    {
                        if (item==resultRequest.CodGha)
                        {
                            
                            chartDto.MainCount = chartDto.MainCount + resultRequest.TCount;

                            if (item == 300940)//==مرکز علوم فنو مکانیزه
                            {
                                chartDto.Title = " ارشد شیراز";
                            }

                            if (item == 300095)//==مرکز توپخانه
                            {
                                chartDto.Title = " ارشد اصفهان";
                            }

                        }



                    }
                    
                    lstChartModel.Add(new ProblemReportViewModel
                    {
                        DimensionOne =chartDto.Title,
                        Quantity = chartDto.MainCount,


                    });


                   
                }
            }

           var res= lstChartModel.OrderBy(x => x.Quantity);
            #endregion

            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            return Page();
        }
    }
}


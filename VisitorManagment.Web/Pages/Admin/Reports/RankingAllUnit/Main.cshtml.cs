using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VisitorManagment.Core.DTOs.ReportsAdmin;
using VisitorManagment.Core.Services.Interfaces.Reports;
using VisitorManagment.Core.Services.Interfaces;
using System.Globalization;
using System.Linq;
using System;
using VisitorManagment.DataLayer.Entities.VisitorManagment;
using VisitorManagment.Core.DTOs.Ranking;
using VisitorManagment.Core.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace VisitorManagment.Web.Pages.Admin.Reports.RankingAllUnit
{
    [Authorize]
    public class MainModel : PageModel
    {
        private readonly IWebApiService _webApiService;
        private readonly IPersonService _personService;
        private readonly IChartService _chartService;
        private readonly IProblemNezReportService _ProblemNez;
        private readonly IHamishReportService _HamishReportService;
        private readonly IRequestGhaReportService _requestGhaReportService;
        private readonly IHameshService _hameshService;


        public MainModel(IWebApiService webApiService, IPersonService personService,
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
        public List<AllOrganViewModelDto> listOrganViewModel { get; set; }


        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public IActionResult OnGet(int filterGharargah = 0)
        {
            ViewData["Orgns"] = new SelectList(_webApiService.GetAllOrgan1().Data, "Id", "Title");
            listOrganViewModel = _webApiService.GetAllOrgan1().Data;

            #region Initial
            lstChartModel = new List<ProblemReportViewModel>();
            chartDto = new ChartNomrehArzyabiGha();

            lstChartModel = new List<ProblemReportViewModel>();
            chartDto = new ChartNomrehArzyabiGha();
            searchPageReportViewModel = new SearchPageReportViewModel();
            listOrganViewModel=new List<AllOrganViewModelDto>();
            #endregion


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
        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost(List<int> unitCode, string startDateSearch = "", string endDateSearch = "")
        {
            ViewData["Orgns"] = new SelectList(_webApiService.GetAllOrgan().Data, "Id", "Title");
            #region Initial
            lstChartModel = new List<ProblemReportViewModel>();
            chartDto = new ChartNomrehArzyabiGha();
            searchPageReportViewModel = new SearchPageReportViewModel();
            listOrganViewModel = new List<AllOrganViewModelDto>();
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
                .GetNomrehArzyabiGharargah(1, startDateEnglish, endDateEnglish)
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
    }
}

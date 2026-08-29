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

namespace VisitorManagment.Web.Pages.Admin.Reports
{
    [Authorize]

    public class FarmandehActivityReportModel : PageModel
    {
        private readonly IWebApiService _webApiService;
        private readonly IPersonService _personService;
        private readonly IChartService _chartService;
        private readonly IProblemNezReportService _ProblemNez;
        private readonly IHamishReportService _HamishReportService;
        public FarmandehActivityReportModel(IWebApiService webApiService , IPersonService personService ,
            IChartService chartService, IProblemNezReportService ProblemNez, IHamishReportService hamishReportService)
        {
            _webApiService = webApiService;
            _personService = personService;
            _chartService = chartService;
            _ProblemNez = ProblemNez;
            _HamishReportService = hamishReportService;
        }

        [BindProperty]
        public List<ChartFarmandehActivityDto> BarLineDataSet { get; set; }
        public List<SimpleReportViewModel> lstModelPieChart { get; set; }
        public List<SimpleReportViewModel> lstModelBarChart { get; set; }
        public SearchPageReportFarmandehActivityReportViewModel searchPageReportViewModel { get; set; }

        public FarmandehReportDTO dto { get; set; }
        public void OnGet()
        {
            ViewData["FarmandehInfo"] = new SelectList(_personService.GetFarmandehInfos(), "Id", "Title");
            searchPageReportViewModel = new SearchPageReportFarmandehActivityReportViewModel();

            lstModelPieChart = new List<SimpleReportViewModel>();
            lstModelBarChart = new List<SimpleReportViewModel>();
            lstModelPieChart.Add(new SimpleReportViewModel
            {
                DimensionOne = "موردی یافت نشد",
                Quantity = 1
            });
            lstModelBarChart.Add(new SimpleReportViewModel
            {
                DimensionOne = "موردی یافت نشد",
                Quantity = 1
            });
            dto = new FarmandehReportDTO()
            {
                TotalNazarieh = 0,
                TotalResolveRequest = 0,
                TotalReturnRequest = 0,
                TotalWaitingRequest = 0,
                TotalRequest = 0,
                TransferCount = 0,
                OtherCount = 0,
                MoseadatAnyMoneyCount = 0,
                MorakhasiNoUseCount = 0,
                MadrakTahsiliCount = 0,
                CourseCount = 0,
                EadehBeKhetmatCount = 0,
                EastekhtamCount = 0,
                EbghaCount = 0,
                MahkomiatCount = 0,
                MaskanCount = 0,
                MosaedatCount = 0,
                RahaeiCount = 0,
                RankMaskanCount = 0,
                ShekaiatCount = 0,
                VamCount = 0,
                
            };
        
        }

        public IActionResult OnPost(int PrsnCd , string startDateSearch = "", string endDateSearch = "")
        {
            ViewData["FarmandehInfo"] = new SelectList(_personService.GetFarmandehInfos(), "Id", "Title");

            lstModelPieChart = new List<SimpleReportViewModel>();
            lstModelBarChart = new List<SimpleReportViewModel>();
            searchPageReportViewModel = new SearchPageReportFarmandehActivityReportViewModel();

            ViewData["avatar"] = _personService.GetAvatarUserByPrsnCd(PrsnCd);
            var personalCode = PrsnCd.ToString();
            #region تبدیل تاریخ تقویم
            DateTime? startDateEnglish = null;
            DateTime? endDateEnglish = null;

            if ((startDateSearch != "" && startDateSearch!=null) || (endDateSearch != "" && endDateSearch!=null))
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
                


                if (startDateEnglish > endDateEnglish)
                {
                    ViewData["startDateBiggerThanEndDate"] = "true";
                    return Page();

                }

            }


            #endregion
            searchPageReportViewModel = new SearchPageReportFarmandehActivityReportViewModel()
            {
                StartDate = startDateSearch,
                EndDate = endDateSearch,
                PrsnCd = PrsnCd,
            };



            //====================================گرفتن اطلاعات فرمانده==============================//
            var dtoFarmandehInfo = _personService.GetPrsnInfoByPrsnId(personalCode);

            //====================================گزارش براساس نوع اقدامات فرماندهان   ==============================//
            var result = _HamishReportService.ReportFarmandehByActionCode(personalCode , startDateEnglish , endDateEnglish);

            //====================================گزارش براساس مشکلات   ==============================//
            var resultProblem = _HamishReportService.ReportProblemFarmandehInfo(personalCode , startDateEnglish, endDateEnglish);

            if (result.Files.TotalNazarieh == 0 && result.Files.TotalResolveRequest == 0 && result.Files.TotalReturnRequest == 0 && result.Files.TotalWaitingRequest == 0)
            {
                lstModelPieChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "موردی یافت نشد",
                    Quantity = 1
                });
                //dto = new FarmandehReportDTO()
                //{
                //    TotalNazarieh = 0,
                //    TotalResolveRequest = 0,
                //    TotalReturnRequest = 0,
                //    TotalWaitingRequest = 0,
                //    TotalRequest = 0,
                //    Rank = dtoFarmandehInfo.Rank,
                //    PrsnCd = dtoFarmandehInfo.PrsnCd,
                //    BranchTitle = dtoFarmandehInfo.BranchTitle, 
                //    EntesabDate = dtoFarmandehInfo.EntesabDate,
                //    FullName = dtoFarmandehInfo.FullName,
                //    Job = dtoFarmandehInfo.Job,
                //    Organ = dtoFarmandehInfo.Organ,
                //};

                lstModelBarChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "موردی یافت نشد",
                    Quantity = 1
                });

                dto = new FarmandehReportDTO()
                {
                    TotalNazarieh = 0,
                    TotalResolveRequest = 0,
                    TotalReturnRequest = 0,
                    TotalWaitingRequest = 0,
                    TotalRequest = 0,
                    Rank = dtoFarmandehInfo.Rank,
                    PrsnCd = dtoFarmandehInfo.PrsnCd,
                    BranchTitle = dtoFarmandehInfo.BranchTitle,
                    EntesabDate = dtoFarmandehInfo.EntesabDate,
                    FullName = dtoFarmandehInfo.FullName,
                    Job = dtoFarmandehInfo.Job,
                    Organ = dtoFarmandehInfo.Organ,
                    TransferCount =0,
                    OtherCount = 0,
                    MoseadatAnyMoneyCount = 0,  
                    MorakhasiNoUseCount = 0,
                    MadrakTahsiliCount = 0,
                    CourseCount =0,
                    EadehBeKhetmatCount = 0,
                    EastekhtamCount = 0,
                    EbghaCount = 0,
                    MahkomiatCount = 0,
                    MaskanCount = 0,
                    MosaedatCount = 0,  
                    RahaeiCount = 0,
                    RankMaskanCount = 0,    
                    ShekaiatCount = 0,  
                    VamCount = 0,   
                };
            }
            else
            {
                //=============================== pie Chart===================================//
                lstModelPieChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "عودت شده",
                    Quantity = result.Files.TotalReturnRequest
                });
                lstModelPieChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "اقدام شده",
                    Quantity = result.Files.TotalResolveRequest
                });
                lstModelPieChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "ارجاع شده به رده بالاتر",
                    Quantity = result.Files.TotalNazarieh
                });
                lstModelPieChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "بررسی نشده",
                    Quantity = result.Files.TotalWaitingRequest
                });

                //=============================== bar Chart==================================//
                lstModelBarChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست سایر موارد",
                    Quantity = resultProblem.OtherCount
                });
                lstModelBarChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست وام",
                    Quantity = resultProblem.VamCount
                });
                lstModelBarChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مساعدت مالی",
                    Quantity = resultProblem.MosaedatCount
                });
                lstModelBarChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست انتقال",
                    Quantity = resultProblem.TransferCount
                });
                lstModelBarChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست رهایی",
                    Quantity = resultProblem.RahaeiCount
                });
                lstModelBarChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مسکن سازمانی",
                    Quantity = resultProblem.MaskanCount
                });
                lstModelBarChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست امتیاز پروژه مسکن",
                    Quantity = resultProblem.RankMaskanCount
                });
                lstModelBarChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست شکایات",
                    Quantity = resultProblem.ShekaiatCount
                });
                lstModelBarChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست دوره های نظامی",
                    Quantity = resultProblem.CourseCount
                });
                lstModelBarChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مرخصی استفاده نشده",
                    Quantity = resultProblem.MorakhasiNoUseCount
                });
                lstModelBarChart.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مساعدت غیرمالی",
                    Quantity = resultProblem.MoseadatAnyMoneyCount
                });
               
                dto = new FarmandehReportDTO()
                {
                    TotalNazarieh = result.Files.TotalNazarieh,
                    TotalResolveRequest = result.Files.TotalResolveRequest,
                    TotalReturnRequest = result.Files.TotalReturnRequest,
                    TotalWaitingRequest = result.Files.TotalWaitingRequest,
                    TotalRequest = result.Files.TotalRequest,
                    Rank = dtoFarmandehInfo.Rank,
                    PrsnCd = dtoFarmandehInfo.PrsnCd,
                    BranchTitle = dtoFarmandehInfo.BranchTitle,
                    EntesabDate = dtoFarmandehInfo.EntesabDate,
                    FullName = dtoFarmandehInfo.FullName,
                    Job = dtoFarmandehInfo.Job,
                    Organ = dtoFarmandehInfo.Organ,
                    TransferCount = resultProblem.TransferCount,
                    VamCount = resultProblem.VamCount,
                    ShekaiatCount = resultProblem.ShekaiatCount,
                    RankMaskanCount = resultProblem.RankMaskanCount,
                    RahaeiCount = resultProblem.RahaeiCount,
                    MosaedatCount = resultProblem.MosaedatCount,
                    MaskanCount = resultProblem.MaskanCount,
                    MahkomiatCount = resultProblem.MahkomiatCount,  
                    EbghaCount = resultProblem.EbghaCount,
                    EastekhtamCount = resultProblem.EastekhtamCount,
                    CourseCount = resultProblem.CourseCount,
                    EadehBeKhetmatCount = resultProblem.EadehBeKhetmatCount,
                    MadrakTahsiliCount = resultProblem.MadrakTahsiliCount,  
                    MorakhasiNoUseCount = resultProblem.MorakhasiNoUseCount,
                    MoseadatAnyMoneyCount = resultProblem.MoseadatAnyMoneyCount,    
                    OtherCount = resultProblem.OtherCount,  
                };
            }
            
            ViewData["FarmandehInfo"] = new SelectList(_personService.GetFarmandehInfos(), "Id", "Title");

            return Page();

        }
    }
}

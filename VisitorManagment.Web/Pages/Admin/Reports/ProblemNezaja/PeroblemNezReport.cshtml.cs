using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.DTOs.ReportsAdmin;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Core.Services.Interfaces.Reports;

namespace VisitorManagment.Web.Pages.Admin.Reports
{
    [Authorize]

    public class PeroblemNezReportModel : PageModel
    {
        private readonly IFileService _fileService;
        private readonly IWebApiService _webApiService;
        private readonly IPersonService _personService;
        private readonly IChartService _chartService;
        private readonly IProblemNezReportService  _problemNezReportService;
        public PeroblemNezReportModel(IWebApiService webApiService, IPersonService personService,
            IChartService chartService, IFileService fileService, IProblemNezReportService problemNezReportService)
        {
            _fileService = fileService;
            _webApiService = webApiService;
            _personService = personService;
            _chartService = chartService;
            _problemNezReportService = problemNezReportService;
        }

        [BindProperty]
        public List<ChartFarmandehActivityDto> BarLineDataSet { get; set; }
        public List<SimpleReportViewModel> lstModel { get; set; }
        public FarmandehReportDTO dto { get; set; }
        public ProblemOmdOrganReport dtos { get; set; }
        public ChartProblemOmdOrgan dtose { get; set; }
        public SearchPageReportProblemNezajaReportViewModel searchPageReportViewModel { get; set; }


        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet(int filterGharargah = 0)
        {
            ViewData["RequestSubject"] = new SelectList(_fileService.GetRequestSubject(), "Id", "Title");
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["Yegan"] = new SelectList(_webApiService.GetOrganByGharargahId(filterGharargah).Data, "Id", "Title");
            ViewData["Message"] = "گزارش فراوانی مشکلات در سطح نیروی زمینی";

            var result = _problemNezReportService.GetProblemCountGhCd_OmdCd(0,0 , null ,null );
           
            lstModel = new List<SimpleReportViewModel>();
            searchPageReportViewModel = new SearchPageReportProblemNezajaReportViewModel();
            if (result==null)
            {
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "موردی یافت نشد",
                    Quantity = 1
                });
                dtose = new ChartProblemOmdOrgan()
                {
                    VamCount = 0,
                    MosaedatCount = 0,
                    TransferCount = 0,
                    RahaeiCount = 0,
                    EbghaCount = 0,
                    MaskanCount = 0,
                    RankMaskanCount = 0,
                    EastekhtamCount = 0,
                    EadehBeKhetmatCount = 0,
                    ShekaiatCount = 0,
                    MahkomiatCount = 0,
                    MadrakTahsiliCount = 0,
                    CourseCount = 0,
                    MorakhasiNoUseCount = 0,
                    MoseadatAnyMoneyCount = 0,
                    OtherCount = 0,

                };
            }

            else
            {
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست وام",
                    Quantity = result.VamCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مساعدت مالی",
                    Quantity = result.MosaedatCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست انتقال",
                    Quantity = result.TransferCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست رهایی",
                    Quantity = result.RahaeiCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست ابقا",
                    Quantity = result.EbghaCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مسکن سازمانی",
                    Quantity = result.MaskanCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست امتیاز پروژه مسکن",
                    Quantity = result.RankMaskanCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست استخدام",
                    Quantity = result.EastekhtamCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست اعاده به خدمت",
                    Quantity = result.EadehBeKhetmatCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست شکایات",
                    Quantity = result.ShekaiatCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست محکومیت ها",
                    Quantity = result.MahkomiatCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مدارک تحصیلی",
                    Quantity = result.MadrakTahsiliCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست دوره های نظامی",
                    Quantity = result.CourseCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مرخصی استفاده نشده",
                    Quantity = result.MorakhasiNoUseCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مساعدت غیرمالی",
                    Quantity = result.MoseadatAnyMoneyCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست سایر موارد",
                    Quantity = result.OtherCount
                });

                dtose = new ChartProblemOmdOrgan()
                {
                    VamCount = result.VamCount,
                    MosaedatCount = result.MosaedatCount,
                    TransferCount = result.TransferCount,
                    RahaeiCount = result.RahaeiCount,
                    EadehBeKhetmatCount = result.EadehBeKhetmatCount,
                    MaskanCount = result.MaskanCount,
                    RankMaskanCount = result.RankMaskanCount,
                    EastekhtamCount = result.EastekhtamCount,
                    ShekaiatCount = result.ShekaiatCount,
                    MahkomiatCount = result.MahkomiatCount,
                    MadrakTahsiliCount = result.MadrakTahsiliCount,
                    CourseCount = result.CourseCount,
                    MorakhasiNoUseCount = result.MorakhasiNoUseCount,
                    MoseadatAnyMoneyCount = result.MoseadatAnyMoneyCount,
                    OtherCount = result.OtherCount,
                    EbghaCount = result.EbghaCount,

                    Totalfp = result.VamCount + result.MosaedatCount + result.TransferCount + result.RahaeiCount + result.EadehBeKhetmatCount
                   + result.MaskanCount + result.RankMaskanCount + result.EastekhtamCount + result.ShekaiatCount + result.MahkomiatCount
                   + result.MadrakTahsiliCount + result.CourseCount + result.MorakhasiNoUseCount + result.MoseadatAnyMoneyCount + result.OtherCount
                   + result.EbghaCount,
                };
            }
        }

       
        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost(int Gharargahid, int Yeganid, string startDateSearch = "", string endDateSearch = "")
        {

            ViewData["Message"] = "گزارش فراوانی مشکلات در سطح نیروی زمینی";
            ViewData["RequestSubject"] = new SelectList(_fileService.GetRequestSubject(), "Id", "Title");
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["Yegan"] = new SelectList(_webApiService.GetOrganByGharargahId(Gharargahid).Data, "Id", "Title");
            ViewData["FarmandehInfo"] = new SelectList(_personService.GetFarmandehInfos(), "Id", "Title");

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

            searchPageReportViewModel = new SearchPageReportProblemNezajaReportViewModel()
            {
                StartDate = startDateSearch,
                EndDate = endDateSearch,
                GharargahId = Gharargahid,
                YeganId = Yeganid,
            };

            var result = _problemNezReportService.GetProblemCountGhCd_OmdCd(Gharargahid, Yeganid , startDateEnglish , endDateEnglish);
            lstModel = new List<SimpleReportViewModel>();
            #region محاسبه گزارش
            if (result.VamCount == 0 && result.MosaedatCount == 0 && result.TransferCount == 0 && result.RahaeiCount == 0 && result.EbghaCount == 0 && result.MaskanCount == 0
    && result.RankMaskanCount == 0 && result.EastekhtamCount == 0 && result.EadehBeKhetmatCount == 0 && result.ShekaiatCount == 0 && result.MahkomiatCount == 0
     && result.MadrakTahsiliCount == 0 && result.CourseCount == 0 && result.MorakhasiNoUseCount == 0 && result.MoseadatAnyMoneyCount == 0 && result.OtherCount == 0)
            {
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "موردی یافت نشد",
                    Quantity = 1
                });
                dtose = new ChartProblemOmdOrgan()
                {
                    VamCount = 0,
                    MosaedatCount = 0,
                    TransferCount = 0,
                    RahaeiCount = 0,
                    EbghaCount = 0,
                    MaskanCount = 0,
                    RankMaskanCount = 0,
                    EastekhtamCount = 0,
                    EadehBeKhetmatCount = 0,
                    ShekaiatCount = 0,
                    MahkomiatCount = 0,
                    MadrakTahsiliCount = 0,
                    CourseCount = 0,
                    MorakhasiNoUseCount = 0,
                    MoseadatAnyMoneyCount = 0,
                    OtherCount = 0,

                };
            }
            else
            {
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست وام",
                    Quantity = result.VamCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مساعدت مالی",
                    Quantity = result.MosaedatCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست انتقال",
                    Quantity = result.TransferCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست رهایی",
                    Quantity = result.RahaeiCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست ابقا",
                    Quantity = result.EbghaCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مسکن سازمانی",
                    Quantity = result.MaskanCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست امتیاز پروژه مسکن",
                    Quantity = result.RankMaskanCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست استخدام",
                    Quantity = result.EastekhtamCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست اعاده به خدمت",
                    Quantity = result.EadehBeKhetmatCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست شکایات",
                    Quantity = result.ShekaiatCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست محکومیت ها",
                    Quantity = result.MahkomiatCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مدارک تحصیلی",
                    Quantity = result.MadrakTahsiliCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست دوره های نظامی",
                    Quantity = result.CourseCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مرخصی استفاده نشده",
                    Quantity = result.MorakhasiNoUseCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست مساعدت غیرمالی",
                    Quantity = result.MoseadatAnyMoneyCount
                });
                lstModel.Add(new SimpleReportViewModel
                {
                    DimensionOne = "درخواست سایر موارد",
                    Quantity = result.OtherCount
                });

                dtose = new ChartProblemOmdOrgan()
                {
                    VamCount = result.VamCount,
                    MosaedatCount = result.MosaedatCount,
                    TransferCount = result.TransferCount,
                    RahaeiCount = result.RahaeiCount,
                    EadehBeKhetmatCount = result.EadehBeKhetmatCount,
                    MaskanCount = result.MaskanCount,
                    RankMaskanCount = result.RankMaskanCount,
                    EastekhtamCount = result.EastekhtamCount,
                    ShekaiatCount = result.ShekaiatCount,
                    MahkomiatCount = result.MahkomiatCount,
                    MadrakTahsiliCount = result.MadrakTahsiliCount,
                    CourseCount = result.CourseCount,
                    MorakhasiNoUseCount = result.MorakhasiNoUseCount,
                    MoseadatAnyMoneyCount = result.MoseadatAnyMoneyCount,
                    OtherCount = result.OtherCount,
                    EbghaCount = result.EbghaCount,

                    Totalfp = result.VamCount + result.MosaedatCount + result.TransferCount + result.RahaeiCount + result.EadehBeKhetmatCount
                   + result.MaskanCount + result.RankMaskanCount + result.EastekhtamCount + result.ShekaiatCount + result.MahkomiatCount
                   + result.MadrakTahsiliCount + result.CourseCount + result.MorakhasiNoUseCount + result.MoseadatAnyMoneyCount + result.OtherCount
                   + result.EbghaCount,
                };
            }
            #endregion



            var Gharaghainfo = _webApiService.GetGharargah();
            ViewData["Gharaghainfo"] = Gharaghainfo;
            
            
            if (Gharargahid != 0)
            {
                var GhaTitel = _webApiService.GetGharargah().Data.Where(_ => _.Id == Gharargahid).Select(_ => _.Title).FirstOrDefault();
                ViewData["Message"] = "گزارش فراوانی مشکلات در سطح " + GhaTitel;
            }
            if (Yeganid != 0)
            {
                var GhaTitel = _webApiService.GetGharargah().Data.Where(_ => _.Id == Gharargahid).Select(_ => _.Title).FirstOrDefault();
                var OmdTitel = _webApiService.GetOrganByGharargahId(Gharargahid).Data.Where(_ => _.Id == Yeganid).Select(_ => _.Title).FirstOrDefault();
                ViewData["Message"] = "گزارش فراوانی مشکلات در سطح  " + GhaTitel + " یگان " + OmdTitel;
            }
           
            return Page();
        }

        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public JsonResult OnGetYegan(int id)
        {
            ViewData["Message"] = "گزارش فراوانی مشکلات در سطح نیروی زمینی";
            ViewData["RequestSubject"] = new SelectList(_fileService.GetRequestSubject(), "Id", "Title");
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["Yegan"] = new SelectList(_webApiService.GetOrganByGharargahId(0).Data, "Id", "Title");
            ViewData["FarmandehInfo"] = new SelectList(_personService.GetFarmandehInfos(), "Id", "Title");

            var result = _webApiService.GetOrganByGharargahId(id).Data;

            return new JsonResult(result);
        }


    }
}

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

namespace VisitorManagment.Web.Pages.Admin.Reports
{
    [Authorize]

    public class RequestGhaReportModel : PageModel
    {
        private readonly IPermissionService _permissionService;
        private readonly IWebApiService _webApiService;

        private readonly IRequestGhaReportService _requestGhaReportService;

        public RequestGhaReportModel(IPermissionService permissionService, IWebApiService webApiService,IRequestGhaReportService requestGhaReportService)
        {
            _permissionService = permissionService;

            _webApiService = webApiService;
            _requestGhaReportService = requestGhaReportService;
        }
        [BindProperty]
       
        //مدل گزارش دایره ای براساس نوع اقدامات
        public List<HameshRequestGhaModel> lstRequestPieModel { get; set; }
        public ReqoestGhaReportViewModal PieDto { get; set; }
        
        //مدل نمودار میله ای گزارش فراوانی مشکلات
        public List<ProblemReportViewModel> lstChartModel { get; set; }     
        public ChartProblemOmdRequestGha chartDto { get; set; }
        public SearchPageReportRequestGhaReportViewModel searchPageReportViewModel { get; set; }

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet()
        {
            searchPageReportViewModel = new SearchPageReportRequestGhaReportViewModel();

            var userId = User.FindFirst("Id").Value;
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["RoleList"] = new SelectList(_permissionService.GetRoles(roleTypeId.ToString()), "RoleId", "Title");

            #region  فراوانی مشکلات از جدول ViwFiles
            //var result = _requestGhaReportService.GetProblemCountOmdOrganInfo(0, 0);

            //lstChartModel = new List<ProblemReportViewModel>();

            //if (result == null)
            //{
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "موردی یافت نشد",
            //        Quantity = 1
            //    });
            //    chartDto = new ChartProblemOmdOrgan()
            //    {
            //        VamCount = 0,
            //        MosaedatCount = 0,
            //        TransferCount = 0,
            //        RahaeiCount = 0,
            //        EbghaCount = 0,
            //        MaskanCount = 0,
            //        RankMaskanCount = 0,
            //        EastekhtamCount = 0,
            //        EadehBeKhetmatCount = 0,
            //        ShekaiatCount = 0,
            //        MahkomiatCount = 0,
            //        MadrakTahsiliCount = 0,
            //        CourseCount = 0,
            //        MorakhasiNoUseCount = 0,
            //        MoseadatAnyMoneyCount = 0,
            //        OtherCount = 0,

            //    };
            //}

            //else
            //{
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست وام",
            //        Quantity = result.VamCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست مساعدت مالی",
            //        Quantity = result.MosaedatCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست انتقال",
            //        Quantity = result.TransferCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست رهایی",
            //        Quantity = result.RahaeiCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست ابقا",
            //        Quantity = result.EbghaCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست مسکن سازمانی",
            //        Quantity = result.MaskanCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست امتیاز پروژه مسکن",
            //        Quantity = result.RankMaskanCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست استخدام",
            //        Quantity = result.EastekhtamCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست اعاده به خدمت",
            //        Quantity = result.EadehBeKhetmatCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست شکایات",
            //        Quantity = result.ShekaiatCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست محکومیت ها",
            //        Quantity = result.MahkomiatCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست مدارک تحصیلی",
            //        Quantity = result.MadrakTahsiliCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست دوره های نظامی",
            //        Quantity = result.CourseCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست مرخصی استفاده نشده",
            //        Quantity = result.MorakhasiNoUseCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست مساعدت غیرمالی",
            //        Quantity = result.MoseadatAnyMoneyCount
            //    });
            //    lstChartModel.Add(new ProblemReportViewModel
            //    {
            //        DimensionOne = "درخواست سایر موارد",
            //        Quantity = result.OtherCount
            //    });

            //    chartDto = new ChartProblemOmdOrgan()
            //    {
            //        VamCount = result.VamCount,
            //        MosaedatCount = result.MosaedatCount,
            //        TransferCount = result.TransferCount,
            //        RahaeiCount = result.RahaeiCount,
            //        EadehBeKhetmatCount = result.EadehBeKhetmatCount,
            //        MaskanCount = result.MaskanCount,
            //        RankMaskanCount = result.RankMaskanCount,
            //        EastekhtamCount = result.EastekhtamCount,
            //        ShekaiatCount = result.ShekaiatCount,
            //        MahkomiatCount = result.MahkomiatCount,
            //        MadrakTahsiliCount = result.MadrakTahsiliCount,
            //        CourseCount = result.CourseCount,
            //        MorakhasiNoUseCount = result.MorakhasiNoUseCount,
            //        MoseadatAnyMoneyCount = result.MoseadatAnyMoneyCount,
            //        OtherCount = result.OtherCount,
            //        EbghaCount = result.EbghaCount,

            //    };
            //}
            #endregion

            #region درخواست قرارگاه و فراوانی مشکلات از جدول ViwHamesh
            var ListresultRequest = _requestGhaReportService.GetTotalHelpAmountForServiceType(0, 0 , null , null);
            lstRequestPieModel = new List<HameshRequestGhaModel>();
            lstChartModel = new List<ProblemReportViewModel>();
            PieDto = new ReqoestGhaReportViewModal();
            chartDto = new ChartProblemOmdRequestGha();

            if (ListresultRequest.Count == 0)
            {
                #region اگر result بر ای درخواست قرارگاه خالی باشد
                lstRequestPieModel.Add(new HameshRequestGhaModel
                {
                    DimensionOne = "موردی یافت نشد",
                    Quantity = 1
                });
                PieDto = new ReqoestGhaReportViewModal()
                {
                    CountEghdam = 0,
                    CountHamesh = 0,
                    CountRequestRegect = 0,
                };
                #endregion

                #region
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "موردی یافت نشد",
                    Quantity = 1
                });
                chartDto = new ChartProblemOmdRequestGha()
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
                #endregion
            }
            else
            {
                foreach (var resultRequest in ListresultRequest)
                {
                    #region درخواست قرارگاه
                    if (resultRequest.ActionTypeId == 1)
                    {
                        PieDto.CountEghdam = PieDto.CountEghdam + resultRequest.TCount;

                    }
                    if (resultRequest.ActionTypeId == 3)
                    {
                        PieDto.CountRequestRegect = PieDto.CountRequestRegect + resultRequest.TCount;
                    }
                    if (resultRequest.ActionTypeId == 2)
                    {
                        PieDto.CountHamesh = PieDto.CountHamesh + resultRequest.TCount;
                    }
                    if (resultRequest.ActionTypeId == 1002)
                    {
                        PieDto.CountEntezar = PieDto.CountEntezar + resultRequest.TCount;
                    }
                    #endregion

                    #region فراوانی مشکلات
                    if (resultRequest.RequestSubjectId == 1)
                    {
                        chartDto.VamCount = chartDto.VamCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 2)
                    {
                        chartDto.MosaedatCount = chartDto.MosaedatCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 3)
                    {
                        chartDto.TransferCount = chartDto.TransferCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 4)
                    {
                        chartDto.RahaeiCount = chartDto.RahaeiCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 5)
                    {
                        chartDto.EbghaCount = chartDto.EbghaCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 6)
                    {
                        chartDto.MaskanCount = chartDto.MaskanCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 7)
                    {
                        chartDto.RankMaskanCount = chartDto.RankMaskanCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 8)
                    {
                        chartDto.EastekhtamCount = chartDto.EastekhtamCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 9)
                    {
                        chartDto.EadehBeKhetmatCount = chartDto.EadehBeKhetmatCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 10)
                    {
                        chartDto.ShekaiatCount = chartDto.ShekaiatCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 11)
                    {
                        chartDto.MahkomiatCount = chartDto.MahkomiatCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 12)
                    {
                        chartDto.MadrakTahsiliCount = chartDto.MadrakTahsiliCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 13)
                    {
                        chartDto.CourseCount = chartDto.CourseCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 14)
                    {
                        chartDto.MorakhasiNoUseCount = chartDto.MorakhasiNoUseCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 15)
                    {
                        chartDto.MoseadatAnyMoneyCount = chartDto.MoseadatAnyMoneyCount + resultRequest.TCount;

                    }
                    if (resultRequest.RequestSubjectId == 16)
                    {
                        chartDto.OtherCount = chartDto.OtherCount + resultRequest.TCount;
                    }
                    #endregion

                }

                #region  lstRequestPieModel Addدرخواست قرارگاه
                // Count All
                PieDto.CountAll = PieDto.CountEghdam + PieDto.CountRequestRegect + PieDto.CountHamesh + PieDto.CountEntezar;
                ////lstRequestPieModel.Add
                lstRequestPieModel.Add(new HameshRequestGhaModel
                {
                    DimensionOne = "اقدام شده",
                    Quantity = PieDto.CountEghdam
                });
                lstRequestPieModel.Add(new HameshRequestGhaModel
                {
                    DimensionOne = "  رد درخواست و عودت",
                    Quantity = PieDto.CountRequestRegect
                });
                lstRequestPieModel.Add(new HameshRequestGhaModel
                {
                    DimensionOne = "ثبت نظریه",
                    Quantity = PieDto.CountHamesh
                });
                lstRequestPieModel.Add(new HameshRequestGhaModel
                {
                    DimensionOne = " در انتظار",
                    Quantity = PieDto.CountEntezar
                });
                #endregion

                #region  lstChartModelAdd فراوانی مشکلات
         
                #region جمع کل فراوانی مشکلات
                chartDto.CountAll = chartDto.VamCount + chartDto.MosaedatCount + chartDto.TransferCount + chartDto.RahaeiCount + chartDto.EbghaCount + chartDto.MaskanCount
                    + chartDto.RankMaskanCount + chartDto.EastekhtamCount + chartDto.EadehBeKhetmatCount + chartDto.ShekaiatCount + chartDto.MahkomiatCount
                    + chartDto.MadrakTahsiliCount + chartDto.CourseCount + chartDto.MorakhasiNoUseCount + chartDto.MoseadatAnyMoneyCount + chartDto.OtherCount;
                #endregion

                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست وام",
                    Quantity = chartDto.VamCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست مساعدت مالی",
                    Quantity = chartDto.MosaedatCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست انتقال",
                    Quantity = chartDto.TransferCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست رهایی",
                    Quantity = chartDto.RahaeiCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست ابقا",
                    Quantity = chartDto.EbghaCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست مسکن سازمانی",
                    Quantity = chartDto.MaskanCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست امتیاز پروژه مسکن",
                    Quantity = chartDto.RankMaskanCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست استخدام",
                    Quantity = chartDto.EastekhtamCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست اعاده به خدمت",
                    Quantity = chartDto.EadehBeKhetmatCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست شکایات",
                    Quantity = chartDto.ShekaiatCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست محکومیت ها",
                    Quantity = chartDto.MahkomiatCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست مدارک تحصیلی",
                    Quantity = chartDto.MadrakTahsiliCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست دوره های نظامی",
                    Quantity = chartDto.CourseCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست مرخصی استفاده نشده",
                    Quantity = chartDto.MorakhasiNoUseCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست مساعدت غیرمالی",
                    Quantity = chartDto.MoseadatAnyMoneyCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست سایر موارد",
                    Quantity = chartDto.OtherCount
                });
                #endregion
            }
            #endregion
        }

        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost(int GharargahId, int RoleId, string startDateSearch = "", string endDateSearch = "")
        {
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["RoleList"] = new SelectList(_permissionService.GetRoles(roleTypeId.ToString()), "RoleId", "Title");

            lstRequestPieModel = new List<HameshRequestGhaModel>();
            lstChartModel = new List<ProblemReportViewModel>();
            PieDto = new ReqoestGhaReportViewModal();
            chartDto = new ChartProblemOmdRequestGha();
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


            searchPageReportViewModel = new SearchPageReportRequestGhaReportViewModel()
            {
                StartDate = startDateSearch,
                EndDate = endDateSearch,
                GharargahId = GharargahId,
                RoleId = RoleId,
            };

            #region درخواست قرارگاه و فراوانی مشکلات  

            var ListresultRequest = _requestGhaReportService.GetTotalHelpAmountForServiceType(GharargahId, RoleId , startDateEnglish , endDateEnglish);

            if (ListresultRequest.Count == 0)
            {
              
                lstRequestPieModel.Add(new HameshRequestGhaModel
                {
                    DimensionOne = "موردی یافت نشد",
                    Quantity = 1
                });

                PieDto = new ReqoestGhaReportViewModal()
                {
                    CountEghdam = 0,
                    CountHamesh = 0,
                    CountRequestRegect = 0,
                };

        
                #region
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "موردی یافت نشد",
                    Quantity = 1
                });

                chartDto = new ChartProblemOmdRequestGha()
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
                #endregion
            }

            else
            {
                foreach (var resultRequest in ListresultRequest)
                {

                    if (resultRequest.ActionTypeId == 1)
                    {
                        PieDto.CountEghdam = PieDto.CountEghdam + resultRequest.TCount;
                      
                    }
                    if (resultRequest.ActionTypeId == 2)
                    {
                        PieDto.CountHamesh = PieDto.CountHamesh + resultRequest.TCount;
                    }
                    if (resultRequest.ActionTypeId == 3)
                    {
                        PieDto.CountRequestRegect = PieDto.CountRequestRegect + resultRequest.TCount;
                    }
                    if (resultRequest.ActionTypeId == 1002)
                    {
                        PieDto.CountEntezar = PieDto.CountEntezar + resultRequest.TCount;
                    }
                    
      
                    #region فراوانی مشکلات همون موضوع درخواست های ملاقات
                    if (resultRequest.RequestSubjectId == 1)
                    {
                        chartDto.VamCount = chartDto.VamCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 2)
                    {
                        chartDto.MosaedatCount = chartDto.MosaedatCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 3)
                    {
                        chartDto.TransferCount = chartDto.TransferCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 4)
                    {
                        chartDto.RahaeiCount = chartDto.RahaeiCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 5)
                    {
                        chartDto.EbghaCount = chartDto.EbghaCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 6)
                    {
                        chartDto.MaskanCount = chartDto.MaskanCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 7)
                    {
                        chartDto.RankMaskanCount = chartDto.RankMaskanCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 8)
                    {
                        chartDto.EastekhtamCount = chartDto.EastekhtamCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 9)
                    {
                        chartDto.EadehBeKhetmatCount = chartDto.EadehBeKhetmatCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 10)
                    {
                        chartDto.ShekaiatCount = chartDto.ShekaiatCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 11)
                    {
                        chartDto.MahkomiatCount = chartDto.MahkomiatCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 12)
                    {
                        chartDto.MadrakTahsiliCount = chartDto.MadrakTahsiliCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 13)
                    {
                        chartDto.CourseCount = chartDto.CourseCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 14)
                    {
                        chartDto.MorakhasiNoUseCount = chartDto.MorakhasiNoUseCount + resultRequest.TCount;
                    }
                    if (resultRequest.RequestSubjectId == 15)
                    {
                        chartDto.MoseadatAnyMoneyCount = chartDto.MoseadatAnyMoneyCount + resultRequest.TCount;

                    }
                    if (resultRequest.RequestSubjectId == 16)
                    {
                        chartDto.OtherCount = chartDto.OtherCount + resultRequest.TCount;
                    }
                 
                    #endregion
            
                    ViewData["GharagaAndRoleInfo"] = resultRequest.CodGhaTitle;
                }

                #region  درخواست ها براساس قرارگاه و نوع درخواست های آنها

                // تعداد کل براساس نوع اقدامات
                PieDto.CountAll = PieDto.CountEghdam + PieDto.CountRequestRegect + PieDto.CountHamesh + PieDto.CountEntezar;

                lstRequestPieModel.Add(new HameshRequestGhaModel
                {
                    DimensionOne = "اقدام شده",
                    Quantity = PieDto.CountEghdam
                });
                lstRequestPieModel.Add(new HameshRequestGhaModel
                {
                    DimensionOne = "  رد درخواست و عودت",
                    Quantity = PieDto.CountRequestRegect
                });
                lstRequestPieModel.Add(new HameshRequestGhaModel
                {
                    DimensionOne = "ثبت نظریه",
                    Quantity = PieDto.CountHamesh
                });
                lstRequestPieModel.Add(new HameshRequestGhaModel
                {
                    DimensionOne = " در انتظار",
                    Quantity = PieDto.CountEntezar
                });
                #endregion

                #region   فراوانی مشکلات
            
                #region جمع کل فراوانی مشکلات
                chartDto.CountAll = chartDto.VamCount + chartDto.MosaedatCount + chartDto.TransferCount + chartDto.RahaeiCount + chartDto.EbghaCount + chartDto.MaskanCount
                    + chartDto.RankMaskanCount + chartDto.EastekhtamCount + chartDto.EadehBeKhetmatCount + chartDto.ShekaiatCount + chartDto.MahkomiatCount
                    + chartDto.MadrakTahsiliCount + chartDto.CourseCount + chartDto.MorakhasiNoUseCount + chartDto.MoseadatAnyMoneyCount + chartDto.OtherCount;
                #endregion

                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست وام",
                    Quantity = chartDto.VamCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست مساعدت مالی",
                    Quantity = chartDto.MosaedatCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست انتقال",
                    Quantity = chartDto.TransferCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست رهایی",
                    Quantity = chartDto.RahaeiCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست ابقا",
                    Quantity = chartDto.EbghaCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست مسکن سازمانی",
                    Quantity = chartDto.MaskanCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست امتیاز پروژه مسکن",
                    Quantity = chartDto.RankMaskanCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست استخدام",
                    Quantity = chartDto.EastekhtamCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست اعاده به خدمت",
                    Quantity = chartDto.EadehBeKhetmatCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست شکایات",
                    Quantity = chartDto.ShekaiatCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست محکومیت ها",
                    Quantity = chartDto.MahkomiatCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست مدارک تحصیلی",
                    Quantity = chartDto.MadrakTahsiliCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست دوره های نظامی",
                    Quantity = chartDto.CourseCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست مرخصی استفاده نشده",
                    Quantity = chartDto.MorakhasiNoUseCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست مساعدت غیرمالی",
                    Quantity = chartDto.MoseadatAnyMoneyCount
                });
                lstChartModel.Add(new ProblemReportViewModel
                {
                    DimensionOne = "درخواست سایر موارد",
                    Quantity = chartDto.OtherCount
                });
                #endregion
            }
            #endregion

            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["RoleList"] = new SelectList(_permissionService.GetRoles(roleTypeId.ToString()), "RoleId", "Title");
            return Page();

        }
    }

}

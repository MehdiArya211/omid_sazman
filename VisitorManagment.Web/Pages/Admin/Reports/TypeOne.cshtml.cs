using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Admin.Reports
{
    [Authorize]
    public class TypeOneModel : PageModel
    {
        private readonly IFileService _fileService;
        private readonly IWorkFlowService _workFlowService;
        private readonly ICartableService _cartableService;
        private readonly IHameshService _hameshService;
        private readonly IWebApiService _webApiService;
        private readonly IUserService _userService;
        private readonly IChartService _chartService;
        //Guid sessionFileId = new Guid();
        public TypeOneModel(IFileService fileService, IWorkFlowService workFlowService, ICartableService cartableService,
            IHameshService hameshService, IWebApiService webApiService, IUserService userService,IChartService chartService)
        {
            _fileService = fileService;
            _workFlowService = workFlowService;
            _cartableService = cartableService;
            _hameshService = hameshService;
            _webApiService = webApiService;
            _userService = userService;
            _chartService = chartService;
        }
        [BindProperty]
        public List<Chart> BarLineDataSet { get; set; }

        public IActionResult OnGet(int pageId = 1, string filterCaption = "", int requestsubject = 0, int filterGharargah = 0, int filterYegan = 0, string startDateSearch = "", string endDateSearch = "")
        {


            var userid = int.Parse(User.FindFirst("Id").Value);
            var unitCode = int.Parse(User.FindFirst("UnitCode").Value);
            ViewData["RequestSubject"] = new SelectList(_fileService.GetRequestSubject(), "Id", "Title");
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["Yegan"] = new SelectList(_webApiService.GetOrganByGharargahId(filterGharargah).Data, "Id", "Title");
            ViewData["ListMoavenatHa"] = new SelectList(_fileService.GetRolesJustMooavenatHa(), "RoleId", "Title");

            #region Convert string date time to english date time
            DateTime? startDateEnglish = null;
            DateTime? endDateEnglish = null;
            //string stEnghlish="";
            //string enEnghlish="";
            //string startDate = "";
            //string endDate = "";
            if (startDateSearch != "" && endDateSearch != "")
            {
                //زمان شروع رو با / جدا میکنه
                var startDate = startDateSearch.Split('/');
                //سال و ماه و روز تاریخ شروع رو به اینت تبدیل میکنیم
                var yearstart = Int32.Parse(string.Join("", startDate[0].Select(c => char.GetNumericValue(c))));
                var monthstart = Int32.Parse(string.Join("", startDate[1].Select(c => char.GetNumericValue(c))));
                var daystart = Int32.Parse(string.Join("", startDate[2].Select(c => char.GetNumericValue(c))));
                //زمان شروع شمسی رو به میلادی تبدیل میکنیم
                startDateEnglish = new DateTime(yearstart, monthstart, daystart, new PersianCalendar());
                //stEnghlish = new DateTime(yearstart, monthstart, daystart, new PersianCalendar()).ToShortDateString();
                //DateTime t1 = startDateEnglish.ToSortDateString();
                //خط زیرم معادل خط بالاس
                //var startDateEnglish = DateTime.Parse(allstartDate.ToString(CultureInfo.CreateSpecificCulture("en-Us")));
                //********************************************************************************************************************//
                var endDate = endDateSearch.Split('/');
                var yearend = Int32.Parse(string.Join("", endDate[0].Select(c => char.GetNumericValue(c))));
                var monthend = Int32.Parse(string.Join("", endDate[1].Select(c => char.GetNumericValue(c))));
                var dayend = Int32.Parse(string.Join("", endDate[2].Select(c => char.GetNumericValue(c))));
                endDateEnglish = new DateTime(yearend, monthend, dayend, new PersianCalendar());
                //enEnghlish = new DateTime(yearend, monthend, dayend, new PersianCalendar()).ToShortDateString();

                if (startDateEnglish > endDateEnglish)
                {
                    ViewData["startDateBiggerThanEndDate"] = "true";
                    return Page();

                }

            }
            //حالا سرچ بر اساس تاریخ رو بنویس

            #endregion


            var listGharargah = _webApiService.GetGharargah().Data;
            var organ = _webApiService.GetAllOrgan().Data;

            #region Chart
            var report = _chartService.ShowReportOne();

            BarLineDataSet = new List<Chart>();

            var BarLineDataSetMe = new List<Chart>();
            

                foreach (var item in report.Files)
                {
                    new Chart
                    {
                        Label =item.CodeGhaTitle,
                        //تعداد درخواست ها باید بیاد اینجا درستش شکنم
                        Data = new List<int> { 100, 200, 250, 170, 50 },
                        BackgroundColor = new[] { "#ffcdb2" },
                        BorderColor = "#b5838d"
                    };

            //    BarLineDataSet.Add();
                }

            
            
         

            
 

            #endregion



            return Page();
        }

        
        public JsonResult OnGetYegan(int id)
        {
            ViewData["RequestSubject"] = new SelectList(_fileService.GetRequestSubject(), "Id", "Title");
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            //ViewData["Yegan"] = new SelectList(_webApiService.GetOrganByGharargahId(0).Data, "Id", "Title");
            ViewData["ListMoavenatHa"] = new SelectList(_fileService.GetRolesJustMooavenatHa(), "RoleId", "Title");

            var result = _webApiService.GetOrganByGharargahId(id).Data;

            return new JsonResult(result);
        }

        public JsonResult OnGetAdvanceSearch( int requestsubject = 0, int filterGharargah = 0, int filterYegan = 0, string startDateSearch = "", string endDateSearch = "")
        {
            ViewData["RequestSubject"] = new SelectList(_fileService.GetRequestSubject(), "Id", "Title");
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
           // ViewData["Yegan"] = new SelectList(_webApiService.GetOrganByGharargahId(filterGharargah).Data, "Id", "Title");
            ViewData["ListMoavenatHa"] = new SelectList(_fileService.GetRolesJustMooavenatHa(), "RoleId", "Title");

            var result ="";

            return new JsonResult(result);
        }
    }
}

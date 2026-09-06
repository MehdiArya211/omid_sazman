using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ITOWebApiClient.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Web.Pages.Visitor.ViewRequestCirculation
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IFileService _fileService;
        private readonly IWorkFlowService _workFlowService;
        private readonly ICartableService _cartableService;
        private readonly IHameshService _hameshService;
        private readonly IWebApiService _webApiService;
        private readonly IUserService _userService;
        //Guid sessionFileId = new Guid();
        public IndexModel(IFileService fileService, IWorkFlowService workFlowService, ICartableService cartableService, IHameshService hameshService, IWebApiService webApiService , IUserService userService)
        {
            _fileService = fileService;
            _workFlowService = workFlowService;
            _cartableService = cartableService;
            _hameshService = hameshService;
            _webApiService = webApiService;
            _userService = userService;
        }
        [BindProperty]
        public ListFileViewModel listviewmodel { get; set; }
        public List<Users> users { get; set; }

        /// <summary>
        /// هامش های گردش درخواست
        /// </summary>
        public ListHameshViewModel listHameshViewModel { get; set; }

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public IActionResult OnGet(int pageId = 1, string filterCaption = "", int requestsubject = 0, int filterGharargah = 0, int filterYegan = 0 , string startDateSearch = "", string endDateSearch = "")
        {

            listHameshViewModel = new ListHameshViewModel();
            listHameshViewModel.hameshes = new List<HameshInfoViewModel>();
            var userid = int.Parse(User.FindFirst("Id").Value);
            var unitCode = int.Parse(User.FindFirst("UnitCode").Value);
            ViewData["RequestSubject"] = new SelectList(_fileService.GetRequestSubject(), "Id", "Title");
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["Yegan"] = new SelectList(_webApiService.GetOrganByGharargahId(filterGharargah).Data, "Id", "Title");
            ViewData["ListMoavenatHa"] = new SelectList(_fileService.GetRolesJustMooavenatHa(), "RoleId", "Title");

            #region Convert string date time to english date time
            DateTime? startDateEnglish=null;
            DateTime? endDateEnglish=null;

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


            //listviewmodel = _fileService.GetListFileForCirculation(userid, unitCode, pageId, requestsubject, filterGharargah, filterYegan, filterCaption, stEnghlish, enEnghlish);
            listviewmodel = _fileService.GetListFileForCirculation(userid, startDateEnglish, endDateEnglish, unitCode, pageId, requestsubject, filterGharargah, filterYegan , filterCaption);
            users = new List<Users>();

            return Page();
        }

        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public JsonResult OnGetYegan(int id)
        {
            var result = _webApiService.GetOrganByGharargahId(id).Data;

            return new JsonResult(result);
        }

        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public JsonResult OnGetGetFileInfo(int fileid)
        {
            var fileId = _fileService.GetFile(fileid);
            return new JsonResult(fileId);
        }




    }
}

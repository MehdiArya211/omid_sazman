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
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Web.Pages.Visitor.File.AdminViewRequestCirculation
{
    [Authorize]
    public class ListFileForAdminsModel : PageModel
    {
        private readonly IFileService _fileService;
        private readonly IWorkFlowService _workFlowService;
        private readonly ICartableService _cartableService;
        private readonly IHameshService _hameshService;
        private readonly IWebApiService _webApiService;
        //Guid sessionFileId = new Guid();
        public ListFileForAdminsModel(IFileService fileService, IWorkFlowService workFlowService, ICartableService cartableService, IHameshService hameshService, IWebApiService webApiService)
        {
            _fileService = fileService;
            _workFlowService = workFlowService;
            _cartableService = cartableService;
            _hameshService = hameshService;
            _webApiService = webApiService;
        }
        [BindProperty]
        public ListFileViewModel listviewmodel { get; set; }
        public List<Users> users { get; set; }

        public void OnGet(int pageId = 1, string filterCaption = "", int requestsubject = 0, int filterGharargah = 0, int filterYegan = 0, int filterMoavenat = 0, string startDateSearch = "", string endDateSearch = "")
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            var unitCode = int.Parse(User.FindFirst("Id").Value);
            ViewData["RequestSubject"] = new SelectList(_fileService.GetRequestSubject(), "Id", "Title");
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["Yegan"] = new SelectList(_webApiService.GetOrganByGharargahId(filterGharargah).Data, "Id", "Title");
            ViewData["ListMoavenatHa"] = new SelectList(_fileService.GetRolesJustMooavenatHa(), "RoleId", "Title");
            #region Convert string date time to english date time
            DateTime? startDateEnglish = null;
            DateTime? endDateEnglish = null;

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

                //********************************************************************************************************************//
                var endDate = endDateSearch.Split('/');
                var yearend = Int32.Parse(string.Join("", endDate[0].Select(c => char.GetNumericValue(c))));
                var monthend = Int32.Parse(string.Join("", endDate[1].Select(c => char.GetNumericValue(c))));
                var dayend = Int32.Parse(string.Join("", endDate[2].Select(c => char.GetNumericValue(c))));
                endDateEnglish = new DateTime(yearend, monthend, dayend, new PersianCalendar());

                if (startDateEnglish > endDateEnglish)
                {
                    ViewData["startDateBiggerThanEndDate"] = "true";
                    //return Page();

                }

            }
            //حالا سرچ بر اساس تاریخ رو بنویس

            #endregion
            if (filterMoavenat!=0)
            {
              //  var filterMoavenatTitle = _fileService.GetRoleTitleByRoleType(filterMoavenat).Title;
                listviewmodel = _fileService.GetListFileForCirculationAdmin(userId, startDateEnglish, endDateEnglish, unitCode, pageId, requestsubject, filterGharargah, filterYegan, filterMoavenat, filterCaption);
            }

            else
            {
                listviewmodel = _fileService.GetListFileForCirculationAdmin(userId, startDateEnglish, endDateEnglish, unitCode, pageId, requestsubject, filterGharargah, filterYegan, filterMoavenat, filterCaption);
            }
           
           
            users = new List<Users>();
        }

        public JsonResult OnGetYegan(int id)
        {
            var result = _webApiService.GetOrganByGharargahId(id);

            return new JsonResult(result.Data.Select(c => new { c.Id, c.Title }));
        }
    }
}

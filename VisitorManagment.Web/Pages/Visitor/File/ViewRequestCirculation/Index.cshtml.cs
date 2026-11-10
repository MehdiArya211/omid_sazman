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
        public IActionResult OnGet(int pageId = 1, string filterCaption = "", int requestsubject = 0,
            int filterGharargah = 0, int filterYegan = 0, string startDateSearch = "", string endDateSearch = "")
        {
            listHameshViewModel = new ListHameshViewModel
            {
                hameshes = new List<HameshInfoViewModel>()
            };

            if (!int.TryParse(User.FindFirst("Id")?.Value, out var userId) ||
                !int.TryParse(User.FindFirst("UnitCode")?.Value, out var unitCode))
            {
                return Forbid();
            }

            ViewData["RequestSubject"] = new SelectList(_fileService.GetRequestSubject(), "Id", "Title");
            ViewData["Gharargah"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["Yegan"] = new SelectList(_webApiService.GetOrganByGharargahId(filterGharargah).Data, "Id", "Title");
            ViewData["ListMoavenatHa"] = new SelectList(_fileService.GetRolesJustMooavenatHa(), "RoleId", "Title");

            DateTime? startDateEnglish = null;
            DateTime? endDateEnglish = null;
            var hasStartDate = !string.IsNullOrWhiteSpace(startDateSearch);
            var hasEndDate = !string.IsNullOrWhiteSpace(endDateSearch);

            if (hasStartDate != hasEndDate)
            {
                ViewData["invalidDateRange"] = "true";
                listviewmodel = new ListFileViewModel { files = new List<FactPersonalViewModel>() };
                return Page();
            }

            if (hasStartDate && hasEndDate)
            {
                if (!TryParsePersianDate(startDateSearch, out var parsedStart) ||
                    !TryParsePersianDate(endDateSearch, out var parsedEnd))
                {
                    ViewData["invalidDateRange"] = "true";
                    listviewmodel = new ListFileViewModel { files = new List<FactPersonalViewModel>() };
                    return Page();
                }

                startDateEnglish = parsedStart;
                endDateEnglish = parsedEnd;
                if (startDateEnglish > endDateEnglish)
                {
                    ViewData["startDateBiggerThanEndDate"] = "true";
                    listviewmodel = new ListFileViewModel { files = new List<FactPersonalViewModel>() };
                    return Page();
                }
            }

            listviewmodel = _fileService.GetListFileForCirculation(
                userId, startDateEnglish, endDateEnglish, unitCode, pageId,
                requestsubject, filterGharargah, filterYegan, filterCaption);
            users = new List<Users>();
            return Page();
        }

        private static bool TryParsePersianDate(string value, out DateTime date)
        {
            date = default;
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var normalized = new string(value.Trim().Select(character =>
            {
                if (character >= '\u06F0' && character <= '\u06F9')
                    return (char)('0' + character - '\u06F0');
                if (character >= '\u0660' && character <= '\u0669')
                    return (char)('0' + character - '\u0660');
                return character;
            }).ToArray());

            var parts = normalized.Split('/');
            if (parts.Length != 3 ||
                !int.TryParse(parts[0], out var year) ||
                !int.TryParse(parts[1], out var month) ||
                !int.TryParse(parts[2], out var day))
                return false;

            try
            {
                date = new DateTime(year, month, day, new PersianCalendar());
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
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

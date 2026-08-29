using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Web.Pages.Visitor.File.Archived
{
    [Authorize]
    public class ListArchivedFileModel : PageModel
    {
        #region Ctor
        private readonly IFileService _fileService;
        private readonly IWorkFlowService _workFlowService;
        private readonly ICartableService _cartableService;
        private readonly IHameshService _hameshService;
        private readonly IWebApiService _webApiService;
        public ListArchivedFileModel(IFileService fileService, IWorkFlowService workFlowService, ICartableService cartableService,
            IHameshService hameshService, IWebApiService webApiService)
        {
            _fileService = fileService;
            _workFlowService = workFlowService;
            _cartableService = cartableService;
            _hameshService = hameshService;
            _webApiService = webApiService;
        }
        #endregion


        [BindProperty]
        public ListFileViewModel listviewmodel { get; set; }
        public List<Users> users { get; set; }

        public ListHameshViewModel listHameshViewModel { get; set; }
        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet(int pageId = 1, string filterCaption = "", int requestsubject = 0, int filterAvamerSadereh = 0, string filterGharargah = "")
        {
            users = new List<Users>();
            listHameshViewModel = new ListHameshViewModel();
            var userid = int.Parse(User.FindFirst("Id").Value);
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);
            ViewData["RequestSubject"] = new SelectList(_fileService.GetRequestSubject(), "Id", "Title");
            ViewData["GharargahList"] = new SelectList(_webApiService.GetGharargah().Data, "Id", "Title");
            ViewData["PersonalCodeUserLogined"] = User.FindFirst("UserName").Value;
            ViewData["ListAvamerSadereh"] = new SelectList(_fileService.GetAvamerSadereh(), "Id", "Title");
            ViewData["roleTypeId"] = _hameshService.GetRoleTypePerson(int.Parse(userid.ToString())).RoleTypeId;


            listviewmodel = _fileService.GetListArchivedFile(userid , requestsubject, filterAvamerSadereh, filterGharargah, filterCaption);
            ViewData["FileCount"] = listviewmodel.files.Count;

        }

        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public JsonResult OnGetGetFileInfo(int fileid)
        {
            var fileId = _fileService.GetFile(fileid);
            return new JsonResult(fileId);
        }


        #region هامش های ثبت شده برای درخواست ملاقات

        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public IActionResult OnGetHamesh(int fileId)
        {
            listHameshViewModel = _hameshService.GetHameshIdByFileId(fileId);
            return new PartialViewResult
            {
                ViewName = "_GardeshDarkhast",
                ViewData = new ViewDataDictionary<ListHameshViewModel>(ViewData, listHameshViewModel)
            };

        }
        #endregion
    }
}

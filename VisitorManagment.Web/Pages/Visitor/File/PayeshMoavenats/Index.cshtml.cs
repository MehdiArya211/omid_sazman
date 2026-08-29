using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Web.Pages.Visitor.File.PayeshMoavenats
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IFileService _fileService;
        private readonly IWorkFlowService _workFlowService;
        private readonly ICartableService _cartableService;
        private readonly IHameshService _hameshService;
        private readonly IWebApiService _webApiService;
        //Guid sessionFileId = new Guid();
        public IndexModel(IFileService fileService, IWorkFlowService workFlowService, ICartableService cartableService, IHameshService hameshService, IWebApiService webApiService)
        {
            _fileService = fileService;
            _workFlowService = workFlowService;
            _cartableService = cartableService;
            _hameshService = hameshService;
            _webApiService = webApiService;
        }
        [BindProperty]
        public ListCountCartableMoavenat listCountCartableMoavenat { get; set; }
        public ListFileViewModel listviewmodel { get; set; }
        public List<Users> users { get; set; }

        public ListHameshViewModel listHameshViewModel { get; set; }

        public void OnGet( int filterMoavenat = 0)
        {
            listHameshViewModel = new ListHameshViewModel();
            listCountCartableMoavenat = _fileService.GetListCountCartableMoavenat();
            listviewmodel =_fileService.GetListFileWithoutFilter(); 
            var userId = int.Parse(User.FindFirst("Id").Value);
            var unitCode = int.Parse(User.FindFirst("Id").Value);
            ViewData["ListMoavenatHa"] = new SelectList(_fileService.GetRolesJustMooavenatHa(), "RoleId", "Title");
         
            if (filterMoavenat != 0)
            {
               
                listviewmodel = _fileService.GetListFileForPayeshMoavenat(filterMoavenat);
                listviewmodel.MoavenatId = filterMoavenat;
            }

            users = new List<Users>();
        }

        #region هامش های ثبت شده برای درخواست ملاقات

        public IActionResult OnGetHamesh(int fileId)
        {
            ViewData["ListMoavenatHa"] = new SelectList(_fileService.GetRolesJustMooavenatHa(), "RoleId", "Title");

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

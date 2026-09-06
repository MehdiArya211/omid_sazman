using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Visitor.File.MeetingInfo.MeetingHold.HameshInfoMeetingHold
{
    [Authorize]
    public class ListPervHameshModel : PageModel
    {

        private readonly IFileService _fileService;
        private readonly IWebApiService _webApiService;
        private readonly ICartableService _cartableService;
        private readonly IUserService _userService;
        private readonly IHameshService _hameshService;
        public ListPervHameshModel(IFileService fileService, IWebApiService webApiService, ICartableService cartableService, IUserService userService, IHameshService hameshService)
        {
            _fileService = fileService;
            _webApiService = webApiService;
            _cartableService = cartableService;
            _userService = userService;
            _hameshService = hameshService;
        }

        [BindProperty]
        public ListHameshViewModel listHameshViewModel { get; set; }

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet(int id, int pageId = 1, int filterMeetingStatus = 1, string filterCaption = "")
        {
            listHameshViewModel = _hameshService.GetHameshIdByFileId(id, pageId, filterMeetingStatus, filterCaption);
            ViewData["meetingId"] = listHameshViewModel.MeetingId;
        }
    }
}

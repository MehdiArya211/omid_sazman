using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace VisitorManagment.Web.Pages.Admin.Users
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private IHameshService _hameshService;

        public IndexModel(IUserService userService , IHameshService hameshService)
        {
            _userService = userService;
            _hameshService = hameshService;
        }

        public UserForAdminViewModel UserForAdminViewModel { get; set; }

       

        public IActionResult OnGet(int pageId = 1, string filterUserName = "", string filterEmail = "")
        {
            //کاربر غیر ادمین صفحه رو بهش نشون نده

            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);

            //if (roleTypeId != 100 && roleTypeId != 101 && roleTypeId != 102)
            //{
            //    return NotFound();
            //}

            var userIdLoggin = User.FindFirst("Id").Value;

            UserForAdminViewModel = _userService.GetUsers(roleTypeId , userIdLoggin, pageId, filterEmail, filterUserName );

            return Page();
        }





    }
}

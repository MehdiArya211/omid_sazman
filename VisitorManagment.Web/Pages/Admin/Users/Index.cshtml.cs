using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using VisitorManagment.Web.Helpers;

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

       

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
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

        /// <summary>
        /// رمز کاربر را به کد پرسنلی او بازنشانی می‌کند و تغییر رمز در اولین ورود را اجباری می‌سازد.
        /// این عملیات صرفاً برای مدیر کل سامانه در دسترس است.
        /// </summary>
        public IActionResult OnPostResetPassword(int userId)
        {
            if (!User.IsSystemAdministrator()) return Forbid();

            var resetSucceeded = _userService.ResetPasswordToPersonnelCode(userId);
            TempData["OperationTitle"] = resetSucceeded ? "بازنشانی موفق" : "بازنشانی ناموفق";
            TempData["OperationMessage"] = resetSucceeded
                ? "رمز موقت کاربر برابر کد پرسنلی او شد و در اولین ورود باید آن را تغییر دهد."
                : "کاربر موردنظر یافت نشد یا امکان بازنشانی رمز او وجود ندارد.";
            TempData["OperationIcon"] = resetSucceeded ? "success" : "error";
            return RedirectToPage();
        }





    }
}

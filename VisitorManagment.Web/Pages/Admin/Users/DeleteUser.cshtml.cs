using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VisitorManagment.Web.Pages.Admin.Users
{
    [Authorize]
    public class DeleteUserModel : PageModel
    {
        private IUserService _userService;
        private IHameshService _hameshService;
        private IWebApiService _webApiService;

        public DeleteUserModel(IUserService userService , IHameshService hameshService,
            IWebApiService webApiService)
        {
            _userService = userService;
            _hameshService = hameshService;
            _webApiService = webApiService;
        }

        public InformationUserViewModel InformationUserViewModel { get; set; }
        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public IActionResult OnGet(int id)
        {
            //کاربر غیر ادمین صفحه رو بهش نشون نده

            var userId = User.FindFirst("Id").Value;
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);

            if (roleTypeId != 100 && roleTypeId != 101 && roleTypeId != 102)
            {
                return NotFound();
            }
            ViewData["UserId"] = id;
            InformationUserViewModel = _userService.GetUserInformation(id);

            return Page();
        }

        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost(int UserId)
        {
            if (UserId <= 0)
            {
                TempData["OperationTitle"] = "خطا در حذف";
                TempData["OperationMessage"] = "شناسه کاربر معتبر نیست.";
                TempData["OperationIcon"] = "error";
                return RedirectToPage("Index");
            }

            _userService.DeleteUser(UserId);
            #region لاگ ، سامانه فجر
            var userName = User.FindFirst("UserName").Value;
            string UserIdLog = User.FindFirst("Id").Value;
           // _webApiService.AddLog(UserIdLog, userName, "Admin/Users/DeleteUser");
            #endregion
            TempData["OperationTitle"] = "حذف موفق";
            TempData["OperationMessage"] = "کاربر با موفقیت حذف شد.";
            TempData["OperationIcon"] = "success";
            return RedirectToPage("Index");
        }
    }
}

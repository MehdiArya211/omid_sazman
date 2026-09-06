using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.Web.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IUserService _userService;
        private readonly VisitorManagmentContext _context;
        private readonly IWebApiService _webApiService;


        public ChangePasswordModel(ILogger<IndexModel> logger, IUserService userService, VisitorManagmentContext context, IWebApiService webApiService)
        {
            _logger = logger;
            _userService = userService;
            _context = context;
            _webApiService = webApiService;

        }

        [BindProperty]
        public ForgetPasswordViewModel forgetPasswordViewModel { get; set; }
        #region اعضا و متدهای کلاس


        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public void OnGet()
        {

        }

        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            if (forgetPasswordViewModel.Password != forgetPasswordViewModel.RePassword)
            {
                ModelState.AddModelError("RePassword", "کلمه عبور با تکرار کلمه عبور مطابقت ندارد!");
                return Page();
            }

            var result = _userService.ForgetPassword(forgetPasswordViewModel);

            if (result==true)
            {
                ViewData["successChangePassword"] = true;
                return Page();
            }
            else
            {
                ModelState.AddModelError("UserName", "نام کاربری وارد شده وجود ندارد!");
            }

            return Page();
        }
        #endregion
    }
}

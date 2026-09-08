using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages
{
    /// <summary>
    /// صفحه تغییر اجباری رمز پس از بازنشانی حساب توسط مدیر سامانه.
    /// </summary>
    [Authorize]
    public class ChangePasswordRequiredModel : PageModel
    {
        #region Fields and constructor

        private readonly IUserService _userService;

        public ChangePasswordRequiredModel(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        #region Properties

        [BindProperty]
        public RequiredPasswordChangeViewModel Input { get; set; }

        #endregion

        #region Page handlers

        /// <summary>
        /// مطمئن می‌شود فقط کاربری که واقعاً نیازمند تغییر رمز است این صفحه را مشاهده کند.
        /// </summary>
        public IActionResult OnGet()
        {
            var userId = GetCurrentUserId();
            return userId > 0 && _userService.IsPasswordChangeRequired(userId)
                ? Page()
                : RedirectToPage("/Visitor/Index");
        }

        /// <summary>
        /// رمز موقت و پیچیدگی رمز جدید را کنترل می‌کند و پس از موفقیت، نشست قبلی را خاتمه می‌دهد.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var userId = GetCurrentUserId();
            if (userId <= 0 || !_userService.ChangeRequiredPassword(userId, Input.CurrentPassword, Input.NewPassword))
            {
                ModelState.AddModelError(string.Empty, "رمز موقت صحیح نیست یا امکان تغییر رمز وجود ندارد.");
                return Page();
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["PasswordChanged"] = "رمز عبور با موفقیت تغییر کرد؛ اکنون با رمز جدید وارد شوید.";
            return RedirectToPage("/Index");
        }

        #endregion

        #region Helpers

        private int GetCurrentUserId()
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : 0;
        }

        #endregion
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Security;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.Web.Pages
{
    /// <summary>تغییر رمز توسط صاحب حساب، پس از بررسی رمز فعلی و پیچیدگی رمز جدید.</summary>
    [Authorize]
    public class AccountPasswordModel : PageModel
    {
        private readonly VisitorManagmentContext _context;
        public AccountPasswordModel(VisitorManagmentContext context) { _context = context; }
        [BindProperty] public RequiredPasswordChangeViewModel Input { get; set; } = new RequiredPasswordChangeViewModel();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (!int.TryParse(User.FindFirst("Id")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var id))
                return Challenge();
            var user = _context.Users.SingleOrDefault(item => item.Id == id && item.IsActive && !item.IsDelete);
            if (user == null || user.Password != PasswordHelper.EncodePasswordMd5(Input.CurrentPassword))
            {
                ModelState.AddModelError("", "رمز فعلی صحیح نیست.");
                return Page();
            }
            if (Input.NewPassword == Input.CurrentPassword || Input.NewPassword == user.UserName)
            {
                ModelState.AddModelError("", "رمز جدید نباید با رمز فعلی یا نام کاربری یکسان باشد.");
                return Page();
            }
            user.Password = PasswordHelper.EncodePasswordMd5(Input.NewPassword);
            user.ActiveCode = Guid.NewGuid().ToString("N");
            user.EditDate = DateTime.Now;
            await _context.SaveChangesAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["PasswordChanged"] = "رمز تغییر کرد؛ با رمز جدید وارد شوید.";
            return RedirectToPage("/Index");
        }
    }
}

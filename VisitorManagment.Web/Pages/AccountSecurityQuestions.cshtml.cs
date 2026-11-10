using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.Security;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages
{
    [Authorize]
    public class AccountSecurityQuestionsModel : PageModel
    {
        private readonly IUserService _userService;
        public AccountSecurityQuestionsModel(IUserService userService) => _userService = userService;

        [BindProperty] public string CurrentPassword { get; set; }
        [BindProperty] public string Answer1 { get; set; }
        [BindProperty] public string Answer2 { get; set; }
        [BindProperty] public string Answer3 { get; set; }

        public IActionResult OnGet()
        {
            if (!TryGetUserId(out var userId)) return Challenge();
            if (_userService.HasSecurityAnswers(userId))
                return RedirectToPage("/Visitor/Index");

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!TryGetUserId(out var userId)) return Challenge();
            if (string.IsNullOrWhiteSpace(CurrentPassword))
                ModelState.AddModelError(nameof(CurrentPassword), "رمز فعلی را وارد کنید.");
            ValidateAnswer(Answer1, nameof(Answer1));
            ValidateAnswer(Answer2, nameof(Answer2));
            ValidateAnswer(Answer3, nameof(Answer3));
            if (!ModelState.IsValid) { ClearAnswers(); return Page(); }

            if (!_userService.SaveSecurityAnswers(userId, CurrentPassword, Answer1, Answer2, Answer3))
            {
                ModelState.AddModelError(string.Empty, "رمز فعلی صحیح نیست یا ذخیرهٔ سؤال‌ها انجام نشد.");
                ClearAnswers();
                return Page();
            }

            TempData["SecurityQuestionsSaved"] = "پاسخ‌های امنیتی با موفقیت ذخیره شد.";
            return RedirectToPage();
        }

        private void ValidateAnswer(string answer, string key)
        {
            var normalized = SecurityAnswerHasher.Normalize(answer);
            if (normalized.Length < 2 || normalized.Length > 100)
                ModelState.AddModelError(key, "پاسخ باید بین ۲ تا ۱۰۰ نویسه باشد.");
        }

        private void ClearAnswers()
        {
            Answer1 = Answer2 = Answer3 = null;
            ModelState.Remove(nameof(Answer1));
            ModelState.Remove(nameof(Answer2));
            ModelState.Remove(nameof(Answer3));
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(User.FindFirst("Id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId)
                && userId > 0;
        }
    }
}

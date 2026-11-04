using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Security;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages
{
    public class ForgetPasswordModel : PageModel
    {
        private const string UsernameSessionKey = "PasswordRecovery.UserName";
        private const string QuestionSessionKey = "PasswordRecovery.QuestionId";
        private readonly IUserService _userService;

        public ForgetPasswordModel(IUserService userService) => _userService = userService;

        [BindProperty]
        public string UserName { get; set; }

        [BindProperty]
        public string Answer { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public bool QuestionSelected { get; private set; }
        public int AttemptsRemaining { get; private set; } = 5;
        public string QuestionText { get; private set; }

        public void OnGet()
        {
            HttpContext.Session.Remove(UsernameSessionKey);
            HttpContext.Session.Remove(QuestionSessionKey);
        }

        public IActionResult OnPostAskQuestion()
        {
            if (string.IsNullOrWhiteSpace(UserName) || UserName.Trim().Length > 10)
            {
                ModelState.AddModelError(nameof(UserName), "نام کاربری معتبر را وارد کنید.");
                return Page();
            }

            var questionId = RandomNumberGenerator.GetInt32(1, 4);
            var normalizedUserName = UserName.Trim();
            HttpContext.Session.SetString(UsernameSessionKey, normalizedUserName);
            HttpContext.Session.SetInt32(QuestionSessionKey, questionId);
            UserName = normalizedUserName;
            QuestionSelected = true;
            QuestionText = SecurityQuestionCatalog.GetText(questionId);
            return Page();
        }

        public async Task<IActionResult> OnPostResetAsync()
        {
            if (!TryLoadQuestion(out var userName, out var questionId))
            {
                ModelState.AddModelError(string.Empty, "فرایند بازیابی منقضی شده است؛ دوباره شروع کنید.");
                return Page();
            }

            QuestionSelected = true;
            QuestionText = SecurityQuestionCatalog.GetText(questionId);
            UserName = userName;

            if (string.IsNullOrWhiteSpace(Answer))
                ModelState.AddModelError(nameof(Answer), "پاسخ سؤال امنیتی را وارد کنید.");
            if (!PasswordPolicy.IsValid(NewPassword))
                ModelState.AddModelError(nameof(NewPassword), "رمز باید ۸ تا ۶۴ نویسه و شامل حروف کوچک و بزرگ انگلیسی، عدد و نماد باشد.");
            if (!string.Equals(NewPassword, ConfirmPassword, StringComparison.Ordinal))
                ModelState.AddModelError(nameof(ConfirmPassword), "رمز عبور و تکرار آن یکسان نیستند.");
            if (!ModelState.IsValid) return Page();

            var result = _userService.ResetPasswordWithSecurityAnswer(userName, questionId, Answer, NewPassword);
            AttemptsRemaining = result.AttemptsRemaining;
            Answer = null;
            ModelState.Remove(nameof(Answer));
            switch (result.Status)
            {
                case PasswordRecoveryStatus.Success:
                    HttpContext.Session.Remove(UsernameSessionKey);
                    HttpContext.Session.Remove(QuestionSessionKey);
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    TempData["PasswordChanged"] = "رمز عبور با موفقیت تغییر کرد؛ اکنون با رمز جدید وارد شوید.";
                    return RedirectToPage("/Index");
                case PasswordRecoveryStatus.Locked:
                    ModelState.AddModelError(string.Empty, "پنج پاسخ اشتباه ثبت شد. بازیابی تا ۱۵ دقیقه دیگر موقتاً غیرفعال است.");
                    break;
                case PasswordRecoveryStatus.NotConfigured:
                case PasswordRecoveryStatus.UserNotFound:
                    ModelState.AddModelError(string.Empty, "اطلاعات بازیابی معتبر نیست یا برای این حساب ثبت نشده است. در صورت نیاز با پشتیبانی سامانه تماس بگیرید.");
                    break;
                case PasswordRecoveryStatus.InvalidPassword:
                    ModelState.AddModelError(nameof(NewPassword), "رمز جدید باید قوی باشد و با نام کاربری یکسان نباشد.");
                    break;
                default:
                    ModelState.AddModelError(string.Empty, "پاسخ امنیتی صحیح نیست. تعداد تلاش باقی‌مانده: " + AttemptsRemaining);
                    break;
            }
            return Page();
        }

        private bool TryLoadQuestion(out string userName, out int questionId)
        {
            userName = HttpContext.Session.GetString(UsernameSessionKey);
            questionId = HttpContext.Session.GetInt32(QuestionSessionKey) ?? 0;
            return !string.IsNullOrWhiteSpace(userName) && SecurityQuestionCatalog.GetText(questionId) != null;
        }
    }
}

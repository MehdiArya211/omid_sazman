using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Security;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.User;
using VisitorManagment.DataLayer.Entities;

namespace VisitorManagment.Web.Pages
{
    public class ForgetPasswordModel : PageModel
    {
        private const string UsernameSessionKey = "PasswordRecovery.UserName";
        private const string QuestionSessionKey = "PasswordRecovery.QuestionId";
        private readonly IUserService _userService;
        private readonly VisitorManagmentContext _context;
        private readonly ILogger<ForgetPasswordModel> _logger;

        public ForgetPasswordModel(IUserService userService, VisitorManagmentContext context, ILogger<ForgetPasswordModel> logger)
        {
            _userService = userService;
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "نام کاربری باید حداکثر ۱۰ نویسه باشد")]
        public string UserName { get; set; }

        [BindProperty]
        [Display(Name = "پاسخ امنیتی")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "پاسخ امنیتی باید بین ۲ تا ۱۰۰ نویسه باشد")]
        public string Answer { get; set; }

        [BindProperty]
        [Display(Name = "رمز عبور جدید")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [StringLength(64, MinimumLength = 8, ErrorMessage = "رمز عبور باید بین ۸ تا ۶۴ نویسه باشد")]
        [RegularExpression(@"\A(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,64}\z",
            ErrorMessage = "رمز باید شامل حرف بزرگ و کوچک انگلیسی، عدد و نشانه باشد")]
        public string NewPassword { get; set; }

        [BindProperty]
        [Display(Name = "تکرار رمز عبور جدید")]
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید")]
        [Compare(nameof(NewPassword), ErrorMessage = "رمز عبور جدید و تکرار آن یکسان نیستند")]
        public string ConfirmPassword { get; set; }

        public bool QuestionSelected { get; private set; }
        public int AttemptsRemaining { get; private set; } = 5;
        public string QuestionText { get; private set; }
        public List<LoginSlide> LoginSlides { get; private set; } = new List<LoginSlide>();

        public async Task OnGetAsync()
        {
            await LoadLoginSlidesAsync(HttpContext.RequestAborted);
            HttpContext.Session.Remove(UsernameSessionKey);
            HttpContext.Session.Remove(QuestionSessionKey);
        }

        public IActionResult OnPostAskQuestion()
        {
            LoadFallbackLoginSlides();
            ModelState.Remove(nameof(Answer));
            ModelState.Remove(nameof(NewPassword));
            ModelState.Remove(nameof(ConfirmPassword));
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
            LoadFallbackLoginSlides();
            if (!TryLoadQuestion(out var userName, out var questionId))
            {
                ModelState.AddModelError(string.Empty, "فرایند بازیابی منقضی شده است؛ دوباره شروع کنید.");
                return Page();
            }

            QuestionSelected = true;
            QuestionText = SecurityQuestionCatalog.GetText(questionId);
            UserName = userName;
            if (string.IsNullOrWhiteSpace(Answer) || SecurityAnswerHasher.Normalize(Answer).Length < 2)
                ModelState.AddModelError(nameof(Answer), "پاسخ سؤال امنیتی را وارد کنید.");

            if (!ModelState.IsValid) return Page();

            _logger.LogInformation("Password recovery reset handler started.");
            PasswordRecoveryResult result;
            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(HttpContext.RequestAborted))
            {
                timeout.CancelAfter(TimeSpan.FromSeconds(30));
                try
                {
                    result = await _userService.ResetPasswordWithSecurityAnswerAsync(
                        userName, questionId, Answer, NewPassword, timeout.Token);
                }
                catch (OperationCanceledException) when (!HttpContext.RequestAborted.IsCancellationRequested)
                {
                    ModelState.AddModelError(string.Empty, "پاسخ سامانه بیش از حد طول کشید. لطفاً کمی بعد دوباره تلاش کنید.");
                    return Page();
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Password recovery failed while processing a reset request.");
                    ModelState.AddModelError(string.Empty, "در انجام درخواست بازیابی خطایی رخ داد. لطفاً دوباره تلاش کنید یا با پشتیبانی تماس بگیرید.");
                    return Page();
                }
            }
            _logger.LogInformation("Password recovery reset handler completed with status {Status}.", result.Status);
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

        private async Task LoadLoginSlidesAsync(CancellationToken cancellationToken)
        {
            LoginSlides = await _context.LoginSlides.AsNoTracking()
                .Where(item => item.IsActive)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.Id)
                .ToListAsync(cancellationToken);

            if (LoginSlides.Count == 0)
                LoadFallbackLoginSlides();
        }

        private void LoadFallbackLoginSlides()
        {
            LoginSlides = new List<LoginSlide>
            {
                new LoginSlide { ImagePath = "/assets/media/image/photo2.jpg", Title = "دسترسی سریع و یکپارچه", Description = "ثبت و پیگیری درخواست‌ها در یک محیط سازمانی", AltText = "محیط کاری دیجیتال" },
                new LoginSlide { ImagePath = "/assets/media/image/photo3.jpg", Title = "پیگیری شفاف درخواست‌ها", Description = "مشاهده وضعیت و گردش درخواست در هر مرحله", AltText = "پیگیری درخواست‌ها" },
                new LoginSlide { ImagePath = "/assets/media/image/photo1.jpg", Title = "مدیریت ساده‌تر", Description = "دسترسی به خدمات موردنیاز", AltText = "سامانه هوشمند مدیریتی" }
            };
        }

        private bool TryLoadQuestion(out string userName, out int questionId)
        {
            userName = HttpContext.Session.GetString(UsernameSessionKey);
            questionId = HttpContext.Session.GetInt32(QuestionSessionKey) ?? 0;
            return !string.IsNullOrWhiteSpace(userName) && SecurityQuestionCatalog.GetText(questionId) != null;
        }
    }
}

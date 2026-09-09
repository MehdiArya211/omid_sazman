using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ITOWebApiClient;
using VisitorManagment.DataLayer.Context;
using Microsoft.AspNetCore.Http;
using VisitorManagment.Core.Services.Interfaces.Ranking;
using System.Net;
using System;
using System.Threading.Tasks;
using VisitorManagment.DataLayer.Entities.User;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace VisitorManagment.Web.Pages
{

    public class IndexModel : PageModel
    {
        #region Fields and constructor

        private readonly ILogger<IndexModel> _logger;
        private readonly IUserService _userService;
        private readonly IPersonService _personService;
        private readonly IWebApiService _webApiService;
        private readonly ApiTokenCacheClient _apiTokenClient;
        private readonly VisitorManagmentContext _context;
        private readonly IHameshService _hameshService;
        private readonly IRankingService _rankingService;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public IndexModel(ILogger<IndexModel> logger, IUserService userService, IWebApiService webApiService,
            ApiTokenCacheClient apiTokenClient, VisitorManagmentContext context, IPersonService personService,
            IHameshService hameshService, IRankingService rankingService,
            IWebHostEnvironment environment, IConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _webApiService = webApiService;
            _apiTokenClient = apiTokenClient;
            _context = context;
            _personService = personService;
            _hameshService = hameshService;
            _rankingService = rankingService;
            _environment = environment;
            _configuration = configuration;
        }

        #endregion

        #region Properties

        [BindProperty]
        public LoginViewModel LoginViewModel { get; set; }
        public ItoLogInfoViewModel itoLogInfoViewModel { get; set; }

        /// <summary>
        /// در محیط توسعه، نقص تنظیمات ورود آزمایشی را بدون نمایش مقدار رمز اعلام می‌کند.
        /// </summary>
        public string DevelopmentLoginConfigurationWarning
        {
            get
            {
                if (!_environment.IsDevelopment() || !_configuration.GetValue<bool>("DevelopmentLogin:Enabled"))
                    return null;
                if (!HasDevelopmentLoginPassword())
                    return "ورود آزمایشی فعال است اما رمز یا هش رمز DevelopmentLogin تنظیم نشده است.";
                if (GetDevelopmentLoginUsernames().Count == 0)
                    return "ورود آزمایشی فعال است اما هیچ نام کاربری در AllowedUsernames ثبت نشده است.";
                if (!IsLocalRequest())
                    return "ورود آزمایشی فقط از localhost قابل استفاده است.";
                return null;
            }
        }

        #endregion

        #region Page handlers

        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public void OnGet()
        {
        }

        /// <summary>
        /// اعتبار فرم ورود، وضعیت کاربر و نقش سازمانی او را بررسی می‌کند؛ سپس
        /// Claimهای موردنیاز سامانه را در کوکی احراز هویت ثبت می‌کند.
        /// </summary>
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Users user;
            try
            {
                user = _userService.LoginUser(LoginViewModel);
                if (user == null)
                    user = TryDevelopmentLogin(LoginViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "خطایی در هنگام ورود به سیستم رخ داده است");
                // لاگ خطا
                _logger.LogError(ex, "Error in user login.");
                return Page();
            }

            if (user == null)
            {
                ModelState.AddModelError("", "نام کاربری یا کلمه عبور اشتباه است");
                return Page();
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "حساب کاربری شما فعال نمی باشد");
                return Page();
            }

            HameshInfoViewModel role;
            int roleTypeId;
            string roleTypeTitle;
            int departmentTypeId;

            try
            {
                role = _hameshService.GetRoleTypePerson(user.Id);
                if (role == null || role.RoleTypeId <= 0)
                {
                    ModelState.AddModelError("", "برای این کاربر نقش معتبری تعریف نشده است");
                    return Page();
                }
                roleTypeId = role.RoleTypeId;
                roleTypeTitle = role.RoleTypeTitle;
                departmentTypeId = _rankingService.GetDepartmentTypeWithUnitCode(user.UnitCode);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "خطایی در دریافت اطلاعات کاربر رخ داده است");
                // لاگ خطا
                _logger.LogError(ex, "Error in fetching user role or department information.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.RankTitle} {user.FirstName} {user.LastName}".Trim()),
                new Claim(ClaimTypes.Email, user.UserName ?? string.Empty),
                new Claim("Id", user.Id.ToString()),
                new Claim("PersonalCode", user.UserName ?? string.Empty),
                new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                new Claim("UnitDutyCode", user.UnitDutyCode.ToString()),
                new Claim("UnitCode", user.UnitCode.ToString()),
                new Claim("UnitCodeTitle", user.UnitTitle ?? string.Empty),
                new Claim("CodGha", user.CodGha.ToString()),
                new Claim("CodGhaTitle", user.CodGhaTitle ?? string.Empty),
                new Claim("UserName", user.UserName ?? string.Empty),
                new Claim("UserAvatar", user.UserAvatar ?? string.Empty),
                new Claim("RoleId", user.UserRoles.FirstOrDefault(item => item.UserId == user.Id)?.RoleId.ToString() ?? string.Empty),
                new Claim("RoleTypeId", roleTypeId.ToString()),
                new Claim("RoleTypeTitle", roleTypeTitle ?? string.Empty),
                new Claim("RoleTypeIdFinal", role.RoleTypeIdFinal?.ToString() ?? string.Empty),
                new Claim("RoleTypeTitleFinal", role.RoleTypeTitleFinal ?? string.Empty),
                new Claim("DepartmentTypeId", departmentTypeId.ToString()),
                new Claim("MustChangePassword", _userService.IsPasswordChangeRequired(user.Id).ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                IsPersistent = LoginViewModel.RemmemberMe
            };

            try
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "خطایی در ورود به سیستم رخ داده است");
                // لاگ خطا
                _logger.LogError(ex, "Error in user sign-in.");
                return Page();
            }

            #region Log, Samaneh Fajr
            var userName = user.UserName;
            var userIdStr = user.Id.ToString();
            //_webApiService.AddLog(userIdStr, userName, "Index");
            #endregion

            #region IP and PC Info
            try
            {
                var pcName = Dns.GetHostName();
                var ipUser = Dns.GetHostEntry(pcName).AddressList[1].ToString();
                _userService.AddUserLoginHistory(user.UserName, DateTime.Now, ipUser, false);
            }
            catch (Exception ex)
            {
                // لاگ خطا
                _logger.LogError(ex, "Error in fetching IP or PC information.");
            }
            #endregion

            return _userService.IsPasswordChangeRequired(user.Id)
                ? RedirectToPage("/ChangePasswordRequired")
                : RedirectToPage("/Visitor/Index");
        }

        #endregion

        #region Development login

        /// <summary>
        /// ورود آزمایشی را فقط در محیط Development، در صورت فعال‌بودن تنظیم و برای نام‌های
        /// کاربری صریحاً مجازشده انجام می‌دهد. رمز از تنظیمات امن محیط خوانده می‌شود و در کد قرار ندارد.
        /// </summary>
        private Users TryDevelopmentLogin(LoginViewModel login)
        {
            if (!_environment.IsDevelopment() ||
                !_configuration.GetValue<bool>("DevelopmentLogin:Enabled") ||
                !IsLocalRequest() ||
                login == null ||
                string.IsNullOrWhiteSpace(login.UserName) ||
                string.IsNullOrEmpty(login.Password))
                return null;

            var configuredPassword = _configuration["DevelopmentLogin:Password"];
            var configuredPasswordHash = _configuration["DevelopmentLogin:PasswordSha256"];
            var allowedUsernames = GetDevelopmentLoginUsernames();

            if (!HasDevelopmentLoginPassword() ||
                !allowedUsernames.Contains(login.UserName, StringComparer.OrdinalIgnoreCase) ||
                !MatchesDevelopmentPassword(login.Password, configuredPassword, configuredPasswordHash))
            {
                if (!HasDevelopmentLoginPassword() || allowedUsernames.Count == 0)
                    _logger.LogWarning("Development login is enabled but Password/PasswordSha256 or AllowedUsernames is not configured.");
                return null;
            }

            var user = _context.Users
                .Include(item => item.UserRoles)
                .SingleOrDefault(item => item.UserName == login.UserName && item.IsActive && !item.IsDelete);

            if (user != null)
                _logger.LogWarning("Development login used for test account {UserName}.", user.UserName);

            return user;
        }

        /// <summary>
        /// وجود رمز ورود توسعه را بررسی می‌کند. مقدار متنی متغیر محیطی بر هش موجود در
        /// تنظیمات توسعه اولویت دارد تا در صورت نیاز بتوان رمز را بدون تغییر کد جایگزین کرد.
        /// </summary>
        private bool HasDevelopmentLoginPassword()
        {
            return !string.IsNullOrEmpty(_configuration["DevelopmentLogin:Password"]) ||
                   !string.IsNullOrEmpty(_configuration["DevelopmentLogin:PasswordSha256"]);
        }

        /// <summary>
        /// رمز ورودی را با رمز امن محیط یا هش SHA-256 مخصوص تست محلی مقایسه می‌کند.
        /// </summary>
        private static bool MatchesDevelopmentPassword(string suppliedPassword, string configuredPassword,
            string configuredPasswordHash)
        {
            if (!string.IsNullOrEmpty(configuredPassword))
                return SecureEquals(suppliedPassword, configuredPassword);

            if (string.IsNullOrWhiteSpace(configuredPasswordHash)) return false;

            using (var sha256 = SHA256.Create())
            {
                var suppliedHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(suppliedPassword));
                byte[] configuredHash;
                try
                {
                    configuredHash = HexToBytes(configuredPasswordHash.Trim());
                }
                catch (FormatException)
                {
                    return false;
                }

                return suppliedHash.Length == configuredHash.Length &&
                       CryptographicOperations.FixedTimeEquals(suppliedHash, configuredHash);
            }
        }

        /// <summary>
        /// رشته هگزادسیمال تنظیمات را بدون وابستگی به APIهای نسخه‌های جدید دات‌نت تبدیل می‌کند.
        /// </summary>
        private static byte[] HexToBytes(string value)
        {
            if (value.Length == 0 || value.Length % 2 != 0) throw new FormatException();

            var result = new byte[value.Length / 2];
            for (var index = 0; index < result.Length; index++)
                result[index] = Convert.ToByte(value.Substring(index * 2, 2), 16);
            return result;
        }

        /// <summary>
        /// حتی در صورت اشتباه در تنظیم محیط سرور، ورود آزمایشی را به درخواست محلی محدود می‌کند.
        /// </summary>
        private bool IsLocalRequest()
        {
            var remoteAddress = HttpContext?.Connection?.RemoteIpAddress;
            return remoteAddress != null && IPAddress.IsLoopback(remoteAddress);
        }

        /// <summary>
        /// فهرست کاربران آزمایشی را هم از آرایه تنظیمات و هم از مقدار متنی جداشده با ویرگول
        /// یا نقطه‌ویرگول می‌خواند تا تنظیم در IIS Express، CLI و Environment Variable یکسان عمل کند.
        /// </summary>
        private List<string> GetDevelopmentLoginUsernames()
        {
            var section = _configuration.GetSection("DevelopmentLogin:AllowedUsernames");
            var usernames = section.GetChildren()
                .Select(item => item.Value)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item.Trim())
                .ToList();

            if (usernames.Count > 0) return usernames;

            var inlineValue = section.Value;
            return string.IsNullOrWhiteSpace(inlineValue)
                ? new List<string>()
                : inlineValue.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => item.Trim())
                    .Where(item => item.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
        }

        /// <summary>
        /// دو مقدار حساس را با زمان ثابت مقایسه می‌کند تا اختلاف زمان مقایسه قابل بهره‌برداری نباشد.
        /// </summary>
        private static bool SecureEquals(string suppliedValue, string configuredValue)
        {
            var suppliedBytes = Encoding.UTF8.GetBytes(suppliedValue);
            var configuredBytes = Encoding.UTF8.GetBytes(configuredValue);
            return suppliedBytes.Length == configuredBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(suppliedBytes, configuredBytes);
        }

        #endregion

    }
}

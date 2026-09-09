using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Core.Services.Interfaces.Ranking;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.User;
using VisitorManagment.Web.Helpers;

namespace VisitorManagment.Web.Pages
{
    /// <summary>
    /// امکان مشاهده سامانه با سطح دسترسی کاربران دیگر را فقط برای تست محلی مدیر سامانه فراهم می‌کند.
    /// این صفحه در محیط Production یا درخواست غیرمحلی هیچ عملیاتی انجام نمی‌دهد.
    /// </summary>
    [Authorize]
    public class DevelopmentImpersonationModel : PageModel
    {
        #region Constants and fields

        private const string DevelopmentAdministratorUserName = "95003599";
        private readonly VisitorManagmentContext _context;
        private readonly IHameshService _hameshService;
        private readonly IRankingService _rankingService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DevelopmentImpersonationModel> _logger;

        public DevelopmentImpersonationModel(VisitorManagmentContext context, IHameshService hameshService,
            IRankingService rankingService, IUserService userService, IWebHostEnvironment environment,
            IConfiguration configuration, ILogger<DevelopmentImpersonationModel> logger)
        {
            _context = context;
            _hameshService = hameshService;
            _rankingService = rankingService;
            _userService = userService;
            _environment = environment;
            _configuration = configuration;
            _logger = logger;
        }

        #endregion

        #region Page handlers

        public IActionResult OnGet()
        {
            return RedirectToPage("/Visitor/Index");
        }

        /// <summary>
        /// پس از کنترل محیط، مدیر و کاربر مقصد، کوکی احراز هویت را با Claimهای کاربر مقصد جایگزین می‌کند.
        /// </summary>
        public async Task<IActionResult> OnPostStartAsync(int userId)
        {
            if (!IsDevelopmentLocalRequest())
                return DevelopmentError("این قابلیت فقط در محیط Development و روی localhost فعال است.");

            if (!User.IsSystemAdministrator() ||
                User.FindFirst("PersonalCode")?.Value != DevelopmentAdministratorUserName)
                return DevelopmentError("فقط مدیر تعیین‌شده سامانه اجازه ورود آزمایشی به کارتابل کاربران را دارد.");

            if (User.FindFirst("IsImpersonating")?.Value == "true")
                return DevelopmentError("ابتدا به حساب مدیر برگردید و سپس کاربر دیگری را انتخاب کنید.");

            var administratorId = User.FindFirst("Id")?.Value;
            if (!int.TryParse(administratorId, out var originalUserId))
                return DevelopmentError("شناسه حساب مدیر معتبر نیست؛ یک‌بار از سامانه خارج و دوباره وارد شوید.");

            var targetUser = LoadActiveUser(userId);
            if (targetUser == null)
            {
                TempData["OperationTitle"] = "ورود آزمایشی ناموفق";
                TempData["OperationMessage"] = "کاربر مقصد فعال نیست یا یافت نشد.";
                TempData["OperationIcon"] = "error";
                return RedirectToPage("/Admin/Users/Index");
            }

            bool signedIn;
            try
            {
                signedIn = await SignInAsAsync(targetUser, originalUserId, DevelopmentAdministratorUserName);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    "Development impersonation failed for administrator {Administrator} and target user {TargetUser}.",
                    DevelopmentAdministratorUserName, targetUser.UserName);
                TempData["OperationTitle"] = "ورود به کارتابل انجام نشد";
                TempData["OperationMessage"] = "اطلاعات نقش یا یگان کاربر مقصد کامل نیست. جزئیات خطا در لاگ ثبت شد.";
                TempData["OperationIcon"] = "error";
                return RedirectToPage("/Admin/Users/Index");
            }

            if (!signedIn) return RedirectToPage("/Admin/Users/Index");

            _logger.LogWarning("Development impersonation started. Administrator {Administrator} is viewing as {TargetUser}.",
                DevelopmentAdministratorUserName, targetUser.UserName);
            return RedirectToPage("/Visitor/Index");
        }

        /// <summary>
        /// هویت مدیر اولیه را از Claimهای محافظت‌شده کوکی بازیابی و حالت آزمایشی را متوقف می‌کند.
        /// </summary>
        public async Task<IActionResult> OnPostStopAsync()
        {
            if (!IsDevelopmentLocalRequest() || User.FindFirst("IsImpersonating")?.Value != "true")
                return Forbid();

            var originalIdValue = User.FindFirst("OriginalUserId")?.Value;
            var originalUserName = User.FindFirst("OriginalUserName")?.Value;
            if (!int.TryParse(originalIdValue, out var originalUserId) ||
                originalUserName != DevelopmentAdministratorUserName)
                return Forbid();

            var administrator = LoadActiveUser(originalUserId);
            if (administrator == null || administrator.UserName != DevelopmentAdministratorUserName)
                return Forbid();

            var signedIn = await SignInAsAsync(administrator, null, null);
            if (!signedIn) return Forbid();

            _logger.LogWarning("Development impersonation stopped for administrator {Administrator}.",
                DevelopmentAdministratorUserName);
            return RedirectToPage("/Admin/Users/Index");
        }

        #endregion

        #region Authentication helpers

        private IActionResult DevelopmentError(string message)
        {
            TempData["OperationTitle"] = "ورود به کارتابل انجام نشد";
            TempData["OperationMessage"] = message;
            TempData["OperationIcon"] = "error";
            return RedirectToPage("/Admin/Users/Index");
        }

        private Users LoadActiveUser(int userId)
        {
            return _context.Users
                .Include(user => user.UserRoles)
                .SingleOrDefault(user => user.Id == userId && user.IsActive && !user.IsDelete);
        }

        /// <summary>
        /// Claimهای مورد استفاده در ورود عادی را برای کاربر مقصد بازسازی می‌کند تا منوها و دسترسی‌ها واقعی باشند.
        /// </summary>
        private async Task<bool> SignInAsAsync(Users user, int? originalUserId, string originalUserName)
        {
            var role = _hameshService.GetRoleTypePerson(user.Id);
            if (role == null || role.RoleTypeId <= 0)
            {
                TempData["OperationTitle"] = "ورود آزمایشی ناموفق";
                TempData["OperationMessage"] = "برای کاربر مقصد نقش معتبری تعریف نشده است.";
                TempData["OperationIcon"] = "error";
                return false;
            }

            var departmentTypeId = _rankingService.GetDepartmentTypeWithUnitCode(user.UnitCode);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.RankTitle} {user.FirstName} {user.LastName}".Trim()),
                new Claim(ClaimTypes.Email, user.UserName ?? string.Empty),
                new Claim("Id", user.Id.ToString()),
                new Claim("PersonalCode", user.UserName ?? string.Empty),
                new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                new Claim("UnitDutyCode", user.UnitDutyCode?.ToString() ?? string.Empty),
                new Claim("UnitCode", user.UnitCode.ToString()),
                new Claim("UnitCodeTitle", user.UnitTitle ?? string.Empty),
                new Claim("CodGha", user.CodGha?.ToString() ?? string.Empty),
                new Claim("CodGhaTitle", user.CodGhaTitle ?? string.Empty),
                new Claim("UserName", user.UserName ?? string.Empty),
                new Claim("UserAvatar", user.UserAvatar ?? string.Empty),
                new Claim("RoleId", user.UserRoles.FirstOrDefault()?.RoleId.ToString() ?? string.Empty),
                new Claim("RoleTypeId", role.RoleTypeId.ToString()),
                new Claim("RoleTypeTitle", role.RoleTypeTitle ?? string.Empty),
                new Claim("RoleTypeIdFinal", role.RoleTypeIdFinal?.ToString() ?? string.Empty),
                new Claim("RoleTypeTitleFinal", role.RoleTypeTitleFinal ?? string.Empty),
                new Claim("DepartmentTypeId", departmentTypeId.ToString()),
                // در مشاهده آزمایشی، وضعیت واقعی رمز در دیتابیس دست‌نخورده می‌ماند اما مدیر در فرم تغییر رمز گرفتار نمی‌شود.
                new Claim("MustChangePassword", (originalUserId.HasValue
                    ? false
                    : _userService.IsPasswordChangeRequired(user.Id)).ToString())
            };

            if (originalUserId.HasValue)
            {
                claims.Add(new Claim("IsImpersonating", "true"));
                claims.Add(new Claim("OriginalUserId", originalUserId.Value.ToString()));
                claims.Add(new Claim("OriginalUserName", originalUserName));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = false });
            return true;
        }

        private bool IsDevelopmentLocalRequest()
        {
            var remoteAddress = HttpContext.Connection.RemoteIpAddress;
            return _environment.IsDevelopment() &&
                   _configuration.GetValue<bool>("DevelopmentLogin:Enabled") &&
                   remoteAddress != null &&
                   IPAddress.IsLoopback(remoteAddress);
        }

        #endregion
    }
}

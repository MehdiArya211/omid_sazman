using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
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

        public IndexModel(ILogger<IndexModel> logger, IUserService userService, IWebApiService webApiService,
            ApiTokenCacheClient apiTokenClient, VisitorManagmentContext context, IPersonService personService,
            IHameshService hameshService, IRankingService rankingService)
        {
            _logger = logger;
            _userService = userService;
            _webApiService = webApiService;
            _apiTokenClient = apiTokenClient;
            _context = context;
            _personService = personService;
            _hameshService = hameshService;
            _rankingService = rankingService;
        }

        #endregion

        #region Properties

        [BindProperty]
        public LoginViewModel LoginViewModel { get; set; }
        public ItoLogInfoViewModel itoLogInfoViewModel { get; set; }

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
                new Claim("DepartmentTypeId", departmentTypeId.ToString())
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

            return RedirectToPage("/Visitor/Index");
        }

        #endregion

    }
}

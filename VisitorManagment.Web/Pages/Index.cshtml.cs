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
using VisitorManagment.DataLayer.Entities.User;

namespace VisitorManagment.Web.Pages
{

    public class IndexModel : PageModel
    {
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



        [BindProperty]
        public LoginViewModel LoginViewModel { get; set; }
        public ItoLogInfoViewModel itoLogInfoViewModel { get; set; }

        public void OnGet()
        {
        }

    

        public IActionResult OnPost()
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
            int? roleTypeIdFinal;
            string roleTypeTitle;
            string roleTypeTitleFinal;
            int departmentTypeId;

            try
            {
                role = _hameshService.GetRoleTypePerson(user.Id);
                roleTypeId = role.RoleTypeId;
                roleTypeIdFinal = role.RoleTypeIdFinal;
                roleTypeTitle = role.RoleTypeTitle;
                roleTypeTitleFinal = role.RoleTypeTitle;
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
        new Claim(ClaimTypes.Name, $"{user.RankTitle} {user.FirstName} {user.LastName}"),
        new Claim(ClaimTypes.Email, user.UserName),
        new Claim("Id", user.Id.ToString()),
        new Claim("PersonalCode", user.UserName),
        new Claim("FullName", $"{user.FirstName} {user.LastName}"),
        new Claim("UnitDutyCode", user.UnitDutyCode.ToString()),
        new Claim("UnitCode", user.UnitCode.ToString()),
        new Claim("UnitCodeTitle", user.UnitTitle),
        new Claim("CodGha", user.CodGha.ToString()),
        new Claim("CodGhaTitle", user.CodGhaTitle),
        new Claim("UserName", user.UserName),
        new Claim("UserAvatar", user.UserAvatar),
        new Claim("RoleId", user.UserRoles.FirstOrDefault(u => u.UserId == user.Id)?.RoleId.ToString() ?? string.Empty),
        new Claim("RoleTypeId", roleTypeId.ToString()),
        new Claim("RoleTypeTitle", roleTypeTitle),
        new Claim("RoleTypeIdFinal", role.RoleTypeIdFinal.ToString()),
        new Claim("RoleTypeTitleFinal", role.RoleTypeTitleFinal.ToString()),
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
                HttpContext.SignInAsync(principal, properties);
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


        //mehdi 1403-05-13
        public IActionResult OnPost1()
        {
            if (!ModelState.IsValid)
            {

                return Page();
            }
            var user = _userService.LoginUser(LoginViewModel);

            if (user != null)
            {
                if (user.IsActive)
                {
                    var role = _hameshService.GetRoleTypePerson(user.Id);
                    var roleTypeId = _hameshService.GetRoleTypePerson(user.Id).RoleTypeId;
                    var roleTypeTitle = _hameshService.GetRoleTypePerson(user.Id).RoleTypeTitle;
                    var departmentTypeId = _rankingService.GetDepartmentTypeWithUnitCode(user.UnitCode);

                    //=== ToDo 
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
                        new Claim(ClaimTypes.Name ,user.RankTitle + " " +  user.FirstName + " " + user.LastName),
                        new Claim(ClaimTypes.Email , user.UserName),


                        new Claim("Id" , user.Id.ToString()),
                        new Claim("PersonalCode" , user.UserName.ToString()),
                        new Claim("FullName" , user.FirstName + " " + user.LastName),
                        new Claim("UnitDutyCode" , user.UnitDutyCode.ToString()),
                        new Claim("UnitCode" , user.UnitCode.ToString()),
                        new Claim("UnitCodeTitle" , user.UnitTitle.ToString()),
                        new Claim("CodGha" , user.CodGha.ToString()),
                        new Claim("CodGhaTitle" , user.CodGhaTitle.ToString()),
                        new Claim("UserName" , user.UserName.ToString()),
                        new Claim("UserAvatar" , user.UserAvatar),
                        new Claim("RoleId" , user.UserRoles.FirstOrDefault(u=>u.UserId == user.Id).RoleId.ToString()),
                        new Claim("RoleTypeId" ,roleTypeId.ToString()),
                        new Claim("RoleTypeTitle" ,roleTypeTitle.ToString()),
                        new Claim("RoleTypeIdFinal" ,role.RoleTypeIdFinal.ToString()),
                        new Claim("DepartmentTypeId" ,departmentTypeId.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);
                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = LoginViewModel.RemmemberMe
                    };

                    //==== login user

                    HttpContext.SignInAsync(principal, properties);

                    #region لاگ ، سامانه فجر
                    var userName = user.UserName;
                    string UserId = user.Id.ToString();
                    //_webApiService.AddLog(UserId, userName, "Index");
                    #endregion
                    #region مشخصات Ip و Pc

                    var userId = user.UserName;
                    string pcName = Dns.GetHostName();
                    string IpUser = Dns.GetHostEntry(pcName).AddressList[1].ToString();
                    // Add User Login Faild History
                    _userService.AddUserLoginHistory(userId.ToString(), DateTime.Now, IpUser, false);
                    #endregion
                    return RedirectToPage("/Visitor/Index");
                }
                ModelState.AddModelError("", "حساب کاربری شما فعال نمی باشد");
            }

            ModelState.AddModelError("", "نام کاربری یا کلمه عبور اشتباه است");
            return Page();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITOWebApiClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Admin.Users
{
    
    public class LogoutModel : PageModel
    {
        private readonly ApiTokenCacheClient _apiTokenClient;
        private readonly IWebApiService _webApiService;
        public LogoutModel(ApiTokenCacheClient apiTokenClient , IWebApiService webApiService)
        {


            _apiTokenClient = apiTokenClient;
            _webApiService = webApiService;
        }
        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>
        public IActionResult OnGet()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            #region لاگ ، سامانه فجر
            var userName = User.FindFirst("UserName").Value;
            string UserId = User.FindFirst("Id").Value;
            //_webApiService.AddLog(UserId, userName, "LogOut");
            #endregion
            return RedirectToPage("/Index");
        }
    }
}

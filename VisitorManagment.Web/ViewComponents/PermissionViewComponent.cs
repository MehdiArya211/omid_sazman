
using VisitorManagment.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisitorManagment.Web.ViewComponents
{
   [Authorize]
    public class PermissionViewComponent : ViewComponent
    {
        private readonly IPermissionService _permissionService;
        public PermissionViewComponent(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        [Authorize]
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            return await Task.FromResult((IViewComponentResult)View("Permission", _permissionService.GetPermissionsForUser(userId)));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Admin.RolePermission
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IFileService _fileService;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly IWorkFlowService _workFlowService;
        private readonly IHameshService _hameshService;
        public IndexModel(IFileService fileService, IUserService userService, IPermissionService permissionService, IWorkFlowService workFlowService, IHameshService hameshService)
        {
            _fileService = fileService;
            _userService = userService;
            _permissionService = permissionService;
            _workFlowService = workFlowService;
            _hameshService = hameshService;
        }

        [BindProperty]
        public CreateUserAccessViewModel createUserAccessViewModel { get; set; }
       
        public int userid;
        public void OnGet()
        {
            var userId = User.FindFirst("Id").Value;
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);

            ViewData["RoleList"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");
            ViewData["PermissionList"] = new SelectList(_permissionService.GetAllPermission(), "PermissionId", "PermissionTitle");
        }

        public IActionResult OnPost(List<int> RoleListId, List<int> PermissionListId)
        {

            ViewData["RoleList"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");
            ViewData["PermissionList"] = new SelectList(_permissionService.GetAllPermission(), "PermissionId", "PermissionTitle");
            _permissionService.AddRolesToRolePermission(RoleListId, PermissionListId);
            //ViewData["successcreate"] = true;
            return Page();
        }
    }
}

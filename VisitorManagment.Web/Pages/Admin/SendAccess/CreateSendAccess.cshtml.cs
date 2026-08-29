using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages.Admin.SendAccess
{
     [Authorize]
    // [AutoValidateAntiforgeryToken]
    public class CreateSendAccessModel : PageModel
    {
        private readonly IWorkFlowService _workFlowService;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;

        public CreateSendAccessModel(IPermissionService permissionService, IUserService userService,
            IWorkFlowService workFlowService)
        {
            _workFlowService = workFlowService;
            _permissionService = permissionService;
            _userService = userService;

        }
        [BindProperty]
        public CreateUserAccessViewModel createUserAccessViewModel { get; set; }
        public List<WorkFlowViewModel> ListAccsessPersonal { get; set; }
        public List<WorkFlowViewModel> ListUnAccsessPersonal { get; set; }

        public int? RoleId;

        public void OnGet(int? roleId = 0)
        {
            RoleId = roleId;

            ViewData["AllRole"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");

            ViewData["WorkFlow"] = new SelectList(_workFlowService.GetrcvrList(roleId ?? 0), "Id", "Title");

            // لیست منو هایی که دسترسی دارد
            ListAccsessPersonal = _workFlowService.GetReciverList(roleId ?? 0);

            // لیست منو هایی که دسترسی ندارد
            ListUnAccsessPersonal = _workFlowService.GetUnAccessList(roleId ?? 0);


            ViewData["ShowForm"] = false;

            if (roleId != 0)
            {
                ViewData["ShowForm"] = true;
                ViewData["RoleId"] = roleId;

            }

        }

        /// <summary>
        /// دسترسی دارند
        /// </summary>
        /// <param name="UnAccessRoleId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IActionResult OnPostRegAccess(List<int> UnAccessRoleId, int roleId)
        {
            RoleId = roleId;

            ViewData["AllRole"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");

            ViewData["WorkFlow"] = new SelectList(_workFlowService.GetrcvrList(roleId), "Id", "Title");



            var userId = User.FindFirst("Id").Value;

            ViewData["ShowForm"] = true;
            ViewData["RoleId"] = roleId;

            _workFlowService.AddAccessToRole(UnAccessRoleId, roleId, int.Parse(userId));
            // لیست پرسنلی که دسترسی دارد
            ListAccsessPersonal = _workFlowService.GetReciverList(roleId);

            // لیست پرسنلی که دسترسی ندارد
            ListUnAccsessPersonal = _workFlowService.GetUnAccessList(roleId);
            return Page();
        }


        /// <summary>
        /// دسترسی ندارند
        /// </summary>
        /// <param name="AccessRoleId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IActionResult OnPostUnAccess(List<int> AccessRoleId, int roleId)
        {
            RoleId = roleId;

            ViewData["AllRole"] = new SelectList(_permissionService.GetAllRoles(), "RoleId", "Title");

            ViewData["WorkFlow"] = new SelectList(_workFlowService.GetrcvrList(roleId), "Id", "Title");



            var userId = User.FindFirst("Id").Value;

            ViewData["ShowForm"] = true;
            ViewData["RoleId"] = roleId;


            _workFlowService.RemoveAccessToRole(AccessRoleId, roleId, int.Parse(userId));
            // لیست پرسنلی که دسترسی دارد
            ListAccsessPersonal = _workFlowService.GetReciverList(roleId);

            // لیست پرسنلی که دسترسی ندارد
            ListUnAccsessPersonal = _workFlowService.GetUnAccessList(roleId);
            return Page();
        }

    }
}

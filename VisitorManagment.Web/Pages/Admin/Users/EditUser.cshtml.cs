using System.Collections.Generic;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.Classes;
using VisitorManagment.Web.Helpers;
using System.Linq;

namespace VisitorManagment.Web.Pages.Admin.Users
{
    [Authorize]
    public class EditUserModel : PageModel
    {

        private readonly IUserService _userService;
        private IPermissionService _permissionService;
        private IHameshService _hameshService;
        private IWebApiService _webApiService;

        public EditUserModel(IUserService userService, IPermissionService permissionService, IHameshService hameshService, IWebApiService webApiService)
        {
            _userService = userService;
            _permissionService = permissionService;
            _hameshService = hameshService;
            _webApiService = webApiService;
        }


        [BindProperty]
        //public CreateUserViewModel CreateUserViewModel { get; set; }
        public EditUserViewModel editUserViewModel { get; set; }

        public IActionResult OnGet(int id)
        {
            //کاربر غیر ادمین صفحه رو بهش نشون نده

            var userId = User.FindFirst("Id").Value;
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);

            //if (roleTypeId != 100 && roleTypeId != 101 && roleTypeId != 102)
            //{
            //    return NotFound();
            //}


            ViewData["RolesTitle"] = new SelectList(_permissionService.GetRoles(roleTypeId.ToString()), "RoleId", "Title");

            editUserViewModel = _userService.GetUserForShowInEditMode(id);
            return Page();
        }

        public JsonResult OnGetGetPersonalId0(string personalno)
        {
            var result = new FactPersonalViewModel();


            var user = _webApiService.GetPersonalByPersonalNo(personalno).Data;
            //اگه سرویس با خطا مواجه شد خطا نشون بده
            if (user.Respond == "ERROR Service")
            {
                return new JsonResult(new { message = "سرویس با خطا مواجه شد .دقایقی دیگر مجددا امتحان کنید" });
            }

            if (user == null)
            {
                return new JsonResult(new { message = "پرسنلی با این مشخصات یافت نشد" });
            }


            SessionHelper.SetObjectAsJson(HttpContext.Session, "result", user);
            return new JsonResult(user);


            //if (personalno == "adminagh")
            //{
            //    UserInfoViewModel adminAgh = _userService.GetUserByPersonalNo(personalno);
            //    return new JsonResult(adminAgh);
            //}

            //var user = _webApiService.GetPersonalByPersonalNo(personalno);

            ////UserInput
            //int unitCodeUserInput;
            //int CodeGhaUserInput;
            ////UserLogin
            //var userName = User.FindFirst("UserName").Value;
            //int unitCodeUserLogin;
            //int CodeGhaUserLogin;



            //if (userName == "adminagh")
            //{
            //    result = user.Data;


            //    if (result == null)
            //    {
            //        return new JsonResult(new { message = "پرسنلی با این مشخصات یافت نشد" });
            //    }


            //    SessionHelper.SetObjectAsJson(HttpContext.Session, "result", result);
            //    return new JsonResult(result);
            //}



            //if (personalno != "adminagh")
            //{
            //    unitCodeUserInput = user.Data.UnitCode;
            //    CodeGhaUserInput = _webApiService.GetGhararghahByOmdOrgCode(user.Data.UnitCode).Data.Id ?? 0;
            //    unitCodeUserLogin = int.Parse(User.FindFirst("UnitCode").Value);
            //    CodeGhaUserLogin = int.Parse(User.FindFirst("CodGha").Value);
            //    //
            //    //اگر کد قرارگاه یا یگان عمده نفر وارد شد با نفری که لاگین کرده یکیی بود اطلاعاتش رو نشون بده بهمون
            //    //if (unitCodeUserInput == unitCodeUserLogin || unitCodeUserLogin == CodeGhaUserInput)
            //    //{
            //    var result1 = user;

            //    if (result1 == null && personalno != "adminagh")
            //    {
            //        return new JsonResult(new { message = "پرسنلی با این مشخصات یافت نشد" });
            //    }

            //    SessionHelper.SetObjectAsJson(HttpContext.Session, "result", result1);
            //    return new JsonResult(result1);
            //    //}
            //    //else
            //    //{
            //    //    return new JsonResult(new { message = "این شخص جزء پرسنل یگان شما نمیباشد" });
            //    //}
            //}


           // return new JsonResult(result1);
        }


        public JsonResult OnGetGetPersonalId(string personalno)
        {
            if (string.IsNullOrWhiteSpace(personalno))
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "کد پرسنلی معتبر نیست"
                });
            }

            var response = _webApiService.GetPersonalByPersonalNo(personalno);

            if (response == null || response.IsSuccess == false || response.Data == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = response?.Message ?? "پرسنلی با این مشخصات یافت نشد"
                });
            }

            SessionHelper.SetObjectAsJson(HttpContext.Session, "result", response.Data);

            return new JsonResult(new
            {
                success = true,
                data = response.Data
            });
        }


        public IActionResult OnPost(string password, int userId, string fname, string lname, string rankTitle)
        {
            var roleTypeId = int.Parse(User.FindFirst("RoleTypeId").Value);
            ViewData["RolesTitle"] = new SelectList(_permissionService.GetRoles(roleTypeId.ToString()), "RoleId", "Title");

            if (editUserViewModel == null || userId <= 0)
            {
                ModelState.AddModelError("", "اطلاعات کاربر معتبر نیست.");
                return Page();
            }

            editUserViewModel.RankTitle = rankTitle;
            if (editUserViewModel.UserRolesId == 0)
            {
                //return Redirect("/Admin/Users/EditUser/" + userId);
                ModelState.AddModelError("", "نقش را انتخاب نمایید");
                return Page();
            }

            var EditUserId = int.Parse(User.FindFirst("Id").Value);
            ////==== Check FileExtentions

            if (editUserViewModel.UserAvatar != null && !FileUploadCheck.CheckImageFileExtension(editUserViewModel.UserAvatar))
            {
                ModelState.AddModelError("", "فایل انتخابی معتبر نمی باشد");
                return Page();
            }

            _userService.EditUserFromAdmin(editUserViewModel, password, EditUserId, userId);

            //Add Roles
            _permissionService.EditRolesToUser(editUserViewModel.UserRolesId, userId);


            TempData["OperationTitle"] = "ویرایش موفق";
            TempData["OperationMessage"] = "اطلاعات کاربر با موفقیت ویرایش شد.";
            TempData["OperationIcon"] = "success";
            return Redirect("/Admin/Users");

        }

    }
}

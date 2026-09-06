using System;
using System.Linq;
using ITOWebApiClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagment.Core.Classes;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Web.Helpers;

namespace VisitorManagment.Web.Pages.Admin.Users
{
    [Authorize]
    public class CreateUsersModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;
        private readonly ApiTokenCacheClient _apiTokenClient;
        private readonly IWebApiService _webApiService;
        private readonly IHameshService _hameshService;


        public CreateUsersModel(IUserService userService, IPermissionService permissionService, ApiTokenCacheClient apiTokenCacheClient, IWebApiService webApiService , IHameshService hameshService)
        {
            _userService = userService;
            _permissionService = permissionService;
            _apiTokenClient = apiTokenCacheClient;
            _webApiService =webApiService;
            _hameshService = hameshService;
        }


        [BindProperty]
        public CreateUserViewModel CreateUserViewModel { get; set; }
        #region اعضا و متدهای کلاس


        /// <summary>
        /// اطلاعات موردنیاز صفحه را بارگذاری می‌کند.
        /// </summary>

        public IActionResult OnGet()
        {
            //کاربر غیر ادمین صفحه رو بهش نشون نده
            CreateUserViewModel = new CreateUserViewModel();
            var userIdClaim = User.FindFirst("Id")?.Value;
            var roleTypeIdClaim = User.FindFirst("RoleTypeId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleTypeIdClaim))
            {
                return Unauthorized(); // اگر مقدارها نامعتبر بودند، کاربر غیرمجاز است
            }

            if (!int.TryParse(roleTypeIdClaim, out int roleTypeId))
            {
                return BadRequest("Invalid Role Type ID"); // اگر تبدیل موفقیت‌آمیز نبود، خطای ورودی داده نامعتبر
            }

            ViewData["RolesTitle"] = new SelectList(_permissionService.GetRoles(roleTypeId.ToString()), "RoleId", "Title");
            return Page();
        }

        /// <summary>
        /// درخواست دریافت اطلاعات صفحه را پردازش می‌کند.
        /// </summary>
        public JsonResult OnGetGetPersonalId(string personalno)
        {
            // اعتبارسنجی اولیه ورودی
            if (string.IsNullOrEmpty(personalno))
            {
                return new JsonResult(new { message = "شماره پرسنلی معتبر نیست" });
            }

            // درخواست اطلاعات پرسنلی
            var response = _webApiService.GetPersonalByPersonalNo(personalno);

            // بررسی خطاهای سرویس
            if (response == null || response.Data == null)
            {
                return new JsonResult(new { message = "پرسنلی با این مشخصات یافت نشد" });
            }

            if (response.Data.Respond == "ERROR Service")
            {
                return new JsonResult(new { message = "سرویس با خطا مواجه شد. دقایقی دیگر مجددا امتحان کنید" });
            }

            // ذخیره نتیجه در سشن
            SessionHelper.SetObjectAsJson(HttpContext.Session, "result", response.Data);

            // بازگرداندن اطلاعات پرسنلی
            return new JsonResult(response.Data);
        }


        /// <summary>
        /// اطلاعات ارسال‌شده فرم را بررسی و پردازش می‌کند.
        /// </summary>
        public IActionResult OnPost(int roleId, string password, IFormFile userAvatar)
        {
            try
            {
                // بررسی وجود کلیدهای کاربری
                var roleTypeIdClaim = User.FindFirst("RoleTypeId")?.Value;
                var userIdClaim = User.FindFirst("Id")?.Value;

                if (string.IsNullOrEmpty(roleTypeIdClaim) || string.IsNullOrEmpty(userIdClaim))
                {
                    return Page();
                }

                // دریافت نقش‌های کاربری
                var roles = _permissionService.GetRoles(roleTypeIdClaim);
                if (roles == null || !roles.Any())
                {
                    ModelState.AddModelError("", "خطایی در دریافت نقش‌ها رخ داده است.");
                    return Page();
                }

                ViewData["RolesTitle"] = new SelectList(roles, "RoleId", "Title");

                if (roleId <= 0)
                {
                    ModelState.AddModelError("", "انتخاب نقش کاربر الزامی است.");
                    return Page();
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    ModelState.AddModelError("", "رمز عبور الزامی است.");
                    return Page();
                }
                if (password.Length > 200)
                {
                    ModelState.AddModelError("", "رمز عبور نمی‌تواند بیشتر از ۲۰۰ کاراکتر باشد.");
                    return Page();
                }
                if (password.Length < 8 || !password.Any(char.IsLower) || !password.Any(char.IsUpper) ||
                    !password.Any(char.IsDigit) || !password.Any(character => !char.IsLetterOrDigit(character)))
                {
                    ModelState.AddModelError("", "رمز عبور باید حداقل ۸ کاراکتر و شامل حرف بزرگ، حرف کوچک، عدد و نماد باشد.");
                    return Page();
                }

                // بازیابی اطلاعات کاربر از سشن
                var user = HttpContext.Session.GetObjectFromJson<CreateUserViewModel>("result");
                if (user == null)
                {
                    ModelState.AddModelError("", "خطایی در بازیابی اطلاعات کاربر از سشن رخ داده است.");
                    return Page();
                }

                // تنظیم اطلاعات کاربر
                user.Password = password;
                user.AddUserId = int.Parse(userIdClaim);
                user.UserAvatar = userAvatar;

                // بررسی فرمت فایل آپلود شده
                if (user.UserAvatar != null && !FileUploadCheck.CheckImageFileExtension(user.UserAvatar))
                {
                    ModelState.AddModelError("", "فایل انتخابی معتبر نمی‌باشد.");
                    return Page();
                }
                if (user.UserAvatar != null && user.UserAvatar.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "حجم تصویر پروفایل نمی‌تواند بیشتر از ۲ مگابایت باشد.");
                    return Page();
                }

                // بررسی وجود کاربر
                if (_userService.IsExistUserName(user.PersonalCode))
                {
                    ModelState.AddModelError("", "کاربر با این مشخصات قبلاً ثبت شده است.");
                    return Page();
                }

                // افزودن کاربر جدید
                int newUserId;
                try
                {
                    newUserId = _userService.AddUserFromAdmin(user);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "خطا در ثبت کاربر جدید: " + ex.Message);
                    return Page();
                }

                // افزودن نقش‌ها به کاربر
                try
                {
                    _permissionService.AddRolesToUser(roleId, newUserId);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "خطا در افزودن نقش‌ها به کاربر: " + ex.Message);
                    return Page();
                }

                // هدایت به صفحه اصلی
                TempData["OperationTitle"] = "ثبت موفق";
                TempData["OperationMessage"] = "کاربر با موفقیت ثبت شد.";
                TempData["OperationIcon"] = "success";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "خطای سیستمی رخ داده است: " + ex.Message);
                return Page();
            }
        }





        #endregion
    }
}

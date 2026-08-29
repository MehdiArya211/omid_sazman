using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.Web.Pages
{
    public class SignUpModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IUserService _userService;
        private readonly VisitorManagmentContext _context;


        public SignUpModel(ILogger<IndexModel> logger, IUserService userService, VisitorManagmentContext context)
        {
            _logger = logger;
            _userService = userService;
            _context = context;

        }

        [BindProperty]
        public SignUpViewModel signUpViewModel { get; set; }
        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            
            if (!ModelState.IsValid)
                return Page();


           var result= _userService.SignUpUser(signUpViewModel);

            //کاربر ثبت نام شده باشد
            if (result!=0)
            {
                ViewData["successcreate"] = true;
            }

            else
            {
                ViewData["faildcreate"] = true;
            }

            ViewData["successcreate"] = true;

            return Page();

            
        }
    }
}

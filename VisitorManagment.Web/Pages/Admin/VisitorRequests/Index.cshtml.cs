using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Web.Helpers;

namespace VisitorManagment.Web.Pages.Admin.VisitorRequests
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IAdminVisitorRequestService _requestService;

        public IndexModel(IAdminVisitorRequestService requestService)
        {
            _requestService = requestService;
        }

        [BindProperty(SupportsGet = true)]
        public string PersonalCode { get; set; }

        public AdminVisitorRequestSearchViewModel Result { get; private set; }
        public string SearchMessage { get; private set; }

        public IActionResult OnGet()
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            if (string.IsNullOrWhiteSpace(PersonalCode)) return Page();

            PersonalCode = PersonalCode.Trim();
            if (PersonalCode.Length > 10 || !PersonalCode.All(char.IsDigit))
            {
                SearchMessage = "کد پرسنلی باید فقط شامل عدد و حداکثر ۱۰ رقم باشد.";
                return Page();
            }

            Result = _requestService.GetRequestsByPersonalCode(PersonalCode);
            if (Result == null)
                SearchMessage = "برای این کد پرسنلی، مراجعه‌کننده یا درخواستی یافت نشد.";
            else if (Result.Requests.Count == 0)
                SearchMessage = "مشخصات مراجعه‌کننده یافت شد، اما هنوز درخواستی برای او ثبت نشده است.";

            return Page();
        }
    }
}

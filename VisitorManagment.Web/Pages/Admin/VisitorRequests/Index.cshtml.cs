using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
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
        public List<WorkflowReceiverViewModel> Receivers { get; private set; } = new List<WorkflowReceiverViewModel>();

        public IActionResult OnGet()
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            Receivers = _requestService.GetActiveReceivers();
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

        public IActionResult OnPostTransfer(int fileId, int receiverUserId, string transferReason, string personalCode)
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            var administratorUserId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
            var result = _requestService.TransferRequest(fileId, receiverUserId, administratorUserId, transferReason);
            TempData[result.Status ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToPage(new { PersonalCode = personalCode });
        }
    }
}

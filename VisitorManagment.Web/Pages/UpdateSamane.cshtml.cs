using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.DTOs.Maintenance;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages
{
    [AllowAnonymous]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class UpdateSamaneModel : PageModel
    {
        private readonly IMaintenanceService _maintenance;
        public UpdateSamaneModel(IMaintenanceService maintenance) { _maintenance = maintenance; }
        public MaintenanceState State { get; private set; }
        public async Task<IActionResult> OnGetAsync()
        {
            State = await _maintenance.GetStateAsync();
            if (!State.IsEnabled) return RedirectToPage(User.Identity?.IsAuthenticated == true ? "/Visitor/Index" : "/Index");
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            Response.Headers["Retry-After"] = "60";
            return Page();
        }
    }
}

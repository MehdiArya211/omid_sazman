using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.Services.Interfaces;

namespace VisitorManagment.Web.Pages
{
    [AllowAnonymous]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class MaintenanceStatusModel : PageModel
    {
        private readonly IMaintenanceService _maintenance;
        public MaintenanceStatusModel(IMaintenanceService maintenance) { _maintenance = maintenance; }
        public async Task<IActionResult> OnGetAsync()
        {
            var state = await _maintenance.GetStateAsync();
            var blocked = state.IsEnabled && !await _maintenance.IsDeveloperAsync(User);
            return new JsonResult(new { blocked, maintenanceUrl = Url.Page("/UpdateSamane") });
        }
    }
}

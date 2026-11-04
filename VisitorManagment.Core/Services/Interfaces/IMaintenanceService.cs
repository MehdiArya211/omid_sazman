using System.Security.Claims;
using System.Threading.Tasks;
using VisitorManagment.Core.DTOs.Maintenance;

namespace VisitorManagment.Core.Services.Interfaces
{
    public interface IMaintenanceService
    {
        Task<MaintenanceState> GetStateAsync();
        Task<bool> IsDeveloperAsync(ClaimsPrincipal user);
        Task SetStateAsync(ClaimsPrincipal user, bool enabled, string message, int expectedRevision);
    }
}

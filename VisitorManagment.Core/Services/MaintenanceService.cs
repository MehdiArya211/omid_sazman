using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VisitorManagment.Core.DTOs.Maintenance;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.Core.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly VisitorManagmentContext _context;
        private readonly IConfiguration _configuration;

        public MaintenanceService(VisitorManagmentContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> IsDeveloperAsync(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true || user.FindFirst("IsImpersonating")?.Value == "true")
                return false;
            if (!int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("Id")?.Value, out var id) || id <= 0)
                return false;

            // نام کاربری از دیتابیس خوانده می‌شود؛ Claim قدیمی یا نقش مدیر، مجوز توسعه‌دهنده نمی‌دهد.
            var allowedNames = _configuration.GetSection("Maintenance:DeveloperUsernames").GetChildren()
                .Select(item => item.Value?.Trim()).Where(value => !string.IsNullOrWhiteSpace(value)).ToArray();
            if (allowedNames.Length == 0) return false;
            var username = await _context.Users.AsNoTracking()
                .Where(item => item.Id == id && item.IsActive && !item.IsDelete)
                .Select(item => item.UserName).SingleOrDefaultAsync();
            return username != null && allowedNames.Contains(username, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<MaintenanceState> GetStateAsync()
        {
            return await _context.MaintenanceSettings.AsNoTracking().Where(item => item.Id == 1)
                .Select(item => new MaintenanceState
                {
                    IsEnabled = item.IsEnabled, Message = item.Message, Revision = item.Revision,
                    UpdatedAt = item.UpdatedAt, UpdatedByUserId = item.UpdatedByUserId
                }).SingleOrDefaultAsync() ?? new MaintenanceState();
        }

        public async Task SetStateAsync(ClaimsPrincipal user, bool enabled, string message, int expectedRevision)
        {
            if (!await IsDeveloperAsync(user)) throw new UnauthorizedAccessException();
            message = string.IsNullOrWhiteSpace(message) ? MaintenanceState.DefaultMessage : message.Trim();
            if (message.Length > 1000) throw new ArgumentException("متن پیام نباید بیشتر از ۱۰۰۰ نویسه باشد.");
            var setting = await _context.MaintenanceSettings.SingleOrDefaultAsync(item => item.Id == 1);
            if (setting == null) throw new InvalidOperationException("ابتدا Migration تنظیمات به‌روزرسانی را روی دیتابیس اجرا کنید.");
            if (setting.Revision != expectedRevision)
                throw new DbUpdateConcurrencyException("تنظیمات در صفحه دیگری تغییر کرده است؛ صفحه را تازه‌سازی کنید.");
            setting.IsEnabled = enabled;
            setting.Message = message;
            setting.UpdatedAt = DateTime.UtcNow;
            setting.UpdatedByUserId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("Id").Value);
            setting.Revision++;
            await _context.SaveChangesAsync();
        }
    }
}

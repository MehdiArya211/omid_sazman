using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VisitorManagment.Core.DTOs.Maintenance;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Web.Hubs;

namespace VisitorManagment.Web.Pages.Developer
{
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class MaintenanceModel : PageModel
    {
        private readonly IMaintenanceService _maintenance;
        private readonly IHubContext<OnlineUsersHub> _presence;
        private readonly ILogger<MaintenanceModel> _logger;
        public MaintenanceModel(IMaintenanceService maintenance, IHubContext<OnlineUsersHub> presence, ILogger<MaintenanceModel> logger)
        { _maintenance = maintenance; _presence = presence; _logger = logger; }

        [BindProperty] public MaintenanceInput Input { get; set; } = new MaintenanceInput();
        public MaintenanceState State { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await _maintenance.IsDeveloperAsync(User)) return Forbid();
            await LoadAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!await _maintenance.IsDeveloperAsync(User)) return Forbid();
            if (!ModelState.IsValid) { State = await _maintenance.GetStateAsync(); return Page(); }
            try
            {
                await _maintenance.SetStateAsync(User, Input.IsEnabled, Input.Message, Input.Revision);
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.Clear();
                await LoadAsync();
                ModelState.AddModelError(string.Empty, "تنظیمات در صفحه دیگری تغییر کرده است. وضعیت تازه نمایش داده شد؛ دوباره تصمیم خود را ثبت کنید.");
                return Page();
            }
            catch (InvalidOperationException exception)
            {
                State = await _maintenance.GetStateAsync();
                ModelState.AddModelError(string.Empty, exception.Message);
                return Page();
            }
            // اعلان فقط پس از ذخیره موفق ارسال می‌شود؛ قطع SignalR نباید نتیجه ذخیره را ناموفق نشان دهد.
            try { await _presence.Clients.All.SendAsync("MaintenanceChanged"); }
            catch (Exception exception) { _logger.LogWarning(exception, "Maintenance saved; presence notification failed."); }
            _logger.LogWarning("Maintenance mode changed to {Enabled} by user {UserId}.", Input.IsEnabled, User.FindFirst("Id")?.Value);
            TempData["OperationTitle"] = "تنظیمات ذخیره شد";
            TempData["OperationMessage"] = Input.IsEnabled ? "سامانه در حالت به‌روزرسانی قرار گرفت؛ حساب شما دسترسی دارد." : "سامانه به حالت عادی بازگشت.";
            TempData["OperationIcon"] = "success";
            return RedirectToPage();
        }

        private async Task LoadAsync()
        {
            State = await _maintenance.GetStateAsync();
            Input = new MaintenanceInput { IsEnabled = State.IsEnabled, Message = State.Message, Revision = State.Revision };
        }

        public class MaintenanceInput
        {
            public bool IsEnabled { get; set; }
            [MaxLength(1000, ErrorMessage = "پیام نباید بیشتر از ۱۰۰۰ نویسه باشد.")]
            public string Message { get; set; }
            public int Revision { get; set; }
        }
    }
}

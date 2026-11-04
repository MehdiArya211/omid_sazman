using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Web.Helpers;
using VisitorManagment.Web.Hubs;

namespace VisitorManagment.Web.Pages.Admin.OnlineUsers
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public IndexModel(IUserService userService) => _userService = userService;

        public string SelectedDay { get; private set; }
        public IReadOnlyList<OnlineUserRow> Users { get; private set; } = Array.Empty<OnlineUserRow>();

        public IActionResult OnGet(string day = null)
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            var selectedDate = ParseDay(day) ?? DateTime.Today;
            SelectedDay = selectedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            Users = BuildRows(selectedDate);
            Response.Headers["Cache-Control"] = "no-store, no-cache";
            return Page();
        }

        public IActionResult OnGetData(string day = null)
        {
            if (!User.IsSystemAdministrator()) return Forbid();
            var selectedDate = ParseDay(day) ?? DateTime.Today;
            var rows = BuildRows(selectedDate).Select(user => new
            {
                user.UserId,
                user.PersonalCode,
                user.FullName,
                user.UnitTitle,
                firstLoginTime = user.FirstLoginTime?.ToString("HH:mm", CultureInfo.InvariantCulture)
            });
            Response.Headers["Cache-Control"] = "no-store, no-cache";
            return new JsonResult(new
            {
                day = selectedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                onlineCount = rows.Count(),
                users = rows
            });
        }

        private IReadOnlyList<OnlineUserRow> BuildRows(DateTime day)
        {
            var firstLogins = _userService.GetFirstLoginTimes(day);
            return OnlineUsersHub.GetOnlineUsers()
                .Select(user =>
                {
                    firstLogins.TryGetValue(user.PersonalCode ?? string.Empty, out var firstLogin);
                    return new OnlineUserRow
                    {
                        UserId = user.UserId,
                        PersonalCode = user.PersonalCode,
                        FullName = user.FullName,
                        UnitTitle = user.UnitTitle,
                        FirstLoginTime = firstLogin == default ? (DateTime?)null : firstLogin
                    };
                })
                .OrderBy(user => user.FullName)
                .ToList();
        }

        private static DateTime? ParseDay(string value)
        {
            if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var parsed))
                return parsed.Date;
            return null;
        }

        public class OnlineUserRow
        {
            public string UserId { get; set; }
            public string PersonalCode { get; set; }
            public string FullName { get; set; }
            public string UnitTitle { get; set; }
            public DateTime? FirstLoginTime { get; set; }
        }
    }
}

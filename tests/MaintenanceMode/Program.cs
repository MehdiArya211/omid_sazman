using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using VisitorManagment.Core.DTOs.Maintenance;
using VisitorManagment.Core.Services;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.DataLayer.Entities.User;
using VisitorManagment.DataLayer.Migrations;
using VisitorManagment.Web.Hubs;
using VisitorManagment.Web.Middleware;
using VisitorManagment.Web.Pages.Developer;

internal static class Program
{
    private static int _checks;
    private static ClaimsPrincipal User(int id, bool impersonating = false)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, id.ToString()), new Claim("Id", id.ToString()), new Claim("RoleTypeId", "100") };
        if (impersonating) claims.Add(new Claim("IsImpersonating", "true"));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
    }
    private static void Check(bool ok, string message)
    { if (!ok) throw new Exception(message); _checks++; Console.WriteLine("PASS " + message); }
    private static async Task Throws<T>(Func<Task> action, string message) where T : Exception
    { try { await action(); } catch (T) { Check(true, message); return; } throw new Exception(message); }

    public static async Task Main()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        { ["Maintenance:DeveloperUsernames:0"] = "95003599" }).Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        // A shared InMemory store across scopes, equivalent to reopening the same test database.
        var options = new DbContextOptionsBuilder<VisitorManagmentContext>().UseInMemoryDatabase("maintenance-tests").Options;
        services.AddScoped(provider => new VisitorManagmentContext(options));
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        using var provider = services.BuildServiceProvider();
        using (var scope = provider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<VisitorManagmentContext>();
            await context.Database.EnsureCreatedAsync();
            context.Users.AddRange(
                new Users { Id = 1, UserName = "95003599", FirstName = "Dev", LastName = "Test", Password = "unused", IsActive = true },
                new Users { Id = 2, UserName = "ordinary", FirstName = "User", LastName = "Test", Password = "unused", IsActive = true });
            await context.SaveChangesAsync();
        }
        var developer = User(1);
        var ordinary = User(2);
        using (var scope = provider.CreateScope())
        {
            var maintenance = scope.ServiceProvider.GetRequiredService<IMaintenanceService>();
            Check(!(await maintenance.GetStateAsync()).IsEnabled, "default state is disabled");
            Check(await maintenance.IsDeveloperAsync(developer), "configured active developer is allowed");
            Check(!await maintenance.IsDeveloperAsync(ordinary), "an administrator claim alone grants no developer access");
            Check(!await maintenance.IsDeveloperAsync(User(1, true)), "impersonation cannot bypass maintenance");
            Check(!await maintenance.IsDeveloperAsync(new ClaimsPrincipal()), "anonymous user cannot bypass");
            await Throws<UnauthorizedAccessException>(() => maintenance.SetStateAsync(ordinary, true, "x", 0), "ordinary user cannot enable maintenance");
            await Throws<ArgumentException>(() => maintenance.SetStateAsync(developer, true, new string('a', 1001), 0), "oversized public message is rejected");
            await maintenance.SetStateAsync(developer, true, "test message", 0);
            var state = await maintenance.GetStateAsync();
            Check(state.IsEnabled && state.Revision == 1 && state.UpdatedByUserId == 1 && state.UpdatedAt.HasValue, "enable persists revision and audit");
            await Throws<DbUpdateConcurrencyException>(() => maintenance.SetStateAsync(developer, false, "stale", 0), "stale browser tab cannot overwrite newer settings");
        }
        using (var scope = provider.CreateScope())
            Check((await scope.ServiceProvider.GetRequiredService<IMaintenanceService>().GetStateAsync()).IsEnabled, "new request reads persistent enabled state");

        await Request(provider, "/Visitor/Index", "GET", ordinary, 302, false);
        await Request(provider, "/Visitor/File/PersonalNezami/ListFile", "POST", ordinary, 503, false);
        await Request(provider, "/NezRTCHub/negotiate", "POST", ordinary, 503, false);
        await Request(provider, "/Visitor/Index", "GET", developer, 200, true);
        await Request(provider, "/Index", "GET", new ClaimsPrincipal(), 200, true);
        await Request(provider, "/Index", "POST", new ClaimsPrincipal(), 200, true);
        await Request(provider, "/Captcha/Index", "GET", new ClaimsPrincipal(), 200, true);
        await Request(provider, "/ChangePasswordRequired", "POST", developer, 200, true);
        await Request(provider, "/Admin/Users/Logout", "GET", ordinary, 200, true);
        await Request(provider, "/DevelopmentImpersonation", "POST", ordinary, 200, true); // Existing handler still verifies restoration credentials.
        await Request(provider, "/Developer/Maintenance", "GET", ordinary, 200, true); // The page itself must authorize.
        await Request(provider, "/Index/Unexpected", "GET", ordinary, 302, false);
        await Request(provider, "/Visitor/Index", "GET", ordinary, 503, false, ajax: true);
        await Request(provider, "/Visitor/Index", "GET", ordinary, 302, false, pathBase: "/omid");

        using (var scope = provider.CreateScope())
        {
            var page = new MaintenanceModel(scope.ServiceProvider.GetRequiredService<IMaintenanceService>(), new PresenceContext(), NullLogger<MaintenanceModel>.Instance)
            { PageContext = new PageContext { HttpContext = new DefaultHttpContext { User = ordinary } } };
            Check(await page.OnGetAsync() is ForbidResult, "control page denies a nondeveloper GET");
            Check(await page.OnPostAsync() is ForbidResult, "control page denies a nondeveloper POST even on exempt recovery path");
        }
        var filter = new MaintenanceHubFilter(provider.GetRequiredService<IServiceScopeFactory>());
        var caller = new Caller(ordinary);
        var invocation = new HubInvocationContext(caller, provider, new TestHub(), typeof(TestHub).GetMethod(nameof(TestHub.Ping)), Array.Empty<object>());
        var called = false;
        await Throws<HubException>(async () => { await filter.InvokeMethodAsync(invocation, value => { called = true; return new ValueTask<object>("ok"); }); }, "existing hub invocation is rejected during maintenance");
        Check(caller.Aborted && !called, "blocked hub invocation never reaches the method");
        var connected = false;
        await filter.OnConnectedAsync(new HubLifetimeContext(new Caller(ordinary), provider, new TestHub()), value => { connected = true; return Task.CompletedTask; });
        Check(!connected, "new hub connection cannot run OnConnected during maintenance");
        var devCaller = new Caller(developer);
        await filter.InvokeMethodAsync(new HubInvocationContext(devCaller, provider, new TestHub(), typeof(TestHub).GetMethod(nameof(TestHub.Ping)), Array.Empty<object>()), value => { called = true; return new ValueTask<object>("ok"); });
        Check(called && !devCaller.Aborted, "developer can invoke hub methods while maintenance is enabled");

        using (var scope = provider.CreateScope())
        {
            var maintenance = scope.ServiceProvider.GetRequiredService<IMaintenanceService>();
            var http = new DefaultHttpContext { User = developer };
            var page = new MaintenanceModel(maintenance, new PresenceContext(), NullLogger<MaintenanceModel>.Instance)
            {
                PageContext = new PageContext { HttpContext = http },
                TempData = new TempDataDictionary(http, new TempDataProvider()),
                Input = new MaintenanceModel.MaintenanceInput { IsEnabled = false, Message = "", Revision = 1 }
            };
            Check(await page.OnPostAsync() is RedirectToPageResult, "developer POST disables maintenance even if realtime notification is unavailable");
            Check(!(await maintenance.GetStateAsync()).IsEnabled, "developer control POST persisted disabled state");
            Check((await maintenance.GetStateAsync()).Message == MaintenanceState.DefaultMessage, "empty message uses the default");
        }
        await Request(provider, "/Visitor/Index", "POST", ordinary, 200, true);
        using (var scope = provider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<VisitorManagmentContext>();
            var account = await context.Users.SingleAsync(user => user.Id == 1);
            account.IsActive = false;
            await context.SaveChangesAsync();
            Check(!await scope.ServiceProvider.GetRequiredService<IMaintenanceService>().IsDeveloperAsync(developer), "disabled developer account loses bypass despite an existing cookie");
        }
        var migration = new AddMaintenanceMode();
        var seed = migration.UpOperations.OfType<InsertDataOperation>().Single();
        Check(seed.ColumnTypes.Length == 4, "migration seed has explicit SQL types without a designer model");
        Check(seed.Values[0, 1] is false, "migration seeds maintenance disabled");
        Check(migration.UpOperations.OfType<CreateTableOperation>().Single().CheckConstraints.Any(), "migration enforces singleton settings row");
        Console.WriteLine($"{_checks} checks passed against linked production code (.NET 5 / EF Core 5 InMemory). No SQL Server or full-site test was run.");
    }

    private static async Task Request(ServiceProvider provider, string path, string method, ClaimsPrincipal user, int expectedStatus, bool expectedNext, bool ajax = false, string pathBase = "")
    {
        using var scope = provider.CreateScope();
        var context = new DefaultHttpContext { User = user, RequestServices = scope.ServiceProvider };
        context.Request.Path = path; context.Request.Method = method; context.Request.PathBase = pathBase;
        context.Request.Headers["Accept"] = "text/html";
        if (ajax) context.Request.Headers["X-Requested-With"] = "XMLHttpRequest";
        context.Response.Body = new MemoryStream();
        var next = false;
        await new MaintenanceMiddleware(value => { next = true; return Task.CompletedTask; })
            .InvokeAsync(context, scope.ServiceProvider.GetRequiredService<IMaintenanceService>());
        Check(context.Response.StatusCode == expectedStatus && next == expectedNext, $"HTTP {method} {path} status {expectedStatus}, next={expectedNext}");
        if (expectedStatus == 302) Check(context.Response.Headers["Location"] == pathBase + "/UpdateSamane", "maintenance redirect respects PathBase");
    }

    private class TestHub : Hub { public string Ping() => "pong"; }
    private class Caller : HubCallerContext
    {
        public Caller(ClaimsPrincipal user) { User = user; }
        public bool Aborted { get; private set; }
        public override string ConnectionId => "test";
        public override string UserIdentifier => "test";
        public override ClaimsPrincipal User { get; }
        public override IDictionary<object, object> Items { get; } = new Dictionary<object, object>();
        public override IFeatureCollection Features { get; } = new FeatureCollection();
        public override CancellationToken ConnectionAborted => CancellationToken.None;
        public override void Abort() { Aborted = true; }
    }
    private class TempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }
    private class PresenceContext : IHubContext<OnlineUsersHub>
    {
        public IHubClients Clients => throw new NotSupportedException();
        public IGroupManager Groups => throw new NotSupportedException();
    }
}

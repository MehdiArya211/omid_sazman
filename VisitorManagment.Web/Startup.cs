using ITOWebApiClient;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using VisitorManagment.Core.Services;
using VisitorManagment.Core.Services.Interfaces;
using VisitorManagment.Core.Services.Interfaces.Ranking;
using VisitorManagment.Core.Services.Interfaces.Reports;
using VisitorManagment.Core.Services.Interfaces.RolePermissions;
using VisitorManagment.Core.Services.Notification;
using VisitorManagment.Core.Services.Reports;
using VisitorManagment.Core.Services.SystemChatOnline;
using VisitorManagment.Core.Services.SystemLog;
using VisitorManagment.DataLayer.Context;
using VisitorManagment.Web.Filters;
using VisitorManagment.Web.Hubs;

namespace VisitorManagment.Web
{
	public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            //تغییر روتینگ پیش فرض سامانه
           // services.AddMvc().AddRazorPagesOptions(options => options.Conventions.AddPageRoute("/UpdateSamane", ""));
            services.AddSignalR();
            //دو خط زیر برای سیشن
            services.AddSession();
            services.AddMvc();
            //services.AddControllers().AddNewtonsoftJson();
            //********************************************/////////////////////
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;

                // ثبت Global Filter برای لاگ گرفتن از تمام اکشن‌های کاربر
                options.Filters.Add<UserActionLogFilter>();
            });

            //********************************************/////////////////////

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(System.Net.IPAddress.Parse("0.0.0.0"));
            });
            #region Authentication

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }
            ).AddCookie(options =>
            {
                options.LoginPath = "/Index";
                options.LogoutPath = "/Admin/Users/Logout";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            });

            #endregion

            #region DataBase
            services.AddDbContext<VisitorManagmentContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Connection"));
            });
            #endregion

            #region IOC
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IWebApiService, WebApiService>();
            services.AddTransient<IWorkFlowService, WorkFlowService>();
            services.AddTransient<ICartableService, CartableService>();
            services.AddTransient<IHameshService, HameshService>();
            services.AddTransient<ISpecificationPersonal, SpecificationPersonalService>();
            services.AddTransient<IMeetingService, MeetingService>();
            services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<ISmsService, SmsService>();
            services.AddTransient<IReportStimulService, ReportStimulService>();
            services.AddTransient<IChartService, ChartService>();
            services.AddTransient<IHamishReportService, HamishReportService>();
            services.AddTransient<IProblemNezReportService, ProblemNezReportService>();
            services.AddTransient<IRequestGhaReportService, RequestGhaReportService>();
            //Vam
            services.AddTransient<IVamService, VamService>();
            services.AddTransient<IVamCodeService, VamCodeService>();
            //ranking
            services.AddTransient<IRankingService, RankingService>();
            //role Permission
            services.AddTransient<IRolePermissionService, RolePermissionService>();
            services.AddTransient<IChatRoomService, ChatRoomService>();
            services.AddTransient<IMessageService, MessageService>();
            //notification
            services.AddTransient<INotificationManager, NotificationManager>();
            //log
            services.AddTransient<IUserActionLogger, UserActionLogger>();
            services.AddScoped<UserActionLogFilter>();


            //*****************************************************************
            services.AddSingleton<ApiTokenCacheClient>();
            services.AddHttpClient();
            services.AddDistributedMemoryCache();
            //*****************************************************************


            #endregion

            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Error");
            }
            // خط زیر برای سیشن
            app.UseSession();
            app.UseStaticFiles();

            app.UseRouting();
            //جهت انتقال پورت از کسترل به nginx در سرور لینوکس
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();

            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                //برای برقراری ارتباط بین فایل جی اس وب ار تی سی و کلاس هاب
                
            endpoints.MapHub<Hubs.NezRTCHub>("/NezRTCHub", options =>
                {
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
                });

                endpoints.MapHub<SiteChatHub>("/chathub");
                endpoints.MapHub<SupportHub>("/supporthub");
                // هاب کاربران آنلاین
                endpoints.MapHub<OnlineUsersHub>("/onlineUsersHub");
            });

            //*****************************************************

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });

            //*****************************************************
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;


namespace VisitorManagment.Web
{

    public class Program
    {
        #region اعضا و متدهای کلاس

        /// <summary>
        /// عملیات مربوط به این بخش را انجام می‌دهد.
        /// </summary>

        public static void Main(string[] args)
        {
            // پیکربندی اولیه Serilog
            // این بخش قبل از بالا آمدن برنامه اجرا می‌شود
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path: "Logs/user-actions-.log",   // مسیر ذخیره فایل لاگ
                    rollingInterval: RollingInterval.Day, // ایجاد فایل لاگ روزانه
                    retainedFileCountLimit: 30,       // نگه‌داری لاگ‌ها به مدت 30 روز
                    shared: true                      // امکان دسترسی همزمان چند پروسس
                )
                .CreateLogger();

            try
            {
                Log.Information("Application starting...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                // ثبت خطاهای بحرانی هنگام اجرای برنامه
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                // اطمینان از ذخیره کامل لاگ‌ها قبل از خروج برنامه
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// اطلاعات جدید را اعتبارسنجی و ثبت می‌کند.
        /// </summary>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // اتصال Serilog به Host برنامه
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //خط زیر برای پابلیش لینوکس باید از کامنت در بیاد
                    //webBuilder.UseKestrel();
                    webBuilder.UseStartup<Startup>();
                    //جهت دسترسی IP های سرور کسترل در لینوکس
                    webBuilder.UseUrls("http://*:5000", "https://*:5001");
                });
        #endregion
    }
}

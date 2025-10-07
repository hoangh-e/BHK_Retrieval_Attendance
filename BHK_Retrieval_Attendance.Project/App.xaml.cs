using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using BHK.Retrieval.Attendance.WPF.Configuration.DI;
// TODO: Uncomment sau khi tạo MainWindow
// using BHK.Retrieval.Attendance.WPF.Views.Windows;

namespace BHK_Retrieval_Attendance.Project
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static IConfiguration Configuration { get; private set; } = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Build Configuration
                Configuration = BuildConfiguration();

                // Configure Serilog
                ConfigureLogging();

                // Build Host with DI
                _host = CreateHostBuilder(e.Args).Build();
                ServiceProvider = _host.Services;

                // Start the host
                await _host.StartAsync();

                // TODO: Uncomment sau khi tạo MainWindow
                // var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                // mainWindow.Show();

                // Tạm thời show message để test app startup
                MessageBox.Show("Application Started Successfully!\n\nNuGet packages đã được cài đặt.\nTiếp theo: Tạo cấu trúc project và MainWindow.", 
                    "✅ Build Thành Công", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
                
                Log.Information("Application started successfully");
                
                // Shutdown sau khi show message (vì chưa có MainWindow)
                Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application startup failed: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                    "❌ Startup Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                Log.Fatal(ex, "Application startup failed");
                Current.Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }

            Log.CloseAndFlush();
            base.OnExit(e);
        }

        private static IConfiguration BuildConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
            
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static void ConfigureLogging()
        {
            var logPath = Configuration["Logging:File:Path"] ?? "Logs/app-.log";
            var outputTemplate = Configuration["Logging:File:OutputTemplate"] ?? 
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.File(
                    path: logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: outputTemplate)
                .WriteTo.Debug()
                .CreateLogger();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((context, services) =>
                {
                    // Register all services using Bootstrapper
                    Bootstrapper.ConfigureServices(services, Configuration);
                });
        }
    }
}


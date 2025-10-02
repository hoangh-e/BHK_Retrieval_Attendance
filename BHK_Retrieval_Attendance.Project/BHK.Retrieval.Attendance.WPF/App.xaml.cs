using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using BHK.Retrieval.Attendance.WPF.Configuration.DependencyInjection;
using BHK.Retrieval.Attendance.WPF.ViewModels;
using BHK.Retrieval.Attendance.WPF.Views.Windows;

namespace BHK_Retrieval_Attendance.Project
{
    public partial class App : Application
    {
        private IHost? _host;

        public App()
        {
            _host = CreateHostBuilder().Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host!.StartAsync();

            // Configure Serilog
            ConfigureSerilog();

            // Show main window
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _host.Services.GetRequiredService<MainWindowViewModel>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host!.StopAsync();
            }
            Log.CloseAndFlush();
            base.OnExit(e);
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                          .AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    // Register all services
                    ServiceRegistrar.RegisterServices(services, context.Configuration);
                    ViewModelRegistrar.RegisterViewModels(services);
                    
                    // Register Windows
                    services.AddSingleton<MainWindow>();
                })
                .UseSerilog();
        }

        private void ConfigureSerilog()
        {
            var configuration = _host!.Services.GetRequiredService<IConfiguration>();
            
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}

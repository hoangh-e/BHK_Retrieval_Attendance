using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BHK.Retrieval.Attendance.WPF.ViewModels;
using BHK.Retrieval.Attendance.WPF.Views.Windows;
using BHK.Retrieval.Attendance.WPF.Views.Pages;
using BHK.Retrieval.Attendance.WPF.Services;
using BHK.Retrieval.Attendance.WPF.Models;
using Riss.Devices;

namespace BHK.Retrieval.Attendance.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            _host = CreateHostBuilder().Build();
            
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }

        private IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Logging
                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                        builder.AddDebug();
                    });

                    // Device Services
                    services.AddSingleton<IDeviceService, DeviceService>();
                    services.AddSingleton<DeviceConnection>();

                    // Application Services
                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddTransient<IDialogService, DialogService>();
                    services.AddTransient<IUserService, UserService>();
                    services.AddTransient<IAttendanceService, AttendanceService>();

                    // ViewModels
                    services.AddTransient<MainWindowViewModel>();
                    services.AddTransient<DeviceConnectionViewModel>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<EmployeeSelectionViewModel>();
                    services.AddTransient<EmployeeDetailViewModel>();

                    // Views
                    services.AddSingleton<MainWindow>();
                    services.AddTransient<DeviceConnectionView>();
                    services.AddTransient<LoginView>();
                    services.AddTransient<EmployeeSelectionView>();
                    services.AddTransient<EmployeeDetailView>();
                });
        }
    }
}

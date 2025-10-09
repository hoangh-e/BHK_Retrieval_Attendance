using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using BHK.Retrieval.Attendance.WPF.Views;
using BHK.Retrieval.Attendance.WPF.Views.Pages;
using BHK.Retrieval.Attendance.WPF.ViewModels;
using BHK.Retrieval.Attendance.WPF.ViewModels.Pages;

namespace BHK.Retrieval.Attendance.WPF
{
    public partial class App : Application
    {
        private IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // 1. Đăng ký Services (nếu có)
                    // services.AddSingleton<IYourService, YourService>();
                    
                    // 2. Đăng ký ViewModels TRƯỚC (QUAN TRỌNG)
                    services.AddSingleton<ViewModels.Pages.HomePageViewModel>();  // Sử dụng Singleton
                    services.AddSingleton<MainWindowViewModel>();
                    
                    // 3. Đăng ký Views SAU
                    services.AddSingleton<MainWindow>();
                    services.AddTransient<HomePageView>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            // Lấy MainWindow từ DI Container
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }



        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync();
            }

            base.OnExit(e);
        }
    }
}

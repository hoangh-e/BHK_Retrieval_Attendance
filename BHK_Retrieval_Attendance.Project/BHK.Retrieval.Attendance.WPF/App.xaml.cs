using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using BHK.Retrieval.Attendance.WPF.Configuration.DI;
using BHK.Retrieval.Attendance.WPF.Views.Pages;
using BHK.Retrieval.Attendance.WPF.ViewModels;

namespace BHK.Retrieval.Attendance.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        public App()
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Debug()
                .CreateLogger();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Build host with DI
                _host = Host.CreateDefaultBuilder()
                    .UseSerilog()
                    .ConfigureServices((context, services) =>
                    {
                        // Configure services
                        ConfigureServices(services);
                    })
                    .Build();

                await _host.StartAsync();

                // Show Device Connection View
                ShowDeviceConnectionView();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
                MessageBox.Show($"Application startup failed: {ex.Message}", 
                    "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register application services
            services.AddApplicationServices();

            // Register ViewModels
            services.AddViewModels();

            // Register Views
            services.AddViews();

            // TODO: Add other configurations
            // services.AddAutoMapper(typeof(App).Assembly);
            // services.AddValidatorsFromAssembly(typeof(App).Assembly);
        }

        private void ShowDeviceConnectionView()
        {
            try
            {
                var view = _host.Services.GetRequiredService<DeviceConnectionView>();
                var viewModel = _host.Services.GetRequiredService<DeviceConnectionViewModel>();
                
                view.DataContext = viewModel;

                var window = new Window
                {
                    Title = "BHK Attendance - Device Connection",
                    Content = view,
                    Width = 900,
                    Height = 700,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                window.Show();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error showing Device Connection View");
                MessageBox.Show($"Error: {ex.Message}", "Startup Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                if (_host != null)
                {
                    await _host.StopAsync();
                    _host.Dispose();
                }

                Log.CloseAndFlush();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during application exit");
            }
            finally
            {
                base.OnExit(e);
            }
        }
    }
}

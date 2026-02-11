using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using BHK.Retrieval.Attendance.WPF.Configuration.DI;
using BHK.Retrieval.Attendance.WPF.Views.Pages;
using BHK.Retrieval.Attendance.WPF.Views.Windows;
using BHK.Retrieval.Attendance.WPF.ViewModels;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.Utilities;
using BHK.Retrieval.Attendance.Shared.Options;

namespace BHK.Retrieval.Attendance.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;
        private IConfiguration? _configuration;

        public App()
        {
            // Configure basic Serilog (will be reconfigured from appsettings later)
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
                // Build host with proper configuration
                _host = Host.CreateDefaultBuilder()
                    .ConfigureServices((hostContext, services) =>
                    {
                        // 🎯 COMPOSITION ROOT: Load configuration here
                        _configuration = services.ConfigureAppConfiguration();
                        
                        // Configure options từ appsettings.json
                        services.ConfigureOptions(_configuration);
                        
                        // Configure Serilog from appsettings
                        ConfigureSerilogFromAppSettings(_configuration);
                        
                        // Configure other services
                        ConfigureServices(services, _configuration);
                    })
                    .UseSerilog()
                    .Build();

                await _host.StartAsync();

                // Show MainWindow instead of DeviceConnectionView directly
                ShowMainWindow();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
                
                // ✅ Use DialogHelper for application startup error
                DialogHelper.ShowError(
                    "Ứng dụng không thể khởi động",
                    ex.Message,
                    "Lỗi nghiêm trọng"
                );
                
                Current.Shutdown();
            }
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // 🏗️ Sử dụng ServiceRegistrar mới - đăng ký tất cả services
            services.RegisterServices(configuration);

            // TODO: Add other configurations
            // services.AddAutoMapper(typeof(App).Assembly);
            // services.AddValidatorsFromAssembly(typeof(App).Assembly);
        }

        private void ConfigureSerilogFromAppSettings(IConfiguration configuration)
        {
            // ✅ FIX: Serilog:Using section added in appsettings.json for Single File publish
            // Reconfigure Serilog with settings from appsettings.json
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        /// <summary>
        /// Show MainWindow với ContentControl navigation
        /// </summary>
        private void ShowMainWindow()
        {
            try
            {
                if (_host == null)
                    throw new InvalidOperationException("Host is not initialized");
                
                Log.Information("Resolving MainWindow and ViewModels...");
                
                // ✅ Resolve NavigationService để khởi tạo
                var navigationService = _host.Services.GetRequiredService<INavigationService>();
                
                // ✅ Resolve MainWindow (sẽ tự động inject MainWindowViewModel)
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                
                Log.Information("MainWindow resolved successfully");
                
                // ✅ Navigate đến DeviceConnectionView ngay khi show
                navigationService.NavigateTo<DeviceConnectionViewModel>();
                
                // Show window
                mainWindow.Show();
                
                Log.Information("MainWindow shown with DeviceConnectionView");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error showing MainWindow");
                
                // Fallback
                Log.Information("Falling back to DeviceConnectionView");
                ShowDeviceConnectionView();
            }
        }

        /// <summary>
        /// Show DeviceConnectionView directly (Fallback approach)
        /// </summary>
        private void ShowDeviceConnectionView()
        {
            try
            {
                if (_host == null)
                {
                    throw new InvalidOperationException("Host is not initialized");
                }
                
                var view = _host.Services.GetRequiredService<DeviceConnectionView>();
                var viewModel = _host.Services.GetRequiredService<DeviceConnectionViewModel>();
                
                // Get UI settings from configuration
                var uiOptions = _host.Services.GetRequiredService<IOptions<UIOptions>>();
                var appOptions = _host.Services.GetRequiredService<IOptions<ApplicationOptions>>();
                
                view.DataContext = viewModel;

                var window = new Window
                {
                    Title = $"{appOptions.Value.ApplicationName} - Device Connection",
                    Content = view,
                    Width = uiOptions.Value.WindowWidth,
                    Height = uiOptions.Value.WindowHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    WindowState = Enum.Parse<WindowState>(uiOptions.Value.WindowState)
                };

                window.Show();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error showing Device Connection View");
                
                // ✅ Use DialogHelper for window creation error
                DialogHelper.ShowError(
                    "Lỗi khi hiển thị giao diện kết nối",
                    ex.Message,
                    "Lỗi khởi động"
                );
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

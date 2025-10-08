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
using BHK.Retrieval.Attendance.WPF.ViewModels;
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

                // Show Device Connection View with UI settings from config
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

        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // 🏗️ Clean Architecture Service Registration
            // Core layer services
            services.RegisterCoreServices();

            // Infrastructure layer services (cần configuration)
            services.RegisterInfrastructureServices(configuration);

            // Application layer services (bao gồm IConfigurationService)
            services.AddApplicationServices();

            // WPF layer services
            services.RegisterWpfServices();

            // TODO: Add other configurations
            // services.AddAutoMapper(typeof(App).Assembly);
            // services.AddValidatorsFromAssembly(typeof(App).Assembly);
        }

        private void ConfigureSerilogFromAppSettings(IConfiguration configuration)
        {
            // Reconfigure Serilog with settings from appsettings.json
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private void ShowDeviceConnectionView()
        {
            try
            {
                if (_host == null)
                {
                    throw new InvalidOperationException("Host is not initialized");
                }

                // Test configuration trước khi show view
                var configDemo = _host.Services.GetRequiredService<ConfigurationDemoViewModel>();
                
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

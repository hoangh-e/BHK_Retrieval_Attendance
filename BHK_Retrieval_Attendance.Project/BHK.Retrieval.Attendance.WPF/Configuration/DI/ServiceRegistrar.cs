using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.Services.Implementations;
using BHK.Retrieval.Attendance.WPF.ViewModels;
using BHK.Retrieval.Attendance.WPF.Views.Pages;
using BHK.Retrieval.Attendance.WPF.Views.Windows;
using BHK.Retrieval.Attendance.Shared.Options;
using BHK.Retrieval.Attendance.Infrastructure.Configuration;

namespace BHK.Retrieval.Attendance.WPF.Configuration.DI
{
    /// <summary>
    /// Service registration cho Dependency Injection container
    /// </summary>
    public static class ServiceRegistrar
    {
        /// <summary>
        /// Đăng ký tất cả services vào DI container
        /// </summary>
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration Options
            RegisterOptions(services, configuration);

            // Infrastructure Services (Device Communication)
            services.AddDeviceServices();

            // Application Services
            RegisterApplicationServices(services);

            // ViewModels
            RegisterViewModels(services);

            // Views/Pages
            RegisterViews(services);

            return services;
        }

        /// <summary>
        /// Đăng ký Configuration Options (IOptions pattern)
        /// </summary>
        private static void RegisterOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApplicationOptions>(configuration.GetSection(ApplicationOptions.SectionName));
            services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
            services.Configure<DeviceOptions>(configuration.GetSection(DeviceOptions.SectionName));
            services.Configure<SharePointOptions>(configuration.GetSection(SharePointOptions.SectionName));
            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
            services.Configure<ReportOptions>(configuration.GetSection(ReportOptions.SectionName));
            services.Configure<UIOptions>(configuration.GetSection(UIOptions.SectionName));
        }

        /// <summary>
        /// Đăng ký Application Services
        /// </summary>
        private static void RegisterApplicationServices(IServiceCollection services)
        {
            // Device Service - Scoped
            services.AddScoped<IDeviceService, DeviceService>();

            // Dialog Service - Singleton
            services.AddSingleton<IDialogService, DialogService>();

            // ✅ SỬA: NavigationService - Singleton
            services.AddSingleton<NavigationService>();
            services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<NavigationService>());

            // Configuration Service - Singleton
            services.AddSingleton<IConfigurationService, ConfigurationService>();
        }

        /// <summary>
        /// Đăng ký ViewModels
        /// </summary>
        private static void RegisterViewModels(IServiceCollection services)
        {
            // Transient vì mỗi view sẽ có instance riêng
            services.AddTransient<DeviceConnectionViewModel>();
            services.AddTransient<HomePageViewModel>();
            services.AddTransient<DeviceViewModel>();
            
            // TODO: Thêm các ViewModels khác khi implement
            // services.AddTransient<DashboardViewModel>();
            // services.AddTransient<AttendanceListViewModel>();
            // services.AddTransient<EmployeeListViewModel>();
            // services.AddTransient<SettingsViewModel>();
        }

        /// <summary>
        /// Đăng ký Views/Pages
        /// </summary>
        private static void RegisterViews(IServiceCollection services)
        {
            // Transient vì mỗi lần navigate tạo instance mới
            services.AddTransient<DeviceConnectionView>(sp =>
            {
                var view = new DeviceConnectionView();
                var viewModel = sp.GetRequiredService<DeviceConnectionViewModel>();
                view.DataContext = viewModel;
                return view;
            });

            services.AddTransient<HomePageView>(sp =>
            {
                var viewModel = sp.GetRequiredService<HomePageViewModel>();
                var logger = sp.GetRequiredService<ILogger<HomePageView>>();
                return new HomePageView(viewModel, logger);
            });

            // Main Window - Transient vì window nên được tạo mới mỗi lần cần
            services.AddTransient<BHK.Retrieval.Attendance.WPF.Views.Windows.MainWindow>();

            // TODO: Thêm các Views khác khi implement
            // services.AddTransient<DashboardView>(sp => { ... });
            // services.AddTransient<AttendanceListView>(sp => { ... });
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.Services.Implementations;
using BHK.Retrieval.Attendance.WPF.Services;
using BHK.Retrieval.Attendance.WPF.ViewModels;
using BHK.Retrieval.Attendance.WPF.ViewModels.Dialogs;
using BHK.Retrieval.Attendance.WPF.Views.Pages;
using BHK.Retrieval.Attendance.WPF.Views.Windows;
using BHK.Retrieval.Attendance.Shared.Options;
using BHK.Retrieval.Attendance.Infrastructure.Configuration;
using BHK.Retrieval.Attendance.Core.Interfaces.Services;
using BHK.Retrieval.Attendance.Infrastructure.Services;

namespace BHK.Retrieval.Attendance.WPF.Configuration.DI
{
    /// <summary>
    /// Service registration cho Dependency Injection container
    /// </summary>
    public static class ServiceRegistrar
    {
        /// <summary>
        /// Đăng ký tất cả services vào DI container
        /// ✅ THỨ TỰ QUAN TRỌNG!
        /// </summary>
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Configuration Options
            RegisterOptions(services, configuration);

            // 2. Infrastructure Services (Device Communication)
            services.AddDeviceServices();

            // ✅ 3. ViewModels (TRƯỚC Application Services)
            RegisterViewModels(services);

            // ✅ 4. Application Services (SAU ViewModels)
            RegisterApplicationServices(services);

            // 5. Views/Pages
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
            services.Configure<OneDriveOptions>(configuration.GetSection(OneDriveOptions.SectionName));
            
            // ✅ Settings mới cho Excel export
            services.Configure<OneDriveSettings>(configuration.GetSection("OneDriveSettings"));
            services.Configure<SharePointSettings>(configuration.GetSection("SharePointSettings"));
        }

        /// <summary>
        /// Đăng ký Application Services
        /// ✅ ĐĂNG KÝ SAU ViewModels
        /// </summary>
        private static void RegisterApplicationServices(IServiceCollection services)
        {
            // Device Service - Singleton (giữ connection state trong toàn bộ vòng đời app)
            // ✅ WPF Desktop App cần Singleton, không phải Scoped như Web App
            services.AddSingleton<IDeviceService, DeviceService>();

            // Dialog Service - Singleton
            services.AddSingleton<IDialogService, DialogService>();

            // Configuration Service - Singleton
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            
            // Notification Service - Singleton
            services.AddSingleton<INotificationService, NotificationService>();
            
            // Attendance Service - Scoped
            services.AddScoped<IAttendanceService, AttendanceService>();

            // Path Settings Service - Singleton
            services.AddSingleton<IPathSettingsService, PathSettingsService>();

            // Excel Service - Singleton
            services.AddSingleton<IExcelService, ExcelService>();

            // ✅ Path Configuration Service - Singleton (Excel export paths)
            services.AddSingleton<IPathConfigurationService, PathConfigurationService>();

            // ✅ Excel Table Service - Singleton (Excel read/write operations)
            services.AddSingleton<IExcelTableService, ExcelTableService>();

            // ✅ NavigationService - Singleton (phụ thuộc MainWindowViewModel)
            services.AddSingleton<NavigationService>();
            services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<NavigationService>());

            // TODO: Thêm các services khác khi implement
        }

        /// <summary>
        /// Đăng ký ViewModels
        /// ✅ ĐĂNG KÝ TRƯỚC Application Services
        /// </summary>
        private static void RegisterViewModels(IServiceCollection services)
        {
            // ✅ MainWindowViewModel - Singleton (QUAN TRỌNG!)
            services.AddSingleton<MainWindowViewModel>();
            
            // DeviceConnectionViewModel - Singleton (initial view)
            services.AddSingleton<DeviceConnectionViewModel>();
            
            // ViewModels khác - Transient
            services.AddTransient<HomePageViewModel>();
            services.AddTransient<DeviceViewModel>();
            services.AddTransient<EmployeeViewModel>();
            services.AddTransient<AttendanceManagementViewModel>();
            services.AddTransient<ExportConfigurationDialogViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<AboutViewModel>();
            services.AddTransient<ViewModels.ExportEmployeeViewModel>();
            
            // ✅ Export Dialog ViewModels
            services.AddTransient<ViewModels.Dialogs.ExportAttendanceDialogViewModel>();
            services.AddTransient<ViewModels.Dialogs.ExportEmployeeDialogViewModel>();
            
            // ✅ Factory functions for ViewModels
            services.AddTransient<Func<ViewModels.ExportEmployeeViewModel>>(provider => 
                () => provider.GetRequiredService<ViewModels.ExportEmployeeViewModel>());
            
            // TODO: Thêm các ViewModels khác khi implement
        }

        /// <summary>
        /// Đăng ký Views/Pages
        /// </summary>
        private static void RegisterViews(IServiceCollection services)
        {
            // ✅ CHỈ cần đăng ký MainWindow
            // Các Views khác WPF tự tạo từ DataTemplate
            services.AddSingleton<MainWindow>();
        }
    }
}
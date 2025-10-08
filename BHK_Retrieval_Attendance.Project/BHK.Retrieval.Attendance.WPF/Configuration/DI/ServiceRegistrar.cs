using Microsoft.Extensions.DependencyInjection;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.Services.Implementations;
using BHK.Retrieval.Attendance.WPF.ViewModels;
using BHK.Retrieval.Attendance.WPF.Views.Pages;

namespace BHK.Retrieval.Attendance.WPF.Configuration.DI
{
    /// <summary>
    /// Service registration cho Dependency Injection
    /// </summary>
    public static class ServiceRegistrar
    {
        /// <summary>
        /// Đăng ký các services
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Services
            services.AddSingleton<IDeviceService, DeviceService>();
            services.AddSingleton<IDialogService, DialogService>();

            // TODO: Thêm các services khác
            // services.AddSingleton<INavigationService, NavigationService>();
            // services.AddSingleton<INotificationService, NotificationService>();

            return services;
        }

        /// <summary>
        /// Đăng ký các ViewModels
        /// </summary>
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            // Register ViewModels
            services.AddTransient<DeviceConnectionViewModel>();

            // TODO: Thêm các ViewModels khác
            // services.AddTransient<MainWindowViewModel>();
            // services.AddTransient<DashboardViewModel>();
            // services.AddTransient<EmployeeListViewModel>();

            return services;
        }

        /// <summary>
        /// Đăng ký các Views
        /// </summary>
        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            // Register Views
            services.AddTransient<DeviceConnectionView>();

            // TODO: Thêm các Views khác
            // services.AddTransient<MainWindow>();
            // services.AddTransient<DashboardView>();

            return services;
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.Services.Implementations;
using BHK.Retrieval.Attendance.WPF.ViewModels;
using BHK.Retrieval.Attendance.WPF.Views.Pages;
using BHK.Retrieval.Attendance.Infrastructure.DeviceIntegration.Wrappers.Realand;

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
            // Register Services với dependency injection
            services.AddSingleton<IDeviceService, DeviceService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();

            // TODO: Thêm các services khác
            // services.AddSingleton<INavigationService, NavigationService>();
            // services.AddSingleton<INotificationService, NotificationService>();

            return services;
        }

        /// <summary>
        /// Đăng ký Core services từ Core layer
        /// </summary>
        public static IServiceCollection RegisterCoreServices(this IServiceCollection services)
        {
            // TODO: Đăng ký các services từ Core layer
            // services.AddScoped<IEmployeeService, EmployeeService>();
            // services.AddScoped<IAttendanceService, AttendanceService>();

            return services;
        }

        /// <summary>
        /// Đăng ký Infrastructure services với configuration
        /// </summary>
        public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Device Integration Services
            services.AddSingleton<IRealandDeviceWrapper, RealandDeviceWrapper>();
            
            // TODO: Đăng ký các services từ Infrastructure layer
            // services.AddDbContext<AttendanceDbContext>(options =>
            //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            
            // services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            // services.AddScoped<IAttendanceRepository, AttendanceRepository>();

            return services;
        }

        /// <summary>
        /// Đăng ký WPF specific services
        /// </summary>
        public static IServiceCollection RegisterWpfServices(this IServiceCollection services)
        {
            // Đăng ký ViewModels
            services.AddViewModels();

            // Đăng ký Views
            services.AddViews();

            return services;
        }

        /// <summary>
        /// Đăng ký các ViewModels
        /// </summary>
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            // Register ViewModels
            services.AddTransient<DeviceConnectionViewModel>();
            services.AddTransient<ConfigurationDemoViewModel>();

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

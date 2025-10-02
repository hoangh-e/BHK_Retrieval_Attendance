using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.Services.Implementations;
using BHK.Retrieval.Attendance.Core.Interfaces.Repositories;
using BHK.Retrieval.Attendance.Infrastructure.Persistence.Repositories;
using BHK.Retrieval.Attendance.Shared.Options;

namespace BHK.Retrieval.Attendance.WPF.Configuration.DependencyInjection
{
    public static class ServiceRegistrar
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            // Configuration Options
            services.Configure<DatabaseOptions>(configuration.GetSection("Database"));
            services.Configure<DeviceOptions>(configuration.GetSection("Device"));
            services.Configure<SharePointOptions>(configuration.GetSection("SharePoint"));
            services.Configure<EmailOptions>(configuration.GetSection("Email"));

            // UI Services
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IViewService, ViewService>();
            services.AddSingleton<INotificationService, NotificationService>();

            // Core Services
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();

            // AutoMapper
            services.AddAutoMapper(typeof(App));
        }
    }
}
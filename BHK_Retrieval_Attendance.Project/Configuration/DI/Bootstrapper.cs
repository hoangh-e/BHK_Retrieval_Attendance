using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FluentValidation;
using BHK.Retrieval.Attendance.Shared.Options;
using BHK.Retrieval.Attendance.Infrastructure.Data.Context;

namespace BHK.Retrieval.Attendance.WPF.Configuration.DI
{
    public static class Bootstrapper
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Register Configuration Options
            RegisterOptions(services, configuration);

            // Register Infrastructure Services
            RegisterInfrastructureServices(services, configuration);

            // Register Core Services
            RegisterCoreServices(services);

            // Register WPF Services
            RegisterWpfServices(services);

            // Register ViewModels
            ViewModelRegistrar.RegisterViewModels(services);

            // Register Views
            ViewRegistrar.RegisterViews(services);

            // Register AutoMapper
            RegisterAutoMapper(services);

            // Register FluentValidation
            RegisterValidation(services);
        }

        private static void RegisterOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseOptions>(configuration.GetSection("DatabaseSettings"));
            services.Configure<DeviceOptions>(configuration.GetSection("DeviceSettings"));
            services.Configure<SharePointOptions>(configuration.GetSection("SharePointSettings"));
            services.Configure<EmailOptions>(configuration.GetSection("EmailSettings"));
            services.Configure<ApplicationOptions>(configuration.GetSection("ApplicationSettings"));
        }

        private static void RegisterInfrastructureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Database Context
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? configuration["DatabaseSettings:ConnectionString"];
            
            services.AddDbContext<AttendanceDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register Infrastructure Services (sẽ implement sau)
            // services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            // services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            // services.AddScoped<IDeviceRepository, DeviceRepository>();
        }

        private static void RegisterCoreServices(IServiceCollection services)
        {
            // Register Core Use Cases (sẽ implement sau)
            // services.AddScoped<IEmployeeService, EmployeeService>();
            // services.AddScoped<IAttendanceService, AttendanceService>();
            // services.AddScoped<IDeviceService, DeviceService>();
        }

        private static void RegisterWpfServices(IServiceCollection services)
        {
            // Register WPF-specific services
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IThemeService, ThemeService>();
        }

        private static void RegisterAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Bootstrapper).Assembly);
        }

        private static void RegisterValidation(IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(Bootstrapper).Assembly);
        }
    }
}

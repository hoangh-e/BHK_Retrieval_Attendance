using Microsoft.Extensions.DependencyInjection;
using BHK.Retrieval.Attendance.WPF.ViewModels;

namespace BHK.Retrieval.Attendance.WPF.Configuration.DependencyInjection
{
    public static class ViewModelRegistrar
    {
        public static void RegisterViewModels(IServiceCollection services)
        {
            // Register all ViewModels as Transient
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<AttendanceListViewModel>();
            services.AddTransient<AttendanceDetailViewModel>();
            services.AddTransient<EmployeeListViewModel>();
            services.AddTransient<EmployeeDetailViewModel>();
            services.AddTransient<DeviceListViewModel>();
            services.AddTransient<DeviceConnectionViewModel>();
            services.AddTransient<ReportGeneratorViewModel>();
            services.AddTransient<SettingsViewModel>();
        }
    }
}
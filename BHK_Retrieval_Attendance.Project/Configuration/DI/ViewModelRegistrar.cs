using Microsoft.Extensions.DependencyInjection;
// TODO: Uncomment sau khi tạo ViewModels
// using BHK.Retrieval.Attendance.WPF.ViewModels;

namespace BHK.Retrieval.Attendance.WPF.Configuration.DI
{
    public static class ViewModelRegistrar
    {
        public static void RegisterViewModels(IServiceCollection services)
        {
            // TODO: Uncomment sau khi tạo ViewModels
            /*
            // Register ViewModels as Transient (tạo mới mỗi lần request)
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
            */
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using BHK.Retrieval.Attendance.WPF.Views.Windows;
using BHK.Retrieval.Attendance.WPF.Views.Pages;

namespace BHK.Retrieval.Attendance.WPF.Configuration.DI
{
    public static class ViewRegistrar
    {
        public static void RegisterViews(IServiceCollection services)
        {
            // Register Windows
            services.AddSingleton<MainWindow>();
            services.AddTransient<SettingsWindow>();

            // Register Pages/Views
            services.AddTransient<DashboardView>();
            services.AddTransient<AttendanceListView>();
            services.AddTransient<AttendanceDetailView>();
            services.AddTransient<EmployeeListView>();
            services.AddTransient<EmployeeDetailView>();
            services.AddTransient<DeviceListView>();
            services.AddTransient<DeviceConnectionView>();
            services.AddTransient<ReportGeneratorView>();
        }
    }
}

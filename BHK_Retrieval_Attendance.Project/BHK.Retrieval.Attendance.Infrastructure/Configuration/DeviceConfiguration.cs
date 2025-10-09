using Microsoft.Extensions.DependencyInjection;
using BHK.Retrieval.Attendance.Core.Interfaces.Services;
using BHK.Retrieval.Attendance.Infrastructure.Devices;

namespace BHK.Retrieval.Attendance.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration cho Device Services trong Infrastructure layer
    /// ✅ ĐÚNG - Infrastructure implementation được inject vào Core Interface
    /// </summary>
    public static class DeviceConfiguration
    {
        public static IServiceCollection AddDeviceServices(this IServiceCollection services)
        {
            // ✅ SINGLETON cho WPF Desktop Application
            // Device connection phải được giữ trong toàn bộ vòng đời app
            // Khác với Web App (request-scoped), WPF cần singleton để maintain connection state
            services.AddSingleton<IDeviceCommunicationService, RealandDeviceService>();
            
            return services;
        }
    }
}
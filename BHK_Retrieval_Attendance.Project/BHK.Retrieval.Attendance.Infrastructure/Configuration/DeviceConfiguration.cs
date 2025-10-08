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
            // ✅ ĐÚNG theo Clean Architecture:
            // - Interface trong Core project 
            // - Implementation trong Infrastructure project
            // - WPF project chỉ biết Interface, không biết Implementation
            services.AddScoped<IDeviceCommunicationService, RealandDeviceService>();
            
            return services;
        }
    }
}
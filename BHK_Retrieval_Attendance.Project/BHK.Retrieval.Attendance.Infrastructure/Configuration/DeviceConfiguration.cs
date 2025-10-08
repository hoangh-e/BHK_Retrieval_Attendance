using Microsoft.Extensions.DependencyInjection;
using BHK.Retrieval.Attendance.Core.Interfaces.Services;
using BHK.Retrieval.Attendance.Infrastructure.Devices;

namespace BHK.Retrieval.Attendance.Infrastructure.Configuration
{
    public static class DeviceConfiguration
    {
        public static IServiceCollection AddDeviceServices(this IServiceCollection services)
        {
            services.AddSingleton<IDeviceCommunicationService, RealandDeviceService>();
            return services;
        }
    }
}
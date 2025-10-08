using System.Threading.Tasks;
using BHK.Retrieval.Attendance.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    public class DeviceViewModel
    {
        private readonly IDeviceCommunicationService _deviceService;
        private readonly ILogger<DeviceViewModel> _logger;

        public DeviceViewModel(
            IDeviceCommunicationService deviceService,
            ILogger<DeviceViewModel> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        public async Task ConnectDeviceAsync()
        {
            try
            {
                _logger.LogInformation("Connecting to device...");
                await _deviceService.ConnectAsync("192.168.1.10", 4370);
                _logger.LogInformation("✅ Device connected successfully");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to connect to device");
                throw;
            }
        }

        public async Task GetEmployeeListAsync()
        {
            try
            {
                _logger.LogInformation("Getting employee list from device...");
                var employees = await _deviceService.GetEmployeeListAsync();
                _logger.LogInformation("✅ Retrieved {Count} employees from device", employees.Count());
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to get employee list");
                throw;
            }
        }

        public async Task DisconnectDeviceAsync()
        {
            try
            {
                _logger.LogInformation("Disconnecting from device...");
                await _deviceService.DisconnectAsync();
                _logger.LogInformation("✅ Device disconnected successfully");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to disconnect from device");
                throw;
            }
        }
    }
}
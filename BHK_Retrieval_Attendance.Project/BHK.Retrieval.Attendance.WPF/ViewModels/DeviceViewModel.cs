using System.Threading.Tasks;
using BHK.Retrieval.Attendance.Core.Interfaces.Services;
using BHK.Retrieval.Attendance.WPF.Utilities;
using Microsoft.Extensions.Logging;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    /// <summary>
    /// ViewModel cho quản lý thiết bị (có thể dùng sau này)
    /// </summary>
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
                await _deviceService.ConnectAsync("192.168.1.10", 4370, 0, "");
                _logger.LogInformation("✅ Device connected successfully");
                
                // ✅ Hiển thị dialog thành công
                DialogHelper.ShowSuccess("Kết nối thiết bị thành công!", "Kết nối thành công");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to connect to device");
                
                // ✅ Hiển thị dialog lỗi thay vì throw
                DialogHelper.ShowError(
                    "Không thể kết nối đến thiết bị",
                    ex.Message,
                    "Lỗi kết nối"
                );
            }
        }

        public async Task GetEmployeeListAsync()
        {
            try
            {
                _logger.LogInformation("Getting employee list from device...");
                var employees = await _deviceService.GetEmployeeListAsync();
                _logger.LogInformation("✅ Retrieved {Count} employees from device", employees.Count());
                
                // ✅ Hiển thị dialog thành công
                DialogHelper.ShowSuccess(
                    $"Đã tải {employees.Count()} nhân viên từ thiết bị",
                    "Tải dữ liệu thành công"
                );
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to get employee list");
                
                // ✅ Hiển thị dialog lỗi thay vì throw
                DialogHelper.ShowError(
                    "Không thể lấy danh sách nhân viên",
                    ex.Message,
                    "Lỗi tải dữ liệu"
                );
            }
        }

        public async Task DisconnectDeviceAsync()
        {
            try
            {
                _logger.LogInformation("Disconnecting from device...");
                await _deviceService.DisconnectAsync();
                _logger.LogInformation("✅ Device disconnected successfully");
                
                // ✅ Hiển thị dialog thông báo
                DialogHelper.ShowInformation("Đã ngắt kết nối thiết bị", "Ngắt kết nối");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to disconnect from device");
                
                // ✅ Hiển thị dialog lỗi thay vì throw
                DialogHelper.ShowError(
                    "Lỗi khi ngắt kết nối",
                    ex.Message,
                    "Lỗi ngắt kết nối"
                );
            }
        }
    }
}
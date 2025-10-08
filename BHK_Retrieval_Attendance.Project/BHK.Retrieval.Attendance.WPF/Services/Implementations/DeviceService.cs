using System;
using System.Linq;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.Core.Interfaces.Services;
using BHK.Retrieval.Attendance.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace BHK.Retrieval.Attendance.WPF.Services.Implementations
{
    /// <summary>
    /// Service xử lý giao tiếp với thiết bị chấm công ZDC2911
    /// Tuân thủ Clean Architecture - chỉ gọi Infrastructure qua Interface
    /// </summary>
    public class DeviceService : IDeviceService, IDisposable
    {
        private readonly ILogger<DeviceService> _logger;
        private readonly DeviceOptions _deviceOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDeviceCommunicationService _deviceCommunicationService;
        
        private bool _isConnected;
        private bool _disposed;
        
        // DTO nội bộ thay vì dùng class Device từ Riss.Devices
        private DeviceConnectionInfo? _deviceInfo;

        public DeviceService(
            ILogger<DeviceService> logger,
            IOptions<DeviceOptions> deviceOptions,
            IServiceProvider serviceProvider,
            IDeviceCommunicationService deviceCommunicationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _deviceOptions = deviceOptions?.Value ?? throw new ArgumentNullException(nameof(deviceOptions));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _deviceCommunicationService = deviceCommunicationService ?? throw new ArgumentNullException(nameof(deviceCommunicationService));
            _isConnected = false;
        }

        /// <summary>
        /// Kiểm tra trạng thái kết nối
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// Lấy thông tin Device hiện tại (chỉ trả về DTO, không expose Riss.Device)
        /// </summary>
        public object? CurrentDevice => _deviceInfo;

        /// <summary>
        /// Kết nối tới thiết bị qua TCP/IP
        /// </summary>
        public async Task<bool> ConnectTcpAsync(string ipAddress, int port, int deviceNumber, string password)
        {
            try
            {
                _logger.LogInformation("Starting TCP connection - IP: {IpAddress}, Port: {Port}, DN: {DeviceNumber}", 
                    ipAddress, port, deviceNumber);

                // ===== TEST MODE =====
                if (_deviceOptions.Test)
                {
                    _logger.LogWarning("⚠️ TEST MODE ENABLED - Simulating successful connection");
                    await Task.Delay(1000); // Simulate connection delay
                    
                    // Tạo DTO giả để test
                    _deviceInfo = new DeviceConnectionInfo
                    {
                        DeviceNumber = deviceNumber,
                        IpAddress = ipAddress,
                        Port = port,
                        Password = password,
                        Model = _deviceOptions.DeviceModel,
                        SerialNumber = "TEST-SN-12345678"
                    };
                    
                    _isConnected = true;
                    _logger.LogInformation("✅ [TEST MODE] Device connected successfully (simulated)");
                    return true;
                }

                // ===== PRODUCTION MODE - Gọi Infrastructure Service =====
                _logger.LogInformation("Connecting to device using Infrastructure Service...");

                // Gọi Infrastructure Service qua Interface (tuân thủ Clean Architecture)
                // ✅ Truyền đầy đủ parameters: ip, port, deviceNumber, password
                await _deviceCommunicationService.ConnectAsync(ipAddress, port, deviceNumber, password);
                

                _isConnected = true;
                _logger.LogInformation("✅ Device connected successfully via Infrastructure");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to device via TCP/IP");
                _isConnected = false;
                CleanupConnection();
                return false;
            }
        }

        /// <summary>
        /// Ngắt kết nối với thiết bị
        /// </summary>
        public async Task<bool> DisconnectAsync()
        {
            try
            {
                _logger.LogInformation("Disconnecting from device...");

                // TEST MODE
                if (_deviceOptions.Test)
                {
                    _logger.LogWarning("⚠️ TEST MODE ENABLED - Simulating disconnection");
                    await Task.Delay(500);
                    _isConnected = false;
                    CleanupConnection();
                    _logger.LogInformation("✅ [TEST MODE] Device disconnected successfully (simulated)");
                    return true;
                }

                // PRODUCTION MODE - Gọi Infrastructure Service
                await _deviceCommunicationService.DisconnectAsync();

                CleanupConnection();
                _isConnected = false;
                _logger.LogInformation("Device disconnected successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while disconnecting from device");
                return false;
            }
        }

        /// <summary>
        /// Test kết nối tới thiết bị (không lưu connection)
        /// </summary>
        public async Task<bool> TestConnectionAsync(string ipAddress, int port, int deviceNumber, string password)
        {
            try
            {
                _logger.LogInformation("Testing connection to device...");

                if (_deviceOptions.Test)
                {
                    _logger.LogWarning("⚠️ TEST MODE - Simulating test connection");
                    await Task.Delay(800);
                    _logger.LogInformation("✅ [TEST MODE] Connection test successful (simulated)");
                    return true;
                }

                // PRODUCTION MODE - Test connection qua Infrastructure
                // Note: Cần implement TestConnectionAsync trong IDeviceCommunicationService
                _logger.LogInformation("Testing connection via Infrastructure Service...");
                
                // Tạm thời dùng ConnectAsync rồi DisconnectAsync
                // ✅ Truyền đầy đủ parameters: ip, port, deviceNumber, password
                await _deviceCommunicationService.ConnectAsync(ipAddress, port, deviceNumber, password);
                await _deviceCommunicationService.DisconnectAsync();
                
                _logger.LogInformation("✅ Connection test successful");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection test failed");
                return false;
            }
        }

        /// <summary>
        /// Lấy thông tin thiết bị
        /// </summary>
        public async Task<string> GetDeviceInfoAsync()
        {
            try
            {
                if (!_isConnected || _deviceInfo == null)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Getting device information...");

                // TEST MODE
                if (_deviceOptions.Test)
                {
                    _logger.LogWarning("⚠️ TEST MODE - Returning simulated device info");
                    
                    var testInfo = $"📱 DEVICE INFO (TEST MODE)\n" +
                                  $"Model: {_deviceInfo.Model}\n" +
                                  $"Serial: {_deviceInfo.SerialNumber}\n" +
                                  $"IP: {_deviceInfo.IpAddress}:{_deviceInfo.Port}\n" +
                                  $"Device Number: {_deviceInfo.DeviceNumber}\n" +
                                  $"Status: Connected ✅";

                    await Task.Delay(300); // Simulate data retrieval
                    return testInfo;
                }

                // PRODUCTION MODE - Lấy thông tin từ Infrastructure
                var deviceInfo = $"📱 DEVICE INFO\n" +
                                $"Model: {_deviceInfo.Model}\n" +
                                $"Serial: {_deviceInfo.SerialNumber}\n" +
                                $"IP: {_deviceInfo.IpAddress}:{_deviceInfo.Port}\n" +
                                $"Device Number: {_deviceInfo.DeviceNumber}\n" +
                                $"Status: Connected ✅";

                return await Task.FromResult(deviceInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get device information");
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách nhân viên từ thiết bị
        /// </summary>
        public async Task<string> GetEmployeeListAsync()
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Getting employee list from device...");

                // TEST MODE
                if (_deviceOptions.Test)
                {
                    _logger.LogWarning("⚠️ TEST MODE - Returning simulated employee list");
                    await Task.Delay(1000); // Simulate data retrieval
                    
                    return "👥 EMPLOYEE LIST (TEST MODE)\n" +
                           "1. Nguyễn Văn A (ID: 001)\n" +
                           "2. Trần Thị B (ID: 002)\n" +
                           "3. Lê Văn C (ID: 003)\n" +
                           "4. Phạm Thị D (ID: 004)\n" +
                           "5. Hoàng Văn E (ID: 005)\n" +
                           "\nTotal: 5 employees";
                }

                // PRODUCTION MODE - Lấy từ Infrastructure Service
                var employees = await _deviceCommunicationService.GetEmployeeListAsync();
                var employeeList = string.Join("\n", employees.Select((name, index) => $"{index + 1}. {name}"));
                
                return $"👥 EMPLOYEE LIST\n{employeeList}\n\nTotal: {employees.Count()} employees";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get employee list");
                throw;
            }
        }

        /// <summary>
        /// Clean up connection resources
        /// </summary>
        private void CleanupConnection()
        {
            _deviceInfo = null;
            // Không cần cleanup Riss.Device objects vì không còn dùng nữa
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                if (_isConnected)
                {
                    // Async dispose pattern - log warning nếu chưa disconnect
                    _logger.LogWarning("DeviceService disposed while still connected. Call DisconnectAsync() first.");
                }

                CleanupConnection();
                _disposed = true;
                
                _logger.LogInformation("DeviceService disposed successfully");
            }
        }

        /// <summary>
        /// DTO nội bộ để lưu thông tin kết nối thiết bị
        /// Thay thế cho việc dùng class Device từ Riss.Devices (vi phạm Clean Architecture)
        /// </summary>
        private class DeviceConnectionInfo
        {
            public int DeviceNumber { get; set; }
            public string IpAddress { get; set; } = string.Empty;
            public int Port { get; set; }
            public string Password { get; set; } = string.Empty;
            public string Model { get; set; } = string.Empty;
            public string SerialNumber { get; set; } = string.Empty;
        }
    }
}

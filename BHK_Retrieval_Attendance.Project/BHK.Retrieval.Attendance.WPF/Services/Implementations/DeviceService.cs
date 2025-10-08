using System;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.WPF.Configuration.Settings;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Riss.Devices;

namespace BHK.Retrieval.Attendance.WPF.Services.Implementations
{
    /// <summary>
    /// Service xử lý giao tiếp với thiết bị chấm công ZDC2911
    /// Sử dụng Riss.Devices package
    /// </summary>
    public class DeviceService : IDeviceService, IDisposable
    {
        private readonly ILogger<DeviceService> _logger;
        private readonly DeviceOptions _deviceOptions;
        
        private bool _isConnected;
        private bool _disposed;
        
        // Riss.Device objects
        private Device? _device;
        private object? _deviceConnection; // Connection handle

        public DeviceService(
            ILogger<DeviceService> logger,
            IOptions<DeviceOptions> deviceOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _deviceOptions = deviceOptions?.Value ?? throw new ArgumentNullException(nameof(deviceOptions));
            _isConnected = false;
        }

        /// <summary>
        /// Kiểm tra trạng thái kết nối
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// Lấy thông tin Device hiện tại
        /// </summary>
        public object? CurrentDevice => _device;

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
                    
                    // Tạo device giả để test
                    _device = new Device
                    {
                        DN = deviceNumber,
                        IpAddress = ipAddress,
                        IpPort = port,
                        Password = password,
                        Model = _deviceOptions.DeviceModel,
                        SerialNumber = "TEST-SN-12345678",
                        CommunicationType = CommunicationType.TCPIP
                    };
                    
                    _isConnected = true;
                    _logger.LogInformation("✅ [TEST MODE] Device connected successfully (simulated)");
                    return true;
                }

                // ===== PRODUCTION MODE - Dùng Riss.Device =====
                return await Task.Run(() =>
                {
                    try
                    {
                        _logger.LogInformation("Connecting to device using Riss.Device package...");

                        // Tạo Device object
                        _device = new Device
                        {
                            DN = deviceNumber,
                            IpAddress = ipAddress,
                            IpPort = port,
                            Password = password,
                            Model = _deviceOptions.DeviceModel,
                            CommunicationType = CommunicationType.TCPIP
                        };

                        // TODO: Implement actual connection with Riss.Device
                        // Theo tài liệu ZD2911, cần:
                        // 1. Tạo Device object với thông tin connection
                        // 2. Call Connect method
                        // 3. Verify connection
                        // 4. Get device info (SerialNumber, Model, FirmwareVersion)

                        // VÍ DỤ PSEUDO CODE (cần implement khi có Riss.Device):
                        /*
                        var deviceManager = new DeviceManager();
                        bool connected = deviceManager.Connect(_device);
                        
                        if (connected)
                        {
                            // Get device info after connection
                            _device.SerialNumber = deviceManager.GetDeviceProperty(DeviceProperty.SerialNumber);
                            _device.Model = deviceManager.GetDeviceProperty(DeviceProperty.DeviceModel);
                            // ... get other properties
                            
                            _isConnected = true;
                            _deviceConnection = deviceManager;
                            return true;
                        }
                        */

                        // TẠM THỜI: Giữ simulation cho đến khi có Riss.Device package
                        _logger.LogWarning("⚠️ Riss.Device not fully implemented yet - Using simulation");
                        Task.Delay(1500).Wait(); // Simulate connection time
                        
                        // Giả lập lấy thông tin device
                        _device.SerialNumber = "ZD2911-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        
                        _isConnected = true;
                        _logger.LogInformation("✅ Simulated connection successful");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception occurred during TCP connection");
                        _isConnected = false;
                        CleanupConnection();
                        return false;
                    }
                });
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

                // PRODUCTION MODE
                if (_deviceConnection != null)
                {
                    await Task.Run(() =>
                    {
                        // TODO: Implement actual disconnect with Riss.Device
                        // deviceManager.Disconnect();
                        
                        _logger.LogInformation("✅ Device connection closed");
                    });
                }

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
                _logger.LogInformation("Testing connection - IP: {IpAddress}, Port: {Port}", ipAddress, port);

                // TEST MODE
                if (_deviceOptions.Test)
                {
                    _logger.LogWarning("⚠️ TEST MODE ENABLED - Simulating test connection");
                    await Task.Delay(800);
                    _logger.LogInformation("✅ [TEST MODE] Test connection successful (simulated)");
                    return true;
                }

                // PRODUCTION MODE
                return await Task.Run(() =>
                {
                    try
                    {
                        // TODO: Implement actual test connection with Riss.Device
                        // Create temporary device and test
                        /*
                        var testDevice = new Device
                        {
                            DN = deviceNumber,
                            IpAddress = ipAddress,
                            IpPort = port,
                            Password = password,
                            CommunicationType = CommunicationType.TCPIP
                        };
                        
                        var deviceManager = new DeviceManager();
                        bool result = deviceManager.TestConnection(testDevice);
                        deviceManager.Disconnect(); // Clean up test connection
                        return result;
                        */

                        // TẠM THỜI: Simulation
                        _logger.LogInformation("⚠️ Simulating test connection - Riss.Devices not fully implemented");
                        Task.Delay(800).Wait();
                        _logger.LogInformation("✅ Simulated test connection successful");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception during test connection");
                        return false;
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to test connection");
                return false;
            }
        }

        /// <summary>
        /// Lấy thông tin thiết bị sau khi đã connect
        /// Theo tài liệu ZD2911: Device có properties SerialNumber, Model, FirmwareVersion
        /// </summary>
        public async Task<DeviceInfo> GetDeviceInfoAsync()
        {
            if (!_isConnected || _device == null)
            {
                throw new InvalidOperationException("Device not connected. Please connect first.");
            }

            try
            {
                return await Task.Run(() =>
                {
                    var deviceInfo = new DeviceInfo();

                    // TEST MODE
                    if (_deviceOptions.Test)
                    {
                        deviceInfo.SerialNumber = "TEST-" + _device.SerialNumber ?? "UNKNOWN";
                        deviceInfo.Model = _device.Model ?? _deviceOptions.DeviceModel;
                        deviceInfo.IpAddress = _device.IpAddress ?? "192.168.1.225";
                        deviceInfo.Port = _device.IpPort;
                        deviceInfo.DeviceNumber = _device.DN;
                        deviceInfo.FirmwareVersion = "v2.0.0 (TEST)";
                        deviceInfo.IsTestMode = true;
                        
                        _logger.LogInformation("✅ [TEST MODE] Device info retrieved (simulated)");
                        return deviceInfo;
                    }

                    // PRODUCTION MODE - Lấy thông tin thực từ device
                    // TODO: Implement với Riss.Device
                    /*
                    if (_deviceConnection is DeviceManager manager)
                    {
                        deviceInfo.SerialNumber = manager.GetDeviceProperty(DeviceProperty.SerialNumber);
                        deviceInfo.Model = manager.GetDeviceProperty(DeviceProperty.DeviceModel);
                        deviceInfo.FirmwareVersion = manager.GetDeviceProperty(DeviceProperty.FirmwareVersion);
                        deviceInfo.IpAddress = _device.IpAddress;
                        deviceInfo.Port = _device.IpPort;
                        deviceInfo.DeviceNumber = _device.DN;
                        deviceInfo.IsTestMode = false;
                    }
                    */

                    // TẠM THỜI: Simulation
                    deviceInfo.SerialNumber = _device.SerialNumber ?? "UNKNOWN";
                    deviceInfo.Model = _device.Model ?? _deviceOptions.DeviceModel;
                    deviceInfo.IpAddress = _device.IpAddress ?? "192.168.1.225";
                    deviceInfo.Port = _device.IpPort;
                    deviceInfo.DeviceNumber = _device.DN;
                    deviceInfo.FirmwareVersion = "v2.0.0";
                    deviceInfo.IsTestMode = false;

                    _logger.LogInformation("✅ Device info retrieved: SN={SerialNumber}, Model={Model}", 
                        deviceInfo.SerialNumber, deviceInfo.Model);

                    return deviceInfo;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get device info");
                throw;
            }
        }

        /// <summary>
        /// Cleanup kết nối
        /// </summary>
        private void CleanupConnection()
        {
            try
            {
                // TODO: Implement cleanup với Riss.Devices
                // if (_deviceConnection is DeviceManager manager)
                // {
                //     manager.Dispose();
                // }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during connection cleanup");
            }
            finally
            {
                _deviceConnection = null;
                _device = null;
            }
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            try
            {
                if (_isConnected)
                {
                    DisconnectAsync().GetAwaiter().GetResult();
                }

                CleanupConnection();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during disposal");
            }
            finally
            {
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Model chứa thông tin thiết bị
    /// </summary>
    public class DeviceInfo
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public int DeviceNumber { get; set; }
        public string FirmwareVersion { get; set; } = string.Empty;
        public bool IsTestMode { get; set; }
    }
}

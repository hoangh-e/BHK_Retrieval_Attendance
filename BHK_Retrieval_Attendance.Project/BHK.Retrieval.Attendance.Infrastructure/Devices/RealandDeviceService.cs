using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Riss.Devices; // ✅ CHỈ Infrastructure mới được dùng

namespace BHK.Retrieval.Attendance.Infrastructure.Devices
{
    /// <summary>
    /// Service implementation cho giao tiếp với thiết bị Realand ZDC2911
    /// ✅ CHỈ Ở ĐÂY mới được dùng using Riss.Devices
    /// ✅ CHỈ Ở ĐÂY mới được tạo Device objects
    /// ✅ TUÂN THỦ Clean Architecture - Infrastructure chứa implementation cụ thể
    /// </summary>
    public class RealandDeviceService : IDeviceCommunicationService, IDisposable
    {
        private readonly ILogger<RealandDeviceService>? _logger;
        private Device? _device;
        private DeviceConnection? _deviceConnection;
        private bool _disposed;
        private bool _isConnected;

        public RealandDeviceService(ILogger<RealandDeviceService>? logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// Kết nối tới thiết bị qua TCP/IP
        /// Dựa theo ZDC2911_Demo/CMForm.cs
        /// </summary>
        /// <param name="ip">IP Address của thiết bị</param>
        /// <param name="port">Port của thiết bị</param>
        /// <param name="deviceNumber">Device Number (DN)</param>
        /// <param name="password">Password thiết bị</param>
        public async Task ConnectAsync(string ip, int port, int deviceNumber, string password)
        {
            await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Connecting to device at {ip}:{port}, DN: {deviceNumber}", ip, port, deviceNumber);

                    // ✅ Validation đầu vào
                    if (string.IsNullOrEmpty(ip))
                        throw new ArgumentException("IP address cannot be null or empty", nameof(ip));
                    
                    if (port <= 0 || port > 65535)
                        throw new ArgumentException("Port must be between 1 and 65535", nameof(port));

                    if (deviceNumber <= 0)
                        throw new ArgumentException("Device number must be greater than 0", nameof(deviceNumber));

                    if (string.IsNullOrEmpty(password))
                        throw new ArgumentException("Password cannot be null or empty", nameof(password));

                    // ✅ Khởi tạo Device theo chuẩn ZDC2911_Demo với input parameters
                    _device = new Device
                    {
                        DN = deviceNumber, // ✅ Sử dụng deviceNumber từ input
                        Password = password, // ✅ Sử dụng password từ input
                        Model = "ZDC2911",
                        ConnectionModel = 5, // ✅ QUAN TRỌNG: Phải = 5 cho ZD2911
                        IpAddress = ip,
                        IpPort = port,
                        CommunicationType = CommunicationType.Tcp
                    };

                    // ✅ Tạo connection từ Device
                    _deviceConnection = DeviceConnection.CreateConnection(ref _device);

                    // ✅ KIỂM TRA KẾT QUẢ - QUAN TRỌNG!
                    int result = _deviceConnection.Open();
                    
                    if (result > 0)
                    {
                        // ✅ Kết nối thành công
                        _isConnected = true;
                        _logger?.LogInformation("Infrastructure: ✅ Successfully connected to device. Result code: {result}", result);
                    }
                    else
                    {
                        // ❌ Kết nối thất bại
                        _logger?.LogError("Infrastructure: ❌ Failed to connect. Result code: {result}", result);
                        
                        // Cleanup
                        _deviceConnection?.Close();
                        _deviceConnection = null;
                        _device = null;
                        _isConnected = false;

                        throw new Exception($"Failed to connect to device at {ip}:{port}. Error code: {result}");
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Connection failed");
                    
                    // Cleanup khi có exception
                    try
                    {
                        _deviceConnection?.Close();
                    }
                    catch { }
                    
                    _deviceConnection = null;
                    _device = null;
                    _isConnected = false;
                    
                    throw; // ✅ Re-throw để DeviceService bắt được
                }
            });
        }

        /// <summary>
        /// Lấy danh sách nhân viên từ thiết bị
        /// </summary>
        public async Task<IEnumerable<string>> GetEmployeeListAsync()
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Getting employee list from device");

                    // TODO: Implement theo ZD2911 User Guide
                    // Ví dụ:
                    // object extraProperty = new object();
                    // object extraData = new object();
                    // bool result = _deviceConnection.GetProperty(DeviceProperty.Employee, extraProperty, ref _device, ref extraData);
                    // Parse extraData để lấy danh sách employees

                    // TẠM THỜI: Mock data
                    var employeeNames = new List<string>
                    {
                        "Nguyễn Văn A",
                        "Trần Thị B",
                        "Lê Văn C"
                    };

                    _logger?.LogInformation("Infrastructure: Retrieved {count} employees", employeeNames.Count);
                    return employeeNames;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get employee list");
                    throw;
                }
            });
        }

        /// <summary>
        /// Ngắt kết nối khỏi thiết bị
        /// </summary>
        public async Task DisconnectAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Disconnecting from device");

                    if (_deviceConnection != null)
                    {
                        _deviceConnection.Close();
                        _logger?.LogInformation("Infrastructure: ✅ Successfully disconnected");
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Error during disconnect");
                }
                finally
                {
                    _isConnected = false;
                    _deviceConnection = null;
                    _device = null;
                }
            });
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    if (_deviceConnection != null && _isConnected)
                    {
                        _deviceConnection.Close();
                        _logger?.LogInformation("Infrastructure: Disposing - disconnected device");
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Infrastructure: Error during disposal");
                }
                finally
                {
                    _deviceConnection = null;
                    _device = null;
                    _isConnected = false;
                    _disposed = true;
                    _logger?.LogInformation("Infrastructure: RealandDeviceService disposed");
                }
            }
        }
    }
}
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BHK.Retrieval.Attendance.WPF.Models.Device
{
    /// <summary>
    /// Model cho thông tin kết nối thiết bị TCP/IP
    /// </summary>
    public class DeviceConnectionModel : INotifyPropertyChanged
    {
        // ✅ Loại bỏ giá trị default hardcode - để ViewModel inject từ config
        private string _ipAddress = string.Empty;
        private int _port = 0;
        private int _deviceNumber = 0;
        private string _password = string.Empty;
        private bool _isConnected;
        private string _connectionStatus = "Disconnected";
        private string _deviceModel = string.Empty;
        private string _lastError = string.Empty;
        private DateTime? _lastConnected;
        private DeviceInfo? _deviceInfo;

        /// <summary>
        /// Địa chỉ IP của thiết bị
        /// </summary>
        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                if (_ipAddress != value)
                {
                    _ipAddress = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Cổng UDP của thiết bị
        /// </summary>
        public int Port
        {
            get => _port;
            set
            {
                if (_port != value)
                {
                    _port = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Device Number (DN)
        /// </summary>
        public int DeviceNumber
        {
            get => _deviceNumber;
            set
            {
                if (_deviceNumber != value)
                {
                    _deviceNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Mật khẩu kết nối
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Trạng thái kết nối
        /// </summary>
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged();
                    ConnectionStatus = value ? "Connected" : "Disconnected";
                }
            }
        }

        /// <summary>
        /// Thông báo trạng thái kết nối
        /// </summary>
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                if (_connectionStatus != value)
                {
                    _connectionStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Model thiết bị
        /// </summary>
        public string DeviceModel
        {
            get => _deviceModel;
            set
            {
                if (_deviceModel != value)
                {
                    _deviceModel = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Thông báo lỗi cuối cùng
        /// </summary>
        public string LastError
        {
            get => _lastError;
            set
            {
                if (_lastError != value)
                {
                    _lastError = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Thời gian kết nối cuối cùng
        /// </summary>
        public DateTime? LastConnected
        {
            get => _lastConnected;
            set
            {
                if (_lastConnected != value)
                {
                    _lastConnected = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Thông tin chi tiết thiết bị
        /// </summary>
        public DeviceInfo? DeviceInfo
        {
            get => _deviceInfo;
            set
            {
                if (_deviceInfo != value)
                {
                    _deviceInfo = value;
                    OnPropertyChanged();
                    
                    // Update related properties
                    if (value != null)
                    {
                        DeviceModel = value.DeviceName;
                    }
                }
            }
        }

        /// <summary>
        /// Chuỗi hiển thị thông tin kết nối
        /// </summary>
        public string ConnectionInfo => IsConnected
            ? $"Đã kết nối: {IpAddress}:{Port}"
            : $"Chưa kết nối: {IpAddress}:{Port}";

        /// <summary>
        /// Kiểm tra tính hợp lệ của thông tin kết nối
        /// </summary>
        public bool IsValid => !string.IsNullOrWhiteSpace(IpAddress) && Port > 0 && Port <= 65535;

        /// <summary>
        /// Cập nhật trạng thái kết nối thành công
        /// </summary>
        /// <param name="deviceInfo">Thông tin thiết bị</param>
        public void UpdateConnectionSuccess(DeviceInfo deviceInfo)
        {
            IsConnected = true;
            DeviceInfo = deviceInfo;
            LastConnected = DateTime.Now;
            LastError = string.Empty;
            ConnectionStatus = $"Đã kết nối - {deviceInfo.DeviceName}";
        }

        /// <summary>
        /// Cập nhật trạng thái kết nối thất bại
        /// </summary>
        /// <param name="errorMessage">Thông báo lỗi</param>
        public void UpdateConnectionFailure(string errorMessage)
        {
            IsConnected = false;
            DeviceInfo = null;
            LastError = errorMessage;
            ConnectionStatus = "Kết nối thất bại";
        }

        /// <summary>
        /// Đặt lại trạng thái kết nối
        /// </summary>
        public void ResetConnection()
        {
            IsConnected = false;
            DeviceInfo = null;
            ConnectionStatus = "Chưa kết nối";
            LastError = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

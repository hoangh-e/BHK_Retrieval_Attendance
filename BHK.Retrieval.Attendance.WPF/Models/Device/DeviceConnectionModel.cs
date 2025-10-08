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
        private string _ipAddress = "192.168.1.225";
        private int _port = 4370;
        private int _deviceNumber = 1;
        private string _password = "0";
        private bool _isConnected;
        private string _connectionStatus = "Disconnected";
        private string _deviceModel = "ZDC2911";

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

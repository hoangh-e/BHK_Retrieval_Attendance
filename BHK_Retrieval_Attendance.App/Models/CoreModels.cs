using System;

namespace BHK.Retrieval.Attendance.WPF.Models
{
    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Connected,
        Error
    }

    public enum DeviceConnectionType
    {
        TcpIp = 0,
        Serial = 1,
        Usb = 2
    }

    public class DeviceConnectionConfig
    {
        public DeviceConnectionType ConnectionType { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public string ComPort { get; set; } = string.Empty;
        public int BaudRate { get; set; }
        public int Timeout { get; set; }
    }

    public class DeviceInfo
    {
        public string Name { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public int RecordCount { get; set; }
        public int FreeSpace { get; set; }
        public DateTime DateTime { get; set; }
        public string FirmwareVersion { get; set; } = string.Empty;
    }

    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public ConnectionStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class DeviceDataReceivedEventArgs : EventArgs
    {
        public string DataType { get; set; } = string.Empty;
        public object? Data { get; set; }
    }

    public class Employee
    {
        public int DIN { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string IDNumber { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public bool Enable { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Department { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int AttType { get; set; }
        public DateTime LastActivity { get; set; }
    }

    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    public class LoginResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T> { IsSuccess = true, Data = data };
        }

        public static ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T> { IsSuccess = false, ErrorMessage = errorMessage };
        }
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; } = string.Empty;
        public Microsoft.Extensions.Logging.LogLevel Level { get; set; }
    }
}
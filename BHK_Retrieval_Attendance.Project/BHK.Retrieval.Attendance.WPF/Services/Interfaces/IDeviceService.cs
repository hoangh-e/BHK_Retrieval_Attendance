using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;

namespace BHK.Retrieval.Attendance.WPF.Services.Interfaces
{
    public interface IDeviceService : IDisposable
    {
        #region Connection Management
        Task<bool> ConnectAsync(int deviceNumber, string ipAddress, int port, string password);
        Task<bool> ConnectTcpAsync(string ipAddress, int port, int deviceNumber, string password);
        Task DisconnectAsync();
        bool IsConnected { get; }
        Task<bool> TestConnectionAsync();
        Task<bool> TestConnectionAsync(string ipAddress, int port, int deviceNumber, string password);
        #endregion

        #region Employee Management
        Task<List<EmployeeDto>> GetAllUsersAsync();
        Task<EmployeeDto?> GetUserByIdAsync(ulong din);
        Task<int> GetUserCountAsync();
        Task<bool> AddUserAsync(EmployeeDto user);
        Task<bool> UpdateUserAsync(EmployeeDto user);
        Task<bool> DeleteUserAsync(ulong din);
        Task<bool> ClearAllUsersAsync();
        #endregion

        #region Enrollment Management
        Task<List<EnrollmentDto>> GetUserEnrollmentsAsync(ulong din);
        Task<bool> EnrollFingerprintAsync(ulong din, int fingerprintIndex, byte[] fingerprintData);
        Task<bool> EnrollPasswordAsync(ulong din, string password);
        Task<bool> EnrollCardAsync(ulong din, string cardId);
        Task<bool> ClearFingerprintAsync(ulong din, int fingerprintIndex);
        Task<bool> ClearPasswordAsync(ulong din);
        Task<bool> ClearCardAsync(ulong din);
        #endregion

        #region Attendance Records
        Task<List<AttendanceRecordDto>> GetAttendanceRecordsAsync(DateTime startDate, DateTime endDate);
        Task<int> GetAttendanceRecordCountAsync(DateTime startDate, DateTime endDate);
        Task<bool> ClearAttendanceRecordsAsync();
        #endregion

        #region Device Information
        Task<string> GetDeviceInfoAsync();
        Task<string> GetSerialNumberAsync();
        Task<string> GetFirmwareVersionAsync();
        Task<string> GetDeviceModelAsync();
        Task<DateTime> GetDeviceTimeAsync();
        Task<bool> SyncDeviceTimeAsync();
        #endregion

        #region Utility Methods
        Task<IEnumerable<string>> GetEmployeeListAsync();
        object? CurrentDevice { get; }
        #endregion
    }
}

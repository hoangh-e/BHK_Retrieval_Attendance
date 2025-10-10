using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;

namespace BHK.Retrieval.Attendance.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho Device Communication Service
    /// Clean Architecture - Core Interface, Infrastructure Implementation
    /// </summary>
    public interface IDeviceCommunicationService
    {
        #region Connection Management

        /// <summary>
        /// Kết nối tới thiết bị
        /// </summary>
        /// <param name="ip">IP Address của thiết bị</param>
        /// <param name="port">Port của thiết bị</param>
        /// <param name="deviceNumber">Device Number (DN)</param>
        /// <param name="password">Password thiết bị</param>
        Task ConnectAsync(string ip, int port, int deviceNumber, string password);
        
        /// <summary>
        /// Ngắt kết nối khỏi thiết bị
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Kiểm tra trạng thái kết nối
        /// </summary>
        bool IsConnected { get; }

        #endregion

        #region Employee Management

        /// <summary>
        /// Lấy danh sách tất cả nhân viên từ thiết bị
        /// </summary>
        Task<List<EmployeeDto>> GetAllEmployeesAsync();

        /// <summary>
        /// Lấy thông tin chi tiết một nhân viên theo DIN
        /// </summary>
        Task<EmployeeDto?> GetEmployeeByIdAsync(ulong din);

        /// <summary>
        /// Lấy số lượng nhân viên trong thiết bị
        /// </summary>
        Task<int> GetEmployeeCountAsync();

        /// <summary>
        /// Thêm nhân viên mới vào thiết bị
        /// </summary>
        Task<bool> AddEmployeeAsync(EmployeeDto employee);

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        Task<bool> UpdateEmployeeAsync(EmployeeDto employee);

        /// <summary>
        /// Xóa nhân viên khỏi thiết bị
        /// </summary>
        Task<bool> DeleteEmployeeAsync(ulong din);

        /// <summary>
        /// Xóa tất cả nhân viên khỏi thiết bị
        /// </summary>
        Task<bool> ClearAllEmployeesAsync();

        #endregion

        #region Attendance Records

        /// <summary>
        /// Lấy bản ghi chấm công trong khoảng thời gian
        /// </summary>
        Task<List<AttendanceRecordDto>> GetAttendanceRecordsAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Lấy số lượng bản ghi chấm công
        /// </summary>
        Task<int> GetAttendanceRecordCountAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Xóa bản ghi chấm công
        /// </summary>
        Task<bool> ClearAttendanceRecordsAsync();

        #endregion

        #region Device Information

        /// <summary>
        /// Lấy Serial Number của thiết bị
        /// </summary>
        Task<string> GetSerialNumberAsync();

        /// <summary>
        /// Lấy Firmware Version của thiết bị
        /// </summary>
        Task<string> GetFirmwareVersionAsync();

        /// <summary>
        /// Lấy Model của thiết bị
        /// </summary>
        Task<string> GetDeviceModelAsync();

        /// <summary>
        /// Lấy thời gian hiện tại của thiết bị
        /// </summary>
        Task<DateTime> GetDeviceTimeAsync();

        /// <summary>
        /// Đồng bộ thời gian thiết bị với máy tính
        /// </summary>
        Task<bool> SyncDeviceTimeAsync();

        #endregion

        #region Legacy Methods

        /// <summary>
        /// Lấy danh sách nhân viên từ thiết bị
        /// </summary>
        Task<IEnumerable<string>> GetEmployeeListAsync();

        #endregion
    }
}

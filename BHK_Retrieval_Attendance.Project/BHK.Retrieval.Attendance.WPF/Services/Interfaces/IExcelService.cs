using System.Collections.Generic;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;

namespace BHK.Retrieval.Attendance.WPF.Services.Interfaces;

/// <summary>
/// Service làm việc với file Excel
/// </summary>
public interface IExcelService
{
    /// <summary>
    /// Kiểm tra xem file Excel có hợp lệ không
    /// </summary>
    Task<bool> ValidateExcelFileAsync(string filePath);

    /// <summary>
    /// Lấy danh sách tên table trong file Excel
    /// </summary>
    Task<List<string>> GetTableNamesAsync(string filePath);

    /// <summary>
    /// Kiểm tra xem table có tồn tại trong file không
    /// </summary>
    Task<bool> TableExistsAsync(string filePath, string tableName);

    /// <summary>
    /// Tạo table mới với schema cho điểm danh
    /// </summary>
    Task<bool> CreateAttendanceTableAsync(string filePath, string tableName);

    /// <summary>
    /// Tạo table mới với schema cho nhân viên
    /// </summary>
    Task<bool> CreateEmployeeTableAsync(string filePath, string tableName);

    /// <summary>
    /// Xuất dữ liệu điểm danh ra Excel
    /// </summary>
    Task<bool> ExportAttendanceDataAsync(
        string filePath, 
        string tableName, 
        List<AttendanceDisplayDto> data);

    /// <summary>
    /// Xuất dữ liệu nhân viên ra Excel
    /// </summary>
    Task<bool> ExportEmployeeDataAsync(
        string filePath, 
        string tableName, 
        List<EmployeeDto> data);

    /// <summary>
    /// Đếm số lượng record trong table
    /// </summary>
    Task<int> GetRecordCountAsync(string filePath, string tableName);

    /// <summary>
    /// Đọc dữ liệu nhân viên từ Excel
    /// </summary>
    Task<List<EmployeeDto>> ReadEmployeeDataAsync(string filePath, string tableName);
}

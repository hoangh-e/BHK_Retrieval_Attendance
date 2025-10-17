using System.Collections.Generic;
using System.Threading.Tasks;

namespace BHK.Retrieval.Attendance.WPF.Services.Interfaces
{
    /// <summary>
    /// Service xử lý file Excel - Đọc/Ghi/Tạo table
    /// ✅ TÁI SỬ DỤNG cho tất cả thao tác Excel
    /// </summary>
    public interface IExcelTableService
    {
        /// <summary>
        /// Kiểm tra file Excel có tồn tại và hợp lệ không
        /// </summary>
        Task<bool> ValidateExcelFileAsync(string filePath);

        /// <summary>
        /// Lấy danh sách tên table/sheet trong file Excel
        /// </summary>
        Task<List<string>> GetTableNamesAsync(string filePath);

        /// <summary>
        /// Kiểm tra table có tồn tại trong file không
        /// </summary>
        Task<bool> TableExistsAsync(string filePath, string tableName);

        /// <summary>
        /// Tạo Excel table thực sự với cấu trúc cột cho Attendance
        /// Columns: ID, Date, Time, Verify
        /// </summary>
        Task CreateAttendanceTableAsync(string filePath, string tableName);

        /// <summary>
        /// Tạo Excel table thực sự với cấu trúc cột chi tiết cho Employee
        /// Columns: ID, Name, IDNumber, Department, Sex, Birthday, Created, Status, Comment, EnrollmentCount
        /// </summary>
        Task CreateEmployeeTableAsync(string filePath, string tableName);

        /// <summary>
        /// Đếm số lượng record trong table
        /// </summary>
        Task<int> GetRecordCountAsync(string filePath, string tableName);

        /// <summary>
        /// Xuất dữ liệu Attendance vào table (Update existing + Insert new)
        /// </summary>
        Task ExportAttendanceDataAsync<T>(string filePath, string tableName, List<T> data);

        /// <summary>
        /// Xuất dữ liệu Employee vào table (Update existing + Insert new)
        /// </summary>
        Task ExportEmployeeDataAsync<T>(string filePath, string tableName, List<T> data);

        /// <summary>
        /// Xuất dữ liệu Employee vào table với progress callback chi tiết
        /// </summary>
        Task ExportEmployeeDataAsync<T>(string filePath, string tableName, List<T> data, 
            Action<int, int, string>? progressCallback);

        /// <summary>
        /// Lấy danh sách cột trong table
        /// </summary>
        Task<List<string>> GetTableColumnsAsync(string filePath, string tableName);

        /// <summary>
        /// Kiểm tra cột table có hợp lệ theo định dạng không
        /// </summary>
        Task<bool> ValidateTableColumnsAsync(string filePath, string tableName, string tableType);

        /// <summary>
        /// Refactor cột table theo đúng định dạng
        /// </summary>
        Task RefactorTableColumnsAsync(string filePath, string tableName, string tableType);
    }
}

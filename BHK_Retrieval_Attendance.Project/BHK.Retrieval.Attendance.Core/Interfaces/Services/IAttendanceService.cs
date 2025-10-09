using BHK.Retrieval.Attendance.Core.DTOs.Requests;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;

namespace BHK.Retrieval.Attendance.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface cho nghiệp vụ chấm công
    /// </summary>
    public interface IAttendanceService
    {
        /// <summary>
        /// Lấy danh sách bản ghi chấm công theo filter
        /// </summary>
        Task<List<AttendanceDisplayDto>> GetAttendanceRecordsAsync(AttendanceFilterDto filter);
        
        /// <summary>
        /// Xuất file theo cấu hình
        /// </summary>
        Task<bool> ExportAttendanceAsync(ExportConfigDto config, string filePath);
        
        /// <summary>
        /// Đồng bộ dữ liệu từ thiết bị
        /// </summary>
        Task<int> SyncAttendanceFromDeviceAsync();
    }
}

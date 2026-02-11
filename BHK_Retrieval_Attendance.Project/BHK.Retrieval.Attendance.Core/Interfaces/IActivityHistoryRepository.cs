using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.Core.DTOs;
using BHK.Retrieval.Attendance.Core.Models;

namespace BHK.Retrieval.Attendance.Core.Interfaces
{
    /// <summary>
    /// Repository cho truy cập dữ liệu lịch sử hoạt động (SQLite)
    /// </summary>
    public interface IActivityHistoryRepository
    {
        /// <summary>
        /// Thêm một hoạt động mới
        /// </summary>
        Task<long> AddAsync(ActivityHistory activity);

        /// <summary>
        /// Lấy danh sách hoạt động có phân trang và lọc
        /// </summary>
        Task<(List<ActivityHistory> Items, int TotalCount)> GetPagedAsync(ActivityFilter filter);

        /// <summary>
        /// Lấy một hoạt động theo ID
        /// </summary>
        Task<ActivityHistory?> GetByIdAsync(long id);

        /// <summary>
        /// Xóa hoạt động theo ID
        /// </summary>
        Task<bool> DeleteAsync(long id);

        /// <summary>
        /// Xóa các hoạt động cũ hơn ngày chỉ định
        /// </summary>
        Task<int> DeleteOlderThanAsync(DateTime cutoffDate);

        /// <summary>
        /// Xóa theo địa chỉ IP thiết bị
        /// </summary>
        Task<int> DeleteByDeviceIpAsync(string deviceIp);

        /// <summary>
        /// Xóa tất cả hoạt động
        /// </summary>
        Task<int> DeleteAllAsync();

        /// <summary>
        /// Lấy thống kê hoạt động
        /// </summary>
        Task<ActivityStatistics> GetStatisticsAsync(ActivityFilter filter);

        /// <summary>
        /// Kiểm tra database đã được khởi tạo chưa
        /// </summary>
        Task<bool> IsDatabaseInitializedAsync();

        /// <summary>
        /// Khởi tạo database và schema
        /// </summary>
        Task InitializeDatabaseAsync();
    }
}

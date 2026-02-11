using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.Core.DTOs;
using BHK.Retrieval.Attendance.Core.Enums;
using BHK.Retrieval.Attendance.Core.Models;

namespace BHK.Retrieval.Attendance.Core.Interfaces
{
    /// <summary>
    /// Service quản lý lịch sử hoạt động
    /// </summary>
    public interface IActivityHistoryService
    {
        /// <summary>
        /// Ghi log một hoạt động thành công
        /// </summary>
        Task LogSuccessAsync(
            string deviceIp,
            string? deviceName,
            ActivityType activityType,
            string action,
            string? details = null,
            int? userCount = null,
            int? recordCount = null,
            TimeSpan? duration = null);

        /// <summary>
        /// Ghi log một hoạt động thất bại
        /// </summary>
        Task LogFailedAsync(
            string deviceIp,
            string? deviceName,
            ActivityType activityType,
            string action,
            string errorMessage,
            string? details = null);

        /// <summary>
        /// Ghi log một hoạt động có cảnh báo
        /// </summary>
        Task LogWarningAsync(
            string deviceIp,
            string? deviceName,
            ActivityType activityType,
            string action,
            string warningMessage,
            string? details = null,
            int? userCount = null,
            int? recordCount = null);

        /// <summary>
        /// Ghi log custom (tự xây dựng ActivityHistoryDto)
        /// </summary>
        Task LogActivityAsync(ActivityHistoryDto activityDto);

        /// <summary>
        /// Lấy danh sách hoạt động với phân trang và bộ lọc
        /// </summary>
        Task<(List<ActivityHistory> Items, int TotalCount)> GetActivitiesAsync(ActivityFilter filter);

        /// <summary>
        /// Lấy một hoạt động theo ID
        /// </summary>
        Task<ActivityHistory?> GetActivityByIdAsync(long id);

        /// <summary>
        /// Lấy thống kê hoạt động
        /// </summary>
        Task<ActivityStatistics> GetStatisticsAsync(ActivityFilter filter);

        /// <summary>
        /// Xóa hoạt động cũ hơn số ngày chỉ định
        /// </summary>
        Task<int> ClearOldActivitiesAsync(int days);

        /// <summary>
        /// Xóa hoạt động theo thiết bị
        /// </summary>
        Task<int> ClearActivitiesByDeviceAsync(string deviceIp);

        /// <summary>
        /// Xóa tất cả hoạt động
        /// </summary>
        Task<int> ClearAllActivitiesAsync();
    }
}

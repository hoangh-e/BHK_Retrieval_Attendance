using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.Core.DTOs;
using BHK.Retrieval.Attendance.Core.Enums;
using BHK.Retrieval.Attendance.Core.Interfaces;
using BHK.Retrieval.Attendance.Core.Models;
using Serilog;

namespace BHK.Retrieval.Attendance.Infrastructure.Services
{
    /// <summary>
    /// Service implementation cho quản lý lịch sử hoạt động
    /// </summary>
    public class ActivityHistoryService : IActivityHistoryService
    {
        private readonly IActivityHistoryRepository _repository;

        public ActivityHistoryService(IActivityHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task LogSuccessAsync(
            string deviceIp,
            string? deviceName,
            ActivityType activityType,
            string action,
            string? details = null,
            int? userCount = null,
            int? recordCount = null,
            TimeSpan? duration = null)
        {
            try
            {
                var dto = ActivityHistoryDto.Success(
                    deviceIp,
                    deviceName,
                    activityType,
                    action,
                    details,
                    userCount,
                    recordCount,
                    duration);

                await LogActivityAsync(dto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to log success activity for {Action}", action);
            }
        }

        public async Task LogFailedAsync(
            string deviceIp,
            string? deviceName,
            ActivityType activityType,
            string action,
            string errorMessage,
            string? details = null)
        {
            try
            {
                var dto = ActivityHistoryDto.Failed(
                    deviceIp,
                    deviceName,
                    activityType,
                    action,
                    errorMessage,
                    details);

                await LogActivityAsync(dto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to log failed activity for {Action}", action);
            }
        }

        public async Task LogWarningAsync(
            string deviceIp,
            string? deviceName,
            ActivityType activityType,
            string action,
            string warningMessage,
            string? details = null,
            int? userCount = null,
            int? recordCount = null)
        {
            try
            {
                var dto = ActivityHistoryDto.Warning(
                    deviceIp,
                    deviceName,
                    activityType,
                    action,
                    warningMessage,
                    details,
                    userCount,
                    recordCount);

                await LogActivityAsync(dto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to log warning activity for {Action}", action);
            }
        }

        public async Task LogActivityAsync(ActivityHistoryDto activityDto)
        {
            try
            {
                var entity = new ActivityHistory
                {
                    Timestamp = activityDto.Timestamp,
                    DeviceIp = activityDto.DeviceIp,
                    DeviceName = activityDto.DeviceName,
                    ActivityType = activityDto.ActivityType,
                    Action = activityDto.Action,
                    Status = activityDto.Status,
                    Details = activityDto.Details,
                    UserCount = activityDto.UserCount,
                    RecordCount = activityDto.RecordCount,
                    Duration = activityDto.Duration,
                    ErrorMessage = activityDto.ErrorMessage,
                    CreatedAt = DateTime.Now
                };

                await _repository.AddAsync(entity);

                Log.Debug("Activity logged: {Action} - {Status}", activityDto.Action, activityDto.Status);
            }
            catch (Exception ex)
            {
                // Không throw lỗi ra ngoài để tránh ảnh hưởng đến logic chính
                Log.Error(ex, "Failed to log activity: {Action}", activityDto.Action);
            }
        }

        public async Task<(List<ActivityHistory> Items, int TotalCount)> GetActivitiesAsync(ActivityFilter filter)
        {
            try
            {
                return await _repository.GetPagedAsync(filter);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get activities");
                throw;
            }
        }

        public async Task<ActivityHistory?> GetActivityByIdAsync(long id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get activity by ID: {Id}", id);
                throw;
            }
        }

        public async Task<ActivityStatistics> GetStatisticsAsync(ActivityFilter filter)
        {
            try
            {
                return await _repository.GetStatisticsAsync(filter);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get statistics");
                throw;
            }
        }

        public async Task<int> ClearOldActivitiesAsync(int days)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-days);
                var deletedCount = await _repository.DeleteOlderThanAsync(cutoffDate);

                Log.Information("Cleared {Count} activities older than {Days} days", deletedCount, days);
                return deletedCount;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to clear old activities");
                throw;
            }
        }

        public async Task<int> ClearActivitiesByDeviceAsync(string deviceIp)
        {
            try
            {
                var deletedCount = await _repository.DeleteByDeviceIpAsync(deviceIp);

                Log.Information("Cleared {Count} activities for device {DeviceIp}", deletedCount, deviceIp);
                return deletedCount;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to clear activities by device");
                throw;
            }
        }

        public async Task<int> ClearAllActivitiesAsync()
        {
            try
            {
                var deletedCount = await _repository.DeleteAllAsync();

                Log.Warning("Cleared all {Count} activities", deletedCount);
                return deletedCount;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to clear all activities");
                throw;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.Core.DTOs;
using BHK.Retrieval.Attendance.Core.Enums;
using BHK.Retrieval.Attendance.Core.Interfaces;
using BHK.Retrieval.Attendance.Core.Models;
using Dapper;
using Serilog;

namespace BHK.Retrieval.Attendance.Infrastructure.Data
{
    /// <summary>
    /// Repository implementation cho ActivityHistory sử dụng SQLite + Dapper
    /// </summary>
    public class ActivityHistoryRepository : IActivityHistoryRepository
    {
        private readonly ISQLiteConnectionFactory _connectionFactory;

        public ActivityHistoryRepository(ISQLiteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<long> AddAsync(ActivityHistory activity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                var sql = @"
                    INSERT INTO ActivityHistory (
                        Timestamp, DeviceIp, DeviceName, ActivityType, Action, Status,
                        Details, UserCount, RecordCount, Duration, ErrorMessage, CreatedAt
                    ) VALUES (
                        @Timestamp, @DeviceIp, @DeviceName, @ActivityType, @Action, @Status,
                        @Details, @UserCount, @RecordCount, @Duration, @ErrorMessage, @CreatedAt
                    );
                    SELECT last_insert_rowid();";

                var id = await connection.ExecuteScalarAsync<long>(sql, new
                {
                    Timestamp = activity.Timestamp.ToString("O"),
                    activity.DeviceIp,
                    activity.DeviceName,
                    ActivityType = (int)activity.ActivityType,
                    activity.Action,
                    Status = (int)activity.Status,
                    activity.Details,
                    activity.UserCount,
                    activity.RecordCount,
                    Duration = activity.Duration?.ToString(),
                    activity.ErrorMessage,
                    CreatedAt = activity.CreatedAt.ToString("O")
                });

                return id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add activity to database");
                throw;
            }
        }

        public async Task<(List<ActivityHistory> Items, int TotalCount)> GetPagedAsync(ActivityFilter filter)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                var whereClause = BuildWhereClause(filter, out var parameters);
                var orderBy = "ORDER BY Timestamp DESC";
                var limit = $"LIMIT {filter.PageSize} OFFSET {filter.PageIndex * filter.PageSize}";

                // Query chính với phân trang
                var sql = $@"
                    SELECT 
                        Id, Timestamp, DeviceIp, DeviceName, ActivityType, Action, Status,
                        Details, UserCount, RecordCount, Duration, ErrorMessage, CreatedAt
                    FROM ActivityHistory
                    {whereClause}
                    {orderBy}
                    {limit}";

                // Query đếm tổng số
                var countSql = $@"
                    SELECT COUNT(*)
                    FROM ActivityHistory
                    {whereClause}";

                var items = (await connection.QueryAsync<ActivityHistoryDb>(sql, parameters))
                    .Select(MapToEntity)
                    .ToList();

                var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

                return (items, totalCount);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get paged activities from database");
                throw;
            }
        }

        public async Task<ActivityHistory?> GetByIdAsync(long id)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                var sql = @"
                    SELECT 
                        Id, Timestamp, DeviceIp, DeviceName, ActivityType, Action, Status,
                        Details, UserCount, RecordCount, Duration, ErrorMessage, CreatedAt
                    FROM ActivityHistory
                    WHERE Id = @Id";

                var result = await connection.QueryFirstOrDefaultAsync<ActivityHistoryDb>(sql, new { Id = id });
                return result != null ? MapToEntity(result) : null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get activity by ID from database");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                var sql = "DELETE FROM ActivityHistory WHERE Id = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete activity from database");
                throw;
            }
        }

        public async Task<int> DeleteOlderThanAsync(DateTime cutoffDate)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                var sql = "DELETE FROM ActivityHistory WHERE Timestamp < @CutoffDate";
                var rowsAffected = await connection.ExecuteAsync(sql, new { CutoffDate = cutoffDate.ToString("O") });
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete old activities from database");
                throw;
            }
        }

        public async Task<int> DeleteByDeviceIpAsync(string deviceIp)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                var sql = "DELETE FROM ActivityHistory WHERE DeviceIp = @DeviceIp";
                var rowsAffected = await connection.ExecuteAsync(sql, new { DeviceIp = deviceIp });
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete activities by device IP from database");
                throw;
            }
        }

        public async Task<int> DeleteAllAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                var sql = "DELETE FROM ActivityHistory";
                var rowsAffected = await connection.ExecuteAsync(sql);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete all activities from database");
                throw;
            }
        }

        public async Task<ActivityStatistics> GetStatisticsAsync(ActivityFilter filter)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                var whereClause = BuildWhereClause(filter, out var parameters);

                // Query thống kê tổng quan
                var summaryQuery = $@"
                    SELECT 
                        COUNT(*) as TotalActivities,
                        SUM(CASE WHEN Status = {(int)ActivityStatus.Success} THEN 1 ELSE 0 END) as SuccessCount,
                        SUM(CASE WHEN Status = {(int)ActivityStatus.Failed} THEN 1 ELSE 0 END) as FailedCount,
                        SUM(CASE WHEN Status = {(int)ActivityStatus.Warning} THEN 1 ELSE 0 END) as WarningCount,
                        COUNT(DISTINCT DeviceIp) as DeviceCount
                    FROM ActivityHistory
                    {whereClause}";

                var summary = await connection.QueryFirstAsync<ActivityStatistics>(summaryQuery, parameters);

                // Query thống kê theo device
                var deviceQuery = $@"
                    SELECT DeviceIp, COUNT(*) as Count
                    FROM ActivityHistory
                    {whereClause}
                    GROUP BY DeviceIp
                    ORDER BY Count DESC
                    LIMIT 10";

                var deviceStats = await connection.QueryAsync<(string DeviceIp, int Count)>(deviceQuery, parameters);
                summary.DeviceActivityCounts = deviceStats.ToDictionary(x => x.DeviceIp, x => x.Count);

                // Query thống kê theo activity type
                var typeQuery = $@"
                    SELECT ActivityType, COUNT(*) as Count
                    FROM ActivityHistory
                    {whereClause}
                    GROUP BY ActivityType
                    ORDER BY Count DESC";

                var typeStats = await connection.QueryAsync<(int ActivityType, int Count)>(typeQuery, parameters);
                summary.TopActivityTypes = typeStats.ToDictionary(
                    x => ((ActivityType)x.ActivityType).ToString(), 
                    x => x.Count);

                return summary;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get statistics from database");
                throw;
            }
        }

        public async Task<bool> IsDatabaseInitializedAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                var query = @"
                    SELECT COUNT(*) 
                    FROM sqlite_master 
                    WHERE type='table' AND name='ActivityHistory'";

                var count = await connection.ExecuteScalarAsync<int>(query);
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task InitializeDatabaseAsync()
        {
            var initializer = new DatabaseInitializer(_connectionFactory);
            await initializer.InitializeAsync();
        }

        #region Helper Methods

        private string BuildWhereClause(ActivityFilter filter, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            var conditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(filter.DeviceIp))
            {
                conditions.Add("DeviceIp = @DeviceIp");
                parameters.Add("DeviceIp", filter.DeviceIp);
            }

            if (filter.ActivityType.HasValue)
            {
                conditions.Add("ActivityType = @ActivityType");
                parameters.Add("ActivityType", (int)filter.ActivityType.Value);
            }

            if (filter.Statuses != null && filter.Statuses.Any())
            {
                var statusValues = filter.Statuses.Select(s => (int)s).ToList();
                conditions.Add($"Status IN ({string.Join(",", statusValues)})");
            }

            if (filter.FromDate.HasValue)
            {
                conditions.Add("Timestamp >= @FromDate");
                parameters.Add("FromDate", filter.FromDate.Value.ToString("O"));
            }

            if (filter.ToDate.HasValue)
            {
                conditions.Add("Timestamp <= @ToDate");
                parameters.Add("ToDate", filter.ToDate.Value.ToString("O"));
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                conditions.Add("(Action LIKE @SearchText OR Details LIKE @SearchText)");
                parameters.Add("SearchText", $"%{filter.SearchText}%");
            }

            return conditions.Any() ? "WHERE " + string.Join(" AND ", conditions) : "";
        }

        private ActivityHistory MapToEntity(ActivityHistoryDb db)
        {
            return new ActivityHistory
            {
                Id = db.Id,
                Timestamp = DateTime.Parse(db.Timestamp),
                DeviceIp = db.DeviceIp,
                DeviceName = db.DeviceName,
                ActivityType = (ActivityType)db.ActivityType,
                Action = db.Action,
                Status = (ActivityStatus)db.Status,
                Details = db.Details,
                UserCount = db.UserCount,
                RecordCount = db.RecordCount,
                Duration = !string.IsNullOrWhiteSpace(db.Duration) ? TimeSpan.Parse(db.Duration) : null,
                ErrorMessage = db.ErrorMessage,
                CreatedAt = DateTime.Parse(db.CreatedAt)
            };
        }

        #endregion

        #region Database Model

        /// <summary>
        /// Internal model for Dapper mapping (SQLite stores as TEXT/INTEGER)
        /// </summary>
        private class ActivityHistoryDb
        {
            public long Id { get; set; }
            public string Timestamp { get; set; } = string.Empty;
            public string DeviceIp { get; set; } = string.Empty;
            public string? DeviceName { get; set; }
            public int ActivityType { get; set; }
            public string Action { get; set; } = string.Empty;
            public int Status { get; set; }
            public string? Details { get; set; }
            public int? UserCount { get; set; }
            public int? RecordCount { get; set; }
            public string? Duration { get; set; }
            public string? ErrorMessage { get; set; }
            public string CreatedAt { get; set; } = string.Empty;
        }

        #endregion
    }
}

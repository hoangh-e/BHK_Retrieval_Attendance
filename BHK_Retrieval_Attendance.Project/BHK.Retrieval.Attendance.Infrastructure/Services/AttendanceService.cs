using BHK.Retrieval.Attendance.Core.DTOs.Requests;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;
using BHK.Retrieval.Attendance.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BHK.Retrieval.Attendance.Infrastructure.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ILogger<AttendanceService> _logger;
        private readonly IDeviceCommunicationService _deviceService;

        public AttendanceService(
            ILogger<AttendanceService> logger,
            IDeviceCommunicationService deviceService)
        {
            _logger = logger;
            _deviceService = deviceService;
        }

        public async Task<List<AttendanceDisplayDto>> GetAttendanceRecordsAsync(AttendanceFilterDto filter)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var startDate = filter.StartDate ?? DateTime.Today.AddDays(-7);
                var endDate = filter.EndDate ?? DateTime.Today;

                _logger.LogInformation($"⏱️ START Getting attendance records from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");

                // Lấy dữ liệu chấm công từ thiết bị
                var deviceStopwatch = System.Diagnostics.Stopwatch.StartNew();
                var attendanceRecords = await _deviceService.GetAttendanceRecordsAsync(startDate, endDate);
                deviceStopwatch.Stop();
                _logger.LogInformation($"⏱️ Device GetAttendanceRecords took {deviceStopwatch.ElapsedMilliseconds}ms");

                if (attendanceRecords == null || !attendanceRecords.Any())
                {
                    _logger.LogWarning("No attendance records found");
                    return new List<AttendanceDisplayDto>();
                }

                // Map trực tiếp từ AttRecord sang AttendanceDisplayDto (KHÔNG cần join với employee data)
                var mappingStopwatch = System.Diagnostics.Stopwatch.StartNew();
                var displayRecords = attendanceRecords.Select(record =>
                {
                    var hour = record.Time.Hour;
                    string checkType;
                    
                    // Logic xác định loại chấm công dựa theo giờ
                    if (hour >= 4 && hour <= 11)
                    {
                        checkType = "Check In";
                    }
                    else if (hour >= 13 && hour <= 18)
                    {
                        checkType = "Check Out";
                    }
                    else
                    {
                        checkType = "Khác"; // Ngoài giờ định nghĩa
                    }

                    // Map VerifyMode to readable text
                    string verifyText = record.VerifyMode switch
                    {
                        0 => "PW",
                        1 => "FP",
                        2 => "Card",
                        3 => "Face",
                        4 => "Iris",
                        _ => "Unknown"
                    };

                    // Map Action to readable text
                    string actionText = record.Action switch
                    {
                        0 => "In",
                        1 => "Out",
                        2 => "Break",
                        _ => "Other"
                    };

                    return new AttendanceDisplayDto
                    {
                        DN = record.DN.ToString(),
                        DIN = record.DIN.ToString(),
                        Date = record.Time.ToString("dd/MM/yyyy"),
                        Time = record.Time.ToString("HH:mm:ss"),
                        Type = checkType,
                        Verify = verifyText,
                        Action = actionText,
                        Remark = record.Remark
                    };
                }).ToList();
                mappingStopwatch.Stop();
                _logger.LogInformation($"⏱️ Mapping {displayRecords.Count} records took {mappingStopwatch.ElapsedMilliseconds}ms");

                // Apply time filter nếu cần
                var filterStopwatch = System.Diagnostics.Stopwatch.StartNew();
                if (filter.TimeFilter == TimeFilter.CheckIn)
                {
                    displayRecords = displayRecords
                        .Where(x => x.Type == "Check In")
                        .ToList();
                }
                else if (filter.TimeFilter == TimeFilter.CheckOut)
                {
                    displayRecords = displayRecords
                        .Where(x => x.Type == "Check Out")
                        .ToList();
                }
                filterStopwatch.Stop();

                // Sort theo thời gian mới nhất
                var sortedRecords = displayRecords.OrderByDescending(x => x.Date).ThenByDescending(x => x.Time).ToList();
                
                stopwatch.Stop();
                _logger.LogInformation(
                    $"✅ TOTAL Retrieved {sortedRecords.Count} attendance records in {stopwatch.ElapsedMilliseconds}ms " +
                    $"(Device: {deviceStopwatch.ElapsedMilliseconds}ms, Mapping: {mappingStopwatch.ElapsedMilliseconds}ms, " +
                    $"Filter: {filterStopwatch.ElapsedMilliseconds}ms)");
                
                return sortedRecords;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"❌ Error getting attendance records after {stopwatch.ElapsedMilliseconds}ms");
                return new List<AttendanceDisplayDto>();
            }
        }

        public async Task<bool> ExportAttendanceAsync(ExportConfigDto config, string filePath)
        {
            // TODO: Implement export logic
            await Task.Delay(100);
            _logger.LogInformation($"Export {config.RecordCount} records to {filePath}");
            return true;
        }

        public async Task<int> SyncAttendanceFromDeviceAsync()
        {
            // TODO: Implement sync from device
            await Task.Delay(100);
            return 0;
        }
    }
}

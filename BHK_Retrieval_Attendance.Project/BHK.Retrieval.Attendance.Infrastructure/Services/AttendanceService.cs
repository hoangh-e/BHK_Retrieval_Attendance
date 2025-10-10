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

                // BƯỚC 1: Lấy dữ liệu từ thiết bị (thường chậm nhất)
                var deviceStopwatch = System.Diagnostics.Stopwatch.StartNew();
                var attendanceRecords = await _deviceService.GetAttendanceRecordsAsync(startDate, endDate);
                deviceStopwatch.Stop();
                _logger.LogInformation($"⏱️ Device GetAttendanceRecords took {deviceStopwatch.ElapsedMilliseconds}ms");

                if (attendanceRecords == null || !attendanceRecords.Any())
                {
                    _logger.LogWarning("No attendance records found");
                    return new List<AttendanceDisplayDto>();
                }

                // BƯỚC 2: Lấy danh sách nhân viên để map thông tin
                var employeeStopwatch = System.Diagnostics.Stopwatch.StartNew();
                var employees = await _deviceService.GetAllEmployeesAsync();
                var employeeDict = employees?.ToDictionary(e => e.DIN, e => e) ?? new Dictionary<ulong, EmployeeDto>();
                employeeStopwatch.Stop();
                _logger.LogInformation($"⏱️ Get {employeeDict.Count} employees took {employeeStopwatch.ElapsedMilliseconds}ms");

                // BƯỚC 3: Chuyển đổi sang DTO hiển thị
                var mappingStopwatch = System.Diagnostics.Stopwatch.StartNew();
                var displayRecords = attendanceRecords.Select(record =>
                {
                    // Tìm thông tin nhân viên
                    var employee = employeeDict.ContainsKey(record.DIN) ? employeeDict[record.DIN] : null;

                    return new AttendanceDisplayDto
                    {
                        DIN = record.DIN,
                        EmployeeId = employee?.IDNumber ?? record.DIN.ToString(),
                        EmployeeName = employee?.UserName ?? "Không xác định",
                        CheckTime = record.Time,
                        Date = record.Time.ToString("dd/MM/yyyy"),
                        Time = record.Time.ToString("HH:mm:ss"),
                        VerifyMode = GetVerifyModeText(record.VerifyMode),
                        CheckType = GetCheckTypeText(record.State),
                        DeviceId = 1, // TODO: Get from device config
                        Remark = string.Empty
                    };
                }).ToList();
                mappingStopwatch.Stop();
                _logger.LogInformation($"⏱️ Mapping {displayRecords.Count} records took {mappingStopwatch.ElapsedMilliseconds}ms");

                // BƯỚC 4: Apply time filter
                var filterStopwatch = System.Diagnostics.Stopwatch.StartNew();
                if (filter.TimeFilter == TimeFilter.CheckIn)
                {
                    displayRecords = displayRecords
                        .Where(x => x.CheckTime.Hour >= 4 && x.CheckTime.Hour <= 11)
                        .ToList();
                }
                else if (filter.TimeFilter == TimeFilter.CheckOut)
                {
                    displayRecords = displayRecords
                        .Where(x => x.CheckTime.Hour >= 13 && x.CheckTime.Hour <= 18)
                        .ToList();
                }
                filterStopwatch.Stop();

                // BƯỚC 5: Sort
                var sortedRecords = displayRecords.OrderByDescending(x => x.CheckTime).ToList();
                
                stopwatch.Stop();
                _logger.LogInformation(
                    $"✅ TOTAL Retrieved {sortedRecords.Count} attendance records in {stopwatch.ElapsedMilliseconds}ms " +
                    $"(Device: {deviceStopwatch.ElapsedMilliseconds}ms, Employees: {employeeStopwatch.ElapsedMilliseconds}ms, " +
                    $"Mapping: {mappingStopwatch.ElapsedMilliseconds}ms, Filter: {filterStopwatch.ElapsedMilliseconds}ms)");
                
                return sortedRecords;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"❌ Error getting attendance records after {stopwatch.ElapsedMilliseconds}ms");
                return new List<AttendanceDisplayDto>();
            }
        }

        private string GetVerifyModeText(int verifyMode)
        {
            return verifyMode switch
            {
                0 => "Mật khẩu",
                1 => "Vân tay",
                2 => "Thẻ từ",
                3 => "Khuôn mặt",
                4 => "Mống mắt",
                _ => "Không xác định"
            };
        }

        private string GetCheckTypeText(int state)
        {
            return state switch
            {
                0 => "Check-in",
                1 => "Check-out",
                2 => "Nghỉ giải lao",
                3 => "Bắt đầu làm việc",
                4 => "Kết thúc làm việc",
                _ => "Khác"
            };
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

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
            try
            {
                var startDate = filter.StartDate ?? DateTime.Today.AddDays(-7);
                var endDate = filter.EndDate ?? DateTime.Today;

                _logger.LogInformation($"Getting attendance records from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");

                // Lấy dữ liệu từ thiết bị
                var attendanceRecords = await _deviceService.GetAttendanceRecordsAsync(startDate, endDate);

                if (attendanceRecords == null || !attendanceRecords.Any())
                {
                    _logger.LogWarning("No attendance records found");
                    return new List<AttendanceDisplayDto>();
                }

                // Lấy danh sách nhân viên để map thông tin
                var employees = await _deviceService.GetAllEmployeesAsync();
                var employeeDict = employees?.ToDictionary(e => e.DIN, e => e) ?? new Dictionary<ulong, EmployeeDto>();

                // Chuyển đổi sang DTO hiển thị
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

                // Apply time filter
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

                _logger.LogInformation($"Retrieved {displayRecords.Count} attendance records");
                
                return displayRecords.OrderByDescending(x => x.CheckTime).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance records");
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

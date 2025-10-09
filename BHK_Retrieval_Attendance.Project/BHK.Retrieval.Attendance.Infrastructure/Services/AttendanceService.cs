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
            // TODO: Implement logic lấy từ database hoặc thiết bị
            // Hiện tại return mock data
            await Task.Delay(500); // Simulate async operation

            var mockData = new List<AttendanceDisplayDto>();
            
            // Generate mock data based on filter
            var random = new Random();
            var startDate = filter.StartDate ?? DateTime.Today.AddDays(-7);
            var endDate = filter.EndDate ?? DateTime.Today;

            for (int i = 1; i <= 20; i++)
            {
                var checkTime = startDate.AddHours(random.Next(0, (int)(endDate - startDate).TotalHours));
                
                // Apply time filter
                if (filter.TimeFilter == TimeFilter.CheckIn && (checkTime.Hour < 4 || checkTime.Hour > 11))
                    continue;
                if (filter.TimeFilter == TimeFilter.CheckOut && (checkTime.Hour < 13 || checkTime.Hour > 18))
                    continue;

                mockData.Add(new AttendanceDisplayDto
                {
                    DIN = (ulong)i,
                    EmployeeId = $"NV{i:D4}",
                    EmployeeName = $"Nhân viên {i}",
                    CheckTime = checkTime,
                    Date = checkTime.ToString("dd/MM/yyyy"),
                    Time = checkTime.ToString("HH:mm:ss"),
                    VerifyMode = random.Next(0, 3) switch
                    {
                        0 => "Vân tay",
                        1 => "Thẻ từ",
                        _ => "Mật khẩu"
                    },
                    CheckType = checkTime.Hour < 12 ? "Check-in" : "Check-out",
                    DeviceId = 1,
                    Remark = ""
                });
            }

            return mockData.OrderByDescending(x => x.CheckTime).ToList();
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

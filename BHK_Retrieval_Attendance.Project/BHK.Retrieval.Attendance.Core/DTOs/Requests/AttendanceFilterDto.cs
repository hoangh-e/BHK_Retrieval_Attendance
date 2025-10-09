namespace BHK.Retrieval.Attendance.Core.DTOs.Requests
{
    /// <summary>
    /// DTO cho filter chấm công
    /// </summary>
    public class AttendanceFilterDto
    {
        public FilterType FilterType { get; set; }
        
        // Option 1: Predefined date range
        public PredefinedDateRange? PredefinedRange { get; set; }
        
        // Option 2: Single date
        public DateTime? SingleDate { get; set; }
        
        // Option 3: Custom range
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        // Time filter
        public TimeFilter TimeFilter { get; set; }
    }

    public enum FilterType
    {
        PredefinedRange = 1,
        SingleDate = 2,
        DateRange = 3
    }

    public enum PredefinedDateRange
    {
        Today,
        Yesterday,
        Last3Days,
        Last7Days,
        Last30Days,
        CurrentWeek,     // Từ thứ 2 đến hiện tại
        CurrentMonth     // Từ đầu tháng đến hiện tại
    }

    public enum TimeFilter
    {
        All,
        CheckIn,         // 4h - 11h
        CheckOut         // 13h - 18h
    }
}

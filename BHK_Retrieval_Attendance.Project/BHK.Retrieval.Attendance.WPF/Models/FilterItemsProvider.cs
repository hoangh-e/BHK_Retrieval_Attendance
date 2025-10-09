using System;
using System.Collections.Generic;
using System.Linq;
using BHK.Retrieval.Attendance.Core.DTOs.Requests;

namespace BHK.Retrieval.Attendance.WPF.Models
{
    /// <summary>
    /// Helper class cho ComboBox items
    /// </summary>
    public class ComboBoxItem<T>
    {
        public string Display { get; set; } = string.Empty;
        public T Value { get; set; }
    }

    /// <summary>
    /// Provider cho các items của filter
    /// </summary>
    public static class FilterItemsProvider
    {
        public static List<ComboBoxItem<PredefinedDateRange>> GetPredefinedDateRanges()
        {
            return new List<ComboBoxItem<PredefinedDateRange>>
            {
                new() { Display = "Hôm nay", Value = PredefinedDateRange.Today },
                new() { Display = "Hôm qua", Value = PredefinedDateRange.Yesterday },
                new() { Display = "3 ngày gần nhất", Value = PredefinedDateRange.Last3Days },
                new() { Display = "7 ngày gần nhất", Value = PredefinedDateRange.Last7Days },
                new() { Display = "30 ngày gần nhất", Value = PredefinedDateRange.Last30Days },
                new() { Display = "Tuần hiện tại", Value = PredefinedDateRange.CurrentWeek },
                new() { Display = "Tháng hiện tại", Value = PredefinedDateRange.CurrentMonth }
            };
        }

        public static List<ComboBoxItem<TimeFilter>> GetTimeFilters()
        {
            return new List<ComboBoxItem<TimeFilter>>
            {
                new() { Display = "Tất cả", Value = TimeFilter.All },
                new() { Display = "Check-in (4h - 11h)", Value = TimeFilter.CheckIn },
                new() { Display = "Check-out (13h - 18h)", Value = TimeFilter.CheckOut }
            };
        }
    }
}

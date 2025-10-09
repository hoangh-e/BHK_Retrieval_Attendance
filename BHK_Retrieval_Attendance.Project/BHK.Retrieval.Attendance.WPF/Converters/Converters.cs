using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BHK.Retrieval.Attendance.WPF.Converters
{
    /// <summary>
    /// Converter để lấy chữ cái đầu từ tên (cho avatar)
    /// </summary>
    public class InitialConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string name && !string.IsNullOrWhiteSpace(name))
            {
                // Lấy chữ cái đầu của các từ
                var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (words.Length >= 2)
                {
                    // Lấy chữ cái đầu của 2 từ đầu
                    return $"{words[0][0]}{words[words.Length - 1][0]}".ToUpper();
                }
                else if (words.Length == 1 && words[0].Length > 0)
                {
                    // Lấy chữ cái đầu
                    return words[0][0].ToString().ToUpper();
                }
            }

            return "?";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter để lấy số thứ tự dòng trong DataGrid
    /// </summary>
    public class RowNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataGridRow row)
            {
                var dataGrid = ItemsControl.ItemsControlFromItemContainer(row) as DataGrid;
                if (dataGrid != null)
                {
                    int index = dataGrid.ItemContainerGenerator.IndexFromContainer(row);
                    return (index + 1).ToString();
                }
            }

            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter để đảo ngược Boolean sang Visibility
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }

            return false;
        }
    }

    /// <summary>
    /// Converter cho EnrollType sang text tiếng Việt
    /// </summary>
    public class EnrollTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.GetType().Name == "EnrollType")
            {
                // Sử dụng reflection để xử lý enum từ Riss.Devices
                var enrollType = value.ToString();
                return enrollType switch
                {
                    "FP0" => "Vân tay 0",
                    "FP1" => "Vân tay 1",
                    "FP2" => "Vân tay 2",
                    "FP3" => "Vân tay 3",
                    "FP4" => "Vân tay 4",
                    "FP5" => "Vân tay 5",
                    "FP6" => "Vân tay 6",
                    "FP7" => "Vân tay 7",
                    "FP8" => "Vân tay 8",
                    "FP9" => "Vân tay 9",
                    "PWD" => "Mật khẩu",
                    "Card" => "Thẻ từ",
                    _ => "Không xác định"
                };
            }

            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter cho Sex enum sang text tiếng Việt
    /// </summary>
    public class SexToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var sexValue = value.ToString();
                return sexValue?.ToLower() switch
                {
                    "male" => "Nam",
                    "female" => "Nữ",
                    "nam" => "Nam",
                    "nữ" => "Nữ",
                    _ => sexValue ?? "Không xác định"
                };
            }

            return "Không xác định";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return text.ToLower() switch
                {
                    "nam" => "Male",
                    "nữ" => "Female",
                    _ => "Male"
                };
            }

            return "Male";
        }
    }
}

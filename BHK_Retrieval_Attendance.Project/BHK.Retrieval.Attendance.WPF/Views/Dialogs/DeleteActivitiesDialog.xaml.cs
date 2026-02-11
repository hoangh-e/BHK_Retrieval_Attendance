using System;
using System.Collections.Generic;
using System.Windows;

namespace BHK.Retrieval.Attendance.WPF.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for DeleteActivitiesDialog.xaml
    /// </summary>
    public partial class DeleteActivitiesDialog : Window
    {
        public DeleteOption SelectedOption { get; private set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SelectedDeviceIp { get; set; }
        public List<string> DeviceIpAddresses { get; set; }

        public DeleteActivitiesDialog()
        {
            InitializeComponent();
            DataContext = this;
            
            // Initialize with default date range (last 90 days)
            ToDate = DateTime.Now;
            FromDate = ToDate.Value.AddDays(-90);
            
            DeviceIpAddresses = new List<string>();
        }

        public DeleteActivitiesDialog(List<string> deviceIps) : this()
        {
            DeviceIpAddresses = deviceIps ?? new List<string>();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Determine which option is selected
            if (DeleteByDateRangeRadio.IsChecked == true)
            {
                if (!FromDate.HasValue || !ToDate.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn khoảng thời gian hợp lệ.", 
                                    "Lỗi", 
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Warning);
                    return;
                }

                if (FromDate > ToDate)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc.", 
                                    "Lỗi", 
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Warning);
                    return;
                }

                SelectedOption = DeleteOption.ByDateRange;
            }
            else if (DeleteByDeviceRadio.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(SelectedDeviceIp))
                {
                    MessageBox.Show("Vui lòng chọn thiết bị.", 
                                    "Lỗi", 
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Warning);
                    return;
                }

                SelectedOption = DeleteOption.ByDevice;
            }
            else if (DeleteAllRadio.IsChecked == true)
            {
                var result = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa TẤT CẢ lịch sử hoạt động?\n\nThao tác này KHÔNG THỂ KHÔI PHỤC!",
                    "Xác nhận xóa tất cả",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                SelectedOption = DeleteOption.All;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    /// <summary>
    /// Delete option enum
    /// </summary>
    public enum DeleteOption
    {
        ByDateRange,
        ByDevice,
        All
    }
}

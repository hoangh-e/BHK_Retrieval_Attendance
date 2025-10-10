using System.Windows.Controls;
using BHK.Retrieval.Attendance.WPF.ViewModels;

namespace BHK.Retrieval.Attendance.WPF.Views.Pages
{
    /// <summary>
    /// Interaction logic for AttendanceManagementView.xaml
    /// </summary>
    public partial class AttendanceManagementView : UserControl
    {
        public AttendanceManagementView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event được gọi khi view được load
        /// Không tự động load dữ liệu, user cần click nút "Lọc" để load
        /// </summary>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Không tự động load dữ liệu nữa
            // User sẽ click nút "Lọc" để load dữ liệu theo điều kiện
        }
    }
}

using System.Windows.Controls;
using BHK.Retrieval.Attendance.WPF.ViewModels;

namespace BHK.Retrieval.Attendance.WPF.Views.Pages
{
    /// <summary>
    /// Interaction logic for EmployeeView.xaml
    /// </summary>
    public partial class EmployeeView : UserControl
    {
        public EmployeeView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load dữ liệu khi view được load
        /// </summary>
        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is EmployeeViewModel viewModel)
            {
                await viewModel.LoadEmployeesAsync();
            }
        }
    }
}
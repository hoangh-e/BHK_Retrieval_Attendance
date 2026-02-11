using System.Windows.Controls;
using BHK.Retrieval.Attendance.WPF.ViewModels;

namespace BHK.Retrieval.Attendance.WPF.Views.Pages
{
    /// <summary>
    /// Interaction logic for DeviceConnectionView.xaml
    /// </summary>
    public partial class DeviceConnectionView : UserControl
    {
        public DeviceConnectionView()
        {
            InitializeComponent();
            Loaded += DeviceConnectionView_Loaded;
        }

        private void DeviceConnectionView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Sync connection status khi view được load
            if (DataContext is DeviceConnectionViewModel viewModel)
            {
                viewModel.CheckConnectionStatus();
            }
        }
    }
}

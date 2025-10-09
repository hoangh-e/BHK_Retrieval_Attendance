using System.Windows;
using System.Windows.Controls;
using BHK.Retrieval.Attendance.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BHK.Retrieval.Attendance.WPF.Views.Pages
{
    /// <summary>
    /// Interaction logic for HomePageView.xaml
    /// </summary>
    public partial class HomePageView : UserControl
    {
        // Constructor mặc định cho XAML
        public HomePageView()
        {
            InitializeComponent();
            
            // Tự động inject ViewModel từ DI container
            // Note: In this simplified version, ViewModel should be injected through the DI constructor
            // This default constructor is primarily for XAML designer support
        }
        
        // Constructor với DI (optional, giữ lại nếu cần)
        public HomePageView(HomePageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)    
        {
            if (e.Source is TabControl tabControl)
            {
                var selectedTab = tabControl.SelectedItem as TabItem;
                if (selectedTab != null)
                {
                    // Tab selection changed - logging removed for simplified version
                }
            }
        }
    }
}
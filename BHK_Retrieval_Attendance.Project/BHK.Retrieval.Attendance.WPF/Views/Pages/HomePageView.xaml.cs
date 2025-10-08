using System.Windows.Controls;
using BHK.Retrieval.Attendance.WPF.ViewModels;
using Microsoft.Extensions.Logging;

namespace BHK.Retrieval.Attendance.WPF.Views.Pages
{
    /// <summary>
    /// Interaction logic for HomePageView.xaml
    /// </summary>
    public partial class HomePageView : UserControl
    {
        private readonly ILogger<HomePageView> _logger;

        public HomePageView(HomePageViewModel viewModel, ILogger<HomePageView> logger)
        {
            InitializeComponent();
            DataContext = viewModel;
            _logger = logger;

            // Set default tab to Device Info
            MainTabControl.SelectedIndex = 0;

            _logger.LogInformation("HomePageView initialized");
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)    
        {
            if (e.Source is TabControl tabControl)
            {
                var selectedTab = tabControl.SelectedItem as TabItem;
                if (selectedTab != null)
                {
                    _logger.LogInformation($"Tab changed to: {selectedTab.Name}");
                }
            }
        }
    }
}
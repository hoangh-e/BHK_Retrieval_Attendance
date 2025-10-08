using System.Windows;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BHK.Retrieval.Attendance.WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly INavigationService _navigationService;
        private readonly ILogger<MainWindow> _logger;
        
        public MainWindow(
            INavigationService navigationService,
            ILogger<MainWindow> logger)
        {
            InitializeComponent();
            
            _navigationService = navigationService;
            _logger = logger;
            
            // Set Frame cho NavigationService
            _navigationService.SetNavigationFrame(MainFrame);
            
            _logger.LogInformation("MainWindow initialized");
            
            // Navigate SAU KHI window được load để tránh UI trắng
            Loaded += MainWindow_Loaded;
        }
        
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogInformation("MainWindow loaded, navigating to DeviceConnection");
                
                // Navigate to DeviceConnection view
                _navigationService.NavigateTo("DeviceConnection");
                
                _logger.LogInformation("Navigation completed successfully");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error during initial navigation");
                MessageBox.Show($"Lỗi khi chuyển trang: {ex.Message}", 
                               "Lỗi", 
                               MessageBoxButton.OK, 
                               MessageBoxImage.Error);
            }
        }
    }
}
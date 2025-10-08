using System.Windows;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;

namespace BHK.Retrieval.Attendance.WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly INavigationService _navigationService;
        
        public MainWindow(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
            
            // Set Frame cho NavigationService
            _navigationService.SetNavigationFrame(MainFrame);
            
            // Navigate to DeviceConnection view ban đầu
            _navigationService.NavigateTo("DeviceConnection");
        }
    }
}
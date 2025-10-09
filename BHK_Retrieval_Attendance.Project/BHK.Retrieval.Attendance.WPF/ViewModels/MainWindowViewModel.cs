using System.ComponentModel;
using System.Runtime.CompilerServices;
using BHK.Retrieval.Attendance.WPF.ViewModels.Pages;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private object _currentPage = null!;
        
        public object CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel(HomePageViewModel homePageViewModel)
        {
            // Set trang mặc định
            CurrentPage = homePageViewModel;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

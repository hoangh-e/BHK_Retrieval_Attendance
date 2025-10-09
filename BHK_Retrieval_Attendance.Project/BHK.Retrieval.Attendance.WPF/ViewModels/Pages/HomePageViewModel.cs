using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BHK.Retrieval.Attendance.WPF.ViewModels.Pages
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        // Dependency injection các service nếu cần
        // private readonly IYourService _yourService;

        // Constructor với DI
        // public HomePageViewModel(IYourService yourService)
        // {
        //     _yourService = yourService;
        // }

        // Hoặc constructor mặc định nếu không cần DI
        public HomePageViewModel()
        {
        }

        // Properties và Commands
        private string _title = "Home Page";
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
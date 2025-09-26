using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using BHK.Retrieval.Attendance.WPF.Services;
using BHK.Retrieval.Attendance.WPF.Views.Pages;
using BHK.Retrieval.Attendance.WPF.Models;
using System.Windows.Threading;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly INavigationService _navigationService;
        private readonly IDeviceService _deviceService;
        private readonly IServiceProvider _serviceProvider;
        private readonly DispatcherTimer _timer;

        public MainWindowViewModel(INavigationService navigationService, 
                                  IDeviceService deviceService,
                                  IServiceProvider serviceProvider)
        {
            _navigationService = navigationService;
            _deviceService = deviceService;
            _serviceProvider = serviceProvider;

            // Initialize commands
            ToggleMenuCommand = new RelayCommand(ToggleMenu);
            NavigateToDeviceConnectionCommand = new RelayCommand(NavigateToDeviceConnection);
            NavigateToLoginCommand = new RelayCommand(NavigateToLogin);
            NavigateToEmployeeSelectionCommand = new RelayCommand(NavigateToEmployeeSelection, CanNavigateToEmployeeSelection);
            ExitApplicationCommand = new RelayCommand(ExitApplication);

            // Initialize timer for current time
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // Subscribe to device connection events
            _deviceService.ConnectionStatusChanged += OnDeviceConnectionStatusChanged;

            // Initialize with welcome view
            StatusMessage = "Chào mừng đến với hệ thống chấm công BHK";
            IsMenuVisible = true;
            CurrentTime = DateTime.Now;
            UpdateDeviceConnectionStatus();
        }

        #region Properties

        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private bool _isMenuVisible = true;
        public bool IsMenuVisible
        {
            get => _isMenuVisible;
            set
            {
                _isMenuVisible = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        private string _currentUser = "Chưa đăng nhập";
        public string CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoggedIn));
                ((RelayCommand)NavigateToEmployeeSelectionCommand).RaiseCanExecuteChanged();
            }
        }

        private DateTime _currentTime;
        public DateTime CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                OnPropertyChanged();
            }
        }

        private ConnectionStatus _deviceConnectionStatus = ConnectionStatus.Disconnected;
        public ConnectionStatus DeviceConnectionStatus
        {
            get => _deviceConnectionStatus;
            set
            {
                _deviceConnectionStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DeviceConnectionText));
            }
        }

        public string DeviceConnectionText
        {
            get
            {
                return DeviceConnectionStatus switch
                {
                    ConnectionStatus.Connected => "Đã kết nối",
                    ConnectionStatus.Connecting => "Đang kết nối...",
                    ConnectionStatus.Disconnected => "Chưa kết nối",
                    ConnectionStatus.Error => "Lỗi kết nối",
                    _ => "Không xác định"
                };
            }
        }

        public bool IsLoggedIn => CurrentUser != "Chưa đăng nhập";

        #endregion

        #region Commands

        public ICommand ToggleMenuCommand { get; }
        public ICommand NavigateToDeviceConnectionCommand { get; }
        public ICommand NavigateToLoginCommand { get; }
        public ICommand NavigateToEmployeeSelectionCommand { get; }
        public ICommand ExitApplicationCommand { get; }

        #endregion

        #region Command Implementations

        private void ToggleMenu()
        {
            IsMenuVisible = !IsMenuVisible;
        }

        private void NavigateToDeviceConnection()
        {
            var view = _serviceProvider.GetRequiredService<DeviceConnectionView>();
            CurrentView = view;
            StatusMessage = "Quản lý kết nối thiết bị";
        }

        private void NavigateToLogin()
        {
            var view = _serviceProvider.GetRequiredService<LoginView>();
            var viewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
            viewModel.LoginSuccessful += OnLoginSuccessful;
            view.DataContext = viewModel;
            CurrentView = view;
            StatusMessage = "Đăng nhập vào hệ thống";
        }

        private bool CanNavigateToEmployeeSelection()
        {
            return IsLoggedIn;
        }

        private void NavigateToEmployeeSelection()
        {
            var view = _serviceProvider.GetRequiredService<EmployeeSelectionView>();
            CurrentView = view;
            StatusMessage = "Chọn nhân viên để xem thông tin";
        }

        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Event Handlers

        private void Timer_Tick(object? sender, EventArgs e)
        {
            CurrentTime = DateTime.Now;
        }

        private void OnDeviceConnectionStatusChanged(object? sender, ConnectionStatusChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DeviceConnectionStatus = e.Status;
                StatusMessage = e.Message;
            });
        }

        private void OnLoginSuccessful(object? sender, LoginSuccessfulEventArgs e)
        {
            CurrentUser = e.UserName;
            StatusMessage = $"Đăng nhập thành công. Chào mừng {e.UserName}!";
        }

        private void UpdateDeviceConnectionStatus()
        {
            DeviceConnectionStatus = _deviceService.IsConnected 
                ? ConnectionStatus.Connected 
                : ConnectionStatus.Disconnected;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    // Simple RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
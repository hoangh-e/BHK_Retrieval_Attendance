using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BHK.Retrieval.Attendance.WPF.ViewModels.Base;
using BHK.Retrieval.Attendance.WPF.Views.Pages;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private object _currentContent;
        private bool _isLoginVisible = true;
        private LoginViewModel _loginViewModel;

        public MainWindowViewModel()
        {
            InitializeCommands();
            InitializeViewModels();
            NavigateToLogin();
        }

        #region Properties

        public object CurrentContent
        {
            get => _currentContent;
            set => SetProperty(ref _currentContent, value);
        }

        public bool IsLoginVisible
        {
            get => _isLoginVisible;
            set => SetProperty(ref _isLoginVisible, value);
        }

        public LoginViewModel LoginViewModel
        {
            get => _loginViewModel;
            set => SetProperty(ref _loginViewModel, value);
        }

        #endregion

        #region Commands

        public ICommand NavigateToAttendanceCommand { get; private set; }
        public ICommand NavigateToEmployeeCommand { get; private set; }
        public ICommand NavigateToReportCommand { get; private set; }
        public ICommand NavigateToSettingsCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }

        #endregion

        #region Initialization

        private void InitializeCommands()
        {
            NavigateToAttendanceCommand = new RelayCommand(NavigateToAttendance);
            NavigateToEmployeeCommand = new RelayCommand(NavigateToEmployee);
            NavigateToReportCommand = new RelayCommand(NavigateToReport);
            NavigateToSettingsCommand = new RelayCommand(NavigateToSettings);
            LogoutCommand = new RelayCommand(Logout);
        }

        private void InitializeViewModels()
        {
            LoginViewModel = new LoginViewModel();
            LoginViewModel.LoginSuccessful += OnLoginSuccessful;
        }

        #endregion

        #region Event Handlers

        private void OnLoginSuccessful(object sender, EventArgs e)
        {
            IsLoginVisible = false;
            NavigateToDashboard();
        }

        #endregion

        #region Navigation Methods

        private void NavigateToLogin()
        {
            CurrentContent = new LoginView { DataContext = LoginViewModel };
            IsLoginVisible = true;
        }

        private void NavigateToDashboard()
        {
            CurrentContent = new DashboardView { DataContext = new DashboardViewModel() };
        }

        private void NavigateToAttendance(object parameter = null)
        {
            CurrentContent = new AttendanceListView { DataContext = new AttendanceListViewModel() };
        }

        private void NavigateToEmployee(object parameter = null)
        {
            CurrentContent = new EmployeeListView { DataContext = new EmployeeListViewModel() };
        }

        private void NavigateToReport(object parameter = null)
        {
            CurrentContent = new ReportGeneratorView { DataContext = new ReportGeneratorViewModel() };
        }

        private void NavigateToSettings(object parameter = null)
        {
            CurrentContent = new SettingsView { DataContext = new SettingsViewModel() };
        }

        private void Logout(object parameter = null)
        {
            // Clear user session
            CurrentUser = null;
            
            // Navigate back to login
            NavigateToLogin();
        }

        #endregion
    }

    // Event handler delegate
    public delegate void LoginSuccessfulEventHandler(object sender, EventArgs e);
}
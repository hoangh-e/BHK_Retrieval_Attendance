using System;
using System.Threading.Tasks;
using System.Windows.Input;
using BHK.Retrieval.Attendance.WPF.ViewModels.Base;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username;
        private string _password;
        private bool _isLoading;
        private string _errorMessage;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(async () => await LoginAsync(), CanLogin);
        }

        #region Properties

        public string Username
        {
            get => _username;
            set 
            { 
                SetProperty(ref _username, value);
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set 
            { 
                SetProperty(ref _password, value);
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        #endregion

        #region Commands

        public ICommand LoginCommand { get; }

        #endregion

        #region Events

        public event LoginSuccessfulEventHandler LoginSuccessful;

        protected virtual void OnLoginSuccessful()
        {
            LoginSuccessful?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Methods

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) && 
                   !string.IsNullOrWhiteSpace(Password) && 
                   !IsLoading;
        }

        private async Task LoginAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Simulate login process
                await Task.Delay(1000);

                // TODO: Implement actual authentication logic
                if (Username == "admin" && Password == "123456")
                {
                    // Login successful
                    OnLoginSuccessful();
                }
                else
                {
                    ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi đăng nhập: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }
}
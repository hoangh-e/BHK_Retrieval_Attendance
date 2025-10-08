using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BHK.Retrieval.Attendance.WPF.ViewModels.Base
{
    /// <summary>
    /// Base class cho tất cả ViewModels
    /// </summary>
    public abstract class BaseViewModel : ObservableObject
    {
        private bool _isBusy;
        private string _title;

        /// <summary>
        /// Trạng thái đang xử lý
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Tiêu đề của View
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}

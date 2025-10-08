using System;
using BHK.Retrieval.Attendance.WPF.ViewModels.Base;

namespace BHK.Retrieval.Attendance.WPF.Services.Interfaces
{
    /// <summary>
    /// Service interface cho navigation - ContentControl pattern
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigate đến ViewModel bằng generic type
        /// </summary>
        void NavigateTo<TViewModel>() where TViewModel : BaseViewModel;

        /// <summary>
        /// CurrentViewModel đang được hiển thị
        /// </summary>
        BaseViewModel CurrentViewModel { get; }

        /// <summary>
        /// Event khi navigation thay đổi
        /// </summary>
        event EventHandler<BaseViewModel> Navigated;
    }
}
using System.Windows;
using BHK.Retrieval.Attendance.WPF.ViewModels.Dialogs;

namespace BHK.Retrieval.Attendance.WPF.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for UpdateDialog.xaml
    /// </summary>
    public partial class UpdateDialog : Window
    {
        public UpdateDialog(UpdateDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.SetDialog(this);
        }
    }
}

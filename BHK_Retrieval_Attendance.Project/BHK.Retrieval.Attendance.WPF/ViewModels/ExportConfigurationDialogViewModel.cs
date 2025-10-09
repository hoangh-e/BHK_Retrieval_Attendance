using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BHK.Retrieval.Attendance.Core.DTOs.Requests;
using System.Windows.Input;
using System;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    public partial class ExportConfigurationDialogViewModel : ObservableObject
    {
        [ObservableProperty]
        private ExportFileType _selectedFileType = ExportFileType.Excel;

        [ObservableProperty]
        private string _fileName = $"attendance_{DateTime.Now:yyyy-MM-dd}.xlsx";

        [ObservableProperty]
        private int _recordCount;

        public ICommand ExportCommand { get; }
        public ICommand CancelCommand { get; }

        public ExportConfigurationDialogViewModel()
        {
            ExportCommand = new RelayCommand(OnExport);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private void OnExport()
        {
            // TODO: Logic xuất file
        }

        private void OnCancel()
        {
            // TODO: Đóng dialog
        }

        partial void OnSelectedFileTypeChanged(ExportFileType value)
        {
            // Tự động đổi extension
            var baseName = System.IO.Path.GetFileNameWithoutExtension(FileName);
            FileName = value switch
            {
                ExportFileType.Json => $"{baseName}.json",
                ExportFileType.Excel => $"{baseName}.xlsx",
                ExportFileType.Text => $"{baseName}.txt",
                ExportFileType.Csv => $"{baseName}.csv",
                _ => FileName
            };
        }
    }
}

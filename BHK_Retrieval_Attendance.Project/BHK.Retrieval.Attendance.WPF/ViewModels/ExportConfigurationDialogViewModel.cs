using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BHK.Retrieval.Attendance.Core.DTOs.Requests;
using System.Windows.Input;
using System;
using System.Windows;

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

        // ✅ Thêm property để lưu reference tới dialog window
        public Window? DialogWindow { get; set; }

        public ExportConfigurationDialogViewModel()
        {
            ExportCommand = new RelayCommand(OnExport);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private void OnExport()
        {
            // ✅ Đóng dialog với DialogResult = true
            if (DialogWindow != null)
            {
                DialogWindow.DialogResult = true;
                DialogWindow.Close();
            }
        }

        private void OnCancel()
        {
            // ✅ Đóng dialog với DialogResult = false
            if (DialogWindow != null)
            {
                DialogWindow.DialogResult = false;
                DialogWindow.Close();
            }
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

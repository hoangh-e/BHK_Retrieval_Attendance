using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;

namespace BHK.Retrieval.Attendance.WPF.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel cho dialog cấu hình xuất file
    /// </summary>
    public partial class ExportConfigurationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _selectedFileType = "Excel (.xlsx)";

        [ObservableProperty]
        private string _fileName = string.Empty;

        [ObservableProperty]
        private int _recordCount;

        private readonly List<AttendanceDisplayDto> _data;
        private readonly Window _dialog;

        public bool DialogResult { get; private set; }

        public ExportConfigurationViewModel(Window dialog, string fileName, int recordCount, List<AttendanceDisplayDto> data)
        {
            _dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
            _fileName = fileName;
            _recordCount = recordCount;
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
            _dialog.DialogResult = false;
            _dialog.Close();
        }

        [RelayCommand]
        private void Export()
        {
            try
            {
                // Xác định extension dựa vào loại file được chọn
                string extension = SelectedFileType switch
                {
                    "JSON (.json)" => ".json",
                    "Excel (.xlsx)" => ".xlsx",
                    "Text (.txt)" => ".txt",
                    "CSV (.csv)" => ".csv",
                    _ => ".xlsx"
                };

                // Mở SaveFileDialog
                var saveFileDialog = new SaveFileDialog
                {
                    FileName = Path.GetFileNameWithoutExtension(FileName) + extension,
                    Filter = SelectedFileType switch
                    {
                        "JSON (.json)" => "JSON Files (*.json)|*.json",
                        "Excel (.xlsx)" => "Excel Files (*.xlsx)|*.xlsx",
                        "Text (.txt)" => "Text Files (*.txt)|*.txt",
                        "CSV (.csv)" => "CSV Files (*.csv)|*.csv",
                        _ => "Excel Files (*.xlsx)|*.xlsx"
                    },
                    DefaultExt = extension
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    
                    // TODO: Thực hiện export dựa vào loại file
                    // Hiện tại chỉ tạo file trống để test
                    switch (extension)
                    {
                        case ".json":
                            ExportToJson(filePath);
                            break;
                        case ".xlsx":
                            ExportToExcel(filePath);
                            break;
                        case ".txt":
                            ExportToText(filePath);
                            break;
                        case ".csv":
                            ExportToCsv(filePath);
                            break;
                    }

                    MessageBox.Show($"Đã xuất {_recordCount} bản ghi vào file:\n{filePath}", 
                                    "Thành công", 
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Information);

                    DialogResult = true;
                    _dialog.DialogResult = true;
                    _dialog.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file: {ex.Message}", 
                                "Lỗi", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
            }
        }

        private void ExportToJson(string filePath)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(_data, new System.Text.Json.JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(filePath, json);
        }

        private void ExportToExcel(string filePath)
        {
            // TODO: Implement Excel export using EPPlus or similar
            // Tạm thời tạo file CSV với extension xlsx
            ExportToCsv(filePath);
        }

        private void ExportToText(string filePath)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("DIN\tNgày\tGiờ\tLoại");
            writer.WriteLine(new string('-', 60));
            foreach (var record in _data)
            {
                writer.WriteLine($"{record.DIN}\t{record.Date}\t{record.Time}\t{record.Type}");
            }
        }

        private void ExportToCsv(string filePath)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("DIN,Ngày,Giờ,Loại");
            foreach (var record in _data)
            {
                writer.WriteLine($"{record.DIN},{record.Date},{record.Time},{record.Type}");
            }
        }
    }
}

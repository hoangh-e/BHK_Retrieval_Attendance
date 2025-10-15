using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using BHK.Retrieval.Attendance.WPF.ViewModels;

namespace BHK.Retrieval.Attendance.WPF.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel cho dialog xuất file nhân viên
    /// </summary>
    public partial class ExportEmployeeViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _selectedFileType = "Excel (.xlsx)";

        [ObservableProperty]
        private string _fileName = string.Empty;

        [ObservableProperty]
        private int _recordCount;

        private readonly List<EmployeeDisplayModel> _data;
        private readonly Window _dialog;

        public bool DialogResult { get; private set; }

        public ExportEmployeeViewModel(Window dialog, string fileName, int recordCount, List<EmployeeDisplayModel> data)
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
                    
                    // Xuất file dựa vào loại file
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

                    MessageBox.Show($"Đã xuất {RecordCount} nhân viên vào file:\n{filePath}", 
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
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);
        }

        private void ExportToExcel(string filePath)
        {
            // Tạm thời export dạng CSV với extension xlsx
            ExportToCsv(filePath);
        }

        private void ExportToText(string filePath)
        {
            using var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8);
            writer.WriteLine("DANH SÁCH NHÂN VIÊN");
            writer.WriteLine($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            writer.WriteLine(new string('=', 80));
            writer.WriteLine();
            writer.WriteLine($"{"STT",-5} {"DIN",-15} {"Họ tên",-30} {"Phòng ban",-25}");
            writer.WriteLine(new string('-', 80));
            
            foreach (var employee in _data)
            {
                writer.WriteLine($"{employee.Index,-5} {employee.DIN,-15} {employee.UserName,-30} {employee.Department,-25}");
            }
            
            writer.WriteLine(new string('=', 80));
            writer.WriteLine($"Tổng số: {_data.Count} nhân viên");
        }

        private void ExportToCsv(string filePath)
        {
            using var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8);
            
            // Header với BOM để Excel đọc được tiếng Việt
            writer.WriteLine("STT,DIN,Họ tên,Phòng ban");
            
            foreach (var employee in _data)
            {
                // Escape dấu phẩy và dấu ngoặc kép
                var userName = EscapeCsvField(employee.UserName);
                var department = EscapeCsvField(employee.Department);
                
                writer.WriteLine($"{employee.Index},{employee.DIN},{userName},{department}");
            }
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            // Nếu có dấu phẩy, dấu ngoặc kép hoặc xuống dòng, bọc trong dấu ngoặc kép
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }
    }
}

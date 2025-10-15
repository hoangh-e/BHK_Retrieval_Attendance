using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;

namespace BHK.Retrieval.Attendance.WPF.Services.Implementations
{
    /// <summary>
    /// Service xử lý file Excel - Đọc/Ghi/Tạo table
    /// ✅ TÁI SỬ DỤNG cho tất cả thao tác Excel
    /// </summary>
    public class ExcelTableService : IExcelTableService
    {
        private readonly ILogger<ExcelTableService> _logger;

        public ExcelTableService(ILogger<ExcelTableService> logger)
        {
            _logger = logger;
        }

        public Task<bool> ValidateExcelFileAsync(string filePath)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(filePath))
                        return false;

                    // Kiểm tra extension
                    var ext = Path.GetExtension(filePath).ToLower();
                    if (ext != ".xlsx" && ext != ".xls")
                        return false;

                    // Nếu file chưa tồn tại, cho phép tạo mới
                    if (!File.Exists(filePath))
                    {
                        _logger.LogInformation($"Excel file does not exist, will be created: {filePath}");
                        return true; // Hợp lệ, sẽ tạo mới
                    }

                    // Nếu file đã tồn tại, kiểm tra có mở được không
                    using var workbook = new XLWorkbook(filePath);
                    _logger.LogInformation($"Excel file is valid: {filePath}");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Excel file validation failed: {filePath}");
                    return false;
                }
            });
        }

        public Task<List<string>> GetTableNamesAsync(string filePath)
        {
            return Task.Run(() =>
            {
                var tableNames = new List<string>();

                try
                {
                    if (!File.Exists(filePath))
                    {
                        _logger.LogWarning($"File not found: {filePath}");
                        return tableNames;
                    }

                    using var workbook = new XLWorkbook(filePath);
                    tableNames = workbook.Worksheets.Select(ws => ws.Name).ToList();
                    
                    _logger.LogInformation($"Found {tableNames.Count} worksheets in {filePath}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to get table names from: {filePath}");
                }

                return tableNames;
            });
        }

        public Task<bool> TableExistsAsync(string filePath, string tableName)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(filePath))
                        return false;

                    using var workbook = new XLWorkbook(filePath);
                    var exists = workbook.Worksheets.Any(ws => 
                        ws.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
                    
                    _logger.LogDebug($"Table '{tableName}' exists in {filePath}: {exists}");
                    return exists;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to check table existence: {tableName}");
                    return false;
                }
            });
        }

        public Task CreateAttendanceTableAsync(string filePath, string tableName)
        {
            return Task.Run(() =>
            {
                try
                {
                    XLWorkbook workbook;
                    
                    // Tạo hoặc mở file
                    if (File.Exists(filePath))
                    {
                        workbook = new XLWorkbook(filePath);
                    }
                    else
                    {
                        // Tạo folder nếu chưa có
                        var directory = Path.GetDirectoryName(filePath);
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                            _logger.LogInformation($"Created directory: {directory}");
                        }

                        workbook = new XLWorkbook();
                    }

                    using (workbook)
                    {
                        // Xóa worksheet cũ nếu có
                        var existing = workbook.Worksheets.FirstOrDefault(ws => 
                            ws.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
                        existing?.Delete();

                        // Tạo worksheet mới
                        var worksheet = workbook.Worksheets.Add(tableName);

                        // ✅ Tạo header cho Attendance: ID, Date, Time, Verify
                        worksheet.Cell(1, 1).Value = "ID";
                        worksheet.Cell(1, 2).Value = "Date";
                        worksheet.Cell(1, 3).Value = "Time";
                        worksheet.Cell(1, 4).Value = "Verify";

                        // ✅ Tạo Excel TABLE thực sự cho Attendance
                        var table = worksheet.Range(1, 1, 2, 4).CreateTable($"Table_{tableName}"); // Table với 1 row dữ liệu mẫu

                        // Format header
                        var headerRange = worksheet.Range(1, 1, 1, 4);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // Xóa row mẫu
                        worksheet.Row(2).Delete();

                        // Auto-fit columns
                        worksheet.Columns().AdjustToContents();

                        workbook.SaveAs(filePath);
                        _logger.LogInformation($"Created Attendance table '{tableName}' in {filePath}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to create Attendance table '{tableName}'");
                    throw;
                }
            });
        }

        public Task CreateEmployeeTableAsync(string filePath, string tableName)
        {
            return Task.Run(() =>
            {
                try
                {
                    XLWorkbook workbook;
                    
                    if (File.Exists(filePath))
                    {
                        workbook = new XLWorkbook(filePath);
                    }
                    else
                    {
                        var directory = Path.GetDirectoryName(filePath);
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                            _logger.LogInformation($"Created directory: {directory}");
                        }

                        workbook = new XLWorkbook();
                    }

                    using (workbook)
                    {
                        var existing = workbook.Worksheets.FirstOrDefault(ws => 
                            ws.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
                        existing?.Delete();

                        var worksheet = workbook.Worksheets.Add(tableName);

                        // ✅ Tạo header chi tiết cho Employee
                        worksheet.Cell(1, 1).Value = "ID";
                        worksheet.Cell(1, 2).Value = "Name";
                        worksheet.Cell(1, 3).Value = "IDNumber";
                        worksheet.Cell(1, 4).Value = "Department";
                        worksheet.Cell(1, 5).Value = "Sex";
                        worksheet.Cell(1, 6).Value = "Birthday";
                        worksheet.Cell(1, 7).Value = "Created";
                        worksheet.Cell(1, 8).Value = "Status";
                        worksheet.Cell(1, 9).Value = "Comment";
                        worksheet.Cell(1, 10).Value = "EnrollmentCount";

                        // ✅ Tạo Excel TABLE thực sự (không chỉ là worksheet)
                        var headerRange = worksheet.Range(1, 1, 1, 10);
                        var table = worksheet.Range(1, 1, 2, 10).CreateTable($"Table_{tableName}"); // Table với 1 row dữ liệu mẫu
                        
                        // Format header
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // Xóa row mẫu 
                        worksheet.Row(2).Delete();

                        worksheet.Columns().AdjustToContents();

                        workbook.SaveAs(filePath);
                        _logger.LogInformation($"Created Employee table '{tableName}' in {filePath}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to create Employee table '{tableName}'");
                    throw;
                }
            });
        }

        public Task<int> GetRecordCountAsync(string filePath, string tableName)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(filePath))
                        return 0;

                    using var workbook = new XLWorkbook(filePath);
                    var worksheet = workbook.Worksheets.FirstOrDefault(ws => 
                        ws.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));

                    if (worksheet == null)
                        return 0;

                    // Đếm số dòng có dữ liệu (trừ header)
                    var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
                    var count = Math.Max(0, lastRow - 1);

                    _logger.LogDebug($"Table '{tableName}' has {count} records");
                    return count;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to count records in table '{tableName}'");
                    return 0;
                }
            });
        }

        public Task ExportAttendanceDataAsync<T>(string filePath, string tableName, List<T> data)
        {
            return Task.Run(() =>
            {
                try
                {
                    // ✅ Mở hoặc tạo workbook
                    XLWorkbook workbook;
                    if (File.Exists(filePath))
                    {
                        workbook = new XLWorkbook(filePath);
                    }
                    else
                    {
                        workbook = new XLWorkbook();
                    }

                    using (workbook)
                    {
                        // ✅ BƯỚC 1: Tìm kiếm Excel Table thực sự theo tên
                        IXLTable? targetTable = null;
                        IXLWorksheet? targetWorksheet = null;

                        // Tìm Table trong tất cả các worksheets
                        foreach (var ws in workbook.Worksheets)
                        {
                            targetTable = ws.Tables.FirstOrDefault(t => 
                                t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
                            if (targetTable != null)
                            {
                                targetWorksheet = ws;
                                _logger.LogInformation($"Found existing Excel table '{tableName}' in worksheet '{ws.Name}'");
                                break;
                            }
                        }

                        // ✅ BƯỚC 2: Nếu không tìm thấy Table → Tạo mới trong sheet đầu tiên
                        if (targetTable == null)
                        {
                            _logger.LogInformation($"Table '{tableName}' not found, creating new Excel table");
                            
                            // Lấy sheet đầu tiên hoặc tạo mới nếu không có
                            targetWorksheet = workbook.Worksheets.FirstOrDefault() ?? workbook.Worksheets.Add("Sheet1");
                            
                            // Tạo header Attendance (4 cột) bắt đầu từ A1
                            targetWorksheet.Cell(1, 1).Value = "ID";
                            targetWorksheet.Cell(1, 2).Value = "Date";
                            targetWorksheet.Cell(1, 3).Value = "Time";
                            targetWorksheet.Cell(1, 4).Value = "Verify";

                            // ✅ Tạo Excel Table thực sự từ range A1:D1 (chỉ header)
                            var headerRange = targetWorksheet.Range("A1:D1");
                            targetTable = headerRange.CreateTable(tableName);
                            
                            // Thiết lập theme cho table như Excel thật
                            targetTable.Theme = XLTableTheme.TableStyleMedium2;
                            
                            _logger.LogInformation($"Created new Excel table '{tableName}' with theme in worksheet '{targetWorksheet.Name}'");
                        }

                        // ✅ BƯỚC 3: Đảm bảo có targetTable và targetWorksheet hợp lệ
                        if (targetTable == null || targetWorksheet == null)
                        {
                            throw new InvalidOperationException($"Failed to create or access Excel table '{tableName}'");
                        }

                        // ✅ BƯỚC 4: Clear dữ liệu cũ (giữ lại header)
                        if (targetTable.DataRange != null)
                        {
                            targetTable.DataRange.Clear();
                        }

                        // ✅ BƯỚC 5: Thêm dữ liệu mới vào table
                        if (data.Count > 0)
                        {
                            var startRow = 2; // Bắt đầu sau header

                            foreach (var item in data)
                            {
                                var type = typeof(T);
                                var id = type.GetProperty("ID")?.GetValue(item)?.ToString() ?? "";
                                var date = type.GetProperty("Date")?.GetValue(item)?.ToString() ?? "";
                                var time = type.GetProperty("Time")?.GetValue(item)?.ToString() ?? "";
                                var verify = type.GetProperty("Verify")?.GetValue(item)?.ToString() ?? "";

                                targetWorksheet.Cell(startRow, 1).Value = id;
                                targetWorksheet.Cell(startRow, 2).Value = date;
                                targetWorksheet.Cell(startRow, 3).Value = time;
                                targetWorksheet.Cell(startRow, 4).Value = verify;

                                startRow++;
                            }

                            // ✅ BƯỚC 6: Resize Excel Table để bao gồm tất cả dữ liệu
                            var newTableRange = targetWorksheet.Range(1, 1, startRow - 1, 4); // Từ A1 đến D(lastRow)
                            targetTable.Resize(newTableRange);
                            
                            _logger.LogInformation($"Resized Excel table '{tableName}' to include {data.Count} records");
                        }

                        // Auto-fit columns
                        targetWorksheet.Columns().AdjustToContents();
                        
                        // Lưu file
                        workbook.SaveAs(filePath);
                        
                        _logger.LogInformation($"Successfully exported {data.Count} attendance records to Excel table '{tableName}'");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to export attendance data to Excel table '{tableName}'");
                    throw;
                }
            });
        }

        public Task ExportEmployeeDataAsync<T>(string filePath, string tableName, List<T> data)
        {
            return Task.Run(() =>
            {
                try
                {
                    // ✅ Mở hoặc tạo workbook
                    XLWorkbook workbook;
                    if (File.Exists(filePath))
                    {
                        workbook = new XLWorkbook(filePath);
                    }
                    else
                    {
                        workbook = new XLWorkbook();
                    }

                    using (workbook)
                    {
                        // ✅ BƯỚC 1: Tìm kiếm Excel Table thực sự theo tên
                        IXLTable? targetTable = null;
                        IXLWorksheet? targetWorksheet = null;

                        // Tìm Table trong tất cả các worksheets
                        foreach (var ws in workbook.Worksheets)
                        {
                            targetTable = ws.Tables.FirstOrDefault(t => 
                                t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
                            if (targetTable != null)
                            {
                                targetWorksheet = ws;
                                _logger.LogInformation($"Found existing Excel table '{tableName}' in worksheet '{ws.Name}'");
                                break;
                            }
                        }

                        // ✅ BƯỚC 2: Nếu không tìm thấy Table → Tạo mới trong sheet đầu tiên
                        if (targetTable == null)
                        {
                            _logger.LogInformation($"Table '{tableName}' not found, creating new Excel table");
                            
                            // Lấy sheet đầu tiên hoặc tạo mới nếu không có
                            targetWorksheet = workbook.Worksheets.FirstOrDefault() ?? workbook.Worksheets.Add("Sheet1");
                            
                            // Tạo header Employee (10 cột) bắt đầu từ A1
                            targetWorksheet.Cell(1, 1).Value = "ID";
                            targetWorksheet.Cell(1, 2).Value = "Name";
                            targetWorksheet.Cell(1, 3).Value = "IDNumber";
                            targetWorksheet.Cell(1, 4).Value = "Department";
                            targetWorksheet.Cell(1, 5).Value = "Sex";
                            targetWorksheet.Cell(1, 6).Value = "Birthday";
                            targetWorksheet.Cell(1, 7).Value = "Created";
                            targetWorksheet.Cell(1, 8).Value = "Status";
                            targetWorksheet.Cell(1, 9).Value = "Comment";
                            targetWorksheet.Cell(1, 10).Value = "EnrollmentCount";

                            // ✅ Tạo Excel Table thực sự từ range A1:J1 (chỉ header)
                            var headerRange = targetWorksheet.Range("A1:J1");
                            targetTable = headerRange.CreateTable(tableName);
                            
                            // Thiết lập theme cho table như Excel thật
                            targetTable.Theme = XLTableTheme.TableStyleMedium9;
                            
                            _logger.LogInformation($"Created new Excel table '{tableName}' with theme in worksheet '{targetWorksheet.Name}'");
                        }

                        // ✅ BƯỚC 3: Clear dữ liệu cũ (giữ lại header)
                        if (targetTable.DataRange != null)
                        {
                            targetTable.DataRange.Clear();
                        }

                        // ✅ BƯỚC 4: Đảm bảo có targetTable và targetWorksheet hợp lệ
                        if (targetTable == null || targetWorksheet == null)
                        {
                            throw new InvalidOperationException($"Failed to create or access Excel table '{tableName}'");
                        }

                        // ✅ BƯỚC 5: Thêm dữ liệu mới vào table
                        if (data.Count > 0)
                        {
                            var startRow = 2; // Bắt đầu sau header

                            foreach (var item in data)
                            {
                                var type = typeof(T);
                                var id = type.GetProperty("ID")?.GetValue(item)?.ToString() ?? "";
                                var name = type.GetProperty("Name")?.GetValue(item)?.ToString() ?? "";
                                var idNumber = type.GetProperty("IDNumber")?.GetValue(item)?.ToString() ?? "";
                                var department = type.GetProperty("Department")?.GetValue(item)?.ToString() ?? "";
                                var sex = type.GetProperty("Sex")?.GetValue(item)?.ToString() ?? "";
                                var birthday = type.GetProperty("Birthday")?.GetValue(item)?.ToString() ?? "";
                                var created = type.GetProperty("Created")?.GetValue(item)?.ToString() ?? "";
                                var status = type.GetProperty("Status")?.GetValue(item)?.ToString() ?? "";
                                var comment = type.GetProperty("Comment")?.GetValue(item)?.ToString() ?? "";
                                var enrollmentCount = type.GetProperty("EnrollmentCount")?.GetValue(item)?.ToString() ?? "0";

                                targetWorksheet.Cell(startRow, 1).Value = id;
                                targetWorksheet.Cell(startRow, 2).Value = name;
                                targetWorksheet.Cell(startRow, 3).Value = idNumber;
                                targetWorksheet.Cell(startRow, 4).Value = department;
                                targetWorksheet.Cell(startRow, 5).Value = sex;
                                targetWorksheet.Cell(startRow, 6).Value = birthday;
                                targetWorksheet.Cell(startRow, 7).Value = created;
                                targetWorksheet.Cell(startRow, 8).Value = status;
                                targetWorksheet.Cell(startRow, 9).Value = comment;
                                targetWorksheet.Cell(startRow, 10).Value = enrollmentCount;

                                startRow++;
                            }

                            // ✅ BƯỚC 6: Resize Excel Table để bao gồm tất cả dữ liệu
                            var newTableRange = targetWorksheet.Range(1, 1, startRow - 1, 10); // Từ A1 đến J(lastRow)
                            targetTable.Resize(newTableRange);
                            
                            _logger.LogInformation($"Resized Excel table '{tableName}' to include {data.Count} records");
                        }

                        // Auto-fit columns
                        targetWorksheet.Columns().AdjustToContents();
                        
                        // Lưu file
                        workbook.SaveAs(filePath);
                        
                        _logger.LogInformation($"Successfully exported {data.Count} employee records to Excel table '{tableName}'");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to export employee data to Excel table '{tableName}'");
                    throw;
                }
            });
        }
    }
}

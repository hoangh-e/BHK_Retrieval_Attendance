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
                    
                    // ✅ Lấy tên Excel Tables và hiển thị sheet chứa table đó
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        foreach (var table in worksheet.Tables)
                        {
                            // Format: "TableName (Sheet: SheetName)"
                            tableNames.Add($"{table.Name} (Sheet: {worksheet.Name})");
                        }
                    }
                    
                    _logger.LogInformation($"Found {tableNames.Count} Excel tables in {filePath}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to get Excel table names from: {filePath}");
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
                    
                    // ✅ Kiểm tra Excel Table - hỗ trợ cả tên gốc và format có sheet name
                    string actualTableName = ExtractActualTableName(tableName);
                    
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        var exists = worksheet.Tables.Any(t => 
                            t.Name.Equals(actualTableName, StringComparison.OrdinalIgnoreCase));
                        if (exists)
                        {
                            _logger.LogDebug($"Excel table '{actualTableName}' found in worksheet '{worksheet.Name}'");
                            return true;
                        }
                    }
                    
                    _logger.LogDebug($"Excel table '{tableName}' not found in {filePath}");
                    return false;
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

                        // ✅ Thêm 1 row dữ liệu mẫu (cần thiết để tạo table)
                        worksheet.Cell(2, 1).Value = "Sample";
                        worksheet.Cell(2, 2).Value = DateTime.Today.ToString("yyyy-MM-dd");
                        worksheet.Cell(2, 3).Value = DateTime.Now.ToString("HH:mm:ss");
                        worksheet.Cell(2, 4).Value = "1";

                        // ✅ Tạo Excel TABLE thực sự cho Attendance (phải có ít nhất 1 row data)
                        // Chỉ sử dụng tên table thuần túy, không có prefix
                        string actualTableName = ExtractActualTableName(tableName);
                        var table = worksheet.Range(1, 1, 2, 4).CreateTable(actualTableName);

                        // Format header
                        var headerRange = worksheet.Range(1, 1, 1, 4);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // ✅ Xóa row mẫu sau khi tạo table (để table trống nhưng vẫn có structure)
                        table.DataRange.Delete(XLShiftDeletedCells.ShiftCellsUp);

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

                        // ✅ Thêm 1 row dữ liệu mẫu (cần thiết để tạo table)
                        worksheet.Cell(2, 1).Value = "Sample";
                        worksheet.Cell(2, 2).Value = "Sample Employee";
                        worksheet.Cell(2, 3).Value = "000000";
                        worksheet.Cell(2, 4).Value = "IT";
                        worksheet.Cell(2, 5).Value = "M";
                        worksheet.Cell(2, 6).Value = DateTime.Today.ToString("yyyy-MM-dd");
                        worksheet.Cell(2, 7).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        worksheet.Cell(2, 8).Value = "Active";
                        worksheet.Cell(2, 9).Value = "Sample data";
                        worksheet.Cell(2, 10).Value = "0";

                        // ✅ Tạo Excel TABLE với header và 1 row mẫu (bắt buộc cho ClosedXML)
                        // Chỉ sử dụng tên table thuần túy, không có prefix
                        string actualTableName = ExtractActualTableName(tableName);
                        var table = worksheet.Range(1, 1, 2, 10).CreateTable(actualTableName);
                        
                        // Format header
                        var headerRange = worksheet.Range(1, 1, 1, 10);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // ✅ Giữ lại row mẫu để table không bị rỗng (tránh EmptyTableException)
                        // Row mẫu sẽ được thay thế khi export data thật

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
                    
                    // ✅ Tìm Excel Table - hỗ trợ cả tên gốc và format có sheet name
                    string actualTableName = ExtractActualTableName(tableName);
                    
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        var table = worksheet.Tables.FirstOrDefault(t => 
                            t.Name.Equals(actualTableName, StringComparison.OrdinalIgnoreCase));
                        
                        if (table != null)
                        {
                            // Đếm số rows trong Excel Table (không tính header)
                            var count = table.DataRange?.RowCount() ?? 0;
                            
                            // ✅ Kiểm tra nếu chỉ có 1 row và là sample data → trả về 0
                            if (count == 1 && table.DataRange != null)
                            {
                                var firstRowFirstCell = table.DataRange.FirstCell()?.Value.ToString();
                                if (firstRowFirstCell == "Sample")
                                {
                                    _logger.LogDebug($"Excel table '{actualTableName}' contains only sample data");
                                    return 0; // Coi như table trống
                                }
                            }
                            
                            _logger.LogDebug($"Excel table '{actualTableName}' has {count} data records");
                            return count;
                        }
                    }

                    _logger.LogDebug($"Excel table '{tableName}' not found in {filePath}");
                    return 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to count records in Excel table '{tableName}'");
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
                            
                            // ✅ Kiểm tra xem range A1:D1 đã thuộc table nào khác chưa
                            var headerRange = targetWorksheet.Range("A1:D1");
                            var existingTable = targetWorksheet.Tables.FirstOrDefault(t => 
                                t.RangeAddress.Intersects(headerRange.RangeAddress));
                            
                            if (existingTable != null)
                            {
                                _logger.LogInformation($"Range A1:D1 is already occupied by table '{existingTable.Name}', using that table instead");
                                targetTable = existingTable;
                            }
                            else
                            {
                                // Tạo header Attendance (4 cột) bắt đầu từ A1
                                targetWorksheet.Cell(1, 1).Value = "ID";
                                targetWorksheet.Cell(1, 2).Value = "Date";
                                targetWorksheet.Cell(1, 3).Value = "Time";
                                targetWorksheet.Cell(1, 4).Value = "Verify";

                                // ✅ Tạo Excel Table thực sự từ range A1:D1 (chỉ header)
                                targetTable = headerRange.CreateTable(tableName);
                                
                                // Thiết lập theme cho table như Excel thật
                                targetTable.Theme = XLTableTheme.TableStyleMedium2;
                            }
                            
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
            return ExportEmployeeDataAsync(filePath, tableName, data, null);
        }

        public Task ExportEmployeeDataAsync<T>(string filePath, string tableName, List<T> data, 
            Action<int, int, string>? progressCallback)
        {
            return Task.Run(() =>
            {
                try
                {
                    progressCallback?.Invoke(0, data.Count, "Đang mở file Excel...");
                    
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
                        progressCallback?.Invoke(0, data.Count, "Đang tìm kiếm Excel table...");
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
                            
                            // ✅ Kiểm tra xem range A1:J1 đã thuộc table nào khác chưa
                            var headerRange = targetWorksheet.Range("A1:J1");
                            var existingTable = targetWorksheet.Tables.FirstOrDefault(t => 
                                t.RangeAddress.Intersects(headerRange.RangeAddress));
                            
                            if (existingTable != null)
                            {
                                _logger.LogInformation($"Range A1:J1 is already occupied by table '{existingTable.Name}', using that table instead");
                                targetTable = existingTable;
                            }
                            else
                            {
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
                                targetTable = headerRange.CreateTable(tableName);
                                
                                // Thiết lập theme cho table như Excel thật
                                targetTable.Theme = XLTableTheme.TableStyleMedium9;
                                
                                _logger.LogInformation($"Created new Excel table '{tableName}' with theme in worksheet '{targetWorksheet.Name}'");
                            }
                        }

                        // ✅ BƯỚC 3: Clear dữ liệu cũ (giữ lại header)
                        progressCallback?.Invoke(0, data.Count, "Đang xóa dữ liệu cũ...");
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
                            var currentIndex = 0;

                            foreach (var item in data)
                            {
                                // ✅ Cập nhật progress cho từng nhân viên
                                currentIndex++;
                                progressCallback?.Invoke(currentIndex, data.Count, 
                                    $"Đang chuẩn bị dữ liệu chi tiết của {currentIndex}/{data.Count} nhân viên...");

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
                                var enrollmentCount = type.GetProperty("EnrollmentCount")?.GetValue(item)?.ToString() ?? "";

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

                            progressCallback?.Invoke(data.Count, data.Count, "Đang hoàn thiện Excel table...");
                            
                            // ✅ BƯỚC 6: Resize Excel Table để bao gồm tất cả dữ liệu
                            var newTableRange = targetWorksheet.Range(1, 1, startRow - 1, 10); // Từ A1 đến J(lastRow)
                            targetTable.Resize(newTableRange);
                            
                            _logger.LogInformation($"Resized Excel table '{tableName}' to include {data.Count} records");
                        }

                        progressCallback?.Invoke(data.Count, data.Count, "Đang lưu file...");
                        
                        // Auto-fit columns
                        targetWorksheet.Columns().AdjustToContents();
                        
                        // Lưu file
                        workbook.SaveAs(filePath);
                        
                        progressCallback?.Invoke(data.Count, data.Count, $"✅ Hoàn thành! Đã xuất {data.Count} nhân viên");
                        
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

        public Task<List<string>> GetTableColumnsAsync(string filePath, string tableName)
        {
            return Task.Run(() =>
            {
                var columns = new List<string>();
                try
                {
                    if (!File.Exists(filePath))
                        return columns;

                    using var workbook = new XLWorkbook(filePath);
                    string actualTableName = ExtractActualTableName(tableName);
                    
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        var table = worksheet.Tables.FirstOrDefault(t => 
                            t.Name.Equals(actualTableName, StringComparison.OrdinalIgnoreCase));
                        
                        if (table != null)
                        {
                            // Lấy header columns từ table
                            var headerRow = table.HeadersRow();
                            if (headerRow != null)
                            {
                                var lastColumn = headerRow.LastCellUsed()?.Address.ColumnNumber ?? 0;
                                for (int col = 1; col <= lastColumn; col++)
                                {
                                    var cellValue = headerRow.Cell(col).GetString();
                                    if (!string.IsNullOrWhiteSpace(cellValue))
                                    {
                                        columns.Add(cellValue);
                                    }
                                }
                            }
                            break;
                        }
                    }
                    
                    _logger.LogDebug($"Found {columns.Count} columns in table '{actualTableName}': {string.Join(", ", columns)}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to get columns for table '{tableName}'");
                }
                
                return columns;
            });
        }

        public Task<bool> ValidateTableColumnsAsync(string filePath, string tableName, string tableType)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var actualColumns = await GetTableColumnsAsync(filePath, tableName);
                    var expectedColumns = GetExpectedColumns(tableType);
                    
                    // Kiểm tra số lượng cột và tên cột có khớp không
                    if (actualColumns.Count != expectedColumns.Count)
                    {
                        _logger.LogWarning($"Column count mismatch. Expected: {expectedColumns.Count}, Actual: {actualColumns.Count}");
                        return false;
                    }
                    
                    for (int i = 0; i < expectedColumns.Count; i++)
                    {
                        if (!actualColumns[i].Equals(expectedColumns[i], StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogWarning($"Column mismatch at index {i}. Expected: '{expectedColumns[i]}', Actual: '{actualColumns[i]}'");
                            return false;
                        }
                    }
                    
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to validate table columns for '{tableName}'");
                    return false;
                }
            });
        }

        public Task RefactorTableColumnsAsync(string filePath, string tableName, string tableType)
        {
            return Task.Run(() =>
            {
                try
                {
                    using var workbook = new XLWorkbook(filePath);
                    string actualTableName = ExtractActualTableName(tableName);
                    
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        var table = worksheet.Tables.FirstOrDefault(t => 
                            t.Name.Equals(actualTableName, StringComparison.OrdinalIgnoreCase));
                        
                        if (table != null)
                        {
                            var expectedColumns = GetExpectedColumns(tableType);
                            var currentRange = table.RangeAddress;
                            var currentColumnCount = currentRange.LastAddress.ColumnNumber - currentRange.FirstAddress.ColumnNumber + 1;
                            
                            _logger.LogInformation($"Refactoring table '{actualTableName}' from {currentColumnCount} to {expectedColumns.Count} columns");
                            
                            // ✅ Sử dụng phương pháp resize thay vì delete/recreate
                            if (TryResizeTableInPlace(table, expectedColumns, tableType))
                            {
                                _logger.LogInformation($"Successfully resized table '{actualTableName}' in-place");
                            }
                            else
                            {
                                _logger.LogInformation($"In-place resize failed, using delete/recreate method for table '{actualTableName}'");
                                RefactorTableByRecreation(table, worksheet, expectedColumns, tableType, actualTableName);
                            }
                            
                            workbook.Save();
                            _logger.LogInformation($"Refactored table '{actualTableName}' from {currentColumnCount} to {expectedColumns.Count} columns successfully");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to refactor table columns for '{tableName}': {ex.Message}");
                    throw;
                }
            });
        }

        /// <summary>
        /// Thử resize table tại chỗ mà không cần xóa/tạo lại
        /// </summary>
        private bool TryResizeTableInPlace(IXLTable table, List<string> expectedColumns, string tableType)
        {
            try
            {
                var currentRange = table.RangeAddress;
                var worksheet = table.Worksheet;
                var startRow = currentRange.FirstAddress.RowNumber;
                var startCol = currentRange.FirstAddress.ColumnNumber;
                var currentColumnCount = currentRange.LastAddress.ColumnNumber - currentRange.FirstAddress.ColumnNumber + 1;
                var dataRowCount = table.DataRange?.RowCount() ?? 0;
                
                // ✅ BƯỚC 1: Cập nhật headers
                for (int i = 0; i < expectedColumns.Count; i++)
                {
                    var cell = worksheet.Cell(startRow, startCol + i);
                    cell.Value = expectedColumns[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = tableType == "Employee" ? XLColor.LightGreen : XLColor.LightBlue;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
                
                // ✅ BƯỚC 2: Xử lý cột thừa (nếu thu hẹp)
                if (expectedColumns.Count < currentColumnCount)
                {
                    // Clear các cột thừa
                    for (int col = startCol + expectedColumns.Count; col <= currentRange.LastAddress.ColumnNumber; col++)
                    {
                        var columnRange = worksheet.Range(startRow, col, currentRange.LastAddress.RowNumber, col);
                        columnRange.Clear();
                    }
                }
                
                // ✅ BƯỚC 3: Xử lý cột thiếu (nếu mở rộng)
                if (expectedColumns.Count > currentColumnCount)
                {
                    // Các header đã được thêm ở bước 1
                    // Dữ liệu cho cột mới sẽ để trống (Excel sẽ tự động mở rộng)
                }
                
                // ✅ BƯỚC 4: Resize table range
                var newEndCol = startCol + expectedColumns.Count - 1;
                var newEndRow = Math.Max(startRow, startRow + dataRowCount); // Ít nhất có header
                
                // ClosedXML có thể resize table bằng cách thay đổi RangeAddress
                var newRange = worksheet.Range(startRow, startCol, newEndRow, newEndCol);
                
                // ✅ BƯỚC 5: Cố gắng resize table (có thể fail với một số phiên bản ClosedXML)
                try
                {
                    // Một số phương pháp resize có thể dùng:
                    // Method 1: Thay đổi trực tiếp (có thể không work)
                    table.Resize(newRange);
                    
                    // ✅ BƯỚC 6: Thiết lập theme lại
                    table.Theme = tableType == "Employee" ? XLTableTheme.TableStyleMedium9 : XLTableTheme.TableStyleMedium2;
                    
                    // ✅ BƯỚC 7: Điều chỉnh độ rộng cột
                    worksheet.Columns().AdjustToContents();
                    
                    return true;
                }
                catch (Exception resizeEx)
                {
                    _logger.LogWarning($"Table resize failed: {resizeEx.Message}. Will use delete/recreate method.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"In-place resize failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Phương pháp dự phòng: Xóa và tạo lại table (phương pháp cũ)
        /// </summary>
        private void RefactorTableByRecreation(IXLTable table, IXLWorksheet worksheet, List<string> expectedColumns, string tableType, string actualTableName)
        {
            var currentRange = table.RangeAddress;
            var headerRow = table.HeadersRow();
            
            if (headerRow != null)
            {
                // ✅ BƯỚC 1: Lưu dữ liệu hiện tại (trừ header)
                List<List<object>> existingData = new List<List<object>>();
                if (table.DataRange != null)
                {
                    foreach (var row in table.DataRange.Rows())
                    {
                        var rowData = new List<object>();
                        var maxCol = Math.Min(row.LastCellUsed()?.Address.ColumnNumber ?? 0, expectedColumns.Count);
                        for (int col = 1; col <= maxCol; col++)
                        {
                            rowData.Add(row.Cell(col).Value);
                        }
                        existingData.Add(rowData);
                    }
                }
                
                // ✅ BƯỚC 2: Xóa table cũ
                var startRow = currentRange.FirstAddress.RowNumber;
                var startCol = currentRange.FirstAddress.ColumnNumber;
                table.Delete(XLShiftDeletedCells.ShiftCellsUp);
                
                // ✅ BƯỚC 3: Tạo header mới với đúng số cột
                for (int i = 0; i < expectedColumns.Count; i++)
                {
                    var cell = worksheet.Cell(startRow, startCol + i);
                    cell.Value = expectedColumns[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = tableType == "Employee" ? XLColor.LightGreen : XLColor.LightBlue;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
                
                // ✅ BƯỚC 4: Tạo table mới với range đúng kích thước
                var newEndColumn = startCol + expectedColumns.Count - 1;
                var newEndRow = startRow + Math.Max(0, existingData.Count); // Ít nhất 1 row cho header
                
                var newRange = worksheet.Range(startRow, startCol, newEndRow, newEndColumn);
                var newTable = newRange.CreateTable(actualTableName);
                
                // ✅ BƯỚC 5: Khôi phục dữ liệu với số cột mới
                if (existingData.Count > 0)
                {
                    for (int row = 0; row < existingData.Count; row++)
                    {
                        var rowData = existingData[row];
                        for (int col = 0; col < expectedColumns.Count; col++)
                        {
                            var cellValue = col < rowData.Count ? rowData[col]?.ToString() ?? "" : "";
                            worksheet.Cell(startRow + 1 + row, startCol + col).Value = cellValue;
                        }
                    }
                }
                
                // ✅ BƯỚC 6: Thiết lập theme cho table
                newTable.Theme = tableType == "Employee" ? XLTableTheme.TableStyleMedium9 : XLTableTheme.TableStyleMedium2;
                
                // ✅ BƯỚC 7: Điều chỉnh độ rộng cột
                worksheet.Columns().AdjustToContents();
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Trích xuất tên table thực từ display name có format "TableName (Sheet: SheetName)"
        /// </summary>
        private string ExtractActualTableName(string displayTableName)
        {
            if (string.IsNullOrEmpty(displayTableName))
                return displayTableName;

            // Nếu có format "TableName (Sheet: SheetName)", lấy phần tên table
            if (displayTableName.Contains(" (Sheet: "))
            {
                return displayTableName.Split(" (Sheet: ")[0];
            }

            return displayTableName;
        }

        /// <summary>
        /// Lấy danh sách cột mong đợi theo loại table
        /// </summary>
        private List<string> GetExpectedColumns(string tableType)
        {
            return tableType.ToLower() switch
            {
                "employee" => new List<string> { "ID", "Name", "IDNumber", "Department", "Sex", "Birthday", "Created", "Status", "Comment", "EnrollmentCount" },
                "attendance" => new List<string> { "ID", "Date", "Time", "Verify" },
                _ => new List<string>()
            };
        }

        #endregion
    }
}

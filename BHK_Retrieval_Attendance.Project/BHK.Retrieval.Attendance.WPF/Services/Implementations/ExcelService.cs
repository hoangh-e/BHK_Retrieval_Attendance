using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;

namespace BHK.Retrieval.Attendance.WPF.Services.Implementations;

public class ExcelService : IExcelService
{
    private readonly ILogger<ExcelService> _logger;

    public ExcelService(ILogger<ExcelService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> ValidateExcelFileAsync(string filePath)
    {
        return await Task.Run(() =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return false;

                if (!File.Exists(filePath))
                    return false;

                var extension = Path.GetExtension(filePath).ToLower();
                if (extension != ".xlsx" && extension != ".xls")
                    return false;

                // Try to open the file
                using var workbook = new XLWorkbook(filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Excel file: {FilePath}", filePath);
                return false;
            }
        });
    }

    public async Task<List<string>> GetTableNamesAsync(string filePath)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var workbook = new XLWorkbook(filePath);
                var tableNames = new List<string>();

                foreach (var worksheet in workbook.Worksheets)
                {
                    // Lấy tất cả tables trong worksheet
                    tableNames.AddRange(worksheet.Tables.Select(t => t.Name));
                }

                // Nếu không có table, trả về tên các worksheet
                if (tableNames.Count == 0)
                {
                    tableNames.AddRange(workbook.Worksheets.Select(w => w.Name));
                }

                return tableNames;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting table names from: {FilePath}", filePath);
                return new List<string>();
            }
        });
    }

    public async Task<bool> TableExistsAsync(string filePath, string tableName)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var workbook = new XLWorkbook(filePath);
                
                // Kiểm tra trong tables
                foreach (var worksheet in workbook.Worksheets)
                {
                    if (worksheet.Tables.Any(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase)))
                        return true;
                }

                // Kiểm tra worksheet name
                return workbook.Worksheets.Any(w => w.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking table existence: {TableName} in {FilePath}", tableName, filePath);
                return false;
            }
        });
    }

    public async Task<bool> CreateAttendanceTableAsync(string filePath, string tableName)
    {
        return await Task.Run(() =>
        {
            try
            {
                XLWorkbook workbook;
                bool isNewFile = !File.Exists(filePath);

                if (isNewFile)
                {
                    workbook = new XLWorkbook();
                }
                else
                {
                    workbook = new XLWorkbook(filePath);
                }

                using (workbook)
                {
                    // Tạo worksheet mới
                    var worksheet = workbook.Worksheets.Add(tableName);

                    // Tạo header
                    worksheet.Cell(1, 1).Value = "ID";
                    worksheet.Cell(1, 2).Value = "Date";
                    worksheet.Cell(1, 3).Value = "Time";
                    worksheet.Cell(1, 4).Value = "Verify";

                    // Format header
                    var headerRange = worksheet.Range(1, 1, 1, 4);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // Tạo table
                    var dataRange = worksheet.Range(1, 1, 2, 4); // Include 1 empty row for table
                    var table = dataRange.CreateTable(tableName);

                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();

                    workbook.SaveAs(filePath);
                    _logger.LogInformation("Created attendance table: {TableName} in {FilePath}", tableName, filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating attendance table: {TableName} in {FilePath}", tableName, filePath);
                return false;
            }
        });
    }

    public async Task<bool> CreateEmployeeTableAsync(string filePath, string tableName)
    {
        return await Task.Run(() =>
        {
            try
            {
                XLWorkbook workbook;
                bool isNewFile = !File.Exists(filePath);

                if (isNewFile)
                {
                    workbook = new XLWorkbook();
                }
                else
                {
                    workbook = new XLWorkbook(filePath);
                }

                using (workbook)
                {
                    var worksheet = workbook.Worksheets.Add(tableName);

                    // Tạo header
                    worksheet.Cell(1, 1).Value = "ID";
                    worksheet.Cell(1, 2).Value = "Name";
                    worksheet.Cell(1, 3).Value = "Created";
                    worksheet.Cell(1, 4).Value = "Status";

                    // Format header
                    var headerRange = worksheet.Range(1, 1, 1, 4);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                    headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // Tạo table
                    var dataRange = worksheet.Range(1, 1, 2, 4);
                    var table = dataRange.CreateTable(tableName);

                    worksheet.Columns().AdjustToContents();

                    workbook.SaveAs(filePath);
                    _logger.LogInformation("Created employee table: {TableName} in {FilePath}", tableName, filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee table: {TableName} in {FilePath}", tableName, filePath);
                return false;
            }
        });
    }

    public async Task<bool> ExportAttendanceDataAsync(
        string filePath, 
        string tableName, 
        List<AttendanceDisplayDto> data)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheets.FirstOrDefault(w => w.Name == tableName);

                if (worksheet == null)
                {
                    _logger.LogWarning("Worksheet {TableName} not found", tableName);
                    return false;
                }

                // Tìm table
                var table = worksheet.Tables.FirstOrDefault(t => t.Name == tableName);
                int startRow = table != null ? table.DataRange.FirstRow().RowNumber() : 2;

                // Clear existing data (except header)
                if (startRow > 1)
                {
                    worksheet.Rows(startRow, worksheet.LastRowUsed()?.RowNumber() ?? startRow).Delete();
                }

                // Ghi dữ liệu mới
                int currentRow = 2; // Bắt đầu từ row 2 (row 1 là header)
                foreach (var record in data)
                {
                    worksheet.Cell(currentRow, 1).Value = record.DN;
                    worksheet.Cell(currentRow, 2).Value = record.DIN;
                    worksheet.Cell(currentRow, 3).Value = record.Date;
                    worksheet.Cell(currentRow, 4).Value = record.Time;
                    worksheet.Cell(currentRow, 5).Value = record.Type;
                    worksheet.Cell(currentRow, 6).Value = record.Verify;
                    worksheet.Cell(currentRow, 7).Value = record.Action;
                    worksheet.Cell(currentRow, 8).Value = record.Remark;
                    currentRow++;
                }

                // Update table range nếu có table
                if (table != null)
                {
                    var newRange = worksheet.Range(1, 1, currentRow - 1, 8);
                    table.Resize(newRange);
                }

                worksheet.Columns().AdjustToContents();
                workbook.Save();

                _logger.LogInformation("Exported {Count} attendance records to {FilePath}", data.Count, filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting attendance data to {FilePath}", filePath);
                return false;
            }
        });
    }

    public async Task<bool> ExportEmployeeDataAsync(
        string filePath, 
        string tableName, 
        List<EmployeeDto> data)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheets.FirstOrDefault(w => w.Name == tableName);

                if (worksheet == null)
                {
                    _logger.LogWarning("Worksheet {TableName} not found", tableName);
                    return false;
                }

                // Đọc dữ liệu hiện có để so sánh
                var existingData = new Dictionary<string, (int Row, DateTime Created, string Status)>();
                int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

                for (int row = 2; row <= lastRow; row++)
                {
                    var id = worksheet.Cell(row, 1).GetString();
                    var created = worksheet.Cell(row, 3).GetDateTime();
                    var status = worksheet.Cell(row, 4).GetString();

                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        existingData[id] = (row, created, status);
                    }
                }

                // Cập nhật hoặc thêm mới
                int currentRow = 2;
                foreach (var employee in data)
                {
                    var empId = employee.IDNumber;

                    if (existingData.ContainsKey(empId))
                    {
                        // Cập nhật dòng hiện có
                        var existingRow = existingData[empId].Row;
                        worksheet.Cell(existingRow, 2).Value = employee.UserName;
                        // Giữ nguyên Created date
                        worksheet.Cell(existingRow, 4).Value = employee.Enable ? "Active" : "Inactive";
                        existingData.Remove(empId); // Remove để tracking những ID đã xử lý
                    }
                    else
                    {
                        // Thêm dòng mới vào cuối
                        int newRow = lastRow + 1;
                        worksheet.Cell(newRow, 1).Value = empId;
                        worksheet.Cell(newRow, 2).Value = employee.UserName;
                        worksheet.Cell(newRow, 3).Value = DateTime.Now;
                        worksheet.Cell(newRow, 4).Value = employee.Enable ? "Active" : "Inactive";
                        lastRow++;
                    }
                }

                worksheet.Columns().AdjustToContents();
                workbook.Save();

                _logger.LogInformation("Exported {Count} employee records to {FilePath}", data.Count, filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting employee data to {FilePath}", filePath);
                return false;
            }
        });
    }

    public async Task<int> GetRecordCountAsync(string filePath, string tableName)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheets.FirstOrDefault(w => w.Name == tableName);

                if (worksheet == null)
                    return 0;

                var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
                return Math.Max(0, lastRow - 1); // Trừ header row
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting record count from {FilePath}", filePath);
                return 0;
            }
        });
    }

    public async Task<List<EmployeeDto>> ReadEmployeeDataAsync(string filePath, string tableName)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheets.FirstOrDefault(w => w.Name == tableName);

                if (worksheet == null)
                    return new List<EmployeeDto>();

                var employees = new List<EmployeeDto>();
                int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

                for (int row = 2; row <= lastRow; row++)
                {
                    var id = worksheet.Cell(row, 1).GetString();
                    var name = worksheet.Cell(row, 2).GetString();
                    var status = worksheet.Cell(row, 4).GetString();

                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        employees.Add(new EmployeeDto
                        {
                            IDNumber = id,
                            UserName = name,
                            Enable = status.Equals("Active", StringComparison.OrdinalIgnoreCase)
                        });
                    }
                }

                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading employee data from {FilePath}", filePath);
                return new List<EmployeeDto>();
            }
        });
    }
}

using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace BHK.Retrieval.Attendance.WPF.Utilities
{
    /// <summary>
    /// Extension methods để fix các bug nội bộ của ClosedXML
    /// </summary>
    public static class ClosedXmlExtensions
    {
        /// <summary>
        /// Xóa table cụ thể đang chiếm ô A1 (hoặc cell bất kỳ)
        /// Đây là giải pháp CHỈ XÓA TABLE CẦN THIẾT, không động vào tables khác
        /// </summary>
        public static void RemoveTableAtCell(this IXLWorksheet worksheet, int row, int col, ILogger? logger = null)
        {
            var targetCell = worksheet.Cell(row, col);
            var tableAtCell = worksheet.Tables.FirstOrDefault(t => t.AsRange().Contains(targetCell));

            if (tableAtCell != null)
            {
                logger?.LogWarning($"Cell {targetCell.Address} is inside table '{tableAtCell.Name}' range {tableAtCell.RangeAddress} -> removing it");
                
                var tableName = tableAtCell.Name;
                var tableRange = tableAtCell.RangeAddress.ToString();
                
                // ✅ THEO ĐÚNG HƯỚNG DẪN:
                
                // BƯỚC 1: Clear AutoFilter TRƯỚC (nếu có)
                if (worksheet.AutoFilter.IsEnabled)
                {
                    logger?.LogInformation("Clearing AutoFilter before table removal");
                    worksheet.AutoFilter.Clear();
                }
                
                // BƯỚC 2: Unmerge ranges TRƯỚC
                foreach (var mr in worksheet.MergedRanges.ToList())
                {
                    if (mr.Contains(targetCell))
                    {
                        logger?.LogInformation($"Unmerging range {mr.RangeAddress} that contains {targetCell.Address}");
                        mr.Unmerge();
                    }
                }
                
                // BƯỚC 3: Clear nội dung cells của table
                logger?.LogInformation($"Clearing table range {tableRange}");
                tableAtCell.AsRange().Clear(XLClearOptions.All);
                
                // BƯỚC 4: XÓA TABLE khỏi worksheet.Tables collection sử dụng REFLECTION
                logger?.LogInformation($"Attempting to remove table '{tableName}' from worksheet.Tables collection");
                RemoveTableFromCollection(worksheet, tableAtCell, logger);
                
                logger?.LogInformation($"Tables remaining after removal: {worksheet.Tables.Count()}");
            }
            else
            {
                logger?.LogInformation($"Cell {targetCell.Address} is not part of any table");
            }
        }

        /// <summary>
        /// Xóa table khỏi worksheet.Tables collection bằng reflection
        /// </summary>
        private static void RemoveTableFromCollection(IXLWorksheet worksheet, IXLTable table, ILogger? logger = null)
        {
            try
            {
                // Cách 1: Thử tìm và xóa từ internal collection
                var tablesProperty = worksheet.GetType().GetProperty("Tables", BindingFlags.Public | BindingFlags.Instance);
                if (tablesProperty != null)
                {
                    var tablesCollection = tablesProperty.GetValue(worksheet);
                    if (tablesCollection != null)
                    {
                        // Thử tìm method Remove trong collection
                        var removeMethod = tablesCollection.GetType().GetMethod("Remove", new[] { typeof(IXLTable) });
                        if (removeMethod != null)
                        {
                            removeMethod.Invoke(tablesCollection, new object[] { table });
                            logger?.LogInformation($"Successfully removed table using Tables.Remove()");
                            return;
                        }
                        
                        // Thử tìm method Delete trên table object
                        var deleteMethod = table.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            .FirstOrDefault(m => m.Name == "Delete" && m.GetParameters().Length == 0);
                        
                        if (deleteMethod != null)
                        {
                            deleteMethod.Invoke(table, null);
                            logger?.LogInformation($"Successfully deleted table using table.Delete()");
                            return;
                        }
                    }
                }
                
                // Cách 2: Xóa trực tiếp từ internal _tables dictionary
                var tablesList = worksheet.GetType()
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => f.Name.Contains("table", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                foreach (var field in tablesList)
                {
                    var value = field.GetValue(worksheet);
                    if (value is System.Collections.IDictionary dict)
                    {
                        var keysToRemove = new List<object>();
                        foreach (var key in dict.Keys)
                        {
                            if (dict[key] == table || dict[key] == null)
                            {
                                keysToRemove.Add(key);
                            }
                        }
                        
                        foreach (var key in keysToRemove)
                        {
                            dict.Remove(key);
                            logger?.LogInformation($"Removed table from internal field '{field.Name}' with key '{key}'");
                        }
                        
                        if (keysToRemove.Any())
                        {
                            return;
                        }
                    }
                }
                
                logger?.LogWarning("Could not find a way to remove table from collection");
            }
            catch (Exception ex)
            {
                logger?.LogError($"Error removing table from collection: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa ghost entry của table khỏi internal dictionary
        /// Xử lý bug: ClosedXML giữ reference null sau khi Delete()
        /// </summary>
        public static void ForceDeleteTableInternal(this IXLWorksheet worksheet, string tableName, ILogger? logger = null)
        {
            var field = worksheet.GetType().GetField("_tables", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                logger?.LogWarning("Cannot access _tables field via reflection");
                return;
            }

            var dict = field.GetValue(worksheet) as IDictionary<string, IXLTable>;
            if (dict == null)
            {
                logger?.LogWarning("_tables field is null");
                return;
            }

            // Tìm key (có thể lowercase hoặc case-insensitive)
            var key = dict.Keys.FirstOrDefault(k => string.Equals(k, tableName, StringComparison.OrdinalIgnoreCase));
            if (key != null)
            {
                logger?.LogWarning($"Removing ghost entry '{key}' from worksheet._tables");
                dict.Remove(key);
            }

            // Quét bỏ luôn các entries có value null (bị dispose)
            var badKeys = dict.Where(kvp => kvp.Value == null).Select(kvp => kvp.Key).ToList();
            foreach (var k in badKeys)
            {
                logger?.LogWarning($"Removing null entry '{k}' from worksheet._tables");
                dict.Remove(k);
            }
            
            logger?.LogInformation($"Tables count after cleanup: {worksheet.Tables.Count()}");
        }

        /// <summary>
        /// Đảm bảo tên cột trong DataTable là duy nhất
        /// Tránh lỗi: "The header row contains more than one field name 'DIN'"
        /// </summary>
        public static void EnsureUniqueColumnNames(this DataTable dataTable)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var name = dataTable.Columns[i].ColumnName?.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    name = $"Column{i + 1}";
                }

                // Nếu tên đã tồn tại, thêm suffix
                if (!seen.Add(name))
                {
                    int n = 2;
                    string uniqueName;
                    do
                    {
                        uniqueName = $"{name}_{n}";
                        n++;
                    }
                    while (!seen.Add(uniqueName));
                    
                    dataTable.Columns[i].ColumnName = uniqueName;
                }
                else
                {
                    dataTable.Columns[i].ColumnName = name;
                }
            }
        }

        /// <summary>
        /// Force clear toàn bộ tables collection nội bộ của worksheet (Legacy - dùng cho trường hợp khẩn cấp)
        /// ⚠️ DEPRECATED: Dùng RemoveTableAtCell() thay thế để chỉ xóa table cần thiết
        /// </summary>
        [Obsolete("Use RemoveTableAtCell() for safer, more targeted table removal")]
        public static void ForceClearTables(this IXLWorksheet worksheet, ILogger? logger = null)
        {
            // Dùng reflection để access private field "_tables"
            var field = worksheet.GetType()
                .GetField("_tables", BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (field == null) return;

            var dict = field.GetValue(worksheet) as IDictionary<string, IXLTable>;
            if (dict == null) return;

            // KHÔNG xóa range/cells của table, chỉ xóa table definition
            // Lý do: tránh lỗi "cell is already part of a table" khi shift cells
            // Range sẽ được clear riêng bởi caller
            
            // Loại bỏ các entry null hoặc ghost (tables đã disposed)
            var badKeys = dict.Where(kvp => kvp.Value == null).Select(kvp => kvp.Key).ToList();
            foreach (var key in badKeys)
            {
                dict.Remove(key);
            }

            // Clear toàn bộ dictionary để gỡ bỏ tất cả table definitions
            dict.Clear();

            // Clear AutoFilter để tránh conflict (1 sheet chỉ có 1 AutoFilter)
            if (worksheet.AutoFilter.IsEnabled)
            {
                worksheet.AutoFilter.Clear();
            }
        }
    }
}

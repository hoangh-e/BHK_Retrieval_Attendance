using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace BHK.Retrieval.Attendance.Infrastructure.Data
{
    /// <summary>
    /// Factory để tạo kết nối SQLite cho Activity History database
    /// </summary>
    public interface ISQLiteConnectionFactory
    {
        IDbConnection CreateConnection();
        string DatabasePath { get; }
    }

    public class SQLiteConnectionFactory : ISQLiteConnectionFactory
    {
        private readonly string _databasePath;

        public SQLiteConnectionFactory(string? databasePath = null)
        {
            // Nếu không cung cấp path, dùng default: %LocalAppData%\BHK\Attendance\ActivityHistory.db
            if (string.IsNullOrWhiteSpace(databasePath))
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var bhkFolder = Path.Combine(localAppData, "BHK", "Attendance");
                _databasePath = Path.Combine(bhkFolder, "ActivityHistory.db");
            }
            else
            {
                _databasePath = databasePath;
            }

            // Đảm bảo thư mục tồn tại
            var directory = Path.GetDirectoryName(_databasePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public string DatabasePath => _databasePath;

        public IDbConnection CreateConnection()
        {
            var connectionString = $"Data Source={_databasePath};Version=3;";
            return new SQLiteConnection(connectionString);
        }
    }
}

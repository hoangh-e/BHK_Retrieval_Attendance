using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Serilog;

namespace BHK.Retrieval.Attendance.Infrastructure.Data
{
    /// <summary>
    /// Khởi tạo database và chạy migration scripts
    /// </summary>
    public interface IDatabaseInitializer
    {
        Task InitializeAsync();
    }

    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ISQLiteConnectionFactory _connectionFactory;

        public DatabaseInitializer(ISQLiteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InitializeAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();

                // Kiểm tra xem bảng đã tồn tại chưa
                var tableExists = await TableExistsAsync(connection, "ActivityHistory");

                if (!tableExists)
                {
                    Log.Information("Initializing ActivityHistory database...");
                    await RunMigrationAsync(connection, "001_InitialCreate.sql");
                    Log.Information("ActivityHistory database initialized successfully");
                }
                else
                {
                    Log.Debug("ActivityHistory database already exists");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize ActivityHistory database");
                throw;
            }
        }

        private async Task<bool> TableExistsAsync(IDbConnection connection, string tableName)
        {
            var query = @"
                SELECT COUNT(*) 
                FROM sqlite_master 
                WHERE type='table' AND name=@TableName";

            var count = await connection.ExecuteScalarAsync<int>(query, new { TableName = tableName });
            return count > 0;
        }

        private async Task RunMigrationAsync(IDbConnection connection, string migrationFileName)
        {
            // Đọc file migration từ Migrations folder
            var migrationPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "Migrations",
                migrationFileName);

            if (!File.Exists(migrationPath))
            {
                throw new FileNotFoundException($"Migration file not found: {migrationPath}");
            }

            var sql = await File.ReadAllTextAsync(migrationPath);

            // Thực thi migration script
            await connection.ExecuteAsync(sql);

            Log.Information("Migration {MigrationFile} executed successfully", migrationFileName);
        }
    }
}

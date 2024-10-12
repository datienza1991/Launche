using Infrastructure.Database;
using Infrastructure.Models;
using System.Data.SQLite;

namespace Infrastructure.Repositories
{
    public interface IDevAppRepository
    {
        Task<bool> Add(IDEPath param);
        Task<bool> Edit(IDEPath param);
        Task<bool> Delete(long id);
        Task<IDEPath> GetById(int id);
        Task<IEnumerable<IDEPath>> GetAll();
    }
    public class DevAppRepository(ICreateSqliteConnection createSqliteConnection) : IDevAppRepository
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public async Task<bool> Add(IDEPath param)
        {
            var tableName = $"{nameof(IDEPath)}s";
            using var connection = createSqliteConnection.Execute();
            connection.Open();

            string createTableSql = $"INSERT INTO IDEPaths ( Path ) VALUES ( @path );";
            using var command = new SQLiteCommand(createTableSql, connection);
            command.Parameters.AddWithValue("@path", param.Path);
            var rows = await command.ExecuteNonQueryAsync();

            return rows != 0;
        }

        public async Task<bool> Delete(long id)
        {
            using var connection = createSqliteConnection.Execute();

            connection.Open();

            string createTableSql = @"
                PRAGMA foreign_keys = ON;
                DELETE FROM IdePaths WHERE Id = @id;";

            using var command = new SQLiteCommand(createTableSql, connection);

            command.Parameters.AddWithValue("@id", id);

            var rows = await command.ExecuteNonQueryAsync();
            return rows != 0;
        }

        public Task<bool> Edit(IDEPath param)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IDEPath>> GetAll()
        {
            var tableName = $"{nameof(Infrastructure.Models.IDEPath)}s";
            var iDEPaths = new List<Infrastructure.Models.IDEPath>();
            var connection = createSqliteConnection.Execute();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM {tableName}";
            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                _ = int.TryParse(reader[nameof(Infrastructure.Models.IDEPath.Id)]?.ToString(), out int id);
                var path = reader[nameof(Infrastructure.Models.IDEPath.Path)]?.ToString() ?? "";

                iDEPaths.Add(new() { Id = id, Path = path });
            }

            return iDEPaths;
        }

        public async Task<IDEPath> GetById(int id)
        {
            var tableName = $"{nameof(IDEPath)}s";
            var vsCodePath = new Infrastructure.Models.IDEPath();
            var connection = createSqliteConnection.Execute();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM {tableName} LIMIT 1";
            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {

                var path = reader[nameof(Infrastructure.Models.IDEPath.Path)]?.ToString() ?? "";

                vsCodePath.Id = id;
                vsCodePath.Path = path;
            }

            return vsCodePath;
        }
    }
}

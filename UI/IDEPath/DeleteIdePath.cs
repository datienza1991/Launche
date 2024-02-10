using System.Data.SQLite;
using UI.Database;

namespace UI.IDEPath
{
    public interface IDeleteIdePath : IExecuteAsync<int, bool>;

    public class DeleteIdePath(ICreateSqliteConnection createSqliteConnection) : IDeleteIdePath
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public async Task<bool> ExecuteAsync(int id)
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
    }
}

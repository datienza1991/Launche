using System.Data.SQLite;
using UI.Database;

namespace UI.Group
{
    public interface IDelete : IExecuteAsync<int, bool>;

    public class Delete(ICreateSqliteConnection createSqliteConnection) : IDelete
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public async Task<bool> ExecuteAsync(int id)
        {
            using var connection = createSqliteConnection.Execute();

            connection.Open();

            string createTableSql = @"
                PRAGMA foreign_keys = ON;
                DELETE FROM Groups WHERE Id = @id;";

            using var command = new SQLiteCommand(createTableSql, connection);

            command.Parameters.AddWithValue("@id", id);

            var rows = await command.ExecuteNonQueryAsync();
            return rows != 0;
        }
    }
}

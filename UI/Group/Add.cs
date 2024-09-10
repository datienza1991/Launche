using System.Data.SQLite;
using UI.Database;

namespace UI.Group
{
    public interface IAdd : IExecuteAsync<Group, bool>;

    internal class Add(ICreateSqliteConnection createSqliteConnection) : IAdd
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public async Task<bool> ExecuteAsync(Group param)
        {
            using var connection = createSqliteConnection.Execute();

            connection.Open();

            string createTableSql = @"
                PRAGMA foreign_keys = ON; 
                INSERT INTO Groups (Name) VALUES (@name);";

            using var command = new SQLiteCommand(createTableSql, connection);

            command.Parameters.AddWithValue("@path", param.Name);

            var rows = await command.ExecuteNonQueryAsync();

            return rows != 0;
        }
    }
}

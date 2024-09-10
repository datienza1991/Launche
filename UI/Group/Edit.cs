using System.Data.SQLite;
using UI.Database;

namespace UI.Group
{
    public interface IEdit : IExecuteAsync<Group, bool>;

    public class Edit(ICreateSqliteConnection createSqliteConnection) : IEdit
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public async Task<bool> ExecuteAsync(Group param)
        {
            using var connection = this.createSqliteConnection.Execute();

            connection.Open();

            string createTableSql = @"UPDATE Groups SET Name = @name WHERE Id = @id;";

            using var command = new SQLiteCommand(createTableSql, connection);

            command.Parameters.AddWithValue("@name", param.Name);
            command.Parameters.AddWithValue("@id", param.Id);

            var rows = await command.ExecuteNonQueryAsync();
            return rows != 0;
        }
    }
}

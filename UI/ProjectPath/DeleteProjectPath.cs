using System.Data.SQLite;
using UI.Database;

namespace UI.ProjectPath
{
    public interface IDeleteProjectPath : IExecuteAsync<int, bool>;

    public class DeleteProjectPath(ICreateSqliteConnection createSqliteConnection) : IDeleteProjectPath
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public async Task<bool> ExecuteAsync(int id)
        {
            using var connection = this.createSqliteConnection.Execute();

            connection.Open();

            string createTableSql = $"DELETE FROM ProjectPaths WHERE Id = @id;";

            using var command = new SQLiteCommand(createTableSql, connection);

            command.Parameters.AddWithValue("@id", id);

            var rows = await command.ExecuteNonQueryAsync();
            return rows != 0;
        }
    }
}

using System.Data.SQLite;
using UI.Database;

namespace UI.ProjectPath
{
    public interface ISortUpProjectPath : IExecuteAsync<int, bool>;

    public class SortUpProjectPath(ICreateSqliteConnection createSqliteConnection) : ISortUpProjectPath
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public async Task<bool> ExecuteAsync(int param)
        {
            using var connection = this.createSqliteConnection.Execute();

            connection.Open();

            string createTableSql = @"
                update ProjectPaths
                    set SortId = @SortIdTop + @SortIdDown - SortId
                    where SortId in (@SortIdTop, @SortIdDown)";

            using var command = new SQLiteCommand(createTableSql, connection);

            var top = param - 1;

            command.Parameters.AddWithValue("@SortIdTop", top);
            command.Parameters.AddWithValue("@SortIdDown", param);

            var rows = await command.ExecuteNonQueryAsync();
            return rows != 0;
        }
    }
}

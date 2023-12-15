using System.Data.SQLite;
using UI.Database;

namespace UI.ProjectPath
{
    public interface IAddProjectPath : IExecuteAsync<ProjectPathModel, bool>;

    public class AddProjectPath(ICreateSqliteConnection createSqliteConnection) : IAddProjectPath
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public async Task<bool> ExecuteAsync(ProjectPathModel param)
        {
            using var connection = this.createSqliteConnection.Execute();

            connection.Open();

            string createTableSql = $"INSERT INTO ProjectPaths( Path , Name ) VALUES ( @path , @name );";
            
            using var command = new SQLiteCommand(createTableSql, connection);
            
            command.Parameters.AddWithValue("@path", param.Path);
            command.Parameters.AddWithValue("@name", param.Name);

            var rows = await command.ExecuteNonQueryAsync();

            return rows != 0;
        }
    }
}

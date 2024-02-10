using System.Data.SQLite;
using UI.Database;

namespace UI.ProjectPath
{
    public interface IAddProjectPath : IExecuteAsync<ProjectPath, bool>;

    public class AddProjectPath(ICreateSqliteConnection createSqliteConnection) : IAddProjectPath
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public async Task<bool> ExecuteAsync(ProjectPath param)
        {
            using var connection = this.createSqliteConnection.Execute();

            connection.Open();

            string createTableSql = @"
                PRAGMA foreign_keys = ON; 
                INSERT INTO ProjectPaths( Path , Name, IDEPathId ) VALUES ( @path , @name, @idePath );";

            using var command = new SQLiteCommand(createTableSql, connection);

            command.Parameters.AddWithValue("@path", param.Path);
            command.Parameters.AddWithValue("@name", param.Name);
            command.Parameters.AddWithValue("@idePath", param.IDEPathId);

            var rows = await command.ExecuteNonQueryAsync();

            return rows != 0;
        }
    }
}

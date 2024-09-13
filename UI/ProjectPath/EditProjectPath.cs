using System.Data.SQLite;
using UI.Database;

namespace UI.ProjectPath
{
    public interface IEditProjectPath : IExecuteAsync<ProjectPath, bool>;

    public class EditProjectPath(ICreateSqliteConnection createSqliteConnection) : IEditProjectPath
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public async Task<bool> ExecuteAsync(ProjectPath param)
        {
            using var connection = this.createSqliteConnection.Execute();

            connection.Open();

            string createTableSql = $"UPDATE ProjectPaths SET  " +
                $"Path = @path, " +
                $"Name = @name, " +
                $"IDEPathId = @idePathId," +
                $"Filename = @fileName," +
                $"GroupId = @groupId " +
                $"WHERE Id = @id;";

            using var command = new SQLiteCommand(createTableSql, connection);

            command.Parameters.AddWithValue("@path", param.Path);
            command.Parameters.AddWithValue("@name", param.Name);
            command.Parameters.AddWithValue("@idePathId", param.IDEPathId);
            command.Parameters.AddWithValue("@fileName", param.Filename);
            command.Parameters.AddWithValue("@id", param.Id);
            command.Parameters.AddWithValue("@groupId", param.GroupId);

            var rows = await command.ExecuteNonQueryAsync();
            return rows != 0;
        }
    }
}

using UI.Database;

namespace UI.ProjectPath;

public interface IGetLastProjectPath : IExecuteAsync<Project>;
internal class GetLastProjectPath(ICreateSqliteConnection createSqliteConnection) : IGetLastProjectPath
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<Project> ExecuteAsync()
    {
        var tableName = $"{nameof(Project)}s";
        var projectPath = new Project();
        var connection = this.createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName} ORDER BY Id DESC LIMIT 1;";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(Project.Id)]?.ToString(), out int id);
            var path = reader[nameof(Project.Path)]?.ToString() ?? "";
            var name = reader[nameof(Project.Name)]?.ToString() ?? "";
            var idePathId = int.Parse(reader[nameof(Project.IDEPathId)]?.ToString() ?? "0");
            var filename = reader[nameof(Project.Filename)]?.ToString() ?? "";

            projectPath.Id = id;
            projectPath.Path = path;
            projectPath.Name = name;
            projectPath.IDEPathId = idePathId;
            projectPath.Filename = filename;
        }

        return projectPath;
    }
}


using UI.Database;

namespace UI.ProjectPath;

public interface IGetLastProjectPath : IExecuteAsync<ProjectPath>;
internal class GetLastProjectPath(ICreateSqliteConnection createSqliteConnection) : IGetLastProjectPath
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<ProjectPath> ExecuteAsync()
    {
        var tableName = $"{nameof(ProjectPath)}s";
        var projectPath = new ProjectPath();
        var connection = this.createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName} ORDER BY Id DESC LIMIT 1;";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(ProjectPath.Id)]?.ToString(), out int id);
            var path = reader[nameof(ProjectPath.Path)]?.ToString() ?? "";
            var name = reader[nameof(ProjectPath.Name)]?.ToString() ?? "";
            var idePathId = int.Parse(reader[nameof(ProjectPath.IDEPathId)]?.ToString() ?? "0");
            var filename = reader[nameof(ProjectPath.Filename)]?.ToString() ?? "";

            projectPath.Id = id;
            projectPath.Path = path;
            projectPath.Name = name;
            projectPath.IDEPathId = idePathId;
            projectPath.Filename = filename;
        }

        return projectPath;
    }
}


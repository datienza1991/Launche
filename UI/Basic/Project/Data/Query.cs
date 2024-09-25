using UI.Database;

namespace UI.Basic.Project.Data;

public interface IQuery
{
    Task<ProjectPath.ProjectPath> GetLast();
    Task<List<ProjectPath.ProjectPath>> GetAll();
}

public class Query(ICreateSqliteConnection createSqliteConnection) : IQuery
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<ProjectPath.ProjectPath> GetLast()
    {
        var tableName = $"{nameof(ProjectPath)}s";
        var projectPath = new ProjectPath.ProjectPath();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName} ORDER BY Id DESC LIMIT 1;";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(ProjectPath.ProjectPath.Id)]?.ToString(), out int id);
            var path = reader[nameof(ProjectPath.ProjectPath.Path)]?.ToString() ?? "";
            var name = reader[nameof(ProjectPath.ProjectPath.Name)]?.ToString() ?? "";
            var idePathId = int.Parse(reader[nameof(ProjectPath.ProjectPath.IDEPathId)]?.ToString() ?? "0");
            var filename = reader[nameof(ProjectPath.ProjectPath.Filename)]?.ToString() ?? "";

            projectPath.Id = id;
            projectPath.Path = path;
            projectPath.Name = name;
            projectPath.IDEPathId = idePathId;
            projectPath.Filename = filename;
        }

        return projectPath;
    }

    public async Task<List<ProjectPath.ProjectPath>> GetAll()
    {
        var projectPaths = new List<ProjectPath.ProjectPath>();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM ProjectPaths ORDER BY SortId";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var id = int.Parse(reader[nameof(ProjectPath.ProjectPath.Id)]?.ToString() ?? "0");
            var path = reader[nameof(ProjectPath.ProjectPath.Path)]?.ToString() ?? "";
            var name = reader[nameof(ProjectPath.ProjectPath.Name)]?.ToString() ?? "";
            var idePathId = reader[nameof(ProjectPath.ProjectPath.IDEPathId)]?.ToString() ?? "";
            var sortId = reader[nameof(ProjectPath.ProjectPath.SortId)]?.ToString() ?? "";
            var fileName = reader[nameof(ProjectPath.ProjectPath.Filename)]?.ToString() ?? "";
            var isGroupId = int.TryParse(reader[nameof(ProjectPath.ProjectPath.GroupId)]?.ToString(), out int groupId);

            projectPaths.Add
            (
                new()
                {
                    Id = id,
                    Path = path,
                    Name = name,
                    IDEPathId = int.Parse(idePathId),
                    SortId = int.Parse(sortId),
                    Filename = fileName,
                    GroupId = isGroupId ? groupId : null,
                }
            );
        }

        return projectPaths;
    }
}


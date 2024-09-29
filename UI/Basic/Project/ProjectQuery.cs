using UI.Database;

namespace UI.Basic.Project;

public interface IProjectQuery
{
    Task<ProjectPath.Project> GetLast();
    Task<List<ProjectPath.Project>> GetAll();
}

public class ProjectQuery(ICreateSqliteConnection createSqliteConnection) : IProjectQuery
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<ProjectPath.Project> GetLast()
    {
        var tableName = $"{nameof(ProjectPath)}s";
        var projectPath = new ProjectPath.Project();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {nameof(ProjectPath.Project)}s ORDER BY Id DESC LIMIT 1;";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(ProjectPath.Project.Id)]?.ToString(), out int id);
            var path = reader[nameof(ProjectPath.Project.Path)]?.ToString() ?? "";
            var name = reader[nameof(ProjectPath.Project.Name)]?.ToString() ?? "";
            var idePathId = int.Parse(reader[nameof(ProjectPath.Project.IDEPathId)]?.ToString() ?? "0");
            var filename = reader[nameof(ProjectPath.Project.Filename)]?.ToString() ?? "";

            projectPath.Id = id;
            projectPath.Path = path;
            projectPath.Name = name;
            projectPath.IDEPathId = idePathId;
            projectPath.Filename = filename;
        }

        return projectPath;
    }

    public async Task<List<ProjectPath.Project>> GetAll()
    {
        var projectPaths = new List<ProjectPath.Project>();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {nameof(ProjectPath.Project)}s ORDER BY SortId";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var id = int.Parse(reader[nameof(ProjectPath.Project.Id)]?.ToString() ?? "0");
            var path = reader[nameof(ProjectPath.Project.Path)]?.ToString() ?? "";
            var name = reader[nameof(ProjectPath.Project.Name)]?.ToString() ?? "";
            var idePathId = reader[nameof(ProjectPath.Project.IDEPathId)]?.ToString() ?? "";
            var sortId = reader[nameof(ProjectPath.Project.SortId)]?.ToString() ?? "";
            var fileName = reader[nameof(ProjectPath.Project.Filename)]?.ToString() ?? "";
            var isGroupId = int.TryParse(reader[nameof(ProjectPath.Project.GroupId)]?.ToString(), out int groupId);

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


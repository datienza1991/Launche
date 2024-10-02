using UI.Database;

namespace UI.Basic.Project.Queries;

public interface IProjectQuery
{
    Task<ProjectDetail> GetLast();
    Task<List<ProjectDetail>> GetAll();
}

public class ProjectQuery(ICreateSqliteConnection createSqliteConnection) : IProjectQuery
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<ProjectDetail> GetLast()
    {
        var tableName = $"{nameof(ProjectPath)}s";
        var projectPath = new ProjectDetail();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {nameof(Project)}s ORDER BY Id DESC LIMIT 1;";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(ProjectDetail.Id)]?.ToString(), out int id);
            var path = reader[nameof(ProjectDetail.Path)]?.ToString() ?? "";
            var name = reader[nameof(ProjectDetail.Name)]?.ToString() ?? "";
            var idePathId = int.Parse(reader[nameof(ProjectDetail.IDEPathId)]?.ToString() ?? "0");
            var filename = reader[nameof(ProjectDetail.Filename)]?.ToString() ?? "";

            projectPath.Id = id;
            projectPath.Path = path;
            projectPath.Name = name;
            projectPath.IDEPathId = idePathId;
            projectPath.Filename = filename;
        }

        return projectPath;
    }

    public async Task<List<ProjectDetail>> GetAll()
    {
        var projectPaths = new List<ProjectDetail>();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {nameof(Project)}s ORDER BY SortId";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var id = int.Parse(reader[nameof(ProjectDetail.Id)]?.ToString() ?? "0");
            var path = reader[nameof(ProjectDetail.Path)]?.ToString() ?? "";
            var name = reader[nameof(ProjectDetail.Name)]?.ToString() ?? "";
            var idePathId = reader[nameof(ProjectDetail.IDEPathId)]?.ToString() ?? "";
            var sortId = reader[nameof(ProjectDetail.SortId)]?.ToString() ?? "";
            var fileName = reader[nameof(ProjectDetail.Filename)]?.ToString() ?? "";
            var isGroupId = int.TryParse(reader[nameof(ProjectDetail.GroupId)]?.ToString(), out int groupId);

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


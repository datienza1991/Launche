using UI.Database;

namespace UI.ProjectPath;

public interface IGetProjectPaths : IExecuteAsync<List<Project>>;

public class GetProjectPaths(ICreateSqliteConnection createSqliteConnection) : IGetProjectPaths
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<List<Project>> ExecuteAsync()
    {
        var projectPaths = new List<Project>();
        var connection = this.createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM ProjectPaths ORDER BY SortId";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var id = int.Parse(reader[nameof(Project.Id)]?.ToString() ?? "0");
            var path = reader[nameof(Project.Path)]?.ToString() ?? "";
            var name = reader[nameof(Project.Name)]?.ToString() ?? "";
            var idePathId = reader[nameof(Project.IDEPathId)]?.ToString() ?? "";
            var sortId = reader[nameof(Project.SortId)]?.ToString() ?? "";
            var fileName = reader[nameof(Project.Filename)]?.ToString() ?? "";
            var isGroupId = int.TryParse(reader[nameof(Project.GroupId)]?.ToString(), out int groupId);

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


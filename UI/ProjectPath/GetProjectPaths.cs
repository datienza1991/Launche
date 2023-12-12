using UI.Database;

namespace UI.ProjectPath;

public interface IGetProjectPaths : IExecuteAsync<List<ProjectPathModel>>;

public class GetProjectPaths(ICreateSqliteConnection createSqliteConnection) : IGetProjectPaths
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<List<ProjectPathModel>> ExecuteAsync()
    {
        var projectPaths = new List<ProjectPathModel>();
        var connection = this.createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM ProjectPaths";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(ProjectPathModel.Id)]?.ToString(), out int id);
            var path = reader[nameof(ProjectPathModel.Path)]?.ToString() ?? "";
            var name = reader[nameof(ProjectPathModel.Name)]?.ToString() ?? "";

            projectPaths.Add(new() { Id = id, Path = path, Name = name });
        }

        return projectPaths;
    }
}


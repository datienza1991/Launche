﻿using UI.Database;

namespace UI.ProjectPath;

public interface IGetProjectPaths : IExecuteAsync<List<ProjectPath>>;

public class GetProjectPaths(ICreateSqliteConnection createSqliteConnection) : IGetProjectPaths
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<List<ProjectPath>> ExecuteAsync()
    {
        var projectPaths = new List<ProjectPath>();
        var connection = this.createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM ProjectPaths";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(ProjectPath.Id)]?.ToString(), out int id);
            var path = reader[nameof(ProjectPath.Path)]?.ToString() ?? "";
            var name = reader[nameof(ProjectPath.Name)]?.ToString() ?? "";

            projectPaths.Add(new() { Id = id, Path = path, Name = name });
        }

        return projectPaths;
    }
}

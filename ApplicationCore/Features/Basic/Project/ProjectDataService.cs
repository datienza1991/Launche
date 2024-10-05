using Infrastructure.Database;
using System.Data.SQLite;

namespace ApplicationCore.Features.Basic.Project;

public interface IProjectDataService
{
    Task<ProjectDetail> GetLast();
    Task<IEnumerable<ProjectPathsViewModel>> GetAll();
    Task<bool> Add(Infrastructure.Models.Project param);
    Task<bool> Edit(Infrastructure.Models.Project param);
    Task<bool> Delete(int id);
}

public class ProjectDataService(ICreateSqliteConnection createSqliteConnection) : IProjectDataService
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;
    private const string TABLE = $"{nameof(Project)}s";

    public async Task<bool> Add(Infrastructure.Models.Project param)
    {
        using var connection = createSqliteConnection.Execute();

        connection.Open();

        string createTableSql = @$"
                PRAGMA foreign_keys = ON; 
                INSERT INTO {TABLE}( Path , Name , IDEPathId , SortId,  Filename ) 
                    VALUES ( @path , @name , @idePath , (select max( SortId ) from {TABLE}) + 1 , @fileName );";

        using var command = new SQLiteCommand(createTableSql, connection);

        command.Parameters.AddWithValue("@path", param?.Path);
        command.Parameters.AddWithValue("@name", param?.Name);
        command.Parameters.AddWithValue("@idePath", param?.IDEPathId);
        command.Parameters.AddWithValue("@fileName", param?.Filename);

        var rows = await command.ExecuteNonQueryAsync();

        return rows != 0;
    }

    public async Task<bool> Edit(Infrastructure.Models.Project param)
    {
        using var connection = createSqliteConnection.Execute();

        connection.Open();

        string createTableSql = $"UPDATE {TABLE} SET  " +
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

    public async Task<bool> Delete(int id)
    {
        using var connection = createSqliteConnection.Execute();

        connection.Open();

        string createTableSql = $"DELETE FROM {TABLE} WHERE Id = @id;";

        using var command = new SQLiteCommand(createTableSql, connection);

        command.Parameters.AddWithValue("@id", id);

        var rows = await command.ExecuteNonQueryAsync();
        return rows != 0;
    }

    public async Task<ProjectDetail> GetLast()
    {
        var tableName = $"{nameof(ProjectDetail)}s";
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

    public async Task<IEnumerable<ProjectPathsViewModel>> GetAll()
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

        return projectPaths.Select
        (
            (value, index) => new ProjectPathsViewModel
            {
                Index = index + 1,
                Id = value.Id,
                Name = value.Name,
                Path = value.Path,
                IDEPathId = value.IDEPathId,
                SortId = value.SortId,
                EnableMoveUp = index != 1,
                EnableMoveDown = index != projectPaths.Count,
                Filename = value.Filename,
                GroupId = value.GroupId,
                GroupName = ""
            }
        );
    }
}


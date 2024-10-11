using Infrastructure.Database;
using Infrastructure.Models;
using System.Data.SQLite;

namespace ApplicationCore.Features.Projects;

public interface IProjectRepository
{
    Task<Project> GetLast();
    Task<Project> GetOne(long id);
    Task<IEnumerable<Project>> GetAll();
    Task<bool> Add(Project param);
    Task<bool> Edit(Project param);
    Task<bool> Delete(long id);
}

public class ProjectRepository(ICreateSqliteConnection createSqliteConnection) : IProjectRepository
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;
    private const string TABLE = $"{nameof(Project)}s";

    public async Task<bool> Add(Project param)
    {
        using var connection = createSqliteConnection.Execute();

        connection.Open();

        string createTableSql = @$"
                PRAGMA foreign_keys = ON; 
                INSERT INTO {TABLE}( Path , Name , IDEPathId , SortId,  Filename ) 
                    VALUES ( @path , @name , @idePath , (select max( SortId ) from {TABLE}) + 1 , @fileName );
                SELECT last_insert_rowid();";

        using var command = new SQLiteCommand(createTableSql, connection);

        command.Parameters.AddWithValue("@path", param?.Path);
        command.Parameters.AddWithValue("@name", param?.Name);
        command.Parameters.AddWithValue("@idePath", param?.IDEPathId);
        command.Parameters.AddWithValue("@fileName", param?.Filename);

        var rows = await command.ExecuteNonQueryAsync();

        return rows != 0;
    }

    public async Task<bool> Edit(Project param)
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

    public async Task<bool> Delete(long id)
    {
        using var connection = createSqliteConnection.Execute();

        connection.Open();

        string createTableSql = $"DELETE FROM {TABLE} WHERE Id = @id;";

        using var command = new SQLiteCommand(createTableSql, connection);

        command.Parameters.AddWithValue("@id", id);

        var rows = await command.ExecuteNonQueryAsync();
        return rows != 0;
    }

    public async Task<Project> GetLast()
    {
        var tableName = $"{nameof(Infrastructure.Models.Project)}s";
        var projectPath = new Infrastructure.Models.Project();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {nameof(Project)}s ORDER BY Id DESC LIMIT 1;";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(Infrastructure.Models.Project.Id)]?.ToString(), out int id);
            var path = reader[nameof(Infrastructure.Models.Project.Path)]?.ToString() ?? "";
            var name = reader[nameof(Infrastructure.Models.Project.Name)]?.ToString() ?? "";
            var idePathId = int.Parse(reader[nameof(Infrastructure.Models.Project.IDEPathId)]?.ToString() ?? "0");
            var filename = reader[nameof(Infrastructure.Models.Project.Filename)]?.ToString() ?? "";

            projectPath.Id = id;
            projectPath.Path = path;
            projectPath.Name = name;
            projectPath.IDEPathId = idePathId;
            projectPath.Filename = filename;
        }

        return projectPath;
    }

    public async Task<IEnumerable<Project>> GetAll()
    {
        var projectPaths = new List<Infrastructure.Models.Project>();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {nameof(Project)}s ORDER BY SortId";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var id = int.Parse(reader[nameof(Infrastructure.Models.Project.Id)]?.ToString() ?? "0");
            var path = reader[nameof(Infrastructure.Models.Project.Path)]?.ToString() ?? "";
            var name = reader[nameof(Infrastructure.Models.Project.Name)]?.ToString() ?? "";
            var idePathId = reader[nameof(Infrastructure.Models.Project.IDEPathId)]?.ToString() ?? "";
            var sortId = reader[nameof(Infrastructure.Models.Project.SortId)]?.ToString() ?? "";
            var fileName = reader[nameof(Infrastructure.Models.Project.Filename)]?.ToString() ?? "";
            var isGroupId = int.TryParse(reader[nameof(Infrastructure.Models.Project.GroupId)]?.ToString(), out int groupId);

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

    public async Task<Project> GetOne(long id)
    {
        var projectPath = new Project();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {TABLE} WHERE ID = @id;";
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {

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


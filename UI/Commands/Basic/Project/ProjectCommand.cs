using System.Data.SQLite;
using UI.Database;

namespace UI.Commands.Basic.Project;

public interface IProjectCommand
{
    Task<bool> Add(Project param);
    Task<bool> Edit(Project param);
    Task<bool> Delete(int id);
}

public class ProjectCommand(ICreateSqliteConnection createSqliteConnection) : IProjectCommand
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
                    VALUES ( @path , @name , @idePath , (select max( SortId ) from {TABLE}) + 1 , @fileName );";

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
}


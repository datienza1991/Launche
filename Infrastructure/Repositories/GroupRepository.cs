using Infrastructure.Database;
using Infrastructure.Models;
using System.Data.SQLite;

namespace Infrastructure.Repositories;

public interface IGroupRepository
{
    Task<Group> GetOne(long id);
    Task<List<Group>> GetAll();
    Task<bool> Add(Group param);
    Task<bool> Edit(Group param);
    Task<bool> Delete(long id);
}

public class GroupRepository(ICreateSqliteConnection createSqliteConnection) : IGroupRepository
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;
    private const string TABLE = $"{nameof(Group)}s";

    public async Task<bool> Add(Group param)
    {
        using var connection = createSqliteConnection.Execute();

        connection.Open();

        string createTableSql = @$"
                PRAGMA foreign_keys = ON; 
                INSERT INTO {TABLE}(Name) VALUES (@name);";

        using var command = new SQLiteCommand(createTableSql, connection);

        command.Parameters.AddWithValue("@path", param.Name);

        var rows = await command.ExecuteNonQueryAsync();

        return rows != 0;
    }

    public async Task<bool> Edit(Group param)
    {
        using var connection = createSqliteConnection.Execute();
        connection.Open();

        string createTableSql = @$"UPDATE {TABLE} SET Name = @name WHERE Id = @id;";

        using var command = new SQLiteCommand(createTableSql, connection);

        command.Parameters.AddWithValue("@name", param.Name);
        command.Parameters.AddWithValue("@id", param.Id);

        var rows = await command.ExecuteNonQueryAsync();
        return rows != 0;
    }

    public async Task<bool> Delete(long id)
    {
        using var connection = createSqliteConnection.Execute();

        connection.Open();

        string createTableSql = @$"
                PRAGMA foreign_keys = ON;
                DELETE FROM {TABLE} WHERE Id = @id;";

        using var command = new SQLiteCommand(createTableSql, connection);

        command.Parameters.AddWithValue("@id", id);

        var rows = await command.ExecuteNonQueryAsync();
        return rows != 0;
    }

    public async Task<List<Group>> GetAll()
    {
        var tableName = $"{nameof(Group)}s";
        var groups = new List<Group>();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(Group.Id)]?.ToString(), out int id);
            var path = reader[nameof(Group.Name)]?.ToString() ?? "";

            groups.Add(new() { Id = id, Name = path });
        }

        return groups;
    }

    public async Task<Group> GetOne(long id)
    {
        var tableName = $"{nameof(Group)}s";
        var group = new Group();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.Parameters.AddWithValue("@id", id);
        command.CommandText = $"SELECT * FROM {tableName} WHERE Id = @id";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var path = reader[nameof(Group.Name)]?.ToString() ?? "";

            group.Id = id;
            group.Name = path;
        }

        return group;
    }
}


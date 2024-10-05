using Infrastructure.Database;
using System.Data.SQLite;

namespace ApplicationCore.Features.Basic.Group;

public interface IGroupDataService
{
    Task<GroupDetail> GetOne(int id);
    Task<List<GroupDetail>> GetAll();
    Task<bool> Add(Infrastructure.Models.Group param);
    Task<bool> Edit(Infrastructure.Models.Group param);
    Task<bool> Delete(int id);
}

public class GroupDataService(ICreateSqliteConnection createSqliteConnection) : IGroupDataService
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;
    private const string TABLE = $"{nameof(Group)}s";

    public async Task<bool> Add(Infrastructure.Models.Group param)
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

    public async Task<bool> Edit(Infrastructure.Models.Group param)
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

    public async Task<bool> Delete(int id)
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

    public async Task<List<GroupDetail>> GetAll()
    {
        var tableName = $"{nameof(Group)}s";
        var groups = new List<GroupDetail>();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(GroupDetail.Id)]?.ToString(), out int id);
            var path = reader[nameof(GroupDetail.Name)]?.ToString() ?? "";

            groups.Add(new() { Id = id, Name = path });
        }

        return groups;
    }

    public async Task<GroupDetail> GetOne(int id)
    {
        var tableName = $"{nameof(Group)}s";
        var group = new GroupDetail();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.Parameters.AddWithValue("@id", id);
        command.CommandText = $"SELECT * FROM {tableName} WHERE Id = @id";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var path = reader[nameof(GroupDetail.Name)]?.ToString() ?? "";

            group.Id = id;
            group.Name = path;
        }

        return group;
    }
}


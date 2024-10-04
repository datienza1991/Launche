using System.Data.SQLite;
using UI.Database;

namespace UI.Commands.Basic.Group;

public interface IGroupCommand
{
    Task<bool> Add(Group param);
    Task<bool> Edit(Group param);
    Task<bool> Delete(int id);
}

public class GroupCommand(ICreateSqliteConnection createSqliteConnection) : IGroupCommand
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
        using var connection = this.createSqliteConnection.Execute();
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
}


using UI.Database;

namespace UI.Group;

public interface IGetAll : IExecuteAsync<List<Group>>;

public class GetAll(ICreateSqliteConnection createSqliteConnection) : IGetAll
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<List<Group>> ExecuteAsync()
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
            _ = int.TryParse((string?)(reader[nameof(Group.Id)]?.ToString()), out int id);
            var path = reader[nameof(Group.Name)]?.ToString() ?? "";

            groups.Add(new() { Id = id, Name = path });
        }

        return groups;
    }
}


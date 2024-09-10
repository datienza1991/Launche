using UI.Database;

namespace UI.Group;

public interface IGetOne : IExecuteAsync<int, Group>;

public class GetOne(ICreateSqliteConnection createSqliteConnection) : IGetOne
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<Group> ExecuteAsync(int id)
    {
        var tableName = $"{nameof(Group)}s";
        var group = new Group();
        var connection = this.createSqliteConnection.Execute();
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


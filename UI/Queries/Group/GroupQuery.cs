using UI.Database;

namespace UI.Queries.Group;

public interface IGroupQuery
{
    Task<GroupDetail> GetOne(int id);
    Task<List<GroupDetail>> GetAll();
}

public class GroupQuery(ICreateSqliteConnection createSqliteConnection) : IGroupQuery
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

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
            _ = int.TryParse((string?)(reader[nameof(GroupDetail.Id)]?.ToString()), out int id);
            var path = reader[nameof(GroupDetail.Name)]?.ToString() ?? "";

            groups.Add(new() { Id = id, Name = path });
        }

        return groups;
    }

    public async Task<GroupDetail> GetOne(int id)
    {
        var tableName = $"{nameof(Group)}s";
        var group = new GroupDetail();
        var connection = this.createSqliteConnection.Execute();
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


using Infrastructure.Database;

namespace Infrastructure.IDEPath;

public interface IGetIDEPaths : IExecuteAsync<List<IDEPath>>;

public class GetIDEPaths(ICreateSqliteConnection createSqliteConnection) : IGetIDEPaths
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<List<IDEPath>> ExecuteAsync()
    {
        var tableName = $"{nameof(IDEPath)}s";
        var iDEPaths = new List<IDEPath>();
        var connection = this.createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(IDEPath.Id)]?.ToString(), out int id);
            var path = reader[nameof(IDEPath.Path)]?.ToString() ?? "";

            iDEPaths.Add(new() { Id = id, Path = path });
        }

        return iDEPaths;
    }
}


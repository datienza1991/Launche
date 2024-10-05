using Infrastructure.Database;

namespace ApplicationCore.Features.Basic.IDEPath;

public interface IGetIDEPaths : IExecuteAsync<List<Infrastructure.Models.IDEPath>>;

public class GetIDEPaths(ICreateSqliteConnection createSqliteConnection) : IGetIDEPaths
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<List<Infrastructure.Models.IDEPath>> ExecuteAsync()
    {
        var tableName = $"{nameof(Infrastructure.Models.IDEPath)}s";
        var iDEPaths = new List<Infrastructure.Models.IDEPath>();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(Infrastructure.Models.IDEPath.Id)]?.ToString(), out int id);
            var path = reader[nameof(Infrastructure.Models.IDEPath.Path)]?.ToString() ?? "";

            iDEPaths.Add(new() { Id = id, Path = path });
        }

        return iDEPaths;
    }
}


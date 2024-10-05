using Infrastructure.Database;

namespace ApplicationCore.Features.Basic.IDEPath;

public interface IGetIDEPath : IExecuteAsync<Infrastructure.Models.IDEPath>;

public class GetIDEPath(ICreateSqliteConnection createSqliteConnection) : IGetIDEPath
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<Infrastructure.Models.IDEPath> ExecuteAsync()
    {
        var tableName = $"{nameof(IDEPath)}s";
        var vsCodePath = new Infrastructure.Models.IDEPath();
        var connection = createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName} LIMIT 1";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(Infrastructure.Models.IDEPath.Id)]?.ToString(), out int id);
            var path = reader[nameof(Infrastructure.Models.IDEPath.Path)]?.ToString() ?? "";

            vsCodePath.Id = id;
            vsCodePath.Path = path;
        }

        return vsCodePath;
    }
}


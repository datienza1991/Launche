using UI.Database;

namespace UI.VsCodePath;

public interface IGetVsCodePath : IExecuteAsync<VsCodePath>;

public class GetVsCodePath(ICreateSqliteConnection createSqliteConnection) : IGetVsCodePath
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<VsCodePath> ExecuteAsync()
    {
        var vsCodePath = new VsCodePath();
        var connection = this.createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM VsCodePaths LIMIT 1";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(VsCodePath.Id)]?.ToString(), out int id);
            var path = reader[nameof(VsCodePath.Path)]?.ToString() ?? "";

            vsCodePath.Id = id;
            vsCodePath.Path = path;
        }

        return vsCodePath;
    }
}


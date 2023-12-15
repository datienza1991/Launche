using System.Data.SQLite;
using UI.Database;

namespace UI.VsCodePath;

public interface ISaveVsCodePath : IExecuteAsync<string, bool>;

public class SaveVsCodePath(ICreateSqliteConnection createSqliteConnection) : ISaveVsCodePath
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<bool> ExecuteAsync(string path)
    {
        var id = 1;
        using var connection = this.createSqliteConnection.Execute();
        connection.Open();

        string createTableSql = $"UPDATE VsCodePaths SET Path = @path WHERE Id = {id};";
        using var command = new SQLiteCommand(createTableSql, connection);
        command.Parameters.AddWithValue("@path", path);
        var rows = await command.ExecuteNonQueryAsync();

        return rows != 0;
    }
}


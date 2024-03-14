using System.Data.SQLite;
using UI.Database;

namespace UI.IDEPath;

public interface ISaveIDEPath : IExecuteAsync<string, bool>;

public class SaveIDEPath(ICreateSqliteConnection createSqliteConnection) : ISaveIDEPath
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<bool> ExecuteAsync(string path)
    {
        var tableName = $"{nameof(IDEPath)}s";
        using var connection = this.createSqliteConnection.Execute();
        connection.Open();

        string createTableSql = $"INSERT INTO IDEPaths ( Path ) VALUES ( @path );";
        using var command = new SQLiteCommand(createTableSql, connection);
        command.Parameters.AddWithValue("@path", path);
        var rows = await command.ExecuteNonQueryAsync();

        return rows != 0;
    }
}


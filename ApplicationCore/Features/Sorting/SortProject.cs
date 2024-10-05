using Infrastructure.Database;
using Infrastructure.Models;
using System.Data.SQLite;

namespace ApplicationCore.Features.Sorting;

public interface ISortProject
{
    Task<bool> SortUp(int sortId);
    Task<bool> SortDown(int sortIdrojectId);
}

public class SortProject(ICreateSqliteConnection createSqliteConnection) : ISortProject
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;
    private const string TABLE = $"{nameof(Project)}s";

    public async Task<bool> SortUp(int sortId)
    {
        using var connection = createSqliteConnection.Execute();

        connection.Open();

        string createTableSql = @$"
                update {TABLE}
                    set SortId = @SortIdTop + @SortIdDown - SortId
                    where SortId in (@SortIdTop, @SortIdDown)";

        using var command = new SQLiteCommand(createTableSql, connection);

        var top = sortId - 1;

        command.Parameters.AddWithValue("@SortIdTop", top);
        command.Parameters.AddWithValue("@SortIdDown", sortId);

        var rows = await command.ExecuteNonQueryAsync();
        return rows != 0;
    }

    public async Task<bool> SortDown(int sortId)
    {
        using var connection = createSqliteConnection.Execute();

        connection.Open();

        string createTableSql = @$"
                update {TABLE}
                    set SortId = @SortIdTop + @SortIdDown - SortId
                    where SortId in (@SortIdTop, @SortIdDown)";

        using var command = new SQLiteCommand(createTableSql, connection);

        var top = sortId + 1;

        command.Parameters.AddWithValue("@SortIdTop", top);
        command.Parameters.AddWithValue("@SortIdDown", sortId);

        var rows = await command.ExecuteNonQueryAsync();
        return rows != 0;
    }
}


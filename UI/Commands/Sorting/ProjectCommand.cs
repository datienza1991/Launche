using System.Data.SQLite;
using UI.Commands.Basic.Project;
using UI.Database;

namespace UI.Commands.Sorting;

public interface IProjectCommand
{
    Task<bool> SortUp(int sortId);
    Task<bool> SortDown(int sortIdrojectId);
}

public class ProjectCommand(ICreateSqliteConnection createSqliteConnection) : IProjectCommand
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


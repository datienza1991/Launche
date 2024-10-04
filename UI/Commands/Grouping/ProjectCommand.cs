using System.Data.SQLite;
using UI.Commands.Basic.Project;
using UI.Database;

namespace UI.Commands.Grouping;

public interface IProjectCommand
{
    Task<bool> Group(int projectId, int? groupId);
}

public class ProjectCommand(ICreateSqliteConnection createSqliteConnection) : IProjectCommand
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;
    private const string TABLE = $"{nameof(Project)}s";

    public async Task<bool> Group(int projectId, int? groupId)
    {
        using var connection = createSqliteConnection.Execute();

        connection.Open();

        string createTableSql = $"UPDATE {TABLE} SET  " +
            $"GroupId = @groupId " +
            $"WHERE Id = @id;";

        using var command = new SQLiteCommand(createTableSql, connection);

        command.Parameters.AddWithValue("@id", projectId);
        command.Parameters.AddWithValue("@groupId", projectId);

        var rows = await command.ExecuteNonQueryAsync();
        return rows != 0;
    }
}


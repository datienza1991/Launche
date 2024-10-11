using Infrastructure.Database;
using Infrastructure.Models;
using System.Data.SQLite;

namespace ApplicationCore.Features.Grouping;

public interface IGroupProject
{
    Task<bool> Group(long projectId, int? groupId);
}

public class GroupProject(ICreateSqliteConnection createSqliteConnection) : IGroupProject
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;
    private const string TABLE = $"{nameof(Project)}s";

    public async Task<bool> Group(long projectId, int? groupId)
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


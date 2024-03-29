﻿using UI.Database;

namespace UI.IDEPath;

public interface IGetIDEPath : IExecuteAsync<IDEPath>;

public class GetIDEPath(ICreateSqliteConnection createSqliteConnection) : IGetIDEPath
{
    private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

    public async Task<IDEPath> ExecuteAsync()
    {
        var tableName = $"{nameof(IDEPath)}s";
        var vsCodePath = new IDEPath();
        var connection = this.createSqliteConnection.Execute();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName} LIMIT 1";
        using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            _ = int.TryParse(reader[nameof(IDEPath.Id)]?.ToString(), out int id);
            var path = reader[nameof(IDEPath.Path)]?.ToString() ?? "";

            vsCodePath.Id = id;
            vsCodePath.Path = path;
        }

        return vsCodePath;
    }
}


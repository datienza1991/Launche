using System.Data.SQLite;

namespace UI.Database
{
    public interface IInitializedDatabaseMigration : IExecute;

    public class InitializedDatabaseMigration(
        ICreateSqliteConnection createSqliteConnection,
        ICheckVersionTableIfExists checkVersionTableIfExists, 
        ICreateVersionsDbTable createVersionsDbTable, 
        ICheckVersionIfExists checkVersionIfExists,
        IAddTableSchemaVersion addTableSchemaVersion
        ) : IInitializedDatabaseMigration
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;
        private readonly ICheckVersionTableIfExists checkVersionTableIfExists = checkVersionTableIfExists;
        private readonly ICreateVersionsDbTable createVersionsDbTable = createVersionsDbTable;
        private readonly ICheckVersionIfExists checkVersionIfExists = checkVersionIfExists;
        private readonly IAddTableSchemaVersion addTableSchemaVersion = addTableSchemaVersion;

        public void Execute()
        {
            var isVersionTableExists = this.checkVersionTableIfExists.Execute();
            if (!isVersionTableExists)
            {
                this.createVersionsDbTable.Execute();
            }

            this.AddProjectPathsTable(1);
        }

        private void AddProjectPathsTable(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (!isVersionExists)
            {
                using var connection = this.createSqliteConnection.Execute();
                connection.Open();

                string createTableSql = "CREATE TABLE ProjectPaths (Id INTEGER PRIMARY KEY AUTOINCREMENT,Path TEXT)";
                using var command = new SQLiteCommand(createTableSql, connection);
                command.ExecuteNonQuery();


                this.addTableSchemaVersion.Execute(version);
            }
        }
    }
}

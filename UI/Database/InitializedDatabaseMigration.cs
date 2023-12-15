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
            this.AddVsCodePathsTable(2);
            this.SeedInitialVsCodePath(3);
            this.AddNameOnProjectPathsTable(4);
            this.SeedInitialProjectPaths(5);
            this.SeedNewProjectPath(6);
        }

        private void SeedNewProjectPath(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (isVersionExists) { return; }

            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            string createTableSql = "INSERT INTO ProjectPaths ( Path, Name ) VALUES ( 'New Project Path Name', 'This is Project Path' )";
            using var command = new SQLiteCommand(createTableSql, connection);
            command.ExecuteNonQuery();

            this.addTableSchemaVersion.Execute(version);

        }

        private void SeedInitialProjectPaths(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (isVersionExists) { return; }

            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            string createTableSql = "INSERT INTO ProjectPaths ( Path, Name ) VALUES ( '...', '...' )";
            using var command = new SQLiteCommand(createTableSql, connection);
            command.ExecuteNonQuery();

            this.addTableSchemaVersion.Execute(version);

        }

        private void AddNameOnProjectPathsTable(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (!isVersionExists)
            {
                using var connection = this.createSqliteConnection.Execute();
                connection.Open();

                string createTableSql = "ALTER TABLE ProjectPaths ADD Name TEXT";
                using var command = new SQLiteCommand(createTableSql, connection);
                command.ExecuteNonQuery();


                this.addTableSchemaVersion.Execute(version);
            }
        }

        private void SeedInitialVsCodePath(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (isVersionExists) { return; }

            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            string createTableSql = "INSERT INTO VsCodePaths ( Path ) VALUES ( '...' )";
            using var command = new SQLiteCommand(createTableSql, connection);
            command.ExecuteNonQuery();

            this.addTableSchemaVersion.Execute(version);

        }

        private void AddVsCodePathsTable(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (!isVersionExists)
            {
                using var connection = this.createSqliteConnection.Execute();
                connection.Open();

                string createTableSql = "CREATE TABLE VsCodePaths (Id INTEGER PRIMARY KEY AUTOINCREMENT,Path TEXT)";
                using var command = new SQLiteCommand(createTableSql, connection);
                command.ExecuteNonQuery();

                this.addTableSchemaVersion.Execute(version);
            }
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

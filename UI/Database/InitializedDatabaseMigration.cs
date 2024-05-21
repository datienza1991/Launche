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

        public async void Execute()
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
            this.RemoveAllProjectPaths(7);
            this.RenameVsCodePathsTableToIDEPaths(8);
            this.AddColumnIDEPathIdToProjectPaths(10);
            this.AddForeignIDEPathIdToProjectPaths(11);
            this.SeedDefaultIDEPathOnProjectPaths(12);
            this.SetIDEPathIdToNotNull(13);
            this.AddSortIdToProjectPaths(14);
            await this.UpdateSortIdToIncrement(15);
            this.AddFileNameField(16);
        }

        private void AddFileNameField(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (isVersionExists) { return; }

            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            string query = "ALTER TABLE ProjectPaths ADD Filename TEXT;";
            using var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();

            this.addTableSchemaVersion.Execute(version);
        }

        private async Task UpdateSortIdToIncrement(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (isVersionExists) { return; }

            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM ProjectPaths";
            using var reader = await command.ExecuteReaderAsync();
            var sortId = 1;

            while (reader.Read())
            {
                _ = int.TryParse(reader["Id"]?.ToString(), out int id);

                string query = "Update ProjectPaths Set SortId = @SortId WHERE Id = @Id";
                using var updateCommand = new SQLiteCommand(query, connection);
                updateCommand.Parameters.AddWithValue("@SortId", sortId);
                updateCommand.Parameters.AddWithValue("@Id", id);
                await updateCommand.ExecuteNonQueryAsync();
                sortId++;
            }

            this.addTableSchemaVersion.Execute(version);
        }

        private void AddSortIdToProjectPaths(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (isVersionExists) { return; }

            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            string query = "ALTER TABLE ProjectPaths ADD SortId INTEGER DEFAULT(0);";
            using var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();

            this.addTableSchemaVersion.Execute(version);
        }

        private void SetIDEPathIdToNotNull(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (isVersionExists) { return; }

            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            string query = @"
                CREATE TEMPORARY TABLE temp AS
                    SELECT *
                    FROM ProjectPaths;

                DROP TABLE ProjectPaths;

                CREATE TABLE ProjectPaths (
                	Id INTEGER PRIMARY KEY AUTOINCREMENT,
                	""Path"" TEXT,
                	Name TEXT,
                	IDEPathId INTEGER NOT NULL,
                	FOREIGN KEY (IDEPathId) REFERENCES IDEPaths (Id) ON UPDATE RESTRICT ON DELETE RESTRICT
                );

                INSERT INTO ProjectPaths
                SELECT *
                FROM temp;

                DROP TABLE temp;
                ";
            using var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();

            this.addTableSchemaVersion.Execute(version);
        }

        private void SeedDefaultIDEPathOnProjectPaths(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (isVersionExists) { return; }

            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            string query = @"UPDATE ProjectPaths SET IDEPathId  = (SELECT  Id FROM IDEPaths LIMIT 1);";
            using var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();

            this.addTableSchemaVersion.Execute(version);
        }

        private void AddForeignIDEPathIdToProjectPaths(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (isVersionExists) { return; }

            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            string query = @"
                CREATE TEMPORARY TABLE temp AS
                    SELECT *
                    FROM ProjectPaths;

                DROP TABLE ProjectPaths;

                CREATE TABLE ProjectPaths (
                	Id INTEGER PRIMARY KEY AUTOINCREMENT,
                	""Path"" TEXT,
                	Name TEXT,
                	IDEPathId INTEGER,
                	CONSTRAINT ProjectPaths_FK FOREIGN KEY (IDEPathId) REFERENCES IDEPaths(Id) ON DELETE RESTRICT
                );

                INSERT INTO ProjectPaths
                SELECT *
                FROM temp;

                DROP TABLE temp;
                ";
            using var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();

            this.addTableSchemaVersion.Execute(version);
        }

        private void AddColumnIDEPathIdToProjectPaths(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (isVersionExists) { return; }

            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            string query = "ALTER TABLE ProjectPaths ADD IDEPathId INTEGER;";
            using var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();

            this.addTableSchemaVersion.Execute(version);
        }

        private void RenameVsCodePathsTableToIDEPaths(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (isVersionExists) { return; }

            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            string query = "ALTER TABLE VsCodePaths RENAME TO IDEPaths;";
            using var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();

            this.addTableSchemaVersion.Execute(version);
        }

        private void RemoveAllProjectPaths(int version)
        {
            var isVersionExists = this.checkVersionIfExists.Execute(version);
            if (isVersionExists) { return; }

            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            string createTableSql = "DELETE FROM ProjectPaths;";
            using var command = new SQLiteCommand(createTableSql, connection);
            command.ExecuteNonQuery();

            this.addTableSchemaVersion.Execute(version);
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

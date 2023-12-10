using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Database
{
    public interface ICreateVersionsDbTable : IExecute;


    internal class CreateVersionsDbTable(ICreateSqliteConnection createSqliteConnection) : ICreateVersionsDbTable
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public void Execute()
        {
            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            string createTableSql = @"CREATE TABLE Versions (Id INTEGER PRIMARY KEY AUTOINCREMENT, Version TEXT)";
            using var command = new SQLiteCommand(createTableSql, connection);


            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}

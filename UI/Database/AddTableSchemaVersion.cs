using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UI.Database
{
    public interface IAddTableSchemaVersion : IExecute<int,int>;

    public class AddTableSchemaVersion(ICreateSqliteConnection createSqliteConnection) : IAddTableSchemaVersion
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public int Execute(int number)
        {
            using var connection = this.createSqliteConnection.Execute();
            connection.Open();

            string createTableSql = $"INSERT INTO Versions (Version) VALUES ({number})";
            using var command = new SQLiteCommand(createTableSql, connection);
            var rows = command.ExecuteNonQuery();

            return rows;
        }
    }
}

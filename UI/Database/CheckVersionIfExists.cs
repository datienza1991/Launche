using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UI.Database
{

    public interface ICheckVersionIfExists : IExecute<bool,int>;

    public class CheckVersionIfExists(ICreateSqliteConnection createSqliteConnection) : ICheckVersionIfExists
    {
        private readonly ICreateSqliteConnection createSqliteConnection = createSqliteConnection;

        public bool Execute(int versionNumber)
        {
            var checkVersionsIfExistsQuery = $"SELECT Version FROM Versions WHERE Version='{versionNumber}'";
            using var connection = this.createSqliteConnection.Execute();
            using var command = connection.CreateCommand();
            connection.Open();
            command.CommandText = checkVersionsIfExistsQuery;

            using var reader = command.ExecuteReader();

            return reader.HasRows;
        }
    }
}

﻿
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Database
{
    public interface ICreateSqliteConnection : IExecute<SQLiteConnection>;

    public class CreateSqliteConnection : ICreateSqliteConnection
    {
        public SQLiteConnection Execute()
        {
            var fullPath = Path.GetFullPath(".");
            var path = @$"{fullPath}\file.db";
            var connectionString = $"Data Source={path}";
            if (!System.IO.File.Exists(path))
            {
                Console.WriteLine("Creating Sync DB...");
                SQLiteConnection.CreateFile(path);
            }


            var connection = new SQLiteConnection(connectionString);


            return connection;
        }
    }
}

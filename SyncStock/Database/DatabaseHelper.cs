using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Database
{
    public class DatabaseHelper
    {
        private static readonly string _connectionString = "Server=localhost\\SQLEXPRESS;Database=SyncStock;Trusted_Connection=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}

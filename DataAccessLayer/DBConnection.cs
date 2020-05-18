using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    internal static class DBConnection
    {
        private static String connectionString =
            @"Data Source=localhost\sqlexpress;Initial Catalog=dq_inventory;Integrated Security=True";

        public static SqlConnection GetConnection()
        {
            var conn = new SqlConnection(connectionString);
            return conn;
        }
    }
}

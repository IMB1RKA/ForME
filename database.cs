using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.CodeDom;

namespace test_DataBase
{
    internal class DataBase
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=IMB1RKA-PC\SQLEXPRESS;Initial Catalog=helpme;Integrated Security=True");

        public void openConnction()
        {
            if(sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }
        public void closeConnction()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }
        public SqlConnection getConnection()
        {
            return sqlConnection;
        }
    }
}

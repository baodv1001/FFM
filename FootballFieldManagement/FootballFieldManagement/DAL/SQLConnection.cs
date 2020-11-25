using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.Models
{
    class SQLConnection
    {
        private string strConn;
        protected SqlConnection conn;
        public SQLConnection()
        {
            strConn = @"Data Source=ffm-dbs.database.windows.net;Initial Catalog=ffm-db;User ID=ffm;Password=Football2020";
            conn = new SqlConnection(strConn);
        }
    }
}

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
            strConn = @"Data Source=ffms.database.windows.net;Initial Catalog=ffms-dbs;User ID=trunghuynh2304;Password=FootballFieldManagement2020";
            conn = new SqlConnection(strConn);
        }
    }
}

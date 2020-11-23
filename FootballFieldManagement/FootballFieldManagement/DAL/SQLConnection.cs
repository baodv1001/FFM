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
            strConn = @"Data Source=(local);Initial Catalog=FootballFieldManagement;Integrated Security=True";
            conn = new SqlConnection(strConn);
        }
    }
}

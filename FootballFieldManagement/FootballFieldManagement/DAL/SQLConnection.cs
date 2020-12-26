using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace FootballFieldManagement.DAL
{
    public class SQLConnection
    {
        private string strConn;
        public SqlConnection conn;
        public SQLConnection()
        {
            strConn = ConfigurationManager.ConnectionStrings["FFM"].ConnectionString;
            conn = new SqlConnection(strConn);
        }
    }
}

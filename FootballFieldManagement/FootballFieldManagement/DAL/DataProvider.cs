using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.Models
{
    class DataProvider : SQLConnection
    {
        public DataTable LoadData(string tableName)
        {
            conn.Open();
            string sql = "SELECT * FROM " + tableName;
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }
}

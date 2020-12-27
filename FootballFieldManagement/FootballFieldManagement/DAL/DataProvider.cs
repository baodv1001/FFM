using FootballFieldManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FootballFieldManagement.DAL
{
    public class DataProvider : SQLConnection
    {
        public DataTable LoadData(string tableName)
        {
            DataTable dt = new DataTable();
            conn.Close();
            try
            {
                conn.Open();
                string sql = "SELECT * FROM " + tableName;
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                conn.Close();
                return dt;
            }
            catch
            {
                CustomMessageBox.Show("Mất kết nối đến cơ sở dữ liệu!");
                App.Current.Shutdown();

            }
            return dt;
        }
    }
}

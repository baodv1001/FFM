using FootballFieldManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.DAL
{
    class FieldInfoDAL :DataProvider
    {
        private static FieldInfoDAL instance;
        public static FieldInfoDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new FieldInfoDAL();
                return FieldInfoDAL.instance;
            }
            private set
            {
                FieldInfoDAL.instance = value;
            }
        }

        private FieldInfoDAL()
        {

        }
        public FieldInfo GetFieldInfo(string idFieldInfo)
        {
            try
            {
                conn.Open();
                string queryString = "select * from FieldInfo where idFieldInfo = " + idFieldInfo;

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                FieldInfo res = new FieldInfo(int.Parse(idFieldInfo), int.Parse(dataTable.Rows[0].ItemArray[1].ToString()),
                    DateTime.Parse(dataTable.Rows[0].ItemArray[2].ToString()), DateTime.Parse(dataTable.Rows[0].ItemArray[3].ToString()),
                    int.Parse(dataTable.Rows[0].ItemArray[4].ToString()), dataTable.Rows[0].ItemArray[5].ToString(), 
                    dataTable.Rows[0].ItemArray[6].ToString(), dataTable.Rows[0].ItemArray[7].ToString(), 
                    long.Parse(dataTable.Rows[0].ItemArray[8].ToString()));
                return res;
            }
            catch
            {
                return new FieldInfo();
            }
            finally
            {
                conn.Close();
            }
        }

        public List<FieldInfo> ConvertDBToList()
        {
            DataTable dt;
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            try
            {
                dt = LoadData("FieldInfo");
            }
            catch
            {
                dt = null;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                FieldInfo fieldInfo = new FieldInfo(int.Parse(dt.Rows[i].ItemArray[0].ToString()), int.Parse(dt.Rows[i].ItemArray[1].ToString()),
                    DateTime.Parse(dt.Rows[i].ItemArray[2].ToString()), DateTime.Parse(dt.Rows[i].ItemArray[3].ToString()),
                    int.Parse(dt.Rows[i].ItemArray[4].ToString()), dt.Rows[i].ItemArray[5].ToString(), dt.Rows[i].ItemArray[6].ToString(),
                    dt.Rows[i].ItemArray[7].ToString(), long.Parse(dt.Rows[i].ItemArray[8].ToString()));
                fieldInfos.Add(fieldInfo);
            }
            return fieldInfos;
        }

        //Update idField = null khi xóa Field
        public bool UpdateIdField(string idField)
        {
            try
            {
                conn.Open();
                string query = @"update FieldInfo set idField = NULL where idField = " + idField;
                SqlCommand command = new SqlCommand(query, conn);
                int rs = command.ExecuteNonQuery();
                if(rs >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}

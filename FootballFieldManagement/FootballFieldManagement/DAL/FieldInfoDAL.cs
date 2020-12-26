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
    class FieldInfoDAL : DataProvider
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
                    long.Parse(dataTable.Rows[0].ItemArray[8].ToString()), long.Parse(dataTable.Rows[0].ItemArray[9].ToString()));
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
                    dt.Rows[i].ItemArray[7].ToString(), long.Parse(dt.Rows[i].ItemArray[8].ToString()), long.Parse(dt.Rows[i].ItemArray[9].ToString()));
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
                if (rs >= 1)
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
        public List<FieldInfo> QueryFieldInfoPerDay(string year, string month, string day)
        {
            List<FieldInfo> res = new List<FieldInfo>();
            try
            {
                conn.Close();
                conn.Open();
                string queryString = @"Select idFieldInfo,idField, startingTime,endingTime,status 
                                       From FieldInfo
                                       Where year(startingTime)= @year and month(startingTime)= @month and day(startingTime)= @day 
                                       Order by startingTime,idField ASC ";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@day", day);
                command.ExecuteNonQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    FieldInfo fieldInfo = new FieldInfo(int.Parse(dt.Rows[i].ItemArray[0].ToString()), int.Parse(dt.Rows[i].ItemArray[1].ToString()), DateTime.Parse(dt.Rows[i].ItemArray[2].ToString()), DateTime.Parse(dt.Rows[i].ItemArray[3].ToString()), int.Parse(dt.Rows[i].ItemArray[4].ToString()), " ", " ", " ", 0, 0);
                    res.Add(fieldInfo);
                }
                return res;
            }
            catch
            {
                return res;
            }
            finally
            {
                conn.Close();
            }
        }
        public bool AddIntoDB(FieldInfo fieldInfo)
        {
            try
            {
                conn.Open();
                string query = "INSERT INTO FieldInfo(idFieldInfo,idField,startingTime,endingTime,status,phoneNumber,customerName,note,discount,price) " +
                               "VALUES(@idFieldInfo,@idField,@startingTime,@endingTime,@status,@phoneNumber,@customerName,@note,@discount,@price)";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@idFieldInfo", fieldInfo.IdFieldInfo);
                command.Parameters.AddWithValue("@idField", fieldInfo.IdField);
                command.Parameters.AddWithValue("@startingTime", fieldInfo.StartingTime);
                command.Parameters.AddWithValue("@endingTime", fieldInfo.EndingTime);
                command.Parameters.AddWithValue("@status", fieldInfo.Status);
                command.Parameters.AddWithValue("@phoneNumber", fieldInfo.PhoneNumber);
                command.Parameters.AddWithValue("@customerName", fieldInfo.CustomerName);
                command.Parameters.AddWithValue("@note", fieldInfo.Note);
                command.Parameters.AddWithValue("@discount", fieldInfo.Discount);
                command.Parameters.AddWithValue("@price", fieldInfo.Price);
                int rs = command.ExecuteNonQuery();
                return true;
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
        public bool UpdateOnDB(FieldInfo fieldInfo)
        {
            try
            {
                conn.Open();
                string query = @"Update FieldInfo
                                 Set phoneNumber=@phoneNumber,status=@status,customerName=@customerName,note=@note,discount=@discount
                                 Where idFieldInfo = @idFieldInfo";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@status", fieldInfo.Status);
                command.Parameters.AddWithValue("@phoneNumber", fieldInfo.PhoneNumber);
                command.Parameters.AddWithValue("@customerName", fieldInfo.CustomerName);
                command.Parameters.AddWithValue("@note", fieldInfo.Note);
                command.Parameters.AddWithValue("@discount", fieldInfo.Discount);
                command.Parameters.AddWithValue("@idFieldInfo", fieldInfo.IdFieldInfo);
                command.ExecuteNonQuery();
                return true;
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
        public bool DeleteFromDB(string idFieldInfo)
        {
            try
            {
                conn.Open();
                string query = @"Delete from FieldInfo
                                Where idFieldInfo=" + idFieldInfo;
                SqlCommand command = new SqlCommand(query, conn);
                command.ExecuteNonQuery();
                return true;
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
        public int GetMaxIdFieldInfo()
        {
            int res = 0;
            try
            {
                conn.Open();
                string queryString = "select max(idFieldInfo) from FieldInfo";

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                res = int.Parse(dataTable.Rows[0].ItemArray[0].ToString());
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
            return res;
        }
        public List<FieldInfo> GetFieldInfoByIdField(string idField)
        {
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            try
            {
                conn.Open();
                string queryString = "select * from FieldInfo where (status=1 or status=2) and idField=" + idField;

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    FieldInfo fieldInfo = new FieldInfo(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), int.Parse(dataTable.Rows[i].ItemArray[1].ToString()), DateTime.Parse(dataTable.Rows[i].ItemArray[2].ToString()), DateTime.Parse(dataTable.Rows[i].ItemArray[3].ToString()), int.Parse(dataTable.Rows[i].ItemArray[4].ToString()), " ", " ", " ", 0, 0);
                    fieldInfos.Add(fieldInfo);
                }
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
            return fieldInfos;
        }
        public long GetPriceByFieldInfoId(string idFieldInfo)
        {
            int res = 0;
            try
            {
                conn.Open();
                string queryString = "select price from FieldInfo where idFieldInfo = " + idFieldInfo;

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                res = int.Parse(dataTable.Rows[0].ItemArray[0].ToString());
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
            return res;
        }
    }
}
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
    class TimeFrameDAL : DataProvider
    {
        private static TimeFrameDAL instance;
        public static TimeFrameDAL Instance
        {
            get
            {
                if (instance == null)
                    return new TimeFrameDAL();
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        public List<TimeFrame> ConvertDBToList()
        {
            conn.Open();
            List<TimeFrame> timeFrames = new List<TimeFrame>();
            DataTable dt;
            try
            {
                dt = LoadData("TimeFrame");
            }
            catch
            {
                conn.Close();
                dt = LoadData("TimFrame");
            }
            DataView dv = dt.DefaultView;
            dv.Sort = "startTime ASC";
            dt = dv.ToTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                long price = -1;
                if (dt.Rows[i].ItemArray[4].ToString() != "")
                {
                    price = long.Parse(dt.Rows[i].ItemArray[4].ToString());
                }
                TimeFrame tmp = new TimeFrame(int.Parse(dt.Rows[i].ItemArray[0].ToString()), dt.Rows[i].ItemArray[1].ToString(),
                    dt.Rows[i].ItemArray[2].ToString(), int.Parse(dt.Rows[i].ItemArray[3].ToString()), price);
                timeFrames.Add(tmp);
            }
            return timeFrames;
        }
        public bool AddTimeFrame(TimeFrame time)
        {
            conn.Open();
            try
            {
                string query = @"insert into TimeFrame(id, startTime, endTime, fieldType, price) values(@id, @startTime, @endTime, @fieldType, @price)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", time.Id.ToString());
                cmd.Parameters.AddWithValue("@startTime", time.StartTime);
                cmd.Parameters.AddWithValue("@endTime", time.EndTime);
                cmd.Parameters.AddWithValue("@fieldType", time.FieldType);
                cmd.Parameters.AddWithValue("@price", time.Price);
                int rs = cmd.ExecuteNonQuery();
                if (rs == 1)
                    return true;
                else
                    return false;
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

        //Xóa tất cả những khung giờ có loại sân được truyền vào
        public bool DeleteFieldType(string fieldType)
        {
            try
            {
                conn.Open();
                string query = @"delete from TimeFrame where fieldType = " + fieldType;
                SqlCommand cmd = new SqlCommand(query, conn);
                int rs = cmd.ExecuteNonQuery();
                if (rs < 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return true;
            }
            finally
            {
                conn.Close();
            }
        }

        public bool DeleteTimeFrame(string startTime, string endTime)
        {
            try
            {
                conn.Open();
                string query = @"delete from TimeFrame where startTime = @startTime and endTime = @endTime";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@startTime", startTime);
                cmd.Parameters.AddWithValue("@endTime", endTime);
                int rs = cmd.ExecuteNonQuery();
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
        public bool ClearData()
        {
            try
            {
                conn.Open();
                string query = @"delete from TimeFrame";
                SqlCommand cmd = new SqlCommand(query, conn);
                int rs = cmd.ExecuteNonQuery();
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
        public string GetPrice(int id)
        {
            try
            {
                conn.Open();
                string query = @"select price from TimeFrame where id = " + id.ToString();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows[0].ItemArray[0].ToString() == "")
                {
                    return null;
                }
                else
                {
                    return dt.Rows[0].ItemArray[0].ToString();
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
        }
        public int GetIdMax()
        {
            try
            {
                conn.Open();
                string query = @"select max(id) from TimeFrame";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows[0].ItemArray[0].ToString() == "")
                {
                    return 0;
                }
                else
                {
                    return   int.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
            }
            catch
            {
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }
        public List<TimeFrame> GetTimeFrame( )
        {
            try
            {
                conn.Open();
                string query = @"select startTime, endTime from TimeFrame group by startTime, endTime";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                List<TimeFrame> timeFrames = new List<TimeFrame>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TimeFrame tmp = new TimeFrame(-1, dt.Rows[i].ItemArray[0].ToString(),
                        dt.Rows[i].ItemArray[1].ToString(), -1, -1);
                    timeFrames.Add(tmp);
                }
                return timeFrames;
            }
            catch
            {
                return new List<TimeFrame>();
            }
            finally
            {
                conn.Close();
            }
        }

    }
}

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
    class AttendanceDAL : DataProvider
    {
        private static AttendanceDAL instance;
        public static AttendanceDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new AttendanceDAL();
                return AttendanceDAL.instance;
            }
            private set
            {
                AttendanceDAL.instance = value;
            }
        }
        public AttendanceDAL()
        {

        }
        public int GetMonth()
        {
            try
            {
                conn.Open();
                string query = "SELECT distinct(month) FROM Attendance";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return int.Parse(dt.Rows[0].ItemArray[0].ToString());
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
        public List<Attendance> CovertToList()
        {
            DataTable dt;
            List<Attendance> attendances = new List<Attendance>();
            try
            {
                dt = LoadData("Attendance");
            }
            catch
            {
                conn.Close();
                dt = LoadData("Attendance");
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Attendance attendance = new Attendance(int.Parse(dt.Rows[i].ItemArray[0].ToString()), int.Parse(dt.Rows[i].ItemArray[1].ToString()), int.Parse(dt.Rows[i].ItemArray[2].ToString()));
                attendances.Add(attendance);
            }
            return attendances;
        }
        public bool AddDay(Attendance attendance)
        {
            try
            {
                conn.Open();
                string query = "INSERT INTO Attendance(dayInMonth, idEmployee,month) VALUES(@dayInMonth, @idEmployee, @month)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dayInMonth", attendance.DayInMonth.ToString());
                cmd.Parameters.AddWithValue("@idEmployee", attendance.IdEmployee);
                cmd.Parameters.AddWithValue("@month", attendance.Month);
                if (cmd.ExecuteNonQuery() != 1)
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
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public List<int> WorkedDay(string id)
        {
            try
            {
                conn.Open();
                string query = "SELECT dayInMonth FROM Attendance WHERE idEmployee = " + id;
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                List<int> workedDay = new List<int>();
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    workedDay.Add(int.Parse(dt.Rows[i].ItemArray[0].ToString()));
                }
                return workedDay;
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
        public bool DeleteData()
        {
            try
            {
                conn.Open();
                string query = "DELETE FROM Attendance";
                SqlCommand cmd = new SqlCommand(query, conn);
                if (cmd.ExecuteNonQuery() != 1)
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
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        // Đếm số lượng ngày đi làm của nhân viên
        public int GetCount(string id)
        {
            try
            {
                conn.Open();
                string query = "SELECT count(dayInMonth) FROM Attendance WHERE idEmployee = " + id;
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return int.Parse(dt.Rows[0].ItemArray[0].ToString());
            }
            catch
            {
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }

        //Xóa ngày làm của nhân viên khi xóa nhân viên

        public bool DeleteAttendance(string idEmployee)
        {
            try
            {
                conn.Open();
                string query = "DELETE FROM Attendance where idEmployee = " + idEmployee;
                SqlCommand cmd = new SqlCommand(query, conn);
                int rs = cmd.ExecuteNonQuery();
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
    }
}

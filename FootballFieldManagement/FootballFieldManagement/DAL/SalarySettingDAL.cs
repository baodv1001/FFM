using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballFieldManagement.Models;

namespace FootballFieldManagement.DAL
{
    class SalarySettingDAL : DataProvider
    {
        private static SalarySettingDAL instance;

        public static SalarySettingDAL Instance
        {
            get
            {
                if (instance == null)
                    return new SalarySettingDAL();
                return instance;
            }
            private set { instance = value; }
        }
        public List<SalarySetting> ConvertDBToList()
        {
            DataTable dt;
            List<SalarySetting> tmp = new List<SalarySetting>();
            try
            {

                dt = LoadData("SalarySetting");
            }
            catch
            {
                conn.Close();
                dt = LoadData("SalarySetting");
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SalarySetting newItem = new SalarySetting(long.Parse(dt.Rows[i].ItemArray[0].ToString()), long.Parse(dt.Rows[i].ItemArray[1].ToString()), long.Parse(dt.Rows[i].ItemArray[2].ToString()), dt.Rows[i].ItemArray[3].ToString(), int.Parse(dt.Rows[i].ItemArray[4].ToString()));
                tmp.Add(newItem);
            }
            return tmp;
        }

        public bool AddIntoDB(SalarySetting salarySetting)
        {
            try
            {
                conn.Open();
                string query = @"insert into SalarySetting (salaryBase, moneyPerShift, moneyPerFault, typeEmployee, standardWorkDays) " +
                                "values(@salaryBase, @moneyPerShift, @moneyPerFault, @typeEmployee, @standardWorkDays)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@salaryBase", salarySetting.SalaryBase.ToString());
                cmd.Parameters.AddWithValue("@moneyPerShift", salarySetting.MoneyPerShift.ToString());
                cmd.Parameters.AddWithValue("@moneyPerFault", salarySetting.MoneyPerFault.ToString());
                cmd.Parameters.AddWithValue("@typeEmployee", salarySetting.TypeEmployee);
                cmd.Parameters.AddWithValue("@standardWorkDays", salarySetting.StandardWorkDays.ToString());
                if (cmd.ExecuteNonQuery() < 1)
                    return false;
                else
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

        public bool UpdateDB(SalarySetting salarySetting)
        {
            try
            {
                conn.Open();
                string query = @"update SalarySetting set salaryBase = @salaryBase,moneyPerShift = @moneyPerShift,moneyPerFault = @moneyPerFault, standardWorkDays = @standardWorkDays where typeEmployee = N'" + salarySetting.TypeEmployee + "'";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@salaryBase", salarySetting.SalaryBase.ToString());
                cmd.Parameters.AddWithValue("@moneyPerShift", salarySetting.MoneyPerShift.ToString());
                cmd.Parameters.AddWithValue("@moneyPerFault", salarySetting.MoneyPerFault.ToString());
                cmd.Parameters.AddWithValue("@standardWorkDays", salarySetting.StandardWorkDays.ToString());
                if (cmd.ExecuteNonQuery() < 1)
                    return false;
                else
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

        public SalarySetting GetSalarySettings(string typeEmployee) // lấy ra nhân viên có chức vụ 
        {
            try
            {
                conn.Open();
                string query = @"select * from SalarySetting where typeEmployee = N'" + typeEmployee + "'";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if(dt.Rows.Count > 0)
                {
                    SalarySetting newItem = new SalarySetting(long.Parse(dt.Rows[0].ItemArray[0].ToString()),
                   long.Parse(dt.Rows[0].ItemArray[1].ToString()), long.Parse(dt.Rows[0].ItemArray[2].ToString()),
                   dt.Rows[0].ItemArray[3].ToString(), int.Parse(dt.Rows[0].ItemArray[4].ToString()));
                    return newItem;
                }
                else
                {
                    return null;
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
    }
}

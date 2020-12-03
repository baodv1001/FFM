using FootballFieldManagement.Models;
using FootballFieldManegement.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FootballFieldManagement.DAL
{
    class SalaryDAL : DataProvider
    {
        private static SalaryDAL instance;

        public static SalaryDAL Instance
        {
            get { if (instance == null) instance = new SalaryDAL(); return SalaryDAL.instance; }
            private set { SalaryDAL.instance = value; }
        }
        public SalaryDAL()
        {

        }
        public List<Salary> ConvertDBToList()
        {
            DataTable dt;
            List<Salary> salaries = new List<Salary>();
            try
            {

                dt = LoadData("Salary");
            }
            catch
            {
                conn.Close();
                dt = LoadData("Salary");
            }
            // sort increase
            DataView dv = dt.DefaultView;
            dv.Sort = "idEmployee ASC";
            dt = dv.ToTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Salary salary = new Salary(long.Parse(dt.Rows[i].ItemArray[0].ToString()), int.Parse(dt.Rows[i].ItemArray[1].ToString()), long.Parse(dt.Rows[i].ItemArray[2].ToString()), int.Parse(dt.Rows[i].ItemArray[3].ToString()), long.Parse(dt.Rows[i].ItemArray[4].ToString()), int.Parse(dt.Rows[i].ItemArray[5].ToString()), long.Parse(dt.Rows[i].ItemArray[6].ToString()), int.Parse(dt.Rows[i].ItemArray[7].ToString()));
                salaries.Add(salary);
            }
            //conn.Close();
            return salaries;
        }
        public bool ResetSalary(Salary salary)
        {
            try
            {
                conn.Open();
                string query = "update Salary  set salaryBasic=@salaryBasic,moneyPerShift=@moneyPerShift,moneyPerFault=@moneyPerFault,standardWorkDays = @standardWorkDays where idEmployee=" + salary.IdEmployee;
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@salaryBasic", salary.SalaryBasic.ToString());
                cmd.Parameters.AddWithValue("@moneyPerShift", salary.MoneyPerShift.ToString());
                cmd.Parameters.AddWithValue("@moneyPerFault", salary.MoneyPerFault.ToString());
                cmd.Parameters.AddWithValue("@standardWorkDays", salary.StandardWorkDays.ToString());
                int rs = cmd.ExecuteNonQuery();
                if (rs != 1)
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
        public bool UpdateQuantity(Salary salary)
        {
            try
            {
                conn.Open();
                string query = "update Salary  set numOfShift=@numOfShift,numOfFault=@numOfFault where idEmployee=" + salary.IdEmployee;
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@numOfShift", salary.NumOfShift.ToString());
                cmd.Parameters.AddWithValue("@numOfFault", salary.NumOfFault.ToString());
                int rs = cmd.ExecuteNonQuery();
                if (rs != 1)
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
        public bool UpdateTotalSalary(Salary salary)
        {
            try
            {
                conn.Open();
                string query = "update Salary  set totalSalary=@totalSalary where idEmployee=" + salary.IdEmployee;
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@totalSalary", salary.TotalSalary.ToString());
                int rs = cmd.ExecuteNonQuery();
                if (rs != 1)
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
        public bool AddIntoDB(Salary salary)
        {
            try
            {
                conn.Open();
                string query = "INSERT INTO Salary(salaryBasic, numOfShift, moneyPerShift, numOfFault, moneyPerFault, idEmployee, totalSalary,standardWorkDays) VALUES(@salaryBasic, @numOfShift, @moneyPerShift, @numOfFault, @moneyPerFault, @idEmployee, @totalSalary,@standardWorkDays)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@salaryBasic", salary.SalaryBasic.ToString());
                cmd.Parameters.AddWithValue("@numOfShift", salary.NumOfShift.ToString());
                cmd.Parameters.AddWithValue("@moneyPerShift", salary.MoneyPerShift.ToString());
                cmd.Parameters.AddWithValue("@numOfFault", salary.NumOfFault.ToString());
                cmd.Parameters.AddWithValue("@moneyPerFault", salary.MoneyPerFault.ToString());
                cmd.Parameters.AddWithValue("@idEmployee", salary.IdEmployee.ToString());
                cmd.Parameters.AddWithValue("@totalSalary", salary.TotalSalary.ToString());
                cmd.Parameters.AddWithValue("@standardWorkDays", salary.StandardWorkDays.ToString());
                int rs = cmd.ExecuteNonQuery();
                if (rs != 1)
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
        public bool DeleteSalary(string id)
        {
            try
            {
                conn.Open();
                string query = "delete from Salary where idEmployee = " + id;
                SqlCommand command = new SqlCommand(query, conn);
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
        public string GetPosition(string id)
        {
            try
            {
                conn.Open();
                string query = "select position from Employee where idEmployee = " + id;
                SqlCommand command = new SqlCommand(query, conn);
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);
                return dt.Rows[0].ItemArray[0].ToString();
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

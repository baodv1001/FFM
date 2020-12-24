using FootballFieldManagement.Models;
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
                int idAccount = -1;
                if (dt.Rows[i].ItemArray[4].ToString() != "")
                {
                    idAccount = int.Parse(dt.Rows[i].ItemArray[4].ToString());
                }
                DateTime datePay = new DateTime();
                if (dt.Rows[i].ItemArray[5].ToString() != "")
                {
                    datePay = DateTime.Parse(dt.Rows[i].ItemArray[5].ToString());
                }
                Salary salary = new Salary(int.Parse(dt.Rows[i].ItemArray[0].ToString()), int.Parse(dt.Rows[i].ItemArray[1].ToString()),
                    int.Parse(dt.Rows[i].ItemArray[2].ToString()), long.Parse(dt.Rows[i].ItemArray[3].ToString()), idAccount,
                    datePay, DateTime.Parse(dt.Rows[i].ItemArray[6].ToString()));
                salaries.Add(salary);
            }
            //conn.Close();
            return salaries;
        }
        public bool AddIntoDB(Salary salary)
        {
            try
            {
                conn.Open();
                string query = @"insert into Salary (idEmployee, numOfShift, numOfFault,totalSalary, salaryMonth) values(@idEmployee, @numOfShift, @numOfFault,@totalSalary, @salaryMonth)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idEmployee", salary.IdEmployee.ToString());
                cmd.Parameters.AddWithValue("@numOfShift", salary.NumOfShift.ToString());
                cmd.Parameters.AddWithValue("@numOfFault", salary.NumOfFault.ToString());
                cmd.Parameters.AddWithValue("@totalSalary", salary.TotalSalary.ToString());
                cmd.Parameters.AddWithValue("@salaryMonth", salary.SalaryMonth);
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
        public bool UpdateQuantity(Salary salary)
        {
            try
            {
                conn.Open();
                string query = "update Salary  set numOfShift=@numOfShift,numOfFault=@numOfFault where idEmployee= @idEmployee and month(salaryMonth) = @month and year(salaryMonth) = @year";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@numOfShift", salary.NumOfShift.ToString());
                cmd.Parameters.AddWithValue("@numOfFault", salary.NumOfFault.ToString());
                cmd.Parameters.AddWithValue("@idEmployee", salary.IdEmployee.ToString());
                cmd.Parameters.AddWithValue("@month", salary.SalaryMonth.Month.ToString());
                cmd.Parameters.AddWithValue("@year", salary.SalaryMonth.Year.ToString());
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
                string query = "update Salary  set totalSalary=@totalSalary where idEmployee=@idEmployee and month(salaryMonth) = @month and year(salaryMonth) = @year";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@totalSalary", salary.TotalSalary.ToString());
                cmd.Parameters.AddWithValue("@idEmployee", salary.IdEmployee.ToString());
                cmd.Parameters.AddWithValue("@month", salary.SalaryMonth.Month.ToString());
                cmd.Parameters.AddWithValue("@year", salary.SalaryMonth.Year.ToString());
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
        public bool isExit(string idEmployee, DateTime date)
        {
            try
            {
                conn.Open();
                string query = @"select * from Salary where idEmployee = @idEmployee and  month(salaryMonth) = @month and year(salaryMonth) = @year";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idEmployee", idEmployee.ToString());
                cmd.Parameters.AddWithValue("@month", date.Month.ToString());
                cmd.Parameters.AddWithValue("@year", date.Year.ToString());
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count < 1)
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
                MessageBox.Show("Lỗi!");
                return true;
            }
            finally
            {
                conn.Close();
            }
        }

        /*public List<Salary> GetSalaryInfoById(string idSalaryRecord)
        {
            List<Salary> listSalaryInfo = new List<Salary>();
            try
            {
                conn.Open();
                string queryString = "select * from BillInfo where idBill = " + idSalaryRecord;
                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Salary salary = new Salary(long.Parse(dataTable.Rows[i].ItemArray[0].ToString()), int.Parse(dataTable.Rows[i].ItemArray[1].ToString()),
                        long.Parse(dataTable.Rows[i].ItemArray[2].ToString()), int.Parse(dataTable.Rows[i].ItemArray[3].ToString()),
                        long.Parse(dataTable.Rows[i].ItemArray[4].ToString()), int.Parse(dataTable.Rows[i].ItemArray[5].ToString()),
                        long.Parse(dataTable.Rows[i].ItemArray[6].ToString()), int.Parse(dataTable.Rows[i].ItemArray[7].ToString()));
                    listSalaryInfo.Add(salary);
                }
                return listSalaryInfo;
            }
            catch
            {
                return new List<Salary>();
            }
            finally
            {
                conn.Close();
            }
        }*/
    }
}

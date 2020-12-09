using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballFieldManagement.Models;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Collections.ObjectModel;
using FootballFieldManagement.DAL;
using System.IO;

namespace FootballFieldManegement.DAL
{
    class EmployeeDAL : DataProvider
    {

        private static EmployeeDAL instance;

        public static EmployeeDAL Instance
        {
            get { if (instance == null) instance = new EmployeeDAL(); return EmployeeDAL.instance; }
            private set { EmployeeDAL.instance = value; }
        }

        private EmployeeDAL()
        {

        }
        public List<Employee> ConvertDBToList()
        {
            DataTable dt;
            List<Employee> employees = new List<Employee>();
            try
            {

                dt = LoadData("Employee");
            }
            catch
            {
                conn.Close();
                dt = LoadData("Employee");
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int idAccount = -1;
                if (dt.Rows[i].ItemArray[9].ToString() != "")
                {
                    idAccount = int.Parse(dt.Rows[i].ItemArray[9].ToString());
                }
                Employee employee = new Employee(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[2].ToString(),
                    dt.Rows[i].ItemArray[3].ToString(), dt.Rows[i].ItemArray[4].ToString(),
                    DateTime.Parse(dt.Rows[i].ItemArray[5].ToString()), double.Parse(dt.Rows[i].ItemArray[6].ToString()),
                    dt.Rows[i].ItemArray[7].ToString(), DateTime.Parse(dt.Rows[i].ItemArray[8].ToString()),
                    idAccount, Convert.FromBase64String(dt.Rows[i].ItemArray[10].ToString()));
                employees.Add(employee);
            }
            //conn.Close();
            return employees;
        }
        public bool UpdateIdAccount(Employee employee)
        {
            try
            {
                conn.Open();
                string query;
                if (employee.IdAccount != -1)
                    query = "update Employee set idAccount = " + employee.IdAccount + " where idEmployee = @idEmployee";
                else
                    query = "update Employee set idAccount = NULL where idEmployee = @idEmployee";

                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@idEmployee", employee.IdEmployee);

                int rs = command.ExecuteNonQuery();
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
        public bool AddIntoDB(Employee employee)
        {
            try
            {
                conn.Open();
                string query = "insert into Employee( idEmployee,name,gender,phonenumber,address,dateofBirth,salary,position,startingdate,imageFile) values(@idEmployee,@name,@gender,@phonenumber,@address,@dateofBirth,@salary,@position,@startingdate,@imageFile)";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@idEmployee", employee.IdEmployee);
                command.Parameters.AddWithValue("@name", employee.Name);
                command.Parameters.AddWithValue("@gender", employee.Gender);
                command.Parameters.AddWithValue("@phonenumber", employee.Phonenumber);
                command.Parameters.AddWithValue("@address", employee.Address);
                command.Parameters.AddWithValue("@dateofBirth", employee.DateOfBirth);
                command.Parameters.AddWithValue("@salary", employee.Salary.ToString());
                command.Parameters.AddWithValue("@position", employee.Position);
                command.Parameters.AddWithValue("@startingdate", employee.Startingdate);
                command.Parameters.AddWithValue("@imageFile", Convert.ToBase64String(employee.ImageFile));
                int rs = command.ExecuteNonQuery();
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
        public bool UpdateOnDB(Employee employee)
        {
            try
            {
                conn.Open();
                string query = "update Employee  set name=@name,gender=@gender,phonenumber=@phonenumber,address=@address,dateofBirth=@dateofBirth,salary=@salary,position=@position,startingdate=@startingdate,imageFile=@imageFile where idEmployee=" + employee.IdEmployee;
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@name", employee.Name);
                command.Parameters.AddWithValue("@gender", employee.Gender);
                command.Parameters.AddWithValue("@phonenumber", employee.Phonenumber);
                command.Parameters.AddWithValue("@address", employee.Address);
                command.Parameters.AddWithValue("@dateofBirth", employee.DateOfBirth);
                command.Parameters.AddWithValue("@salary", employee.Salary.ToString());
                command.Parameters.AddWithValue("@position", employee.Position);
                command.Parameters.AddWithValue("@startingdate", employee.Startingdate);
                command.Parameters.AddWithValue("@imageFile", Convert.ToBase64String(employee.ImageFile));
                int rs = command.ExecuteNonQuery();
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
        public bool DeleteEmployee(Employee employee)
        {
            try
            {
                conn.Open();
                string query = "delete from Employee where idEmployee = " + employee.IdEmployee.ToString();
                SqlCommand command = new SqlCommand(query, conn);
                if (command.ExecuteNonQuery() > 0)
                    return true;
                else
                {
                    return false;
                }
            }
            catch
            {
                MessageBox.Show("Xóa thất bại!");
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public void AddEmployee(Employee employee)
        {
            if (ConvertDBToList().Count == 0 || employee.IdEmployee > ConvertDBToList()[ConvertDBToList().Count - 1].IdEmployee)
            {
                if (AddIntoDB(employee))
                    MessageBox.Show("Thêm thành công!");
                else
                    MessageBox.Show("Thêm thất bại!");
            }
            else
            {
                if (UpdateOnDB(employee))
                    MessageBox.Show("Cập nhật thành công!");
                else
                    MessageBox.Show("Cập nhật thất bại!");
            }
            //conn.Close();
        }
        public Employee GetEmployee(string idEmployee) // Lấy thông tin khi biết id nhân viên
        {
            foreach (var employee in ConvertDBToList())
            {
                if (employee.IdEmployee.ToString() == idEmployee)
                {
                    return employee;
                }
            }
            return null;
        }
    }
}

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

namespace FootballFieldManegement.DAL
{
    class EmployeeDAL:DataProvider
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
                Employee employee = new Employee(int.Parse(dt.Rows[i].ItemArray[0].ToString()), dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[2].ToString(), dt.Rows[i].ItemArray[3].ToString(), dt.Rows[i].ItemArray[4].ToString(), DateTime.Parse(dt.Rows[i].ItemArray[5].ToString()), double.Parse(dt.Rows[i].ItemArray[6].ToString()), dt.Rows[i].ItemArray[7].ToString(), DateTime.Parse(dt.Rows[i].ItemArray[8].ToString()), 1);
                employees.Add(employee);
            }
            //conn.Close();
            return employees;
        }
        public void AddIntoDB(Employee employee)
        {
            try
            {
                conn.Open();
                string query = "insert into Employee( name,gender,phonenumber,address,dateofBirth,salary,position,startingdate,idAccount) values(@name,@gender,@phonenumber,@address,@dateofBirth,@salary,@position,@startingdate,@idAccount)";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@name", employee.Name);
                command.Parameters.AddWithValue("@gender", employee.Gender);
                command.Parameters.AddWithValue("@phonenumber", employee.Phonenumber);
                command.Parameters.AddWithValue("@address", employee.Address);
                command.Parameters.AddWithValue("@dateofBirth", employee.DateOfBirth.ToString());
                command.Parameters.AddWithValue("@salary", employee.Salary.ToString());
                command.Parameters.AddWithValue("@position", employee.Position);
                command.Parameters.AddWithValue("@startingdate", employee.Startingdate.ToString());
                command.Parameters.AddWithValue("@idAccount", employee.IdAccount.ToString());
                int rs = command.ExecuteNonQuery();
                if (rs != 1)
                {
                    throw new Exception("Failed Query");
                }
                else
                {
                    MessageBox.Show("Đã thêm thành công");
                }
            }
            catch
            {
                MessageBox.Show("Cập nhật thất bại");
            }
            finally
            {
                conn.Close();
            }
        }
        public void UpdateOnDB(Employee employee)
        {
            try
            {
                conn.Open();
                string query = "update Employee  set name=@name,gender=@gender,phonenumber=@phonenumber,address=@address,dateofBirth=@dateofBirth,salary=@salary,position=@position,startingdate=@startingdate,idAccount=@idAccount where idEmployee=" + employee.IdEmployee;
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@name", employee.Name);
                command.Parameters.AddWithValue("@gender", employee.Gender);
                command.Parameters.AddWithValue("@phonenumber", employee.Phonenumber);
                command.Parameters.AddWithValue("@address", employee.Address);
                command.Parameters.AddWithValue("@dateofBirth", employee.DateOfBirth.ToString());
                command.Parameters.AddWithValue("@salary", employee.Salary.ToString());
                command.Parameters.AddWithValue("@position", employee.Position);
                command.Parameters.AddWithValue("@startingdate", employee.Startingdate.ToString());
                command.Parameters.AddWithValue("@idAccount", employee.IdAccount.ToString());
                int rs = command.ExecuteNonQuery();
                if (rs != 1)
                {
                    throw new Exception("Failed Query");
                }
                else
                {
                    MessageBox.Show("Cập nhật thành công!");
                }
            }
            catch
            {
                MessageBox.Show("Cập nhật thất bại");
            }
            finally
            {
                conn.Close();
            }
        }
        public void DeleteEmployee(Employee employee)
        {
            try
            {
                conn.Open();
                string query = "delete from Employee where idEmployee = " + employee.IdEmployee.ToString();
                SqlCommand command = new SqlCommand(query, conn);
                
                if (command.ExecuteNonQuery() > 0)
                    MessageBox.Show("Xoá thành công!");
                conn.Close();
            }
            catch
            {
                throw new Exception("Failed Query");
            }
            finally
            {
                
            }
        }
        public void AddEmployee(Employee employee)
        {
            if(ConvertDBToList().Count==0 || employee.IdEmployee>ConvertDBToList()[ConvertDBToList().Count-1].IdEmployee)
            {
                AddIntoDB(employee);
            }    
            else
            {
                UpdateOnDB(employee);
            }
            //conn.Close();
        }
    }
}

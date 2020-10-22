using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballFieldManagement.Models;
using FootballFieldManegement.DAL;
using FootballFieldManagement.Views;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Collections.ObjectModel;

namespace FootballFieldManagement.ViewModels
{
    class EmployeeViewModel:HomeViewModel
    {
        public ICommand SaveCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand GetIdCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        private string id;
        public string Id { get => id; set => id = value; }
        public string gender;
        public EmployeeViewModel()
        {

            SaveCommand = new RelayCommand<fAddEmployee>((parameter) => true, (parameter) => AddEmployee(parameter));
            UpdateCommand = new RelayCommand<TextBlock>((parameter) => true, (parameter) => OpenUpdateWindow(parameter));
            GetIdCommand = new RelayCommand<TextBlock>((parameter) => true, (parameter) => id = (parameter.Text));
            DeleteCommand = new RelayCommand<TextBlock>((parameter) => true, (parameter) => Delete(parameter.Text));
            LogoutCommand = new RelayCommand<Window>((parameter) => true, (parameter) => parameter.Close());
        }

        public void AddEmployee(fAddEmployee parameter)
        {
            if (parameter.txtName.Text == "")
            {
                MessageBox.Show("Vui lòng nhập họ tên!");
                parameter.txtName.Focus();
                return;
            }
            if (parameter.cboPosition.Text == "")
            {
                MessageBox.Show("Vui lòng nhập chức vụ!");
                parameter.cboPosition.Focus();
                return;
            }
            else
            {
                if (parameter.cboPosition.Text != "Bảo vệ" && parameter.cboPosition.Text != "Nhân viên quản lý")
                {
                    MessageBox.Show("Vui lòng nhập đúng chức vụ!");
                    parameter.cboPosition.Focus();
                    return;
                }
            }
            if (parameter.dpBirthDate.Text == "")
            {
                MessageBox.Show("Vui lòng nhập ngày sinh!");
                parameter.dpBirthDate.Focus();
                return;
            }
            else
            {
                DateTime dateTime = new DateTime();
                if (!DateTime.TryParse(parameter.dpBirthDate.Text,out dateTime))
                {
                    MessageBox.Show("Vui lòng nhập lại ngày sinh!");
                    parameter.dpBirthDate.Focus();
                    return;
                }                    
            }
            if (parameter.txtAddress.Text == "")
            {
                MessageBox.Show("Vui lòng nhập địa chỉ!");
                parameter.txtAddress.Focus();
                return;
            }
            if (parameter.txtTelephoneNumber.Text == "")
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!");
                parameter.txtTelephoneNumber.Focus();
                return;
            }
            if (parameter.dpWorkDate.Text == "")
            {
                MessageBox.Show("Vui lòng nhập ngày vào làm!");
                parameter.dpWorkDate.Focus();
                return;
            }
            else
            {
                DateTime dateTime = new DateTime();
                if (!DateTime.TryParse(parameter.dpWorkDate.Text, out dateTime))
                {
                    MessageBox.Show("Vui lòng nhập lại ngày vào làm!");
                    parameter.dpWorkDate.Focus();
                    return;
                }
                if(dateTime<parameter.dpBirthDate.DisplayDate)
                {
                    MessageBox.Show("Vui lòng nhập lại ngày vào làm lớn hơn ngày sinh!");
                    parameter.dpWorkDate.Focus();
                    return;
                }
            }
            if (parameter.rdoMale.IsChecked.Value == true)
                gender = "Nam";
            else
                gender = "Nữ";
            Employee employee = new Employee(int.Parse(parameter.txtIDEmployee.Text), parameter.txtName.Text, gender, parameter.txtTelephoneNumber.Text, parameter.txtAddress.Text, parameter.dpBirthDate.DisplayDate, 0, parameter.cboPosition.Text, parameter.dpWorkDate.DisplayDate, 0);
            EmployeeDAL employeeDAL = new EmployeeDAL();
            employeeDAL.AddEmployee(employee);
            parameter.Close();

        }
       
        public void Delete(string id)
        {
            EmployeeDAL employeeDAL = new EmployeeDAL();
            ObservableCollection<Employee> employees = employeeDAL.Employees;
            foreach (var employee in employees)
            {
                if (employee.IdEmployee.ToString() == id)
                {
                    employeeDAL.Delete(employee);
                    break;
                }
            }
        }
        public void OpenUpdateWindow(TextBlock parameter)
        {
            EmployeeDAL employeeDAL = new EmployeeDAL();
            ObservableCollection<Employee> employees = employeeDAL.Employees;
            fAddEmployee child = new fAddEmployee();
            foreach (var employee in employees)
            {
                if (employee.IdEmployee.ToString() == parameter.Text)
                {
                    child.txtIDEmployee.Text = employee.IdEmployee.ToString();
                    child.txtName.Text = employee.Name;
                    child.txtTelephoneNumber.Text = employee.Phonenumber;
                    child.txtAddress.Text = employee.Address;
                    child.cboPosition.Text = employee.Position;
                    if (employee.Gender == "Nam")
                        child.rdoMale.IsChecked = true;
                    else
                        child.rdoFemale.IsChecked = true;
                    child.dpBirthDate.Text = employee.DateOfBirth.ToString();
                    child.dpWorkDate.Text = employee.Startingdate.ToString();
                    break;
                }

            }
            child.ShowDialog();
        }
    }
}

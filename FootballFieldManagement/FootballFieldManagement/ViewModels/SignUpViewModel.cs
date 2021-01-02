using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FootballFieldManagement.Views;
using FootballFieldManagement.ViewModels;
using System.Windows.Controls;
using FootballFieldManagement.DAL;
using FootballFieldManagement.Validations;
using FootballFieldManagement.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;

namespace FootballFieldManagement.ViewModels
{
    class SignUpViewModel : BaseViewModel
    {
        public ICommand SignUpCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand PasswordConfirmChangedCommand { get; set; }
        public ICommand KeyCommand { get; set; }
        public ICommand OpenLoginWinDowCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }
        private ObservableCollection<Employee> itemSourceEmployee = new ObservableCollection<Employee>();
        public ObservableCollection<Employee> ItemSourceEmployee { get => itemSourceEmployee; set { itemSourceEmployee = value; OnPropertyChanged(); } }
        private bool isSignUp;
        public bool IsSignUp { get => isSignUp; set => isSignUp = value; }
        public string TypeEmployee { get; set; }
        private string password;
        public string Password { get => password; set { password = value; OnPropertyChanged(); } }
        private string userName;
        public string UserName { get => userName; set { userName = value; OnPropertyChanged(); } }
        private string passwordConfirm;
        public string PasswordConfirm { get => passwordConfirm; set { passwordConfirm = value; OnPropertyChanged(); } }

        public Employee SelectedEmployee { get => selectedEmployee; set { selectedEmployee = value; OnPropertyChanged("SelectedEmployee"); } }

        private Employee selectedEmployee = new Employee();


        public SignUpViewModel()
        {
            SignUpCommand = new RelayCommand<SignUpWindow>((parameter) => true, (parameter) => SignUp(parameter));

            PasswordChangedCommand = new RelayCommand<PasswordBox>((parameter) => true, (parameter) => EncodingPassword(parameter));
            PasswordConfirmChangedCommand = new RelayCommand<PasswordBox>((parameter) => true, (parameter) => EncodingConfirmPassword(parameter));
            OpenLoginWinDowCommand = new RelayCommand<Window>(parameter => true, parameter => parameter.Close());
            LoadCommand = new RelayCommand<Window>(parameter => true, parameter => SetItemSourcEmloyee());
            ChangePasswordCommand = new RelayCommand<ForgotPasswordWindow>((parameter) => true, (parameter) => ChangePassword(parameter));
        }
        public void EncodingPassword(PasswordBox parameter)
        {
            this.password = parameter.Password;
            this.password = MD5Hash(this.password);
        }
        public void EncodingConfirmPassword(PasswordBox parameter)
        {
            this.passwordConfirm = parameter.Password;
            this.passwordConfirm = MD5Hash(this.passwordConfirm);
        }

        public void SetItemSourcEmloyee()
        {
            itemSourceEmployee.Clear();
            List<Employee> employees = EmployeeDAL.Instance.ConvertDBToList();
            foreach (var employee in employees)
            {
                if ((employee.Position == "Nhân viên quản lý" || employee.Position == "Nhân viên thu ngân") && employee.IdAccount == -1)
                {
                    itemSourceEmployee.Add(employee);
                }
            }
        }
        public void ChangePassword(ForgotPasswordWindow parameter)
        {
            if (string.IsNullOrEmpty(parameter.pwbKey.Password))
            {
                CustomMessageBox.Show("Vui lòng nhập mã xác thực!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.pwbKey.Focus();
                return;
            }
            // Check username
            if (string.IsNullOrEmpty(parameter.txtUsername.Text) || !AccountDAL.Instance.IsExistUserName(parameter.txtUsername.Text))
            {
                parameter.txtUsername.Focus();
                parameter.txtUsername.Text = "";
                return;
            }
            //Check password
            if (string.IsNullOrEmpty(parameter.pwbPassword.Password))
            {
                CustomMessageBox.Show("Vui lòng nhập mật khẩu mới!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.pwbPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.pwbPasswordConfirm.Password))
            {
                CustomMessageBox.Show("Vui lòng xác thực mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.pwbPasswordConfirm.Focus();
                return;
            }
            //Kiểm tra
            SQLConnection connection = new SQLConnection();
            try
            {
                connection.conn.Open();
                string queryString = "select * from Authorizations where authKey = '" + parameter.pwbKey.Password + "'";
                SqlCommand command = new SqlCommand(queryString, connection.conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count < 1)
                {
                    CustomMessageBox.Show("Mã xác thực không đúng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    parameter.pwbKey.Focus();
                    return;
                }
            }
            catch
            {

            }
            finally
            {
                connection.conn.Close();
            }

            if (!Regex.IsMatch(parameter.txtUsername.Text, @"^[a-zA-Z0-9_]+$"))
            {
                parameter.txtUsername.Focus();
                return;
            }
            if (password != passwordConfirm)
            {
                CustomMessageBox.Show("Mật khẩu không trùng khớp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (AccountDAL.Instance.UpdatePasswordByUsername(parameter.txtUsername.Text, password))
            {
                CustomMessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                parameter.txtUsername.Text = null;
                parameter.pwbPassword.Password = "";
                parameter.pwbPasswordConfirm.Password = "";
            }
        }
        public void SignUp(SignUpWindow parameter)
        {
            isSignUp = false;
            if (parameter == null)
            {
                return;
            }
            //Check IdConfirm
            if (string.IsNullOrEmpty(parameter.pwbKey.Password))
            {
                CustomMessageBox.Show("Vui lòng nhập mã xác thực!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.pwbKey.Focus();
                return;
            }
            //Check select employee
            if (string.IsNullOrEmpty(parameter.cboSelectEmployee.Text))
            {
                parameter.cboSelectEmployee.Focus();
                parameter.cboSelectEmployee.Text = "";
                return;
            }
            // Check username
            if (string.IsNullOrEmpty(parameter.txtUsername.Text) || AccountDAL.Instance.IsExistUserName(parameter.txtUsername.Text))
            {
                parameter.txtUsername.Focus();
                parameter.txtUsername.Text = "";
                return;
            }
            //Check password
            if (string.IsNullOrEmpty(parameter.pwbPassword.Password))
            {
                CustomMessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.pwbPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.pwbPasswordConfirm.Password))
            {
                CustomMessageBox.Show("Vui lòng xác thực mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.pwbPasswordConfirm.Focus();
                return;
            }
            //Kiểm tra độ chính xác
            SQLConnection connection = new SQLConnection();
            try
            {
                connection.conn.Open();
                string queryString = "select * from Authorizations where authKey = '" + parameter.pwbKey.Password + "'";
                SqlCommand command = new SqlCommand(queryString, connection.conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count < 1)
                {
                    CustomMessageBox.Show("Mã xác thực không đúng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    parameter.pwbKey.Focus();
                    return;
                }
            }
            catch
            {

            }
            finally
            {
                connection.conn.Close();
            }

            if (!Regex.IsMatch(parameter.txtUsername.Text, @"^[a-zA-Z0-9_]+$"))
            {
                parameter.txtUsername.Focus();
                return;
            }
            if (password != passwordConfirm)
            {
                CustomMessageBox.Show("Mật khẩu không trùng khớp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int type = 0;
            if (selectedEmployee.Position == "Nhân viên quản lý")
                type = 1;
            else
                type = 2;
            int idAccount = AccountDAL.Instance.SetNewID();
            if (idAccount != -1)
            {
                Account newAccount = new Account(idAccount, parameter.txtUsername.Text.ToString(), password, type);
                AccountDAL.Instance.AddIntoDB(newAccount);
                selectedEmployee.IdAccount = idAccount;
                if (EmployeeDAL.Instance.UpdateIdAccount(selectedEmployee))
                {
                    CustomMessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    isSignUp = true;
                    parameter.cboSelectEmployee.Text = "";
                    parameter.txtUsername.Text = null;
                    parameter.pwbPassword.Password = "";
                    parameter.pwbPasswordConfirm.Password = "";
                }
            }
        }
    }
}

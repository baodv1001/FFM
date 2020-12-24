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
                MessageBox.Show("Vui lòng nhập mã xác thực!");
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
                MessageBox.Show("Vui lòng nhập mật khẩu mới!");
                parameter.pwbPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.pwbPasswordConfirm.Password))
            {
                MessageBox.Show("Vui lòng xác thực mật khẩu!");
                parameter.pwbPasswordConfirm.Focus();
                return;
            }
            //Kiểm tra
            if (parameter.pwbKey.Password != "admin")
            {
                MessageBox.Show("Mã xác thực không đúng!");
                parameter.pwbKey.Focus();
                return;
            }
            if (!Regex.IsMatch(parameter.txtUsername.Text, @"^[a-zA-Z0-9_]+$"))
            {
                parameter.txtUsername.Focus();
                return;
            }
            if (password != passwordConfirm)
            {
                MessageBox.Show("Mật khẩu không trùng khớp!");
                return;
            }
            if (AccountDAL.Instance.UpdatePassword(parameter.txtUsername.Text, password))
            {
                MessageBox.Show("Đổi mật khẩu thành công!");
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
                MessageBox.Show("Vui lòng nhập mã xác thực!");
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
                MessageBox.Show("Vui lòng nhập mật khẩu!");
                parameter.pwbPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.pwbPasswordConfirm.Password))
            {
                MessageBox.Show("Vui lòng xác thực mật khẩu!");
                parameter.pwbPasswordConfirm.Focus();
                return;
            }
            //Kiểm tra độ chính xác
            if (parameter.pwbKey.Password != "admin")
            {
                MessageBox.Show("Mã xác thực không đúng!");
                parameter.pwbKey.Focus();
                return;
            }

            if (!Regex.IsMatch(parameter.txtUsername.Text, @"^[a-zA-Z0-9_]+$"))
            {
                parameter.txtUsername.Focus();
                return;
            }
            if (password != passwordConfirm)
            {
                MessageBox.Show("Mật khẩu không trùng khớp!");
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
                    MessageBox.Show("Đăng ký thành công!");
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

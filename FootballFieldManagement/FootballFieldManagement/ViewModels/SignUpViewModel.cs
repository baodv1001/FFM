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
using FootballFieldManagement.Models;
using System.Collections.ObjectModel;

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
        public int setID(List<Account> accounts)
        {
            int id;
            try
            {
                id = (accounts[accounts.Count() - 1].IdAccount + 1);
            }
            catch
            {
                id = 1;
            }
            return id;
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
            List<Account> accounts = AccountDAL.Instance.ConvertDBToList();
            Account account = null;
            if (string.IsNullOrEmpty(parameter.pwbKey.Password))
            {
                MessageBox.Show("Vui lòng nhập mã xác thực!");
                parameter.pwbKey.Focus();
                return;
            }
            if (parameter.pwbKey.Password != "admin")
            {
                MessageBox.Show("Mã xác thực không đúng!");
                parameter.pwbKey.Focus();
                return;
            }
            // Check username
            if (string.IsNullOrEmpty(parameter.txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!");
                parameter.txtUsername.Focus();
                return;
            }
            foreach (char c in parameter.txtUsername.Text)
            {
                if (!((c >= 48 && c <= 57) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122)))
                {
                    MessageBox.Show("Không nhập các ký tự đặc biệt");
                    return;
                }
            }
            account = accounts.Find(x => x.Username == parameter.txtUsername.Text); // Sửa thành hàm find Account
            if (account == null)
            {
                MessageBox.Show("Không tồn tại tên đăng nhập!");
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
            if (password != passwordConfirm)
            {
                MessageBox.Show("Mật khẩu không trùng khớp!");
                return;
            }
            account.Password = password;
            if (AccountDAL.Instance.UpdatePassword(account))
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
            List<Account> accounts = AccountDAL.Instance.ConvertDBToList();
            //Check IdConfirm
            if (string.IsNullOrEmpty(parameter.pwbKey.Password))
            {
                CustomMessageBox.Show("Vui lòng nhập mã xác thực!");
                parameter.pwbKey.Focus();
                return;
            }
            if (parameter.pwbKey.Password != "admin")
            {
                CustomMessageBox.Show("Mã xác thực không đúng!");
                parameter.pwbKey.Focus();
                return;
            }
            //Check select employee
            if (string.IsNullOrEmpty(parameter.cboSelectEmployee.Text))
            {
                CustomMessageBox.Show("Vui lòng chọn nhân viên!");
                parameter.cboSelectEmployee.Focus();
                return;
            }
            foreach (var item in parameter.cboSelectEmployee.ItemsSource)
            {
                if (parameter.cboSelectEmployee.Text.ToString() != item.ToString())
                {
                    CustomMessageBox.Show("Nhân viên không hợp lệ!");
                    return;
                }
            }
            // Check username
            if (string.IsNullOrEmpty(parameter.txtUsername.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập tên đăng nhập!");
                parameter.txtUsername.Focus();
                return;
            }
            foreach (char c in parameter.txtUsername.Text)
            {
                if (!((c >= 48 && c <= 57) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122)))
                {
                    CustomMessageBox.Show("Không nhập các ký tự đặc biệt");
                    return;
                }
            }

            foreach (var acc in accounts)
            {
                if (acc.Username == parameter.txtUsername.Text)
                {
                    CustomMessageBox.Show("Tên tài khoản đã tồn tại!");
                    return;
                }
            }
            //Check password
            if (string.IsNullOrEmpty(parameter.pwbPassword.Password))
            {
                CustomMessageBox.Show("Vui lòng nhập mật khẩu!");
                parameter.pwbPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.pwbPasswordConfirm.Password))
            {
                CustomMessageBox.Show("Vui lòng xác thực mật khẩu!");
                parameter.pwbPasswordConfirm.Focus();
                return;
            }
            if (password != passwordConfirm)
            {
                CustomMessageBox.Show("Mật khẩu không trùng khớp!");
                return;
            }
            int type = 0;
            if (selectedEmployee.Position == "Nhân viên quản lý")
                type = 1;
            else
                type = 2;
            Account newAccount = new Account(setID(accounts), parameter.txtUsername.Text.ToString(), password, type);
            AccountDAL.Instance.AddIntoDB(newAccount);
            selectedEmployee.IdAccount = setID(accounts);
            if (EmployeeDAL.Instance.UpdateIdAccount(selectedEmployee))
            {
                CustomMessageBox.Show("Đăng ký thành công!");
                isSignUp = true;
                parameter.cboSelectEmployee.Text = "";
                parameter.txtUsername.Text = null;
                parameter.pwbPassword.Password = "";
                parameter.pwbPasswordConfirm.Password = "";
            }
        }
    }
}

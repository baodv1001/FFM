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
using FootballFieldManegement.DAL;

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

            PasswordChangedCommand = new RelayCommand<PasswordBox>((parameter) => true, (parameter) =>EncodingPassword(parameter));
            PasswordConfirmChangedCommand = new RelayCommand<PasswordBox>((parameter) => true, (parameter) =>EncodingPassword(parameter));
            OpenLoginWinDowCommand = new RelayCommand<Window>(parameter => true, parameter => parameter.Close());
            LoadCommand = new RelayCommand<Window>(parameter => true, parameter => setItemSourcEmloyee());
        }
        public void EncodingPassword(PasswordBox parameter)
        {
            this.password = parameter.Password;
            this.password = MD5Hash(this.password);
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

        public void setItemSourcEmloyee()
        {
            itemSourceEmployee.Clear();
            List<Employee> employees = EmployeeDAL.Instance.ConvertDBToList();
            foreach (var employee in employees)
            {
                if (employee.Position == "Nhân viên quản lý" && employee.IdAccount == 0)
                {
                    itemSourceEmployee.Add(employee);
                }
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
            if (string.IsNullOrEmpty(parameter.txtKey.Password))
            {
                MessageBox.Show("Vui lòng nhập mã xác thực!");
                parameter.txtKey.Focus();
                return;
            }
            if (parameter.txtKey.Password != "admin")
            {
                MessageBox.Show("Mã xác thực không đúng!");
                parameter.txtKey.Focus();
                return;
            }
            //Check select employee
            if (string.IsNullOrEmpty(parameter.cboSelectEmployee.Text))
            {
                MessageBox.Show("Vui lòng chọn nhân viên!");
                parameter.cboSelectEmployee.Focus();
                return;
            }
            foreach (var item in parameter.cboSelectEmployee.ItemsSource)
            {
                if (parameter.cboSelectEmployee.Text.ToString() != item.ToString())
                {
                    MessageBox.Show("Nhân viên không hợp lệ!");
                    return;
                }
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

            foreach (var acc in accounts)
            {
                if (acc.Username == parameter.txtUsername.Text)
                {
                    MessageBox.Show("Tên tài khoản đã tồn tại!");
                    return;
                }
            }
            //Check password
            if (string.IsNullOrEmpty(parameter.txtPassword.Password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!");
                parameter.txtPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.txtPasswordConfirm.Password))
            {
                MessageBox.Show("Vui lòng xác thực mật khẩu!");
                parameter.txtPasswordConfirm.Focus();
                return;
            }
            if (password != passwordConfirm)
            {
                MessageBox.Show("Mật khẩu không trùng khớp!");
                return;
            }
            Account newAccount = new Account(setID(accounts), parameter.txtUsername.Text.ToString(), password, 1);
            AccountDAL.Instance.AddIntoDB(newAccount);
            selectedEmployee.IdAccount = setID(accounts);
            if (EmployeeDAL.Instance.UpdateOnDB(selectedEmployee))
            {
                MessageBox.Show("Đăng ký thành công!");
                isSignUp = true;
            }
        }
    }
}

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

namespace FootballFieldManagement.ViewModels
{
    class SignUpViewModel : BaseViewModel
    {
        public ICommand SignUpCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand PasswordConfirmChangedCommand { get; set; }

        private bool isSignUp;
        public bool IsSignUp { get => isSignUp; set => isSignUp = value; }
        private string username;
        public string Username { get => username; set { username = value; OnPropertyChanged(); } }
        private string password;
        public string Password { get => password; set { password = value; OnPropertyChanged(); } }
        private string passwordConfirm;
        public string PasswordConfirm { get => passwordConfirm; set { passwordConfirm = value; OnPropertyChanged(); } }
        public SignUpViewModel()
        {
            SignUpCommand = new RelayCommand<SignUpWindow>((parameter) => true, (parameter) =>
            {
                SignUp(parameter);
                if (isSignUp)
                {
                    HomeWindow home = new HomeWindow();
                    parameter.Close();
                    home.ShowDialog();
                }
            });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((parameter) => true, (parameter) =>
            {
                this.password = parameter.Password;
                this.password = MD5Hash(this.password);
            });
            PasswordConfirmChangedCommand = new RelayCommand<PasswordBox>((parameter) => true, (parameter) =>
            {
                this.passwordConfirm = parameter.Password;
                this.passwordConfirm = MD5Hash(this.passwordConfirm);
            });

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
        public void SignUp(SignUpWindow parameter)
        {
            isSignUp = false;
            if (parameter == null)
            {
                return;
            }
            // Check username
            if (string.IsNullOrEmpty(parameter.txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!");
                parameter.txtUsername.Focus();
                return;
            }
            List<Account> accounts = AccountDAL.Instance.ConvertDBToList();
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
            Account newAccount = new Account(setID(accounts), username, password, 1);
            AccountDAL.Instance.AddIntoDB(newAccount);
            isSignUp = true;
            
            
        }
    }
}

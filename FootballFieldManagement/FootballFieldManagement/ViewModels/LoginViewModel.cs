using FootballFieldManagement.DAL;
using FootballFieldManagement.Models;
using FootballFieldManagement.ViewModels;
using FootballFieldManagement.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FootballFieldManagement.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        public ICommand LogInCommand { get; set; }
        public ICommand ConvertToSignUpCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        private string password;
        public string Password { get => password; set { password = value; OnPropertyChanged(); } }
        private bool isLogin;
        public bool IsLogin { get => isLogin; set => isLogin = value; }
        public LoginViewModel()
        {
            LogInCommand = new RelayCommand<LoginWindow>((parameter) => true, (parameter) =>
            {
                Login(parameter);
                HomeWindow home = new HomeWindow();
                if (isLogin)
                {
                    home.ShowDialog();
                    parameter.Show();
                }
            });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((parameter) => true, (parameter) =>
            {
                this.password = parameter.Password;
                this.password = MD5Hash(this.password);
            });
            ConvertToSignUpCommand = new RelayCommand<Window>((parameter) => true, (parameter) =>
            {
                SignUpWindow signUp = new SignUpWindow();
                parameter.Hide();
                signUp.ShowDialog();
                parameter.Show();
            });
        }
        public void Login(LoginWindow parameter)
        {
            isLogin = false;
            if (parameter == null)
            {
                return;
            }
            List<Account> accounts = AccountDAL.Instance.ConvertDBToList();
            //check username
            if (string.IsNullOrEmpty(parameter.txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!");
                parameter.txtUsername.Focus();
                return;
            }
            //check password
            if (string.IsNullOrEmpty(parameter.txtPassword.Password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!");
                parameter.txtPassword.Focus();
                return;
            }
            foreach (var account in accounts)
            {
                if (account.Username == parameter.txtUsername.Text.ToString() && account.Password == password)
                {
                    isLogin = true;
                }
            }
            if (isLogin)
            {
                parameter.Hide();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!");
            }
        }
    }
}

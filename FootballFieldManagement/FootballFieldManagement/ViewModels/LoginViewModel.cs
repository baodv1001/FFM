using FootballFieldManagement.DAL;
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
        public ICommand PasswordChangedCommand { get; set; }
        private string username;
        public string Username { get => username; set { username = value; OnPropertyChanged(); } }
        private string password;
        public string Password { get => password; set { password = value; OnPropertyChanged(); } }
        private bool isLogin;
        public bool IsLogin { get => isLogin; set => isLogin = value; }
        public LoginViewModel()
        {
            LogInCommand = new RelayCommand<Window>((parameter) => true, (parameter) =>
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
        }
        public void Login(Window p)
        {
            isLogin = false;
            if (p == null)
            {
                return;
            }
            if (password == null)
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!");
                return;
            }
            else
            {
                AccountDal accounts = new AccountDal();
                foreach (var account in accounts.ListAccount)
                {
                    if (account.Username == username && account.Password == password)
                    {
                        isLogin = true;
                    }
                }
                if (isLogin)
                {
                    p.Hide();
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!");
                }
            }
        }
    }
}

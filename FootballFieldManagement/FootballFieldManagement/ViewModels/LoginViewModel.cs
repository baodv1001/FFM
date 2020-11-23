using FootballFieldManagement.DAL;
using FootballFieldManagement.Models;
using FootballFieldManagement.ViewModels;
using FootballFieldManagement.Views;
using FootballFieldManegement.DAL;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FootballFieldManagement.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        public ICommand LogInCommand { get; set; }
        public ICommand OpenSignUpWindowCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand OpenCheckAttendanceWindowCommand { get; set; }
        private string password;
        public string Password { get => password; set { password = value; OnPropertyChanged(); } }
        private string userName;
        public string UserName { get => userName; set { userName = value; OnPropertyChanged(); } }
        private bool isLogin;
        public bool IsLogin { get => isLogin; set => isLogin = value; }
        public LoginViewModel()
        {
            LogInCommand = new RelayCommand<LoginWindow>((parameter) => true, (parameter) => Login(parameter));
            PasswordChangedCommand = new RelayCommand<PasswordBox>((parameter) => true, (parameter) => EncodingPassword(parameter));
            OpenSignUpWindowCommand = new RelayCommand<Window>((parameter) => true, (parameter) => OpenSignUpWindow(parameter));
            OpenCheckAttendanceWindowCommand = new RelayCommand<Window>((parameter) => true, (parameter) => OpenCheckAttendanceWindow(parameter));
        }
        public void OpenCheckAttendanceWindow(Window parameter)
        {
            CheckAttendanceWindow wdCheckAttendance = new CheckAttendanceWindow();
            parameter.Hide();
            wdCheckAttendance.ShowDialog();
            parameter.Show();
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
                    CurrentAccount.Type = account.Type == 1 ? true : false; // Kiểm tra quyền
                    List<Employee> employees = EmployeeDAL.Instance.ConvertDBToList();
                    foreach (var employee in employees)
                    {
                        if (employee.IdAccount == account.IdAccount)
                        {
                            //Lấy thông tin người đăng nhập
                            CurrentAccount.DisplayName = employee.Name;
                            CurrentAccount.Image = employee.Image;
                            break;
                        }
                    }
                    isLogin = true;
                }
            }
            if (isLogin)
            {
                HomeWindow home = new HomeWindow();
                SetJurisdiction(home);
                DisplayAccount(home);
                parameter.Hide();
                home.ShowDialog();
                parameter.txtPassword.Password = null;
                parameter.Show();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!");
            }
        }
        public void SetJurisdiction(HomeWindow home)
        {
            if (!CurrentAccount.Type)
            {
                //Không cấp quyền cho nhân viên
                home.btnEmployee.IsEnabled = false;
                home.btnReport.IsEnabled = false;
                home.btnAddGoods.IsEnabled = false;
                home.btnAddEmployee.IsEnabled = false;
                home.btnSetSalary.IsEnabled = false;
                home.icnEmployee.Foreground = Brushes.LightGray;
                home.icnReport.Foreground = Brushes.LightGray;
            }
        }
        public void DisplayAccount(HomeWindow home)
        {
            home.lbAccount.Content = CurrentAccount.DisplayName;
            //ImageBrush imageBrush = new ImageBrush();
            //BitmapImage bitmap = new BitmapImage();
            //bitmap.BeginInit();
            //bitmap.CacheOption = BitmapCacheOption.OnLoad;
            //bitmap.UriSource = new Uri(CurrentAccount.Image, UriKind.Relative);
            //bitmap.EndInit();
            //imageBrush.ImageSource = bitmap;
            //home.imgAccount.Fill = imageBrush;
        }
        public void OpenSignUpWindow(Window parameter)
        {
            SignUpWindow signUp = new SignUpWindow();
            parameter.Hide();
            signUp.ShowDialog();
            parameter.Show();
        }

        public void EncodingPassword(PasswordBox parameter)
        {
            this.password = parameter.Password;
            this.password = MD5Hash(this.password);
        }

    }
}

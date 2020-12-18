using FootballFieldManagement.DAL;
using FootballFieldManagement.Models;
using FootballFieldManagement.ViewModels;
using FootballFieldManagement.Views;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace FootballFieldManagement.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        public ICommand LogInCommand { get; set; }
        public ICommand OpenSignUpWindowCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        private string password;
        public string Password { get => password; set { password = value; OnPropertyChanged(); } }
        private string userName;
        public string UserName { get => userName; set { userName = value; OnPropertyChanged(); } }
        private bool isLogin;
        public bool IsLogin { get => isLogin; set => isLogin = value; }
        public Employee employee;
        public LoginViewModel()
        {
            LogInCommand = new RelayCommand<LoginWindow>((parameter) => true, (parameter) => Login(parameter));
            PasswordChangedCommand = new RelayCommand<PasswordBox>((parameter) => true, (parameter) => EncodingPassword(parameter));
            OpenSignUpWindowCommand = new RelayCommand<Window>((parameter) => true, (parameter) => OpenSignUpWindow(parameter));
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
                    CurrentAccount.Type = account.Type; // Kiểm tra quyền
                    if (CurrentAccount.Type != 0)
                    {
                        List<Employee> employees = EmployeeDAL.Instance.ConvertDBToList();
                        foreach (var employee in employees)
                        {
                            if (employee.IdAccount == account.IdAccount)
                            {
                                //Lấy thông tin người đăng nhập
                                CurrentAccount.DisplayName = employee.Name;
                                CurrentAccount.Image = employee.ImageFile;                                
                                this.employee = employee;
                                break;
                            }
                        }
                    }
                    CurrentAccount.IdAccount = account.IdAccount;
                    CurrentAccount.Password = password;
                    isLogin = true;
                }
            }
            if (isLogin)
            {
                HomeWindow home = new HomeWindow();
                home.lbTitle.Content = new DataProvider().LoadData("Information").Rows[0].ItemArray[0].ToString();
                SetJurisdiction(home);
                DisplayAccount(home);
                DisplayEmployee(employee, home);
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
        public void DisplayEmployee(Employee employee, HomeWindow home)
        {
            if (CurrentAccount.Type!=0)
            {
                home.txtIDEmployee.Text = employee.IdEmployee.ToString();
                home.txtName.Text = employee.Name;
                home.txtPosition.Text = employee.Position;
                home.txtDayOfBirth.Text = employee.DateOfBirth.ToShortDateString();
                home.txtGender.Text = employee.Gender;
                home.txtAddress.Text = employee.Address;
                home.txtPhoneNumber.Text = employee.Phonenumber;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmapImage = Converter.Instance.ConvertByteToBitmapImage(CurrentAccount.Image);
                imageBrush.ImageSource = bitmapImage;
                if (bitmapImage != null)
                {
                    home.imgEmployee.Fill = imageBrush; // Hiển thị hình ảnh 
                }
                home.btnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                home.icnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                home.grdCursor.Margin = new Thickness(0, (175 + 70 * 1), 40, 0);
            }
            else
            {
                home.txtIDEmployee.Text = 0.ToString();
                home.txtName.Text = "Chủ sân";
                home.txtPosition.Text = "Chủ sân";
                home.txtDayOfBirth.Text = " ";
                home.txtGender.Text = " ";
                home.txtAddress.Text = " ";
                home.txtPhoneNumber.Text = " ";
                home.btnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                home.icnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                home.grdCursor.Margin = new Thickness(0, 175, 40, 0);
            }
            home.txtDayOfBirth.IsEnabled = false;
            home.txtGender.IsEnabled = false;
            home.txtAddress.IsEnabled = false;
            home.txtPhoneNumber.IsEnabled = false;
        }
        public void SetJurisdiction(HomeWindow home)
        {
            if (CurrentAccount.Type!=0)
            {
                //Không cấp quyền cho nhân viên
                home.grdBody_Home.Visibility = Visibility.Hidden;
                home.grdBody_Field.Visibility = Visibility.Visible;
                home.txtFieldName.IsEnabled = false;
                home.btnEmployee.IsEnabled = false;
                home.btnReport.IsEnabled = false;
                home.btnAddGoods.IsEnabled = false;
                home.btnPaySalary.IsEnabled = false;
                home.btnSetSalary.IsEnabled = false;
                home.btnHome.IsEnabled = false;
            }
            if(CurrentAccount.Type==1)
            {
                home.btnAddGoods.IsEnabled = true;
                home.btnEmployee.IsEnabled = true;
            }    
        }
        public void DisplayAccount(HomeWindow home)
        {
            if (CurrentAccount.Type != 0)
            {
                home.lbAccount.Content = CurrentAccount.DisplayName;// Hiển thị tên nhân viên
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmapImage = Converter.Instance.ConvertByteToBitmapImage(CurrentAccount.Image);
                imageBrush.ImageSource = bitmapImage;
                if (bitmapImage != null)
                    home.imgAccount.Fill = imageBrush; // Hiển thị hình ảnh 
            }
        }
        public void OpenSignUpWindow(Window parameter)
        {
            SignUpWindow signUp = new SignUpWindow();
            parameter.Opacity = 0.5;
            parameter.WindowStyle = WindowStyle.None;
            signUp.ShowDialog();
            parameter.WindowStyle = WindowStyle.SingleBorderWindow;
            parameter.Opacity = 1;
            parameter.Show();
        }

        public void EncodingPassword(PasswordBox parameter)
        {
            this.password = parameter.Password;
            this.password = MD5Hash(this.password);
        }

    }
}

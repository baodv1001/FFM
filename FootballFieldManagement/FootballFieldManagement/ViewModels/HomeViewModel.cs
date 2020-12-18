using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using FootballFieldManagement.Views;
using FootballFieldManagement.Resources.UserControls;
using System.Windows.Media;
using FootballFieldManagement.Models;
using FootballFieldManagement.DAL;
using System.Linq;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Threading;

namespace FootballFieldManagement.ViewModels
{
    class HomeViewModel : BaseViewModel
    {
        public ICommand LogOutCommand { get; set; }
        public ICommand SwitchTabCommand { get; set; }

        public ICommand E_LoadCommand { get; set; }
        public ICommand E_AddCommand { get; set; }
        public ICommand E_SetSalaryCommand { get; set; }
        public ICommand E_CalculateSalaryCommand { get; set; }
        public ICommand E_PaySalaryCommand { get; set; }

        public ICommand G_AddCommand { get; set; }
        public ICommand G_LoadCommand { get; set; }

        public ICommand GetUidCommand { get; set; }

        public ICommand S_SaveBtnFieldInfoCommand { get; set; }
        public ICommand S_SaveFieldInfoCommand { get; set; }
        public ICommand S_EnableBtnSavePassCommand { get; set; }
        public ICommand S_SaveNewPasswordCommand { get; set; }
        public ICommand OpenCheckAttendanceWindowCommand { get; set; }
        public StackPanel Stack { get => stack; set => stack = value; }

        private StackPanel stack = new StackPanel();
        private string uid;
        public HomeViewModel()
        {
            LogOutCommand = new RelayCommand<Window>((parameter) => true, (parameter) => parameter.Close());
            SwitchTabCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => SwitchTab(parameter));
            GetUidCommand = new RelayCommand<Button>((parameter) => true, (parameter) => uid = parameter.Uid);

            E_LoadCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => LoadEmployeesToView(parameter));
            E_AddCommand = new RelayCommand<StackPanel>((parameter) => true, (parameter) => AddEmployee(parameter));
            E_SetSalaryCommand = new RelayCommand<Window>((parameter) => true, (parameter) => OpenSetSalaryWindow());
            E_CalculateSalaryCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => CalculateSalary(parameter));
            E_PaySalaryCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => PaySalary(parameter));

            S_SaveBtnFieldInfoCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => EnableSaveButtonFieldInfo(parameter));
            S_EnableBtnSavePassCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => EnableButtonSavePass(parameter));
            S_SaveFieldInfoCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => SaveFieldInfo(parameter));
            S_SaveNewPasswordCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => SaveNewPassword(parameter));

            G_AddCommand = new RelayCommand<StackPanel>((parameter) => true, (parameter) => AddGoods(parameter));
            G_LoadCommand = new RelayCommand<StackPanel>((parameter) => true, (parameter) => LoadGoodsToView(parameter));

            OpenCheckAttendanceWindowCommand = new RelayCommand<Window>((parameter) => true, (parameter) => OpenCheckAttendanceWindow(parameter));
        }
        public void SaveNewPassword(HomeWindow parameter)
        {

            if (MD5Hash(parameter.pwbOldPassword.Password) == CurrentAccount.Password)
            {
                MessageBoxResult result = MessageBox.Show("Xác nhận đổi mật khẩu?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (parameter.pwbNewPassword.Password == parameter.pwbConfirmedPassword.Password)
                    {
                        CurrentAccount.Password = MD5Hash(parameter.pwbNewPassword.Password);
                        Account account = new Account(CurrentAccount.IdAccount, CurrentAccount.DisplayName, CurrentAccount.Password, CurrentAccount.Type);
                        if (AccountDAL.Instance.UpdatePassword(account))
                        {
                            MessageBox.Show("Đổi mật khẩu thành công!");
                            parameter.pwbOldPassword.Password = null;
                            parameter.pwbNewPassword.Password = null;
                            parameter.pwbConfirmedPassword.Password = null;
                        }
                        else
                        {
                            MessageBox.Show("Đổi mật khẩu thất bại!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nhập mật khẩu xác thực không khớp!", "Thông báo");
                    }
                }
            }
            else
            {
                MessageBox.Show("Nhập mật khẩu hiện tại không đúng!", "Thông báo");
            }

        }
        public void SaveFieldInfo(HomeWindow homeWindow)
        {
            MessageBoxResult result = MessageBox.Show("Xác nhận sửa tên sân?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                homeWindow.lbTitle.Content = homeWindow.txtFieldName.Text;
                SQLConnection connection = new SQLConnection();
                try
                {
                    connection.conn.Open();

                    string queryString = "update Information set fieldName = @fieldName, address = @address, phoneNumber = @phoneNumber";
                    SqlCommand command = new SqlCommand(queryString, connection.conn);
                    command.Parameters.AddWithValue("@fieldName", homeWindow.txtFieldName.Text);
                    command.Parameters.AddWithValue("@address", homeWindow.txtAdressInfo.Text);
                    command.Parameters.AddWithValue("@phoneNumber", homeWindow.txtPhoneNumberInfo.Text);

                    int rs = command.ExecuteNonQuery();
                    if (rs == 1)
                    {
                        MessageBox.Show("Sửa thông tin sân thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Thực hiện thất bại");
                    }
                }
                catch
                {
                    MessageBox.Show("Thực hiện thất bại");
                }
                finally
                {
                    connection.conn.Close();
                }
            }
        }
        public void EnableSaveButtonFieldInfo(HomeWindow parameter)
        {
            bool isEnable = string.IsNullOrEmpty(parameter.txtFieldName.Text) || string.IsNullOrEmpty(parameter.txtPhoneNumberInfo.Text) || string.IsNullOrEmpty(parameter.txtAdressInfo.Text);
            parameter.btnSaveFieldInfo.IsEnabled = !isEnable;
        }
        public void EnableButtonSavePass(HomeWindow parameter)
        {
            bool isEnable = string.IsNullOrEmpty(parameter.pwbOldPassword.Password) || string.IsNullOrEmpty(parameter.pwbNewPassword.Password) || string.IsNullOrEmpty(parameter.pwbConfirmedPassword.Password);
            parameter.btnSavePassword.IsEnabled = !isEnable;
        }
        public void SwitchTab(HomeWindow parameter)
        {
            int index = int.Parse(uid);

            parameter.grdCursor.Margin = new Thickness(0, (175 + 70 * index), 40, 0);

            parameter.grdBody_Goods.Visibility = Visibility.Hidden;
            parameter.grdBody_Business.Visibility = Visibility.Hidden;
            parameter.grdBody_Home.Visibility = Visibility.Hidden;
            parameter.grdBody_Employee.Visibility = Visibility.Hidden;
            parameter.grdBody_Report.Visibility = Visibility.Hidden;
            parameter.grdBody_Field.Visibility = Visibility.Hidden;
            parameter.grdBody_Setting.Visibility = Visibility.Hidden;

            parameter.btnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnBusiness.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnGoods.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnEmployee.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnReport.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnSetting.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");

            parameter.icnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnBusiness.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnGoods.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnEmployee.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnReport.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnSetting.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");

            switch (index)
            {
                case 0:
                    ReportViewModel reportViewModel = new ReportViewModel(parameter);
                    parameter.grdBody_Home.Visibility = Visibility.Visible;
                    parameter.btnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 1:
                    parameter.grdBody_Business.Visibility = Visibility.Visible;
                    parameter.btnBusiness.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnBusiness.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 2:
                    parameter.grdBody_Field.Visibility = Visibility.Visible;
                    parameter.btnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 3:
                    parameter.grdBody_Goods.Visibility = Visibility.Visible;
                    parameter.btnGoods.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnGoods.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 4:
                    parameter.grdBody_Employee.Visibility = Visibility.Visible;
                    parameter.btnEmployee.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnEmployee.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 5:
                    parameter.cboSelectPeriod_Report.SelectedIndex = -1;
                    parameter.cboSelectTime_Report.SelectedIndex = -1;
                    DispatcherTimer timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(1)
                    };
                    timer.Tick += (s, e) =>
                    {
                        parameter.cboSelectPeriod_Report.SelectedIndex = 0;
                        parameter.cboSelectTime_Report.SelectedIndex = DateTime.Now.Month - 1;
                        timer.Stop();
                    };
                    timer.Start();
                    parameter.grdBody_Report.Visibility = Visibility.Visible;
                    parameter.btnReport.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnReport.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 6:
                    parameter.grdBody_Setting.Visibility = Visibility.Visible;
                    parameter.btnSetting.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnSetting.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                default:
                    break;
            }
        }

        public void OpenCheckAttendanceWindow(Window parameter)
        {
            CheckAttendanceWindow wdCheckAttendance = new CheckAttendanceWindow();
            wdCheckAttendance.ShowDialog();
            parameter.Show();
        }
        //Tab employee
        public void PaySalary(HomeWindow parameter)
        {
            bool sucess = true;
            if (SalaryDAL.Instance.ConvertDBToList().Count == 0)
            {
                MessageBox.Show("Vui lòng thiết lập lương");
                SetSalaryWindow wdSetSalary = new SetSalaryWindow();
                wdSetSalary.ShowDialog();
                return;
            }
            foreach (var salary in SalaryDAL.Instance.ConvertDBToList())
            {
                if (salary.TotalSalary == 0)
                {
                    MessageBox.Show("Vui lòng tính lương!");
                    return;
                }
                salary.TotalSalary = 0;
                salary.NumOfFault = 0;
                salary.NumOfShift = 0;
                if (!SalaryDAL.Instance.UpdateTotalSalary(salary) || !SalaryDAL.Instance.UpdateQuantity(salary))
                {
                    sucess = false;
                    break;
                }
            }
            if (sucess)
            {
                MessageBox.Show("Đã trả lương!");
            }
            else
            {
                MessageBox.Show("Trả lương thất bại!");
            }
        }
        public void CalculateSalary(HomeWindow parameter)
        {
            bool sucess = true;
            DateTime today = DateTime.Today;
            if (today.Day != 1)
            {
                if (SalaryDAL.Instance.ConvertDBToList().Count == 0)
                {
                    MessageBox.Show("Vui lòng thiết lập lương");
                    SetSalaryWindow wdSetSalary = new SetSalaryWindow();
                    wdSetSalary.ShowDialog();
                    return;
                }
                foreach (var salary in SalaryDAL.Instance.ConvertDBToList())
                {
                    if (salary.SalaryBasic == 0)
                    {
                        MessageBox.Show("Vui lòng thiết lập lương cho '" + SalaryDAL.Instance.GetPosition(salary.IdEmployee.ToString()) + "'!");
                        SetSalaryWindow wdSetSalary = new SetSalaryWindow();
                        wdSetSalary.cboTypeEmployee.Text = SalaryDAL.Instance.GetPosition(salary.IdEmployee.ToString());
                        wdSetSalary.cboTypeEmployee.IsEnabled = false;
                        wdSetSalary.ShowDialog();
                        return;
                    }
                    else
                    {
                        int workdays = AttendanceDAL.Instance.GetCount(salary.IdEmployee.ToString());
                        if (workdays < 0)
                        {
                            return;
                        }
                        if (workdays <= salary.StandardWorkDays)
                        {
                            salary.TotalSalary = (salary.SalaryBasic / salary.StandardWorkDays) * workdays + salary.NumOfShift * salary.MoneyPerShift - salary.NumOfFault * salary.MoneyPerFault;
                        }
                        else
                        {
                            salary.NumOfShift += (workdays - salary.StandardWorkDays);
                            salary.TotalSalary = salary.SalaryBasic + salary.NumOfShift * salary.MoneyPerShift - salary.NumOfFault * salary.MoneyPerFault;
                        }
                        if (salary.TotalSalary < 0)
                        {
                            salary.TotalSalary = 0;
                        }
                        if (!SalaryDAL.Instance.UpdateTotalSalary(salary))
                        {
                            sucess = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Chưa đến ngày tính lương!");
                return;
            }
            if (sucess)
            {
                MessageBox.Show("Tính thành công!");
            }
            else
            {
                MessageBox.Show("Tính lỗi!");
            }
        }
        public void OpenSetSalaryWindow()
        {
            SetSalaryWindow setSalaryWindow = new SetSalaryWindow();
            setSalaryWindow.ShowDialog();
        }

        public void AddEmployee(StackPanel parameter)
        {
            stack = parameter;
            AddEmployeeWindow addEmployee = new AddEmployeeWindow();
            try
            {
                addEmployee.txtIDEmployee.Text = (EmployeeDAL.Instance.ConvertDBToList()[EmployeeDAL.Instance.ConvertDBToList().Count - 1].IdEmployee + 1).ToString();
            }
            catch
            {
                addEmployee.txtIDEmployee.Text = "1";
            }
            addEmployee.txbConfirm.Text = "Thêm";
            if (CurrentAccount.Type == 1)
                addEmployee.cboPositionManage.IsEnabled = false;
            addEmployee.ShowDialog();
        }

        public void LoadEmployeesToView(HomeWindow homeWindow)
        {
            int i = 1;
            homeWindow.stkEmployee.Children.Clear();
            bool flag = false;
            foreach (var employee in EmployeeDAL.Instance.ConvertDBToList())
            {
                EmployeeControl temp = new EmployeeControl();
                flag = !flag;
                if (flag)
                {
                    temp.grdMain.Background = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");
                }
                temp.txbSerial.Text = i.ToString();
                i++;
                // load number fault and overtime and salary
                foreach (var salary in SalaryDAL.Instance.ConvertDBToList())
                {
                    if (employee.IdEmployee == salary.IdEmployee)
                    {
                        temp.nsNumOfShift.Text = decimal.Parse(salary.NumOfShift.ToString());
                        temp.nsNumOfFault.Text = decimal.Parse(salary.NumOfFault.ToString());
                        temp.txbTotalSalary.Text = string.Format("{0:n0}", salary.TotalSalary);
                        break;
                    }
                }
                temp.txbId.Text = employee.IdEmployee.ToString();
                temp.txbName.Text = employee.Name.ToString();
                temp.txbPosition.Text = employee.Position.ToString();
                if (CurrentAccount.Type == 1)
                {
                    if (employee.Position == "Nhân viên quản lý")
                    {
                        temp.btnEditEmployee.IsEnabled = false;
                    }
                }
                homeWindow.stkEmployee.Children.Add(temp);
            }
        }

        //Tab goods 
        public void LoadGoodsToView(StackPanel stk)
        {
            stk.Children.Clear();
            List<Goods> goodsList = GoodsDAL.Instance.ConvertDBToList();
            bool flag = false;
            int i = 1;
            foreach (var goods in goodsList)
            {
                GoodsControl temp = new GoodsControl();
                flag = !flag;
                if (flag)
                {
                    temp.grdMain.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFFFF");
                }
                temp.txbId.Text = goods.IdGoods.ToString();
                temp.txbOrderNum.Text = i.ToString();
                temp.txbName.Text = goods.Name.ToString();
                temp.txbQuantity.Text = goods.Quantity.ToString();
                temp.txbUnit.Text = goods.Unit.ToString();
                temp.txbUnitPrice.Text = goods.UnitPrice.ToString();
                if (CurrentAccount.Type == 2)
                {
                    temp.btnDeleteGoods.IsEnabled = false;
                    temp.btnEditGoods.IsEnabled = false;
                }
                stk.Children.Add(temp);
                i++;
            }
        }
        public void AddGoods(StackPanel stk)
        {
            stack = stk;
            AddGoodsWindow wdAddGoods = new AddGoodsWindow();
            List<Goods> goodsList = GoodsDAL.Instance.ConvertDBToList();
            try
            {
                wdAddGoods.txtIdGoods.Text = (goodsList[goodsList.Count() - 1].IdGoods + 1).ToString();
            }
            catch
            {
                wdAddGoods.txtIdGoods.Text = "1";
            }

            wdAddGoods.ShowDialog();
        }
    }
}

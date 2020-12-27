using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballFieldManagement.Models;
using FootballFieldManagement.Views;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.IO;
using System.Globalization;
using FootballFieldManagement.Resources.UserControls;
using FootballFieldManagement.DAL;
using System.Drawing.Printing;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Windows.Threading;

namespace FootballFieldManagement.ViewModels
{
    class EmployeeViewModel : BaseViewModel
    {
        public string SalaryBase { get; set; }
        public string MoneyPerShift { get; set; }
        public string MoneyPerFault { get; set; }
        public string StandardWorkDays { get; set; }
        public string SalaryMonth { get; set; }

        //UC Employee
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        //Add Employee Window
        public ICommand ExitCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        //Set Salary Window
        public ICommand SaveSetSalaryCommand { get; set; } // Click button "Lưu" trong SetSalaryWindow
        public ICommand ValueChangedCommand { get; set; } // Tăng giảm các numericspinner
        public ICommand SelectionChangedCommand { get; set; } // Chọn 1 nhân viên trong window SetSalary
        public ICommand SeparateThousandsCommand { get; set; } // định dạng tiền thành 0,000,000
        //Pay Salary Window
        public ICommand PaySalaryCommand { get; set; }
        public ICommand LoadedPaySalaryCommand { get; set; }
        public ICommand ChangedSalaryMonthCommand { get; set; }
        public ICommand OpenPaySalaryCommand { get; set; }

        public ICommand LoadedSetSalaryCommand { get; set; }

        private string id;
        public string Id { get => id; set => id = value; }
        List<string> iteamSourceMonth = new List<string>(); // chọn tháng trả lương 
        public List<string> IteamSourceMonth { get => iteamSourceMonth; set => iteamSourceMonth = value; }
        public string SelectedMonth { get => selectedMonth; set { selectedMonth = value; OnPropertyChanged(); } }


        private string selectedMonth;


        public string gender;
        public string imageName;
        private HomeWindow home;
        public HomeWindow Home { get => home; set => home = value; }

        public EmployeeViewModel()
        {
            //Add Employee Window
            ExitCommand = new RelayCommand<Window>((parameter) => true, (parameter) => parameter.Close());
            SelectImageCommand = new RelayCommand<Grid>((parameter) => true, (parameter) => SelectImage(parameter));
            SaveCommand = new RelayCommand<AddEmployeeWindow>((parameter) => true, (parameter) => AddEmployee(parameter));
            //UC Employee
            UpdateCommand = new RelayCommand<EmployeeControl>((parameter) => true, (parameter) => OpenUpdateWindow(parameter));
            DeleteCommand = new RelayCommand<EmployeeControl>((parameter) => true, (parameter) => DeleteEmployee(parameter));
            //Set Salary Window
            SeparateThousandsCommand = new RelayCommand<TextBox>((parameter) => true, (parameter) => SeparateThousands(parameter));
            SaveSetSalaryCommand = new RelayCommand<SetSalaryWindow>((parameter) => true, (parameter) => SaveSetSalary(parameter));
            ValueChangedCommand = new RelayCommand<EmployeeControl>((parameter) => true, (parameter) => UpdateQuantity(parameter));
            SelectionChangedCommand = new RelayCommand<SetSalaryWindow>((parameter) => true, (parameter) => SelectionChanged(parameter));
            LoadedSetSalaryCommand = new RelayCommand<SetSalaryWindow>((parameter) => true, (parameter) => LoadSetSalary(parameter));

            //Pay Salary Window
            ChangedSalaryMonthCommand = new RelayCommand<PaySalaryWindow>((parameter) => true, (parameter) => SelectionChangedMonth(parameter));
            PaySalaryCommand = new RelayCommand<PaySalaryWindow>((parameter) => true, (parameter) => PaySalary(parameter));
            LoadedPaySalaryCommand = new RelayCommand<PaySalaryWindow>((parameter) => true, (parameter) => LoadedPaySalary(parameter));
            OpenPaySalaryCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => OpenPaySalaryWindow(parameter));
        }
        public void OpenPaySalaryWindow(HomeWindow home)
        {
            PaySalaryWindow paySalaryWindow = new PaySalaryWindow();
            this.home = home;
            foreach (string item in EmployeeDAL.Instance.GetAllPosition())
            {
                SalarySetting salarySetting = SalarySettingDAL.Instance.GetSalarySettings(item);
                if (salarySetting == null)
                {
                    MessageBox.Show("Vui lòng thiết lập lương cho '" + item + "'!");
                    SetSalaryWindow wdSetSalary = new SetSalaryWindow();
                    wdSetSalary.cboTypeEmployee.SelectedItem = item;
                    wdSetSalary.ShowDialog();
                    return;
                }
            }
            paySalaryWindow.ShowDialog();
        }
        public void LoadedPaySalary(PaySalaryWindow paySalaryWindow)
        {
            iteamSourceMonth.Clear();
            for (int i = 1; i <= DateTime.Now.Month; i++)
            {
                iteamSourceMonth.Add("Tháng " + i.ToString());
            }
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            timer.Tick += (s, e) =>
            {
                paySalaryWindow.cboSalaryMonth.SelectedIndex = DateTime.Now.Month - 1;
                timer.Stop();
            };
            timer.Start();
        }

        //Pay salary window
        public void PaySalary(PaySalaryWindow paySalaryWindow)
        {
            if (string.IsNullOrEmpty(paySalaryWindow.cboSalaryMonth.Text))
            {
                paySalaryWindow.cboSalaryMonth.Text = "";
                paySalaryWindow.cboSalaryMonth.Focus();
                return;
            }
            if (SalarySettingDAL.Instance.ConvertDBToList().Count == 0)
            {
                MessageBox.Show("Vui lòng thiết lập lương");
                SetSalaryWindow wdSetSalary = new SetSalaryWindow();
                wdSetSalary.ShowDialog();
                return;
            }
            if (string.IsNullOrEmpty(paySalaryWindow.cboSalaryMonth.Text))
            {
                paySalaryWindow.cboSalaryMonth.Focus();
                return;
            }
            bool success = true;
            int idSalaryRecord = SalaryRecordDAL.Instance.SetID();
            long total = -1;
            for (int i = 0; i < paySalaryWindow.stkSalaryInfo.Children.Count; i++)
            {
                SalaryEmployeeControl control = (SalaryEmployeeControl)paySalaryWindow.stkSalaryInfo.Children[i];
                total += ConvertToNumber(control.txbTotalSalary.Text);
            }
            if (idSalaryRecord != -1 && total != -1)
            {
                SalaryRecord salaryRecord = new SalaryRecord(idSalaryRecord, DateTime.Now, total, CurrentAccount.IdAccount);
                if (!SalaryRecordDAL.Instance.AddIntoDB(salaryRecord))
                {
                    success = false;
                }
                if (!SalaryDAL.Instance.UpdateIdSalaryRecord(idSalaryRecord, int.Parse(selectedMonth.Split(' ')[1]), DateTime.Now.Year))
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }
            if (success)
            {
                MessageBox.Show("Trả lương thành công!");
                if (selectedMonth.Split(' ')[1] == DateTime.Now.Month.ToString())
                {
                    LoadEmployeesToView(home);
                    home.btnCalculateSalary.IsEnabled = false;
                }
            }
            else
            {
                MessageBox.Show("Trả lương thất bại!");
            }
            paySalaryWindow.Close();
        }

        //Khi chọn tháng thì sẽ tự động tính lương
        public void SelectionChangedMonth(PaySalaryWindow paySalaryWindow)
        {
            if (selectedMonth == null)
            {
                return;
            }
            bool flag = false;
            int i = 1;
            paySalaryWindow.stkSalaryInfo.Children.Clear();
            //Nếu đã trả lương thì không tính lương lại mà hiển thị lương đã có
            if (SalaryDAL.Instance.IsExistIdSalaryRecord(selectedMonth.Split(' ')[1], DateTime.Now.Year.ToString()))
            {
                paySalaryWindow.btnPaySalary.IsEnabled = false;
                paySalaryWindow.btnPaySalary.Content = "Đã trả lương";
                foreach (var salary in SalaryDAL.Instance.GetPaidSalary(selectedMonth.Split(' ')[1], DateTime.Now.Year.ToString()))
                {
                    SalaryEmployeeControl control = new SalaryEmployeeControl();
                    control.txbOrderNum.Text = (i++).ToString();
                    flag = !flag;
                    if (flag)
                    {
                        control.ucrMain.Background = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");
                    }
                    Employee employee = EmployeeDAL.Instance.GetEmployee(salary.IdEmployee.ToString());
                    if (employee.IsDeleted == 0)
                    {
                        control.txbName.Text = employee.Name;
                    }
                    else
                    {
                        control.txbName.Text = employee.Name + " (Đã nghĩ việc)";
                    }
                    control.txbTotalSalary.Text = string.Format("{0:N0}", salary.TotalSalary);
                    paySalaryWindow.stkSalaryInfo.Children.Add(control);
                }
            }
            //Chưa trả lương
            else
            {
                paySalaryWindow.btnPaySalary.Content = "Trả lương";
                paySalaryWindow.btnPaySalary.IsEnabled = true;

                //Hiện danh sách lương của nhân viên và tính lương
                paySalaryWindow.stkSalaryInfo.Children.Clear();
                foreach (var salary in SalaryDAL.Instance.GetSalaryOfEmployee(selectedMonth.Split(' ')[1], DateTime.Now.Year.ToString()))
                {

                    int workdays = AttendanceDAL.Instance.GetCount(salary.IdEmployee.ToString());
                    string positionEmployee = EmployeeDAL.Instance.GetPosition(salary.IdEmployee.ToString());
                    SalarySetting salarySetting = SalarySettingDAL.Instance.GetSalarySettings(positionEmployee);
                    if (workdays < 0)
                    {
                        return;
                    }
                    //Lấy ra salary setting có loại nhân viên trùng khớp với idEmployee
                    if (workdays <= salarySetting.StandardWorkDays)
                    {
                        salary.TotalSalary = (salarySetting.SalaryBase / salarySetting.StandardWorkDays) * workdays + salary.NumOfShift * salarySetting.MoneyPerShift - salary.NumOfFault * salarySetting.MoneyPerFault;
                    }
                    else
                    {
                        salary.TotalSalary = salarySetting.SalaryBase + salary.NumOfShift * salarySetting.MoneyPerShift - salary.NumOfFault * salarySetting.MoneyPerFault;
                    }
                    if (salary.TotalSalary < 0)
                    {
                        salary.TotalSalary = 0;
                    }
                    if (SalaryDAL.Instance.UpdateTotalSalary(salary))
                    {
                        //Hiển thị lên thông tin
                        SalaryEmployeeControl control = new SalaryEmployeeControl();
                        control.txbOrderNum.Text = (i++).ToString();
                        flag = !flag;
                        if (flag)
                        {
                            control.ucrMain.Background = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");
                        }
                        control.txbName.Text = EmployeeDAL.Instance.GetEmployeeByIdEmployee(salary.IdEmployee.ToString()).Name;
                        control.txbTotalSalary.Text = string.Format("{0:N0}", salary.TotalSalary);
                        paySalaryWindow.stkSalaryInfo.Children.Add(control);
                    }
                }
                //Cập nhật lên stack pannel stkEmployee của home window
                for (int j = 0; j < home.stkEmployee.Children.Count; j++)
                {
                    EmployeeControl control = (EmployeeControl)home.stkEmployee.Children[j];
                    control.txbTotalSalary.Text = string.Format("{0:N0}", SalaryDAL.Instance.GetTotalSalary(control.txbId.Text, DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()));
                }
            }
        }
        public void LoadSetSalary(SetSalaryWindow setSalary)
        {
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            timer.Tick += (s, e) =>
            {
                foreach (string item in EmployeeDAL.Instance.GetAllPosition())
                {
                    SalarySetting salarySetting = SalarySettingDAL.Instance.GetSalarySettings(item);
                    if (salarySetting == null)
                    {
                        setSalary.cboTypeEmployee.Text = item;
                        break;
                    }
                }
                timer.Stop();
            };
            timer.Start();
        }
        //Add Employee Window
        public void AddEmployee(AddEmployeeWindow parameter)
        {
            if (string.IsNullOrEmpty(parameter.txtName.Text) || FootballFieldDAL.Instance.isExistFieldName(parameter.txtName.Text))
            {
                parameter.txtName.Text = "";
                parameter.txtName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.cboPosition.Text))
            {
                parameter.cboPosition.Focus();
            }
            if (parameter.dpBirthDate.Text == "")
            {
                parameter.dpBirthDate.Focus();
                return;
            }
            else
            {
                DateTime dateTime = new DateTime();
                if (!DateTime.TryParse(parameter.dpBirthDate.Text, out dateTime))
                {
                    CustomMessageBox.Show("Vui lòng nhập lại ngày sinh!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    parameter.dpBirthDate.Focus();
                    return;
                }
            }
            if (parameter.txtAddress.Text == "")
            {
                parameter.txtAddress.Focus();
                parameter.txtAddress.Text = "";
                return;
            }
            if (parameter.txtTelephoneNumber.Text == "" || !Regex.IsMatch(parameter.txtTelephoneNumber.Text, @"^[0-9]+$"))
            {
                parameter.txtTelephoneNumber.Focus();
                parameter.txtTelephoneNumber.Text = "";
                return;
            }

            if (parameter.dpWorkDate.Text == "")
            {
                CustomMessageBox.Show("Vui lòng nhập ngày vào làm!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.dpWorkDate.Focus();
                return;
            }
            else
            {
                DateTime dateTime = new DateTime();
                if (!DateTime.TryParse(parameter.dpWorkDate.Text, out dateTime))
                {
                    CustomMessageBox.Show("Vui lòng nhập lại ngày vào làm!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    parameter.dpWorkDate.Focus();
                    return;
                }
                if (dateTime < DateTime.Parse(parameter.dpBirthDate.Text))
                {
                    parameter.dpWorkDate.Focus();
                    return;
                }
            }
            if (parameter.rdoMale.IsChecked.Value == true)
                gender = "Nam";
            else
                gender = "Nữ";
            if (parameter.grdSelectImage.Background == null)
            {
                CustomMessageBox.Show("Vui lòng thêm hình ảnh!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            byte[] imgByteArr;
            try
            {
                imgByteArr = Converter.Instance.ConvertImageToBytes(imageName);
            }
            catch
            {
                imgByteArr = EmployeeDAL.Instance.GetEmployeeByIdEmployee(parameter.txtIDEmployee.Text).ImageFile;
            }
            imageName = null;
            Employee employee = new Employee(int.Parse(parameter.txtIDEmployee.Text), parameter.txtName.Text, gender,
                parameter.txtTelephoneNumber.Text, parameter.txtAddress.Text, DateTime.Parse(parameter.dpBirthDate.Text),
                parameter.cboPosition.Text, DateTime.Parse(parameter.dpWorkDate.Text), -1, imgByteArr, 0);
            Employee current = EmployeeDAL.Instance.GetEmployeeByIdEmployee(parameter.txtIDEmployee.Text);
            if (current != null && current.IdAccount != -1)
            {
                if (employee.Position == "Nhân viên thu ngân")
                {
                    AccountDAL.Instance.UpdateType(new Account(current.IdAccount, "", "", 2));
                }
                if (employee.Position == "Nhân viên quản lý")
                {
                    AccountDAL.Instance.UpdateType(new Account(current.IdAccount, "", "", 1));
                }
                if (employee.Position == "Bảo vệ")
                {
                    int temp = current.IdAccount;
                    current.IdAccount = -1;
                    EmployeeDAL.Instance.UpdateIdAccount(current);
                    AccountDAL.Instance.DeleteAccount(temp.ToString());
                }
            }
            EmployeeDAL.Instance.AddEmployee(employee);
            SetSalaryEmployee(parameter);
            parameter.isAdded.Text = "1";
            parameter.Close();
        }
        public void SelectImage(Grid parameter)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imageName = op.FileName;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imageName);
                bitmap.EndInit();
                imageBrush.ImageSource = bitmap;
                parameter.Background = imageBrush;
                if (parameter.Children.Count > 1)
                {
                    parameter.Children.Remove(parameter.Children[0]);
                    parameter.Children.Remove(parameter.Children[1]);
                }
            }
        }
        public void SetPickedDay(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            try
            {
                datePicker.Text = ((DateTime)datePicker.SelectedDate).ToString();
            }
            catch
            {

            }
        }
        //UC Employee
        public void DeleteEmployee(EmployeeControl parameter)
        {
            MessageBoxResult result = CustomMessageBox.Show("Xác nhận xóa nhân viên?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Employee employee = EmployeeDAL.Instance.GetEmployeeByIdEmployee(parameter.txbId.Text);
                Account account = new Account(employee.IdAccount, "", "", 3);
                employee.IsDeleted = 1;
                //Lấy Home Window
                HomeWindow homeWd = (HomeWindow)(((Grid)((Grid)((Grid)((Grid)((ScrollViewer)((StackPanel)(parameter.Parent)).Parent).Parent).Parent).Parent).Parent).Parent);
                if (EmployeeDAL.Instance.UpdateOnDB(employee) && (employee.IdAccount == -1 || AccountDAL.Instance.UpdateType(account)))
                {
                    homeWd.stkEmployee.Children.Remove(parameter);
                    //Chỉnh lại màu
                    bool flag = false;
                    for (int i = 0; i < homeWd.stkEmployee.Children.Count; i++)
                    {
                        EmployeeControl temp = (EmployeeControl)homeWd.stkEmployee.Children[i];
                        flag = !flag;
                        if (flag)
                        {
                            temp.grdMain.Background = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");
                        }
                        else
                        {
                            temp.grdMain.Background = (Brush)new BrushConverter().ConvertFromString("#F4EEFF");
                        }
                        temp.txbSerial.Text = (i + 1).ToString();
                    }
                }
                else
                {
                    CustomMessageBox.Show("Xoá thất bại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
                temp.txbId.Text = employee.IdEmployee.ToString();
                temp.txbName.Text = employee.Name.ToString();
                temp.txbPosition.Text = employee.Position.ToString();
                temp.nsNumOfFault.IsEnabled = false;
                temp.nsNumOfShift.IsEnabled = false;
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
        public void OpenUpdateWindow(EmployeeControl temp)
        {
            Employee employee = EmployeeDAL.Instance.GetEmployeeByIdEmployee(temp.txbId.Text);
            AddEmployeeWindow child = new AddEmployeeWindow();
            if (employee.IdEmployee.ToString() == temp.txbId.Text)
            {
                child.txtIDEmployee.Text = employee.IdEmployee.ToString();

                child.txtName.Text = employee.Name;
                child.txtName.SelectionStart = child.txtName.Text.Length;
                child.txtName.SelectionLength = 0;

                child.txtTelephoneNumber.Text = employee.Phonenumber;
                child.txtTelephoneNumber.SelectionStart = child.txtTelephoneNumber.Text.Length;
                child.txtTelephoneNumber.SelectionLength = 0;

                child.txtAddress.Text = employee.Address;
                child.txtAddress.SelectionStart = child.txtAddress.Text.Length;
                child.txtAddress.SelectionLength = 0;

                child.cboPosition.Text = employee.Position;

                if (employee.Gender == "Nam")
                    child.rdoMale.IsChecked = true;
                else
                    child.rdoFemale.IsChecked = true;
                child.dpBirthDate.SelectedDate = DateTime.Parse(employee.DateOfBirth.ToString());
                child.dpWorkDate.SelectedDate = DateTime.Parse(employee.Startingdate.ToString());
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(employee.ImageFile);
                child.grdSelectImage.Background = imageBrush;
                if (child.grdSelectImage.Children.Count > 1)
                {
                    child.grdSelectImage.Children.Remove(child.grdSelectImage.Children[0]);
                    child.grdSelectImage.Children.Remove(child.grdSelectImage.Children[1]);
                }
            }
            child.btnSave.ToolTip = "Cập nhật thông tin nhân viên";
            if (CurrentAccount.Type == 1)
            {
                child.cboPositionManage.IsEnabled = false;
            }
            child.Title = "Cập nhật thông tin nhân viên";
            child.ShowDialog();
            Employee fixedEmployee = EmployeeDAL.Instance.GetEmployeeByIdEmployee(temp.txbId.Text);
            temp.txbName.Text = fixedEmployee.Name;
            temp.txbPosition.Text = fixedEmployee.Position;
        }
        public void UpdateQuantity(EmployeeControl parameter)//Lưu số lượng tăng ca và lỗi
        {
            Salary salary = new Salary();
            salary.NumOfFault = int.Parse(parameter.nsNumOfFault.Value.ToString());
            salary.NumOfShift = int.Parse(parameter.nsNumOfShift.Value.ToString());
            salary.IdEmployee = int.Parse(parameter.txbId.Text);
            salary.SalaryMonth = DateTime.Now;
            SalaryDAL.Instance.UpdateQuantity(salary);
        }//Lưu số lượng tăng ca và lỗi
         //Set Salary Window      
        public void SelectionChanged(SetSalaryWindow parameter)//select item của combobox loại nhân viên trong SetSalaryWindow
        {

            ComboBoxItem tmp = (ComboBoxItem)parameter.cboTypeEmployee.SelectedItem;
            SalarySetting salarySetting = SalarySettingDAL.Instance.GetSalarySettings(tmp.Content.ToString());
            if (salarySetting != null)
            {
                parameter.txtSalaryBasic.Text = salarySetting.SalaryBase.ToString();
                parameter.txtStandardWorkDays.Text = salarySetting.StandardWorkDays.ToString();
                parameter.txtOvertime.Text = salarySetting.MoneyPerShift.ToString();
                parameter.txtSalaryDeduction.Text = salarySetting.MoneyPerFault.ToString();
            }
            else
            {
                parameter.txtSalaryBasic.Text = null;
                parameter.txtStandardWorkDays.Text = null;
                parameter.txtOvertime.Text = null;
                parameter.txtSalaryDeduction.Text = null;
            }
        }
        public void SaveSetSalary(SetSalaryWindow parameter)
        {
            if (parameter == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(parameter.cboTypeEmployee.Text))
            {
                parameter.cboTypeEmployee.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.txtSalaryBasic.Text))
            {
                parameter.txtSalaryBasic.Focus();
                parameter.txtSalaryBasic.Text = "";
                return;
            }
            if (!Regex.IsMatch(parameter.txtStandardWorkDays.Text, @"^[0-9]+$") || int.Parse(parameter.txtStandardWorkDays.Text) < 1 || int.Parse(parameter.txtStandardWorkDays.Text) > 30)
            {
                parameter.txtStandardWorkDays.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.txtOvertime.Text))
            {
                parameter.txtOvertime.Focus();
                parameter.txtOvertime.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(parameter.txtSalaryDeduction.Text))
            {
                parameter.txtSalaryDeduction.Focus();
                parameter.txtSalaryDeduction.Text = "";
                return;
            }
            SalarySetting salarySetting = new SalarySetting(ConvertToNumber(parameter.txtSalaryBasic.Text),
                ConvertToNumber(parameter.txtOvertime.Text), ConvertToNumber(parameter.txtSalaryDeduction.Text),
                parameter.cboTypeEmployee.Text, int.Parse(parameter.txtStandardWorkDays.Text));
            //update salary setting
            bool isExist = false;
            if (SalarySettingDAL.Instance.GetSalarySettings(parameter.cboTypeEmployee.Text) != null)
            {
                isExist = true;
                if (SalarySettingDAL.Instance.UpdateDB(salarySetting))
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            //set salary 
            if (!isExist)
            {
                if (SalarySettingDAL.Instance.AddIntoDB(salarySetting))
                {
                    MessageBox.Show("Đã thiết lập lương cho '" + salarySetting.TypeEmployee + "'!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                else
                {
                    MessageBox.Show("Thiết lập thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            for (int i = 0; i < 3; i++)
            {
                parameter.cboTypeEmployee.SelectedIndex = i;
                if (SalarySettingDAL.Instance.GetSalarySettings(parameter.cboTypeEmployee.Text) == null)
                {
                    return;
                }
            }
            parameter.Close();
        }
        public void SetSalaryEmployee(AddEmployeeWindow parameter)
        {
            //thêm lương cơ bản cho nhân viên khi thêm một nhân viên
            if (!SalaryDAL.Instance.IsExist(parameter.txtIDEmployee.Text, DateTime.Now))
            {
                Salary salary = new Salary();
                salary.IdEmployee = int.Parse(parameter.txtIDEmployee.Text);
                salary.NumOfFault = 0;
                salary.NumOfShift = 0;
                salary.TotalSalary = -1;
                salary.SalaryMonth = DateTime.Now;
                if (!SalaryDAL.Instance.IsExistIdSalaryRecord(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString()))
                {
                    SalaryDAL.Instance.AddIntoDB(salary);
                }
            }
        }
    }
}

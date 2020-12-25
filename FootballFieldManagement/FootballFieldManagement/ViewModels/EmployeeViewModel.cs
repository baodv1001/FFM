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
        public ICommand LoadedCommand { get; set; }

        private string id;
        public string Id { get => id; set => id = value; }


        public string gender;
        public string imageName;

        public EmployeeViewModel()
        {
            //Add Employee Window
            ExitCommand = new RelayCommand<Window>((parameter) => true, (parameter) => parameter.Close());
            SelectImageCommand = new RelayCommand<Grid>((parameter) => true, (parameter) => SelectImage(parameter));
            SaveCommand = new RelayCommand<AddEmployeeWindow>((parameter) => true, (parameter) => AddEmployee(parameter));
            //UC Employee
            UpdateCommand = new RelayCommand<TextBlock>((parameter) => true, (parameter) => OpenUpdateWindow(parameter));
            DeleteCommand = new RelayCommand<EmployeeControl>((parameter) => true, (parameter) => DeleteEmployee(parameter));
            //Set Salary Window
            SeparateThousandsCommand = new RelayCommand<TextBox>((parameter) => true, (parameter) => SeparateThousands(parameter));
            SaveSetSalaryCommand = new RelayCommand<SetSalaryWindow>((parameter) => true, (parameter) => SaveSetSalary(parameter));
            ValueChangedCommand = new RelayCommand<EmployeeControl>((parameter) => true, (parameter) => UpdateQuantity(parameter));
            SelectionChangedCommand = new RelayCommand<SetSalaryWindow>((parameter) => true, (parameter) => SelectionChanged(parameter));
            LoadedCommand = new RelayCommand<SetSalaryWindow>((parameter) => true, (parameter) => Loaded(parameter));
        }
        public void Loaded(SetSalaryWindow setSalary)
        {
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            timer.Tick += (s, e) =>
            {
                setSalary.cboTypeEmployee.SelectedIndex = 0;
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
                    MessageBox.Show("Vui lòng nhập lại ngày sinh!");
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
                MessageBox.Show("Vui lòng nhập ngày vào làm!");
                parameter.dpWorkDate.Focus();
                return;
            }
            else
            {
                DateTime dateTime = new DateTime();
                if (!DateTime.TryParse(parameter.dpWorkDate.Text, out dateTime))
                {
                    MessageBox.Show("Vui lòng nhập lại ngày vào làm!");
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
                MessageBox.Show("Vui lòng thêm hình ảnh!");
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
            MessageBoxResult result = MessageBox.Show("Xác nhận xóa nhân viên?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Employee employee = EmployeeDAL.Instance.GetEmployeeByIdEmployee(parameter.txbId.Text);
                Account account = new Account(employee.IdAccount, "", "", 3);
                employee.IsDeleted = 1;
                //Lấy Home Window
                HomeWindow home = (HomeWindow)(((Grid)((Grid)((Grid)((Grid)((ScrollViewer)((StackPanel)(parameter.Parent)).Parent).Parent).Parent).Parent).Parent).Parent);
                if (EmployeeDAL.Instance.UpdateOnDB(employee) && (employee.IdAccount == -1 || AccountDAL.Instance.UpdateType(account)))
                {

                    MessageBox.Show("Đã xóa thành công!");
                    LoadEmployeesToView(home);
                }
                else
                {
                    MessageBox.Show("Xoá thất bại");
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
                // load number fault and overtime and salary
                foreach (var salary in SalaryDAL.Instance.ConvertDBToList())
                {
                    if (employee.IdEmployee == salary.IdEmployee)
                    {
                        temp.nsNumOfShift.Text = decimal.Parse(salary.NumOfShift.ToString());
                        temp.nsNumOfFault.Text = decimal.Parse(salary.NumOfFault.ToString());
                        if (salary.TotalSalary == -1)
                        {
                            temp.txbTotalSalary.Text = "0";
                        }
                        else
                        {
                            temp.txbTotalSalary.Text = string.Format("{0:n0}", salary.TotalSalary);
                        }
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
        public void OpenUpdateWindow(TextBlock parameter)
        {
            Employee employee = EmployeeDAL.Instance.GetEmployeeByIdEmployee(parameter.Text);
            AddEmployeeWindow child = new AddEmployeeWindow();
            if (employee.IdEmployee.ToString() == parameter.Text)
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
            foreach (var salarySetting in SalarySettingDAL.Instance.GetSalarySettings(tmp.Content.ToString()))
            {
                parameter.txtSalaryBasic.Text = salarySetting.SalaryBase.ToString();
                parameter.txtStandardWorkDays.Text = salarySetting.StandardWorkDays.ToString();
                parameter.txtOvertime.Text = salarySetting.MoneyPerShift.ToString();
                parameter.txtSalaryDeduction.Text = salarySetting.MoneyPerFault.ToString();
                return;
            }
            parameter.txtSalaryBasic.Text = "";
            parameter.txtStandardWorkDays.Text = "";
            parameter.txtOvertime.Text = "";
            parameter.txtSalaryDeduction.Text = "";
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
            foreach (var tmp in SalarySettingDAL.Instance.GetSalarySettings(parameter.cboTypeEmployee.Text))
            {
                isExist = true;
                if (SalarySettingDAL.Instance.UpdateDB(salarySetting))
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return;
            }
            //set salary 
            if (!isExist)
            {
                if (SalarySettingDAL.Instance.AddIntoDB(salarySetting))
                {
                    MessageBox.Show("Thiết lập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Thiết lập thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            parameter.Close();
        }
        public void SetSalaryEmployee(AddEmployeeWindow parameter)
        {
            //thêm lương cơ bản cho nhân viên khi thêm một nhân viên
            if (!SalaryDAL.Instance.isExit(parameter.txtIDEmployee.Text, DateTime.Now))
            {
                Salary salary = new Salary();
                salary.IdEmployee = int.Parse(parameter.txtIDEmployee.Text);
                salary.NumOfFault = 0;
                salary.NumOfShift = 0;
                salary.TotalSalary = -1;
                salary.SalaryMonth = DateTime.Now;
                if (!SalaryDAL.Instance.AddIntoDB(salary))
                {
                    MessageBox.Show("Lỗi khi thiết lập lương!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

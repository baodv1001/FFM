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

namespace FootballFieldManagement.ViewModels
{
    class EmployeeViewModel : BaseViewModel
    {
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

        private string id;
        public string Id { get => id; set => id = value; }

        public string gender;
        public string imageName;
        private ObservableCollection<int> itemSourceDay = new ObservableCollection<int>();
        public ObservableCollection<int> ItemSourceDay { get => itemSourceDay; set => itemSourceDay = value; }

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
        }
        //Add Employee Window
        public void AddEmployee(AddEmployeeWindow parameter)
        {
            if (string.IsNullOrEmpty(parameter.txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!");
                parameter.txtName.Focus();
                return;
            }
            if (parameter.cboPosition.Text == "")
            {
                MessageBox.Show("Vui lòng nhập chức vụ!");
                parameter.cboPosition.Focus();
                return;
            }
            else
            {
                if (parameter.cboPosition.Text != "Bảo vệ" && parameter.cboPosition.Text != "Nhân viên quản lý" && parameter.cboPosition.Text != "Nhân viên thu ngân")
                {
                    MessageBox.Show("Vui lòng nhập đúng chức vụ!");
                    parameter.cboPosition.Focus();
                    return;
                }
            }
            if (parameter.dpBirthDate.Text == "")
            {
                MessageBox.Show("Vui lòng nhập ngày sinh!");
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
                MessageBox.Show("Vui lòng nhập địa chỉ!");
                parameter.txtAddress.Focus();
                return;
            }
            if (parameter.txtTelephoneNumber.Text == "")
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!");
                parameter.txtTelephoneNumber.Focus();
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
                    MessageBox.Show("Vui lòng nhập lại ngày vào làm lớn hơn ngày sinh!");
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
                imgByteArr = EmployeeDAL.Instance.GetEmployee(parameter.txtIDEmployee.Text).ImageFile;
            }
            imageName = null;
            Employee employee = new Employee(int.Parse(parameter.txtIDEmployee.Text), parameter.txtName.Text, gender,
                parameter.txtTelephoneNumber.Text, parameter.txtAddress.Text, DateTime.Parse(parameter.dpBirthDate.Text), 0,
                parameter.cboPosition.Text, DateTime.Parse(parameter.dpWorkDate.Text), -1, imgByteArr);
            Employee current = EmployeeDAL.Instance.GetEmployee(parameter.txtIDEmployee.Text);
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
            SetBaseSalary(parameter);
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
                List<Employee> employees = EmployeeDAL.Instance.ConvertDBToList();
                foreach (var employee in employees)
                {
                    if (employee.IdEmployee.ToString() == parameter.txbId.Text)
                    {
                        bool isSuccess1 = SalaryDAL.Instance.DeleteSalary(parameter.txbId.Text);
                        bool isSuccess2 = AttendanceDAL.Instance.DeleteAttendance(employee.IdEmployee.ToString());
                        bool isSuccess3 = EmployeeDAL.Instance.DeleteEmployee(employee);
                        bool isSuccess4 = BillDAL.Instance.UpdateIdAccount(employee.IdAccount.ToString());
                        bool isSuccess5 = StockReceiptDAL.Instance.UpdateIdAccount(employee.IdAccount.ToString());
                        bool isSuccess6 = AccountDAL.Instance.DeleteAccount(employee.IdAccount.ToString());
                        if ((isSuccess1 && isSuccess2 && isSuccess3 && isSuccess4 && isSuccess5 && isSuccess6) || (isSuccess3))
                        {
                            MessageBox.Show("Đã xóa thành công!");
                        }
                        else
                        {
                            MessageBox.Show("Xoá thất bại");
                        }
                        break;
                    }
                }
            }
        }
        public void OpenUpdateWindow(TextBlock parameter)
        {
            List<Employee> employees = EmployeeDAL.Instance.ConvertDBToList();

            AddEmployeeWindow child = new AddEmployeeWindow();
            foreach (var employee in employees)
            {
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

                    if (employee.Gender == "Nam  ")
                        child.rdoMale.IsChecked = true;
                    else
                        child.rdoFemale.IsChecked = true;
                    child.dpBirthDate.Text = employee.DateOfBirth.ToString();
                    child.dpWorkDate.Text = employee.Startingdate.ToString();
                    ImageBrush imageBrush = new ImageBrush();
                    imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(employee.ImageFile);
                    child.grdSelectImage.Background = imageBrush;
                    if (child.grdSelectImage.Children.Count > 1)
                    {
                        child.grdSelectImage.Children.Remove(child.grdSelectImage.Children[0]);
                        child.grdSelectImage.Children.Remove(child.grdSelectImage.Children[1]);
                    }
                    break;
                }
            }
            child.btnSave.ToolTip = "Cập nhật thông tin nhân viên";
            if (CurrentAccount.Type == 1)
            {
                child.cboPositionManage.IsEnabled = false;
            }
            child.ShowDialog();
        }
        public void UpdateQuantity(EmployeeControl parameter)
        {
            Salary salary = new Salary();
            salary.NumOfFault = int.Parse(parameter.nsNumOfFault.Value.ToString());
            salary.NumOfShift = int.Parse(parameter.nsNumOfShift.Value.ToString());
            salary.IdEmployee = int.Parse(parameter.txbId.Text);
            SalaryDAL.Instance.UpdateQuantity(salary);
        }//Lưu số lượng tăng ca và lỗi
        //Set Salary Window    
        public void SelectionChanged(SetSalaryWindow parameter)
        {

            foreach (var salary in SalaryDAL.Instance.ConvertDBToList())
            {
                ComboBoxItem tmp = (ComboBoxItem)parameter.cboTypeEmployee.SelectedItem;
                if (SalaryDAL.Instance.GetPosition(salary.IdEmployee.ToString()) == tmp.Content.ToString())
                {
                    parameter.txtSalaryBasic.Text = salary.SalaryBasic.ToString();
                    parameter.cboStandardWorkDays.Text = salary.StandardWorkDays.ToString();
                    parameter.txtOvertime.Text = salary.MoneyPerShift.ToString();
                    parameter.txtSalaryDeduction.Text = salary.MoneyPerFault.ToString();
                    return;
                }
            }
            parameter.txtSalaryBasic.Text = "";
            parameter.cboStandardWorkDays.Text = "";
            parameter.txtOvertime.Text = "";
            parameter.txtSalaryDeduction.Text = "";
        }//select item của combobox loại nhân viên trong SetSalaryWindow
        public void SetItemSourceDay()
        {
            itemSourceDay.Clear();
            for (int i = 1; i <= 31; i++)
            {
                itemSourceDay.Add(i);
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
                MessageBox.Show("Vui lòng chọn loại nhân viên!");
                parameter.cboTypeEmployee.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.txtSalaryBasic.Text))
            {
                MessageBox.Show("Vui lòng nhập mức lương cơ bản!");
                parameter.txtSalaryBasic.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.txtOvertime.Text))
            {
                MessageBox.Show("Vui lòng nhập số tiền mỗi ca!");
                parameter.txtOvertime.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.txtSalaryDeduction.Text))
            {
                MessageBox.Show("Vui lòng nhập số tiền mỗi lỗi!");
                parameter.txtSalaryDeduction.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.cboStandardWorkDays.Text))
            {
                MessageBox.Show("Vui lòng nhập số ngày công chuẩn!");
                parameter.txtSalaryDeduction.Focus();
                return;
            }
            Salary salary = new Salary(ConvertToNumber(parameter.txtSalaryBasic.Text), 0,
                ConvertToNumber(parameter.txtOvertime.Text), 0, ConvertToNumber(parameter.txtSalaryDeduction.Text), 0, 0,
                int.Parse(parameter.cboStandardWorkDays.Text));
            //update salary
            bool isExist = false;
            foreach (var tmp in SalaryDAL.Instance.ConvertDBToList())
            {
                if (SalaryDAL.Instance.GetPosition(tmp.IdEmployee.ToString()) == parameter.cboTypeEmployee.Text)
                {
                    isExist = true;
                    salary.IdEmployee = tmp.IdEmployee;
                    if (!SalaryDAL.Instance.ResetSalary(salary))
                    {
                        MessageBox.Show("Thiết lập lương thất bại!");
                        parameter.Close();
                        return;
                    }
                }
            }
            //set salary
            if (!isExist)
            {
                foreach (Employee employee in EmployeeDAL.Instance.ConvertDBToList())
                {
                    if (employee.Position == parameter.cboTypeEmployee.Text)
                    {
                        salary.IdEmployee = employee.IdEmployee;
                        if (!SalaryDAL.Instance.AddIntoDB(salary))
                        {
                            MessageBox.Show("Thiết lập lương thất bại!");
                            parameter.Close();
                            return;
                        }
                    }
                }
            }
            MessageBox.Show("Thiết lập lương thành công!");
            parameter.Close();
        }
        public void SetBaseSalary(AddEmployeeWindow parameter)
        {
            List<Salary> salaries = SalaryDAL.Instance.ConvertDBToList();
            if (salaries.Count == 0)
            {
                Salary salary1 = new Salary(0, 0, 0, 0, 0, int.Parse(parameter.txtIDEmployee.Text), 0, 0);
                SalaryDAL.Instance.AddIntoDB(salary1);
                return;
            }
            //cập nhật lại lương cho nhân viên khi cập nhật chức vụ
            if (int.Parse(parameter.txtIDEmployee.Text) <= salaries[salaries.Count - 1].IdEmployee)
            {
                foreach (var salary in salaries)
                {
                    if (SalaryDAL.Instance.GetPosition(salary.IdEmployee.ToString()) == parameter.cboPosition.Text && salary.IdEmployee != int.Parse(parameter.txtIDEmployee.Text))
                    {
                        salary.TotalSalary = 0;
                        salary.IdEmployee = int.Parse(parameter.txtIDEmployee.Text);
                        SalaryDAL.Instance.UpdateTotalSalary(salary);
                        SalaryDAL.Instance.ResetSalary(salary);
                        return;
                    }
                }
                Salary salary1 = new Salary(0, 0, 0, 0, 0, int.Parse(parameter.txtIDEmployee.Text), 0, 0);
                SalaryDAL.Instance.ResetSalary(salary1);
                SalaryDAL.Instance.UpdateTotalSalary(salary1);
            }
            //thêm lương cơ bản cho nhân viên khi thêm một nhân viên
            else
            {
                foreach (var salary in salaries)
                {
                    if (SalaryDAL.Instance.GetPosition(salary.IdEmployee.ToString()) == parameter.cboPosition.Text)
                    {
                        salary.IdEmployee = int.Parse(parameter.txtIDEmployee.Text);
                        salary.TotalSalary = 0;
                        salary.NumOfFault = 0;
                        salary.NumOfShift = 0;
                        SalaryDAL.Instance.AddIntoDB(salary);
                        return;
                    }
                }
                Salary salary1 = new Salary(0, 0, 0, 0, 0, int.Parse(parameter.txtIDEmployee.Text), 0, 0);
                SalaryDAL.Instance.AddIntoDB(salary1);
            }
        }
        public void SetMaxValue(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("\\b([1-9]|[12][0-9]|3[01])\\b");

            e.Handled = !regex.IsMatch(e.Text);
        }

        
    }
}

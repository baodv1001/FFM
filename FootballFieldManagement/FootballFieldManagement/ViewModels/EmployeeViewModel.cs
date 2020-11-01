using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballFieldManagement.Models;
using FootballFieldManegement.DAL;
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

namespace FootballFieldManagement.ViewModels
{
    class EmployeeViewModel:HomeViewModel
    {
        public ICommand SaveCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        private string id;
        public string Id { get => id; set => id = value; }
        public string gender;
        public string image;
        public EmployeeViewModel()
        {

            SaveCommand = new RelayCommand<fAddEmployee>((parameter) => true, (parameter) => AddEmployee(parameter));
            UpdateCommand = new RelayCommand<TextBlock>((parameter) => true, (parameter) => OpenUpdateWindow(parameter));
            DeleteCommand = new RelayCommand<TextBlock>((parameter) => true, (parameter) => DeleteEmployee(parameter.Text));
            ExitCommand = new RelayCommand<Window>((parameter) => true, (parameter) => parameter.Close());
            SelectImageCommand = new RelayCommand<Grid>((parameter) => true, (parameter) => SelectImage(parameter));
        }
        public void SelectImage(Grid parameter)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +"JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +"Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                image = op.FileName;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(image);
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
        public void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        public void AddEmployee(fAddEmployee parameter)
        {
            if (parameter.txtName.Text == null)
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
                if (parameter.cboPosition.Text != "Bảo vệ" && parameter.cboPosition.Text != "Nhân viên quản lý")
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
                if (!DateTime.TryParse(parameter.dpBirthDate.Text,out dateTime))
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
                if(dateTime<parameter.dpBirthDate.DisplayDate)
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
            string filename =@"..//..//Resources//Images//"+parameter.txtIDEmployee.Text.ToString()+".png";
            if (parameter.grdSelectImage.Background == null)
            {
                MessageBox.Show("Vui lòng thêm hình ảnh!");
                return;
            }
            else
            {
                try
                {
                    File.Copy(image, filename, true);
                }
                catch
                {

                }
            }
            image = null;
            Employee employee = new Employee(int.Parse(parameter.txtIDEmployee.Text), parameter.txtName.Text, gender, parameter.txtTelephoneNumber.Text, parameter.txtAddress.Text, parameter.dpBirthDate.DisplayDate, 0, parameter.cboPosition.Text, parameter.dpWorkDate.DisplayDate, 0,filename);
            EmployeeDAL.Instance.AddEmployee(employee);
            parameter.Close();

        }
       
        public void DeleteEmployee(string id)
        {
            List<Employee> employees = EmployeeDAL.Instance.ConvertDBToList();
            foreach (var employee in employees)
            {
                if (employee.IdEmployee.ToString() == id)
                {
                    EmployeeDAL.Instance.DeleteEmployee(employee);
                    break;
                }
            }
        }
        public void OpenUpdateWindow(TextBlock parameter)
        {
            List<Employee> employees = EmployeeDAL.Instance.ConvertDBToList();
            fAddEmployee child = new fAddEmployee();
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

                    if (employee.Gender == "Nam")
                        child.rdoMale.IsChecked = true;
                    else
                        child.rdoFemale.IsChecked = true;
                    child.dpBirthDate.Text = employee.DateOfBirth.ToString();
                    child.dpWorkDate.Text = employee.Startingdate.ToString();
                    
                   
                        ImageBrush imageBrush = new ImageBrush();
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.UriSource = new Uri(employee.Image,UriKind.Relative);
                        bitmap.EndInit();
                        imageBrush.ImageSource = bitmap;
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
            child.ShowDialog();
        }
    }
}

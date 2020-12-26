using FootballFieldManagement.DAL;
using FootballFieldManagement.Models;
using FootballFieldManagement.Resources.UserControls;
using FootballFieldManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FootballFieldManagement.ViewModels
{
    class CheckAttendanceViewModel : BaseViewModel
    {
        private ObservableCollection<Employee> itemSourceEmployee = new ObservableCollection<Employee>();
        public ObservableCollection<Employee> ItemSourceEmployee { get => itemSourceEmployee; set { itemSourceEmployee = value; OnPropertyChanged(); } }
        private Employee selectedEmployee = new Employee();
        public Employee SelectedEmployee { get => selectedEmployee; set { selectedEmployee = value; OnPropertyChanged("SelectedEmployee"); } }
        public ICommand LoadCommand { get; set; } // Load lịch tháng hiện tại
        public ICommand ExitCommand { get; set; } // Click button "Thoát"
        public ICommand CheckAttendanceCommand { get; set; } // Click button "Điểm danh"
        public ICommand SelectionChangedCommand { get; set; } // Thay dổi lựa chọn của combobox

        public CheckAttendanceViewModel()
        {
            LoadCommand = new RelayCommand<CheckAttendanceWindow>(parameter => true, parameter => HandelLoadEvent(parameter));
            ExitCommand = new RelayCommand<Window>(parameter => true, parameter => parameter.Close());
            CheckAttendanceCommand = new RelayCommand<CheckAttendanceWindow>(parameter => true, parameter => CheckIn(parameter));
            SelectionChangedCommand = new RelayCommand<CheckAttendanceWindow>(parameter => true, parameter => ShowTableCheckAttendance(parameter));
        }
        public void ShowTableCheckAttendance(CheckAttendanceWindow parameter)
        {
            if (selectedEmployee == null)
            {
                return;
            }
            LoadDay(parameter);
            parameter.btnCheckIn.IsEnabled = true;
            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            List<DateControl> dateControls = new List<DateControl>();
            List<int> days = AttendanceDAL.Instance.WorkedDay(selectedEmployee.IdEmployee.ToString());
            for (int i = 0; i < parameter.wpMonthView.Children.Count; i++)
            {
                dateControls.Add((DateControl)parameter.wpMonthView.Children[i]);
            }
            for (int i = 1; i <= DateTime.Now.Day; i++)
            {
                DateControl dateControl = dateControls.Find(x => x.Name == "txbDate_" + i.ToString());
                if (days.Contains(i))
                {
                    if (i == DateTime.Now.Day)
                    {
                        parameter.btnCheckIn.IsEnabled = false;
                    }
                    dateControl.icCheck.Visibility = Visibility.Visible; // show icon check
                }
                else if (!days.Contains(i) && i < DateTime.Now.Day)
                {
                    dateControl.icClose.Visibility = Visibility.Visible; // show icon close
                }
            }
        }
        public void CheckIn(CheckAttendanceWindow parameter)
        {
            Attendance attendance = new Attendance(DateTime.Now.Day, DateTime.Now.Month, selectedEmployee.IdEmployee);
            if (AttendanceDAL.Instance.AddDay(attendance))
            {
                ShowTableCheckAttendance(parameter);
            }
        }
        public void HandelLoadEvent(CheckAttendanceWindow parameter)
        {
            SetItemSourcEmloyee();
            if (AttendanceDAL.Instance.GetMonth() != DateTime.Now.Month && AttendanceDAL.Instance.GetMonth() != 0)
            {
                if (!AttendanceDAL.Instance.DeleteData())
                {
                    CustomMessageBox.Show("Lỗi hệ thống!");
                    parameter.Close();
                }
            }
            if (parameter.cboSelectEmployee.SelectedIndex == -1)
            {
                parameter.btnCheckIn.IsEnabled = false;
            }
            parameter.txbMonth.Text = "Bảng chấm công tháng " + DateTime.Now.Month;
            LoadDay(parameter);
        }
        public int CheckDayOfWeek(DateTime dt)
        {
            string str = dt.DayOfWeek.ToString();
            switch (str)
            {
                case "Monday":
                    return 2;
                case "Tuesday":
                    return 3;
                case "Wednesday":
                    return 4;
                case "Thursday":
                    return 5;
                case "Friday":
                    return 6;
                case "Saturday":
                    return 7;
                case "Sunday":
                    return 8;
                default:
                    return 2;
            }
        }
        public void LoadDay(CheckAttendanceWindow parameter)
        {
            parameter.wpMonthView.Children.Clear();
            int dayOfWeek = CheckDayOfWeek(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int tmp = daysInMonth - 28 - 9 + dayOfWeek; // số ngày ở dòng cuối cùng
            int maxSquare = 35;

            //set độ dài, rộng của window
            if (tmp > 0)
            {
                parameter.wdCheckAttendance.Height = 800;
                parameter.wpMonthView.Height = 576;
                maxSquare = 42;
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                parameter.Left = (screenWidth / 2) - (parameter.Width / 2);
                parameter.Top = (screenHeight / 2) - (parameter.Height / 2);
            }
            else if (daysInMonth == 28)
            {
                parameter.wdCheckAttendance.Height = 600;
                parameter.wpMonthView.Height = 384;
                maxSquare = 28;
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                parameter.Left = (screenWidth / 2) - (parameter.Width / 2);
                parameter.Top = (screenHeight / 2) - (parameter.Height / 2);
            }
            else
            {
                tmp = 0;
            }
            //Những ngày trống ở đầu
            for (int i = 2; i < dayOfWeek; i++)
            {
                //Empty
                parameter.wpMonthView.Children.Add(new DateControl());
            }

            //Thêm ngày trong tháng
            for (int i = 1; i <= daysInMonth; i++)
            {
                if (i == DateTime.Now.Day)
                {
                    //Today
                    DateControl dateControl = new DateControl();
                    dateControl.txbDate.Text = i.ToString();
                    dateControl.Name = "txbDate_" + i.ToString();
                    dateControl.txbDate.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFFFFFFF");
                    dateControl.eli.Visibility = Visibility.Visible;
                    parameter.wpMonthView.Children.Add(dateControl);
                }
                else
                {
                    //Nomal day
                    DateControl dateControl = new DateControl();
                    dateControl.Name = "txbDate_" + i.ToString();
                    dateControl.txbDate.Text = i.ToString();
                    parameter.wpMonthView.Children.Add(dateControl);
                }
            }

            //Những ngày trống ở cuối
            for (int i = daysInMonth + dayOfWeek - tmp - 1; i <= maxSquare; i++)
            {
                parameter.wpMonthView.Children.Add(new DateControl());
            }
        }
        public void SetItemSourcEmloyee()
        {
            itemSourceEmployee.Clear();
            List<Employee> employees = EmployeeDAL.Instance.ConvertDBToList();
            foreach (var employee in employees)
            {
                itemSourceEmployee.Add(employee);
            }
        }
    }
}

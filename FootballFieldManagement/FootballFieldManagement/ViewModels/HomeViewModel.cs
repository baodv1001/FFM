using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using FootballFieldManagement.Views;
using FootballFieldManegement.DAL;
using FootballFieldManagement.Resources.User_Control;
using System.Windows.Media;
namespace FootballFieldManagement.ViewModels
{
    public class HomeViewModel
    {
        public ICommand LogOutCommand { get; set; }
        public ICommand E_LoadCommand { get; set; }
        public ICommand E_AddCommand { get; set; }
        public StackPanel Stack { get => stack; set => stack = value; }

        private StackPanel stack = new StackPanel();
        public HomeViewModel()
        {
            LogOutCommand = new RelayCommand<Window>((parameter) => true, (parameter) => parameter.Close());
            E_LoadCommand = new RelayCommand<StackPanel>((parameter) => true, (parameter) => AddUCEmployee(parameter));
            E_AddCommand = new RelayCommand<StackPanel>((parameter) => true, (parameter) => AddEmployee(parameter));
        }
        public void AddEmployee(StackPanel parameter)
        {
            stack = parameter;
            fAddEmployee addEmployee = new fAddEmployee(); 
            addEmployee.ShowDialog();
            parameter.Children.Clear();
            AddUCEmployee(parameter);
        }

        public void AddUCEmployee(StackPanel stackPanel)
        {
            int i = 1;
            stackPanel.Children.Clear();
            EmployeeDAL employeeDAL = new EmployeeDAL();
            bool flag = true;
            foreach (var employee in employeeDAL.Employees)
            {
                EmployeeControl temp = new EmployeeControl();
                flag = !flag;
                if (flag == true)
                {
                    temp.Background = (Brush)new BrushConverter().ConvertFromString("#e0e0e0");
                }
                else
                {
                    temp.Background = Brushes.White;
                }
                temp.txbSerial.Text = i.ToString();
                i++;
                temp.txbId.Text = employee.IdEmployee.ToString();
                temp.txbName.Text = employee.Name.ToString();
                temp.txbPosition.Text = employee.Position.ToString();
                stackPanel.Children.Add(temp);
            }
        }
    }
}

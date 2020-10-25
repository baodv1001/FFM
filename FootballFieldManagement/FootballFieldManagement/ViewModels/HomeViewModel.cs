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
            E_LoadCommand = new RelayCommand<StackPanel>((parameter) => true, (parameter) => LoadEmployeesToView(parameter));
            E_AddCommand = new RelayCommand<StackPanel>((parameter) => true, (parameter) => AddEmployee(parameter));
        }
        public void AddEmployee(StackPanel parameter)
        {
            stack = parameter;
            fAddEmployee addEmployee = new fAddEmployee();
            try
            {
                addEmployee.txtIDEmployee.Text = (EmployeeDAL.Instance.ConvertDBToList()[EmployeeDAL.Instance.ConvertDBToList().Count - 1].IdEmployee + 1).ToString();
            }
            catch
            {
                addEmployee.txtIDEmployee.Text   = "1";
            }
            addEmployee.ShowDialog();
        }

        public void LoadEmployeesToView(StackPanel stackPanel)
        {
            int i = 1;
            stackPanel.Children.Clear();
            bool flag = false;
            foreach (var employee in EmployeeDAL.Instance.ConvertDBToList())
            {
                EmployeeControl temp = new EmployeeControl();
                flag = !flag;
                if (flag)
                {
                    temp.grdEmployee.Background = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");
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

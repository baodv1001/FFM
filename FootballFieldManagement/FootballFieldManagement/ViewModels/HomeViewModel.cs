using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using FootballFieldManagement.Views;
using FootballFieldManegement.DAL;
using FootballFieldManagement.Resources.UserControls;
using System.Windows.Media;
using FootballFieldManagement.Models;
using FootballFieldManagement.DAL;
using System.Linq;

namespace FootballFieldManagement.ViewModels
{
    public class HomeViewModel
    {
        public ICommand LogOutCommand { get; set; }
        public ICommand SwitchTabCommand { get; set; }

        public ICommand E_LoadCommand { get; set; }
        public ICommand E_AddCommand { get; set; }

        public ICommand G_AddCommand { get; set; }
        public ICommand G_LoadCommand { get; set; }
        public ICommand GetUidCommand { get; set; }
        public ICommand E_SetSalaryCommand { get; set; }
        public StackPanel Stack { get => stack; set => stack = value; }

        private StackPanel stack = new StackPanel();
        private string uid;
        public HomeViewModel()
        {
            LogOutCommand = new RelayCommand<Window>((parameter) => true, (parameter) => parameter.Close());
            SwitchTabCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => SwitchTab(parameter));

            E_LoadCommand = new RelayCommand<StackPanel>((parameter) => true, (parameter) => LoadEmployeesToView(parameter));
            E_AddCommand = new RelayCommand<StackPanel>((parameter) => true, (parameter) => AddEmployee(parameter));

            GetUidCommand = new RelayCommand<Button>((parameter) => true, (parameter) => uid = parameter.Uid);
            G_AddCommand = new RelayCommand<StackPanel>((parameter) => true, (parameter) => AddGoods(parameter));
            G_LoadCommand = new RelayCommand<StackPanel>((parameter) => true, (parameter) => LoadGoodsToView(parameter));
            E_SetSalaryCommand = new RelayCommand<Window>((parameter) => true, (parameter) => OpenSetSalaryWindow());
        }

        public void OpenSetSalaryWindow()
        {
            SetSalaryWindow setSalaryWindow = new SetSalaryWindow();
            setSalaryWindow.ShowDialog();
        }
        public void SwitchTab(HomeWindow parameter)
        {
            int index = int.Parse(uid);

            parameter.grdCursor.Margin = new Thickness(0, (175 + 70 * index), 40, 0);

            parameter.grdBody_Goods.Visibility = Visibility.Hidden;
            parameter.grdBody_Home.Visibility = Visibility.Hidden;
            parameter.grdBody_Employee.Visibility = Visibility.Hidden;

            parameter.btnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnGoods.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnEmployee.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnReport.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.btnSetting.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");

            parameter.icnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnGoods.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnEmployee.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnReport.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");
            parameter.icnSetting.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF282828");

            switch (index)
            {
                case 0:
                    parameter.grdBody_Home.Visibility = Visibility.Visible;
                    parameter.btnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnHome.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 1:
                    parameter.btnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnField.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 2:
                    parameter.grdBody_Goods.Visibility = Visibility.Visible;
                    parameter.btnGoods.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnGoods.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 3:
                    parameter.grdBody_Employee.Visibility = Visibility.Visible;
                    parameter.btnEmployee.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnEmployee.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 4:
                    parameter.btnReport.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnReport.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                case 5:
                    parameter.btnSetting.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    parameter.icnSetting.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    break;
                default:
                    break;
            }
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
                addEmployee.txtIDEmployee.Text = "1";
            }
            addEmployee.txbConfirm.Text = "Thêm";
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
                if(!CurrentAccount.Type)
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

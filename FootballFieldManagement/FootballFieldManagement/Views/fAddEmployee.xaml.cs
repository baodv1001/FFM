using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FootballFieldManegement.DAL;
using FootballFieldManagement.Models;
using System.Text.RegularExpressions;

namespace FootballFieldManagement.Views
{
    /// <summary>
    /// Interaction logic for fAddEmployee.xaml
    /// </summary>
    public partial class fAddEmployee : Window
    {

        public fAddEmployee()
        {
            InitializeComponent();
            EmployeeDAL employeeDAL = new EmployeeDAL();
            try
            {
                this.txtIDEmployee.Text = (employeeDAL.Employees[employeeDAL.Employees.Count - 1].IdEmployee + 1).ToString();
            }
            catch
            {
                this.txtIDEmployee.Text = "1";
            }

        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}

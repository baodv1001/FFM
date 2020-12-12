using FootballFieldManagement.Resources.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FootballFieldManagement.ViewModels
{
    class PrintViewModel
    {
        public ICommand PrintCommand { get; set; }

        public PrintViewModel()
        {
            PrintCommand = new RelayCommand<BillTemplate>((parameter) => true, (parameter) => Print(parameter));
        }

        public void Print(BillTemplate parameter)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    parameter.btnPrint.Visibility = Visibility.Hidden;
                    printDialog.PrintVisual(parameter.grdPrint, "invoice");
                }
            }
            finally
            {
                parameter.btnPrint.Visibility = Visibility.Visible;
            }
        }
    }
}

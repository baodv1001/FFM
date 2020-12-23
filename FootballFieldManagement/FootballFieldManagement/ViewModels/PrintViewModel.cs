using FootballFieldManagement.Resources.Template;
using FootballFieldManagement.Views;
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
        public ICommand PrintBillCommand { get; set; }
        public ICommand PrintStockReceiptCommand { get; set; }
        public ICommand PrintSalaryRecordCommand { get; set; }

        public PrintViewModel()
        {
            PrintBillCommand = new RelayCommand<BillTemplate>((parameter) => true, (parameter) => PrintBill(parameter));
            PrintStockReceiptCommand = new RelayCommand<StockReceiptTemplate>((parameter) => true, (parameter) => PrintStockReceipt(parameter));
            PrintSalaryRecordCommand = new RelayCommand<SalaryRecordTemplate>((parameter) => true, (parameter) => PrintSalaryRecord(parameter));
        }
        public void PrintStockReceipt(StockReceiptTemplate parameter)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    parameter.btnPrint.Visibility = Visibility.Hidden;
                    printDialog.PrintVisual(parameter.grdPrint, "Stock receipt");
                }
            }
            finally
            {
                parameter.btnPrint.Visibility = Visibility.Visible;
            }
        }
        public void PrintSalaryRecord(SalaryRecordTemplate parameter)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    parameter.btnPrint.Visibility = Visibility.Hidden;
                    printDialog.PrintVisual(parameter.grdPrint, "Salary Record");
                }
            }
            finally
            {
                parameter.btnPrint.Visibility = Visibility.Visible;
            }
        }
        public void PrintBill(BillTemplate parameter)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    parameter.btnPrint.Visibility = Visibility.Hidden;
                    printDialog.PrintVisual(parameter.grdPrint, "Bill");
                }
            }
            finally
            {
                parameter.btnPrint.Visibility = Visibility.Visible;
            }
        }
    }
}

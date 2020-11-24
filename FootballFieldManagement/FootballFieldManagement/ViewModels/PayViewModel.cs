using FootballFieldManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using FootballFieldManagement.DAL;
using FootballFieldManagement.Resources.UserControls;
using FootballFieldManagement.Models;
using System.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;

namespace FootballFieldManagement.ViewModels
{
    public class PayViewModel : INotifyPropertyChanged
    {
        //Binding 
        private decimal total;
        private decimal totalGoods;
        public decimal Total
        {
            get => total;
            set
            {
                total = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
            }
        }
        public decimal TotalGoods
        {
            get => totalGoods;
            set
            {
                totalGoods = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalGoods)));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        //Pay Window
        public ICommand LoadGoodsCommand { get; set; }
        public ICommand LoadBillInfoCommand { get; set; }
        public ICommand LoadTotalCommand { get; set; }
        public ICommand ClosingWdCommnad { get; set; }
        //UC Product Detail
        public ICommand DeleteBillInfoCommand { get; set; }
        public ICommand ChangeQuantityCommand { get; set; }


        public PayViewModel()
        {
            //Pay Window
            LoadGoodsCommand = new RelayCommand<PayWindow>((parameter) => true, (parameter) => LoadGoodsToView(parameter));
            LoadBillInfoCommand = new RelayCommand<PayWindow>((parameter) => true, (parameter) => LoadBillInfoToView(parameter));
            LoadTotalCommand = new RelayCommand<TextBlock>((parameter) => true, (parameter) => LoadTotalMoney(parameter));
            ClosingWdCommnad = new RelayCommand<PayWindow>((parameter) => true, (parameter) => DeleteBill(parameter));
            //UC Product Detail
            DeleteBillInfoCommand = new RelayCommand<ProductDetailsControl>((parameter) => true, (parameter) => DeleteBillInfo(parameter));
            ChangeQuantityCommand = new RelayCommand<ProductDetailsControl>((parameter) => true, (parameter) => UpdateQuantity(parameter));

            Total = TotalGoods = 0;
        }
        //Binding
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //PayWindow
        public void LoadGoodsToView(PayWindow parameter)
        {
            parameter.wrpGoods.Children.Clear();
            DataTable goods = GoodsDAL.Instance.LoadData("Goods");
            for (int i = 0; i < goods.Rows.Count; i++)
            {
                string name = goods.Rows[i].ItemArray[1].ToString();
                if (name.ToLower().Contains(parameter.txtSearch.Text.ToLower()))
                {
                    SellGoodsControl good = new SellGoodsControl();
                    good.txbName.Text = goods.Rows[i].ItemArray[1].ToString();
                    good.txbId.Text = goods.Rows[i].ItemArray[0].ToString();
                    byte[] blob = (byte[])goods.Rows[i].ItemArray[5];
                    BitmapImage bi = Converter.Instance.ConvertByteToBitmapImage(blob);
                    good.imgGood.Source = bi;
                    good.txbPrice.Text = goods.Rows[i].ItemArray[3].ToString();
                    good.txbIdBill.Text = parameter.txbIdBill.Text;
                    //try
                    //{
                    //    good.txbIdBill.Text = (BillDAL.Instance.ConvertDBToList()[BillDAL.Instance.ConvertDBToList().Count - 1].IdBill + 1).ToString();
                    //}
                    //catch
                    //{
                    //    good.txbIdBill.Text = "1";
                    //}
                    parameter.wrpGoods.Children.Add(good);
                }
            }
        }
        public void LoadBillInfoToView(PayWindow parameter)
        {
            TotalGoods = 0;
            parameter.stkPickedGoods.Children.Clear();
            DataTable billInfos = BillInfoDAL.Instance.LoadData("BillInfo");
            ProductDetailsControl infoControl = new ProductDetailsControl();
            for (int i = 0; i < billInfos.Rows.Count; i++)
            {

                infoControl = new ProductDetailsControl();
                infoControl.txbNo.Text = (i + 1).ToString();
                infoControl.txbIdGoods.Text = billInfos.Rows[i].ItemArray[1].ToString();
                infoControl.txbIdBill.Text = billInfos.Rows[i].ItemArray[0].ToString();
                infoControl.txbName.Text = GoodsDAL.Instance.GetGood(billInfos.Rows[i].ItemArray[1].ToString()).Name;
                infoControl.txbPrice.Text = GoodsDAL.Instance.GetGood(billInfos.Rows[i].ItemArray[1].ToString()).UnitPrice.ToString();
                infoControl.nmsQuantity.Text = decimal.Parse(billInfos.Rows[i].ItemArray[2].ToString());
                infoControl.nmsQuantity.MinValue = 1;
                infoControl.nmsQuantity.MaxValue = GoodsDAL.Instance.GetGood(infoControl.txbIdGoods.Text).Quantity;
                infoControl.txbtotal.Text = (infoControl.nmsQuantity.Value * int.Parse(infoControl.txbPrice.Text)).ToString();

                parameter.stkPickedGoods.Children.Add(infoControl);
            }
            TotalGoods = BillInfoDAL.Instance.CountSumMoney();
            Total = TotalGoods + int.Parse(parameter.txbFieldPrice.Text) - int.Parse(parameter.txbDiscount.Text);
        }
        public void LoadTotalMoney(TextBlock parameter)
        {
            parameter.Text = BillInfoDAL.Instance.CountSumMoney().ToString();
        }
        public void DeleteBill(PayWindow parameter)
        {
            BillInfoDAL.Instance.DeleteAllBillInfo("1");
        }
        //UC Product Detail
        public void UpdateQuantity(ProductDetailsControl parameter)
        {
            BillInfo billInfo = new BillInfo(int.Parse(parameter.txbIdBill.Text), int.Parse(parameter.txbIdGoods.Text), int.Parse(parameter.nmsQuantity.Value.ToString()));
            BillInfoDAL.Instance.UpdateOnDB(billInfo);
            parameter.txbtotal.Text = (parameter.nmsQuantity.Value * int.Parse(parameter.txbPrice.Text)).ToString();
            Total -= TotalGoods;
            TotalGoods = BillInfoDAL.Instance.CountSumMoney();
            Total += TotalGoods;
        }
        public void DeleteBillInfo(ProductDetailsControl paramter)
        {
            BillInfo billInfo = new BillInfo(int.Parse(paramter.txbIdBill.Text), int.Parse(paramter.txbIdGoods.Text), 1);
            BillInfoDAL.Instance.DeleteFromDB(billInfo);
            Total -= TotalGoods;
            TotalGoods = BillInfoDAL.Instance.CountSumMoney();
            Total += TotalGoods;
        }
    }
}

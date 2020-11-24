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
using System.Diagnostics;

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
        public ICommand PayBillCommand { get; set; }
        //UC Product Detail
        public ICommand DeleteBillInfoCommand { get; set; }
        public ICommand ChangeQuantityCommand { get; set; }


        public PayViewModel()
        {
            //Pay Window
            LoadGoodsCommand = new RelayCommand<PayWindow>((parameter) => true, (parameter) => LoadGoodsToView(parameter));
            LoadBillInfoCommand = new RelayCommand<PayWindow>((parameter) => true, (parameter) => LoadBillInfoToView(parameter)); // Hiển thị các mặt hàng được chọn
            LoadTotalCommand = new RelayCommand<PayWindow>((parameter) => true, (parameter) => LoadTotalMoney(parameter)); 
            ClosingWdCommnad = new RelayCommand<PayWindow>((parameter) => true, (parameter) => DeleteBillInfos(parameter));
            PayBillCommand = new RelayCommand<PayWindow>((parameter) => true, (parameter) => PayBill(parameter));
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
                    good.imgGood.Source = Converter.Instance.ConvertByteToBitmapImage((byte[])goods.Rows[i].ItemArray[5]);
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
        public void LoadTotalMoney(PayWindow parameter)
        {
            parameter.txbSumOfPrice.Text = (int.Parse(parameter.txbFieldPrice.Text) - int.Parse(parameter.txbDiscount.Text)).ToString();
        }
        public void DeleteBillInfos(PayWindow parameter)
        {
            if (!new StackTrace().GetFrames().Any(x => x.GetMethod().Name == "Close"))
                BillInfoDAL.Instance.DeleteAllBillInfo(parameter.txbIdBill.Text);
        }
        public void PayBill(PayWindow parameter)
        {
            List<BillInfo> billInfos = BillInfoDAL.Instance.GetBillInfos(parameter.txbIdBill.Text);
            string note = @"";
            foreach (var billInfo in billInfos)
            {
                var good = GoodsDAL.Instance.GetGood(billInfo.IdGoods.ToString());
                note += good.Name + " x " + billInfo.Quantity.ToString() + "\t" + (good.UnitPrice * billInfo.Quantity).ToString() + Environment.NewLine;
            }
            Bill bill = new Bill(int.Parse(parameter.txbIdBill.Text), CurrentAccount.IdAccount, DateTime.Now, DateTime.Now, DateTime.Now, 1, double.Parse(parameter.txbDiscount.Text), double.Parse(parameter.txbSumOfPrice.Text), int.Parse(parameter.txbIdFieldInfo.Text), note);
            if (BillDAL.Instance.UpdateOnDB(bill))
            {
                foreach (var billInfo in billInfos)
                {
                    var good = GoodsDAL.Instance.GetGood(billInfo.IdGoods.ToString());
                    good.Quantity -= billInfo.Quantity;
                    GoodsDAL.Instance.UpdateOnDB(good);
                }
                MessageBox.Show("Thanh toán thành công!");
                parameter.Close();
            }

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

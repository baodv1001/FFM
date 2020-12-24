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
using FootballFieldManagement.Resources.Template;
using System.Data.SqlClient;

namespace FootballFieldManagement.ViewModels
{
    public class PayViewModel : BaseViewModel, INotifyPropertyChanged
    {
        //Binding 
        private PayWindow payWindow;
        private string total;
        private string totalGoods;
        public string Total
        {
            get => total;
            set
            {
                total = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
                OnPropertyChanged();
            }
        }
        public string TotalGoods
        {
            get => totalGoods;
            set
            {
                totalGoods = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalGoods)));
                OnPropertyChanged();
            }
        }
        public PayWindow PayWindow { get => payWindow; set => payWindow = value; }
        public event PropertyChangedEventHandler PropertyChanged;
        //Pay Window
        public ICommand LoadGoodsCommand { get; set; }
        public ICommand LoadBillInfoCommand { get; set; }
        public ICommand LoadTotalCommand { get; set; }
        public ICommand ClosingWdCommnad { get; set; }
        public ICommand PayBillCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand PickGoodsCommand { get; set; } // Chọn 1 hàng 
        public ICommand GetWindowCommand { get; set; }
        //UC Product Detail
        public ICommand DeleteBillInfoCommand { get; set; }
        public ICommand ChangeQuantityCommand { get; set; }

        public ICommand ViewBillCommand { get; set; }

        public PayViewModel()
        {
            //Pay Window
            LoadGoodsCommand = new RelayCommand<PayWindow>((parameter) => true, (parameter) => LoadGoodsToView(parameter));
            LoadBillInfoCommand = new RelayCommand<PayWindow>((parameter) => true, (parameter) => LoadBillInfoToView(parameter)); // Hiển thị các mặt hàng được chọn
            LoadTotalCommand = new RelayCommand<PayWindow>((parameter) => true, (parameter) => LoadTotalMoney(parameter));
            ClosingWdCommnad = new RelayCommand<PayWindow>((parameter) => true, (parameter) => DeleteBillInfos(parameter));
            PayBillCommand = new RelayCommand<PayWindow>((parameter) => true, (parameter) => PayBill(parameter));
            BackCommand = new RelayCommand<PayWindow>((parameter) => true, (parameter) => CloseWindow(parameter));
            PickGoodsCommand = new RelayCommand<SellGoodsControl>((parameter) => true, (parameter) => BuyGoods(parameter));
            GetWindowCommand = new RelayCommand<PayWindow>((parameter) => true, (parameter) => SetWindow(parameter));
            //UC Product Detail
            DeleteBillInfoCommand = new RelayCommand<ProductDetailsControl>((parameter) => true, (parameter) => DeleteBillInfo(parameter));
            ChangeQuantityCommand = new RelayCommand<ProductDetailsControl>((parameter) => true, (parameter) => UpdateQuantity(parameter));

            ViewBillCommand = new RelayCommand<PayWindow>((parameter) => true, (parameter) => ViewBill(parameter));

            Total = TotalGoods = "0";
        }
        public void ViewBill(PayWindow payWindow)
        {
            Bill bill = BillDAL.Instance.GetBill(payWindow.txbIdBill.Text);
            //thông tin Bill + FieldInfo
            BillTemplate billTemplate = new BillTemplate();
            billTemplate.txbDiscount.Text = payWindow.txbDiscount.Text;
            billTemplate.txbTotal.Text = payWindow.txbSumOfPrice.Text;
            billTemplate.txbIdBill.Text = payWindow.txbIdBill.Text;
            billTemplate.txbTotalBefore.Text = (int.Parse(payWindow.txbFieldPrice.Text) + int.Parse(payWindow.txbtotalGoodsPrice.Text)).ToString();
            billTemplate.txbCustomerName.Text = payWindow.txbCustomerName.Text;
            billTemplate.txbCustomerPhoneNumber.Text = payWindow.txbCustomerPhone.Text;
            billTemplate.txbInvoiceDate.Text = bill.InvoiceDate.ToShortDateString();
            billTemplate.txbCheckInTime.Text = bill.CheckInTime.ToString("HH:mm");
            billTemplate.txbCheckOutTime.Text = DateTime.Now.ToString("HH:mm");
            billTemplate.txbEmployeeName.Text = EmployeeDAL.Instance.GetEmployeeByIdAccount(CurrentAccount.IdAccount.ToString()).Name;

            List<BillInfo> billInfos = BillInfoDAL.Instance.GetBillInfos(payWindow.txbIdBill.Text);
            int numOfGoods = billInfos.Count();
            if (numOfGoods > 7)
            {
                billTemplate.Height += (numOfGoods - 7) * 35;
            }
            int i = 1;
            BillInfoControl fieldBillInfoControl = new BillInfoControl();
            //Thêm sân vào nha
            fieldBillInfoControl.txbOrderNum.Text = i.ToString();
            i++;
            string idFieldInfo = payWindow.txbIdFieldInfo.Text;
            FieldInfo fieldInfo = FieldInfoDAL.Instance.GetFieldInfo(idFieldInfo);
            string note = fieldInfo.StartingTime.ToString("HH:mm") + " - " + fieldInfo.EndingTime.ToString("HH:mm");
            fieldBillInfoControl.txbName.Text = string.Format("{0} ({1})", payWindow.txbFieldName.Text, note);
            fieldBillInfoControl.txbUnit.Text = "";
            fieldBillInfoControl.txbQuantity.Text = "";
            fieldBillInfoControl.txbUnitPrice.Text = payWindow.txbFieldPrice.Text;
            fieldBillInfoControl.txbTotal.Text = payWindow.txbFieldPrice.Text;
            billTemplate.stkBillInfo.Children.Add(fieldBillInfoControl);
            //Thông tin bill info
            foreach (var billInfo in billInfos)
            {
                BillInfoControl billInfoControl = new BillInfoControl();
                Goods goods = GoodsDAL.Instance.GetGoods(billInfo.IdGoods.ToString());
                billInfoControl.txbOrderNum.Text = i.ToString();
                billInfoControl.txbName.Text = goods.Name;
                billInfoControl.txbUnitPrice.Text = goods.UnitPrice.ToString();
                billInfoControl.txbQuantity.Text = billInfo.Quantity.ToString();
                billInfoControl.txbUnit.Text = goods.Unit;
                billInfoControl.txbTotal.Text = (goods.UnitPrice * billInfo.Quantity).ToString();

                billTemplate.stkBillInfo.Children.Add(billInfoControl);
                i++;
            }

            //Thông tin sân
            SQLConnection connection = new SQLConnection();
            try
            {
                connection.conn.Open();
                string queryString = "select * from Information";
                SqlCommand command = new SqlCommand(queryString, connection.conn);
                command.ExecuteNonQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                billTemplate.txbFieldName.Text = dataTable.Rows[0].ItemArray[0].ToString();
                billTemplate.txbFieldNameBrand.Text = dataTable.Rows[0].ItemArray[0].ToString();
                billTemplate.txbPhoneNumber.Text = dataTable.Rows[0].ItemArray[1].ToString();
                billTemplate.txbAddress.Text = dataTable.Rows[0].ItemArray[2].ToString();
            }
            catch
            {

            }
            finally
            {
                connection.conn.Close();
            }

            billTemplate.ShowDialog();
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
            DataTable goods = GoodsDAL.Instance.LoadDatatable();
            for (int i = 0; i < goods.Rows.Count; i++)
            {
                string name = goods.Rows[i].ItemArray[1].ToString();
                if (name.ToLower().Contains(parameter.txtSearch.Text.ToLower()))
                {
                    SellGoodsControl good = new SellGoodsControl();
                    good.txbName.Text = goods.Rows[i].ItemArray[1].ToString();
                    good.txbId.Text = goods.Rows[i].ItemArray[0].ToString();
                    good.imgGood.Source = Converter.Instance.ConvertByteToBitmapImage(Convert.FromBase64String(goods.Rows[i].ItemArray[4].ToString()));
                    good.txbPrice.Text = string.Format("{0:N0}", long.Parse(goods.Rows[i].ItemArray[3].ToString()));
                    good.txbIdBill.Text = parameter.txbIdBill.Text;
                    parameter.wrpGoods.Children.Add(good);
                }
            }
        }
        public void LoadBillInfoToView(PayWindow parameter)
        {
            TotalGoods = "0";
            parameter.stkPickedGoods.Children.Clear();
            List<BillInfo> billInfos = BillInfoDAL.Instance.GetBillInfos(parameter.txbIdBill.Text);
            for (int i = 0; i < billInfos.Count; i++)
            {
                ProductDetailsControl infoControl = new ProductDetailsControl();
                infoControl.txbNo.Text = (i + 1).ToString();
                infoControl.txbIdGoods.Text = billInfos[i].IdGoods.ToString();
                infoControl.txbIdBill.Text = billInfos[i].IdBill.ToString();
                Goods goods = GoodsDAL.Instance.GetGoods(billInfos[i].IdGoods.ToString());
                infoControl.txbName.Text = goods.Name;
                infoControl.txbPrice.Text = string.Format("{0:N0}", goods.UnitPrice);
                infoControl.nmsQuantity.Text = decimal.Parse(billInfos[i].Quantity.ToString());
                infoControl.nmsQuantity.MinValue = 1;
                infoControl.nmsQuantity.MaxValue = goods.Quantity;
                infoControl.txbtotal.Text = string.Format("{0:N0}", (infoControl.nmsQuantity.Value * ConvertToNumber(infoControl.txbPrice.Text)));
                parameter.stkPickedGoods.Children.Add(infoControl);
            }
            TotalGoods = string.Format("{0:N0}", BillInfoDAL.Instance.CountSumMoney(parameter.txbIdBill.Text));
            Total = string.Format("{0:N0}", ConvertToNumber(TotalGoods) + ConvertToNumber(parameter.txbFieldPrice.Text) - ConvertToNumber(parameter.txbDiscount.Text));
        }
        public void LoadTotalMoney(PayWindow parameter)
        {
            parameter.txbSumOfPrice.Text = string.Format("{0:N0}", (ConvertToNumber(parameter.txbFieldPrice.Text) - ConvertToNumber(parameter.txbDiscount.Text)));
        }
        public void DeleteBillInfos(PayWindow parameter)
        {
            if (!new StackTrace().GetFrames().Any(x => x.GetMethod().Name == "Close"))
            {
                BillInfoDAL.Instance.DeleteAllBillInfo(parameter.txbIdBill.Text);
                BillDAL.Instance.DeleteFromDB(parameter.txbIdBill.Text);
                totalGoods = "0";
            }
        }
        public void PayBill(PayWindow parameter)
        {
            List<BillInfo> billInfos = BillInfoDAL.Instance.GetBillInfos(parameter.txbIdBill.Text);
            string note = @"";
            foreach (var billInfo in billInfos)
            {
                var good = GoodsDAL.Instance.GetGoods(billInfo.IdGoods.ToString());
                note += good.Name + " x " + billInfo.Quantity.ToString() + "\t" + (good.UnitPrice * billInfo.Quantity).ToString() + Environment.NewLine;
            }
            Bill bill = new Bill(int.Parse(parameter.txbIdBill.Text), CurrentAccount.IdAccount, DateTime.Now, DateTime.Now, DateTime.Now, 1, long.Parse(parameter.txbSumOfPrice.Text), int.Parse(parameter.txbIdFieldInfo.Text), note);
            if (BillDAL.Instance.UpdateOnDB(bill))
            {
                FieldInfo fieldInfo = FieldInfoDAL.Instance.GetFieldInfo(parameter.txbIdFieldInfo.Text);
                fieldInfo.Status = 3;
                if (FieldInfoDAL.Instance.UpdateOnDB(fieldInfo))
                {
                    foreach (var billInfo in billInfos)
                    {
                        var good = GoodsDAL.Instance.GetGoods(billInfo.IdGoods.ToString());
                        good.Quantity -= billInfo.Quantity;
                        GoodsDAL.Instance.UpdateOnDB(good);
                    }

                    MessageBox.Show("Thanh toán thành công!");
                    parameter.txbIsPaid.Text = "1";
                }
                parameter.Close();
            }
            else
            {
                totalGoods = "0";
                parameter.txbIsPaid.Text = "0";
            }

        }
        public void CloseWindow(PayWindow parameter)
        {
            BillInfoDAL.Instance.DeleteAllBillInfo(parameter.txbIdBill.Text);
            BillDAL.Instance.DeleteFromDB(parameter.txbIdBill.Text);
            totalGoods = "0";
            parameter.Close();
        }
        public void BuyGoods(SellGoodsControl parameter)
        {
            bool isExist = false;
            if (GoodsDAL.Instance.GetGoods(parameter.txbId.Text).Quantity == 0)
            {
                MessageBox.Show("Đã hết hàng!");
                return;
            }
            List<BillInfo> billInfos = BillInfoDAL.Instance.GetBillInfos(parameter.txbIdBill.Text);
            foreach (var billInfo in billInfos)
            {
                if (billInfo.IdGoods == int.Parse(parameter.txbId.Text))
                {
                    isExist = true;
                    billInfo.Quantity += 1;
                    if (billInfo.Quantity > GoodsDAL.Instance.GetGoods(billInfo.IdGoods.ToString()).Quantity)
                    {
                        billInfo.Quantity -= 1;
                        MessageBox.Show("Đạt số lượng hàng tối đa!");
                        return;
                    }
                    if (BillInfoDAL.Instance.UpdateOnDB(billInfo))
                    {
                        LoadBillInfoToView(payWindow);
                    }
                    return;
                }
            }
            if (!isExist)
            {
                BillInfo billInfo = new BillInfo(int.Parse(parameter.txbIdBill.Text), int.Parse(parameter.txbId.Text), 1);
                BillInfoDAL.Instance.AddIntoDB(billInfo);
                LoadBillInfoToView(payWindow);
            }
        }
        public void SetWindow(PayWindow parameter)
        {
            payWindow = parameter;
        }
        //UC Product Detail
        public void UpdateQuantity(ProductDetailsControl parameter)
        {
            BillInfo billInfo = new BillInfo(int.Parse(parameter.txbIdBill.Text), int.Parse(parameter.txbIdGoods.Text), int.Parse(parameter.nmsQuantity.Value.ToString()));
            BillInfoDAL.Instance.UpdateOnDB(billInfo);
            parameter.txbtotal.Text = string.Format("{0:N0}", (parameter.nmsQuantity.Value * ConvertToNumber(parameter.txbPrice.Text)));
            Total = string.Format("{0:N0}", ConvertToNumber(Total) - ConvertToNumber(TotalGoods));
            TotalGoods = string.Format("{0:N0}", BillInfoDAL.Instance.CountSumMoney(parameter.txbIdBill.Text));
            Total = string.Format("{0:N0}", ConvertToNumber(Total) + ConvertToNumber(TotalGoods));
        }
        public void DeleteBillInfo(ProductDetailsControl paramter)
        {
            BillInfo billInfo = new BillInfo(int.Parse(paramter.txbIdBill.Text), int.Parse(paramter.txbIdGoods.Text), 1);
            BillInfoDAL.Instance.DeleteFromDB(billInfo);
            ((StackPanel)paramter.Parent).Children.Remove(paramter);
            Total = string.Format("{0:N0}", ConvertToNumber(Total) - ConvertToNumber(TotalGoods));
            TotalGoods = string.Format("{0:N0}", BillInfoDAL.Instance.CountSumMoney(paramter.txbIdBill.Text));
            Total = string.Format("{0:N0}", ConvertToNumber(Total) + ConvertToNumber(TotalGoods));
        }
    }
}

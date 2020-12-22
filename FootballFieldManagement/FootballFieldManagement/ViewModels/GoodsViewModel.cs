using FootballFieldManagement.DAL;
using FootballFieldManagement.Models;
using FootballFieldManagement.Views;
using FootballFieldManagement.Resources.UserControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data.SqlClient;
using FootballFieldManagement.Resources.Template;
using System.Diagnostics;

namespace FootballFieldManagement.ViewModels
{
    class GoodsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string imageFileName;
        private string quantity;
        public string Quantity { get => quantity; set => quantity = value; }
        private string importPrice;
        public string ImportPrice { get => importPrice; set => importPrice = value; }
        public int total;
        public int Total
        {
            get => total;
            set
            {
                total = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
            }
        }

        private ImportStockWindow importStockWindow;
        public ImportStockWindow ImportStockWindow { get => importStockWindow; set => importStockWindow = value; }

        private HomeWindow homeWindow;
        public HomeWindow HomeWindow { get => homeWindow; set => homeWindow = value; }
        
        public ICommand LoadStkGoodsCommand { get; set; } //show AddGoodsWindow -> edit

        //GoodsControl
        public ICommand EditGoodsCommand { get; set; } //show AddGoodsWindow -> edit
        public ICommand ImportGoodsCommand { get; set; } //show ImportGoodsWindow
        public ICommand DeleteGoodsCommand { get; set; } //xóa mặt hàng

        //AddGoodsWindow
        public ICommand AddGoodsCommand { get; set; } //thêm mặt hàng
        public ICommand SelectImageCommand { get; set; } //chọn ảnh
        public ICommand SaveCommand { get; set; } //lưu thông tin mặt hàng
        public ICommand ExitCommand { get; set; } //thoát khỏi AddGoodsWindow

        //ImportGoodsWindow
        public ICommand ImportCommand { get; set; } //nhập hàng
        public ICommand ExitImportCommand { get; set; } //thoát khỏi ImportGoodsWindow
        public ICommand CalculateTotalCommand { get; set; } //tính tổng tiền

        //ImportStockWindow
        public ICommand OpenImportStockWindowCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand ExitImportStockCommand { get; set; }
        public ICommand GetWindowCommand { get; set; }
        public ICommand LoadGoodsCommand { get; set; }
        public ICommand PickGoodsCommand { get; set; }
        public ICommand DeleteImportGoodsDetailsCommand { get; set; }
        public ICommand StockReceiptInfoChangeCommand { get; set; }
        public ICommand ImportStockCommand { get; set; }
        public ICommand ViewStockReceiptTemplateCommand { get; set; }

        public GoodsViewModel()
        {
            LoadStkGoodsCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => LoadStkGoods(parameter));
            
            //GoodsControl
            EditGoodsCommand = new RelayCommand<TextBlock>((parameter) => true, (parameter) => ShowEditGoods(parameter));
            ImportGoodsCommand = new RelayCommand<TextBlock>((parameter) => true, (parameter) => ShowImportGoods(parameter));
            DeleteGoodsCommand = new RelayCommand<GoodsControl>((parameter) => true, (parameter) => DeleteGoods(parameter));

            //AddGoodsWindow
            AddGoodsCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => OpenAddGoodsWindow(parameter));
            SelectImageCommand = new RelayCommand<Grid>((parameter) => true, (parameter) => ChooseImage(parameter));
            SaveCommand = new RelayCommand<AddGoodsWindow>((parameter) => true, (parameter) => AddGoods(parameter));
            ExitCommand = new RelayCommand<AddGoodsWindow>((parameter) => true, (parameter) => parameter.Close());

            //ImportGoodsWindow
            ImportCommand = new RelayCommand<ImportGoodsWindow>((parameter) => true, (parameter) => ImportGoods(parameter));
            ExitImportCommand = new RelayCommand<ImportGoodsWindow>((parameter) => true, (parameter) => parameter.Close());
            CalculateTotalCommand = new RelayCommand<ImportGoodsWindow>((parameter) => true, (parameter) => CalculateTotal(parameter));

            //ImportStockWindow
            OpenImportStockWindowCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => OpenImportStockWindow(parameter));
            BackCommand = new RelayCommand<ImportStockWindow>((parameter) => true, (parameter) => BackImportStockWindow(parameter));
            ExitImportStockCommand = new RelayCommand<ImportStockWindow>((parameter) => true, (parameter) => ExitImportStockWindow(parameter));
            GetWindowCommand = new RelayCommand<ImportStockWindow>((parameter) => true, (parameter) => SetWindow(parameter));
            LoadGoodsCommand = new RelayCommand<ImportStockWindow>((parameter) => true, (parameter) => LoadGoodsToView(parameter));
            PickGoodsCommand = new RelayCommand<ImportGoodsControl>((parameter) => true, (parameter) => PickGoods(parameter));
            DeleteImportGoodsDetailsCommand = new RelayCommand<ImportGoodsDetailsControl>((parameter) => true, (parameter) => DeleteImportGoodsDetails(parameter));
            StockReceiptInfoChangeCommand = new RelayCommand<ImportGoodsDetailsControl>((parameter) => true, (parameter) => UpdateStockReceiptInfo(parameter));
            ImportStockCommand = new RelayCommand<ImportStockWindow>((parameter) => true, (parameter) => CompleteStockReceipt(parameter));
            ViewStockReceiptTemplateCommand = new RelayCommand<ImportStockWindow>((parameter) => true, (parameter) => ViewStockReceiptTemplate(parameter));
        }

        //ImportStockWindow
        public void ViewStockReceiptTemplate(ImportStockWindow importStockWindow)
        {
            //Thông tin stock receipt
            string idStockReceipt = importStockWindow.txbIdStockReceipt.Text;
            StockReceiptTemplate stockReceiptTemplate = new StockReceiptTemplate();

            stockReceiptTemplate.txbIdStockReceipt.Text = "#" + idStockReceipt;
            stockReceiptTemplate.txbDate.Text = importStockWindow.txbToday.Text;
            stockReceiptTemplate.txbTotal.Text = importStockWindow.txbTotal.Text;
            stockReceiptTemplate.txbEmployeeName.Text = importStockWindow.txbName.Text;

            //Load các mặt hàng trong stock receipt
            List<StockReceiptInfo> listStockReceiptInfo = StockReceiptInfoDAL.Instance.GetStockReceiptInfoById(idStockReceipt);
            int numOfGoods = listStockReceiptInfo.Count();
            if (numOfGoods > 7)
            {
                stockReceiptTemplate.Height += (numOfGoods - 7) * 31;
            }
            int i = 1;
            foreach (var stockReceiptInfo in listStockReceiptInfo)
            {
                StockReceiptInfoControl stockReceiptInfoControl = new StockReceiptInfoControl();
                Goods goods = GoodsDAL.Instance.GetGoods(stockReceiptInfo.IdGoods.ToString());
                stockReceiptInfoControl.txbOrderNum.Text = i.ToString();
                stockReceiptInfoControl.txbName.Text = goods.Name;
                stockReceiptInfoControl.txbUnit.Text = goods.Unit;
                stockReceiptInfoControl.txbQuantity.Text = stockReceiptInfo.Quantity.ToString();
                stockReceiptInfoControl.txbImportPrice.Text = stockReceiptInfo.ImportPrice.ToString();
                stockReceiptInfoControl.txbTotal.Text = (stockReceiptInfo.ImportPrice * stockReceiptInfo.Quantity).ToString();

                stockReceiptTemplate.stkStockReceiptInfo.Children.Add(stockReceiptInfoControl);
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
                stockReceiptTemplate.txbFieldName.Text = dataTable.Rows[0].ItemArray[0].ToString();
                stockReceiptTemplate.txbFieldNameBrand.Text = dataTable.Rows[0].ItemArray[0].ToString();
                stockReceiptTemplate.txbPhoneNumber.Text = dataTable.Rows[0].ItemArray[1].ToString();
                stockReceiptTemplate.txbAddress.Text = dataTable.Rows[0].ItemArray[2].ToString();
            }
            catch
            {

            }
            finally
            {
                connection.conn.Close();
            }

            stockReceiptTemplate.ShowDialog();
        }
        public void CompleteStockReceipt(ImportStockWindow importStockWindow)
        {
            int idStockReceipt = int.Parse(importStockWindow.txbIdStockReceipt.Text);

            StockReceipt stockReceipt = new StockReceipt(idStockReceipt, CurrentAccount.IdAccount, DateTime.Now, int.Parse(importStockWindow.txbTotal.Text));
            if (StockReceiptDAL.Instance.UpdateOnDB(stockReceipt))
            {
                List<StockReceiptInfo> listStockReceiptInfo = StockReceiptInfoDAL.Instance.GetStockReceiptInfoById(idStockReceipt.ToString());
                foreach (var stockReceiptInfo in listStockReceiptInfo)
                {
                    if (stockReceiptInfo.ImportPrice == 0)
                    {
                        MessageBox.Show("Vui lòng nhập giá nhập kho!");
                        return;
                    }
                    Goods goods = GoodsDAL.Instance.GetGoods(stockReceiptInfo.IdGoods.ToString());
                    goods.Quantity = stockReceiptInfo.Quantity;
                    GoodsDAL.Instance.ImportToDB(goods);
                }
                MessageBox.Show("Nhập hàng thành công!");
                importStockWindow.Close();
                LoadStkGoods(homeWindow);
            }
        }
        public void UpdateStockReceiptInfo(ImportGoodsDetailsControl importGoodsDetailsControl)
        {
            string idStockReceipt = importGoodsDetailsControl.txbIdStockReceipt.Text;
            if (string.IsNullOrEmpty(importGoodsDetailsControl.txtImportPrice.Text))
            {
                importGoodsDetailsControl.txtImportPrice.Text = "0";
            }
            int quantity = int.Parse(importGoodsDetailsControl.nmsQuantity.Text.ToString());
            int importPrice = int.Parse(importGoodsDetailsControl.txtImportPrice.Text);
            StockReceiptInfo stockReceiptInfo = new StockReceiptInfo(int.Parse(idStockReceipt),
                int.Parse(importGoodsDetailsControl.txbIdGoods.Text), quantity, importPrice);
            StockReceiptInfoDAL.Instance.UpdateOnDB(stockReceiptInfo);
            importGoodsDetailsControl.txbtotal.Text = (quantity * importPrice).ToString();
            ImportStockWindow.txbTotal.Text = StockReceiptInfoDAL.Instance.CalculateTotalMoney(idStockReceipt).ToString();
        }
        public void DeleteImportGoodsDetails(ImportGoodsDetailsControl importGoodsDetailsControl)
        {
            string idStockReceipt = importGoodsDetailsControl.txbIdStockReceipt.Text;
            StockReceiptInfoDAL.Instance.DeleteByIdStock(importGoodsDetailsControl.txbIdGoods.Text, idStockReceipt);
            ImportStockWindow.stkPickedGoods.Children.Remove(importGoodsDetailsControl);

            ImportStockWindow.txbTotal.Text = StockReceiptInfoDAL.Instance.CalculateTotalMoney(idStockReceipt).ToString();
        }
        public void LoadImportGoodsDetails(ImportStockWindow importStockWindow)
        {
            int i = 1;
            string idStockReceipt = importStockWindow.txbIdStockReceipt.Text;

            importStockWindow.stkPickedGoods.Children.Clear();

            List<StockReceiptInfo> listStockReceiptInfo = StockReceiptInfoDAL.Instance.GetStockReceiptInfoById(idStockReceipt);
            foreach (var stockReceiptInfo in listStockReceiptInfo)
            {
                Goods goods = GoodsDAL.Instance.GetGoods(stockReceiptInfo.IdGoods.ToString());
                ImportGoodsDetailsControl importGoodsDetailsControl = new ImportGoodsDetailsControl();
                importGoodsDetailsControl.txbNo.Text = i.ToString();
                importGoodsDetailsControl.txbIdStockReceipt.Text = idStockReceipt;
                importGoodsDetailsControl.txbIdGoods.Text = stockReceiptInfo.IdGoods.ToString();
                importGoodsDetailsControl.txbName.Text = goods.Name;
                importGoodsDetailsControl.nmsQuantity.Text = stockReceiptInfo.Quantity;
                importGoodsDetailsControl.nmsQuantity.MinValue = 1;
                int importPrice = 0;
                if (stockReceiptInfo.ImportPrice != 0)
                {
                    importGoodsDetailsControl.txtImportPrice.Text = stockReceiptInfo.ImportPrice.ToString();
                    importPrice = stockReceiptInfo.ImportPrice;
                }
                importGoodsDetailsControl.txbtotal.Text = (importPrice * stockReceiptInfo.Quantity).ToString();

                importStockWindow.stkPickedGoods.Children.Add(importGoodsDetailsControl);
                i++;
            }
            ImportStockWindow.txbTotal.Text = StockReceiptInfoDAL.Instance.CalculateTotalMoney(idStockReceipt).ToString();
        }
        public void PickGoods(ImportGoodsControl importGoodsControl)
        {
            bool isExisted = false;
            List<StockReceiptInfo> listStockReceiptInfo = StockReceiptInfoDAL.Instance.GetStockReceiptInfoById(importGoodsControl.txbIdStockReceipt.Text);
            foreach (var stockReceiptInfo in listStockReceiptInfo)
            {
                if (stockReceiptInfo.IdGoods.ToString() == importGoodsControl.txbIdGoods.Text)
                {
                    isExisted = true;
                    stockReceiptInfo.Quantity += 1;
                    if (StockReceiptInfoDAL.Instance.UpdateOnDB(stockReceiptInfo))
                    {
                        LoadImportGoodsDetails(ImportStockWindow);
                    }
                    return;
                }
            }
            if (!isExisted)
            {
                StockReceiptInfo stockReceiptInfo = new StockReceiptInfo(int.Parse(importGoodsControl.txbIdStockReceipt.Text),
                    int.Parse(importGoodsControl.txbIdGoods.Text), 1, 0);
                StockReceiptInfoDAL.Instance.AddIntoDB(stockReceiptInfo);
                LoadImportGoodsDetails(ImportStockWindow);
            }
        }
        public void LoadGoodsToView(ImportStockWindow parameter)
        {
            parameter.wrpGoods.Children.Clear();
            DataTable goodsList = GoodsDAL.Instance.LoadData("Goods");
            for (int i = 0; i < goodsList.Rows.Count; i++)
            {
                string name = goodsList.Rows[i].ItemArray[1].ToString();
                if (name.ToLower().Contains(parameter.txtSearch.Text.ToLower()))
                {
                    ImportGoodsControl goods = new ImportGoodsControl();
                    goods.txbName.Text = goodsList.Rows[i].ItemArray[1].ToString();
                    goods.txbIdGoods.Text = goodsList.Rows[i].ItemArray[0].ToString();
                    goods.imgGood.Source = Converter.Instance.ConvertByteToBitmapImage(Convert.FromBase64String(goodsList.Rows[i].ItemArray[4].ToString()));
                    goods.txbQuantityOfInventory.Text = goodsList.Rows[i].ItemArray[5].ToString();
                    goods.txbIdStockReceipt.Text = parameter.txbIdStockReceipt.Text;
                    parameter.wrpGoods.Children.Add(goods);
                }
            }
        }
        public void SetWindow(ImportStockWindow importStockWindow)
        {
            ImportStockWindow = importStockWindow;
        }
        public void ExitImportStockWindow(ImportStockWindow importStockWindow)
        {
            if (!new StackTrace().GetFrames().Any(x => x.GetMethod().Name == "Close"))
            {
                StockReceiptInfoDAL.Instance.DeleteByIdStockReceipt(importStockWindow.txbIdStockReceipt.Text);
                StockReceiptDAL.Instance.DeleteFromDB(importStockWindow.txbIdStockReceipt.Text);
            }
        }
        public void BackImportStockWindow(ImportStockWindow importStockWindow)
        {
            StockReceiptInfoDAL.Instance.DeleteByIdStockReceipt(importStockWindow.txbIdStockReceipt.Text);
            StockReceiptDAL.Instance.DeleteFromDB(importStockWindow.txbIdStockReceipt.Text);
            importStockWindow.Close();
        }
        public void OpenImportStockWindow(HomeWindow homeWindow)
        {
            this.homeWindow = homeWindow;
            ImportStockWindow importStockWindow = new ImportStockWindow();
            try
            {
                string idStockReceipt = (StockReceiptDAL.Instance.GetMaxId() + 1).ToString();
                StockReceipt stockReceipt = new StockReceipt(int.Parse(idStockReceipt), CurrentAccount.IdAccount, DateTime.Now, 0);
                StockReceiptDAL.Instance.AddIntoDB(stockReceipt);
                importStockWindow.txbIdStockReceipt.Text = idStockReceipt;
            }
            catch
            {
                StockReceipt stockReceipt = new StockReceipt(1, CurrentAccount.IdAccount, DateTime.Now, 0);
                StockReceiptDAL.Instance.AddIntoDB(stockReceipt);
                importStockWindow.txbIdStockReceipt.Text = "1";
            }
            importStockWindow.txbToday.Text = DateTime.Now.ToString("HH:mm, dd/MM/yyyy");
            importStockWindow.txbId.Text = CurrentAccount.IdEmployee.ToString();
            importStockWindow.txbName.Text = CurrentAccount.DisplayName;
            importStockWindow.ShowDialog();
        }

        //GoodsControl
        public void ShowEditGoods(TextBlock parameter)
        {
            Goods goods = GoodsDAL.Instance.GetGoods(parameter.Text);
            AddGoodsWindow updateWindow = new AddGoodsWindow();
            updateWindow.txtIdGoods.Text = goods.IdGoods.ToString();

            updateWindow.txtName.Text = goods.Name;
            updateWindow.txtName.SelectionStart = updateWindow.txtName.Text.Length;
            updateWindow.txtName.Select(0, updateWindow.txtName.Text.Length);

            updateWindow.cboUnit.Text = goods.Unit;

            updateWindow.txtUnitPrice.Text = goods.UnitPrice.ToString();
            updateWindow.txtUnitPrice.SelectionStart = updateWindow.txtUnitPrice.Text.Length;
            updateWindow.txtUnitPrice.Select(0, updateWindow.txtUnitPrice.Text.Length);
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(goods.ImageFile);
            updateWindow.grdSelectImg.Background = imageBrush;
            if (updateWindow.grdSelectImg.Children.Count > 1)
            {
                updateWindow.grdSelectImg.Children.Remove(updateWindow.grdSelectImg.Children[0]);
                updateWindow.grdSelectImg.Children.Remove(updateWindow.grdSelectImg.Children[1]);
            }
            updateWindow.ShowDialog();
        }
        public void ShowImportGoods(TextBlock parameter)
        {
            Goods goods = GoodsDAL.Instance.GetGoods(parameter.Text);
            ImportGoodsWindow importWindow = new ImportGoodsWindow();
            importWindow.txtImportPrice.Focus();
            importWindow.dpImportDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                importWindow.txtIdStockReceipt.Text = (StockReceiptDAL.Instance.GetMaxId() + 1).ToString();
            }
            catch
            {
                importWindow.txtIdStockReceipt.Text = "1";
            }
            importWindow.txtIdGoods.Text = goods.IdGoods.ToString();

            importWindow.txtName.Text = goods.Name;
            importWindow.txtName.SelectionStart = importWindow.txtName.Text.Length;

            importWindow.cboUnit.Text = goods.Unit;

            importWindow.txtQuantity.SelectionStart = importWindow.txtQuantity.Text.Length;
            importWindow.txtQuantity.Select(0, importWindow.txtQuantity.Text.Length);
            importWindow.txtQuantity.Clear();

            importWindow.txtImportPrice.SelectionStart = importWindow.txtImportPrice.Text.Length;
            importWindow.txtImportPrice.Select(0, importWindow.txtImportPrice.Text.Length);
            importWindow.txtImportPrice.Clear();

            ImageBrush imageBrush = new ImageBrush();
            byte[] blob = goods.ImageFile;
            BitmapImage bi = Converter.Instance.ConvertByteToBitmapImage(blob);
            imageBrush.ImageSource = bi;
            importWindow.grdSelectImg.Background = imageBrush;
            if (importWindow.grdSelectImg.Children.Count > 1)
            {
                importWindow.grdSelectImg.Children.Remove(importWindow.grdSelectImg.Children[0]);
                importWindow.grdSelectImg.Children.Remove(importWindow.grdSelectImg.Children[1]);
            }
            importWindow.ShowDialog();
        }
        public void DeleteGoods(GoodsControl goodsControl)
        {
            MessageBoxResult result = MessageBox.Show("Xác nhận xóa hàng hóa?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                string idGoods = goodsControl.txbId.Text;
                bool isSuccessed1 = StockReceiptInfoDAL.Instance.DeleteFromDB(idGoods);
                bool isSuccessed2 = BillInfoDAL.Instance.DeleteIdGoods(idGoods);
                bool isSuccessed3 = GoodsDAL.Instance.DeleteFromDB(idGoods);
                if (isSuccessed1 && isSuccessed2 && isSuccessed3 || isSuccessed3)
                {
                    homeWindow.stkGoods.Children.Remove(goodsControl);
                    MessageBox.Show("Xoá thành công!");
                }
                else
                {
                    MessageBox.Show("Thực hiện thất bại!");
                }
            }
        }

        //AddGoodsWindow
        public void OpenAddGoodsWindow(HomeWindow homeWindow)
        {
            AddGoodsWindow wdAddGoods = new AddGoodsWindow();
            try
            {
                wdAddGoods.txtIdGoods.Text = (GoodsDAL.Instance.GetMaxId() + 1).ToString();
            }
            catch
            {
                wdAddGoods.txtIdGoods.Text = "1";
            }
            wdAddGoods.ShowDialog();
        }
        public void ChooseImage(Grid parameter)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Chọn ảnh";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imageFileName = op.FileName;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imageFileName);
                bitmap.EndInit();
                imageBrush.ImageSource = bitmap;
                parameter.Background = imageBrush;
                if (parameter.Children.Count > 1)
                {
                    parameter.Children.Remove(parameter.Children[0]);
                    parameter.Children.Remove(parameter.Children[1]);
                }
            }
        }
        public void AddGoods(AddGoodsWindow parameter)
        {
            List<Goods> goodsList = GoodsDAL.Instance.ConvertDBToList();
            if (string.IsNullOrWhiteSpace(parameter.txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên mặt hàng!");
                parameter.txtName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.cboUnit.Text))
            {
                MessageBox.Show("Vui lòng chọn đơn vị tính!");
                parameter.cboUnit.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.txtUnitPrice.Text))
            {
                MessageBox.Show("Vui lòng nhập đơn giá!");
                parameter.txtUnitPrice.Focus();
                return;
            }
            if (parameter.grdSelectImg.Background == null)
            {
                MessageBox.Show("Vui lòng thêm hình ảnh!");
                return;
            }
            byte[] imgByteArr;
            try
            {
                imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
            }
            catch
            {
                imgByteArr = GoodsDAL.Instance.GetGoods(parameter.txtIdGoods.Text).ImageFile;
            }
            imageFileName = null;
            Goods newGoods = new Goods(int.Parse(parameter.txtIdGoods.Text), parameter.txtName.Text,
                parameter.cboUnit.Text, double.Parse(parameter.txtUnitPrice.Text), imgByteArr);
            bool isSuccessed1 = true, isSuccessed2 = true;
            if (goodsList.Count == 0 || newGoods.IdGoods > goodsList[goodsList.Count - 1].IdGoods)
            {
                foreach (var goods in goodsList)
                {
                    if (goods.Name == parameter.txtName.Text)
                    {
                        MessageBox.Show("Mặt hàng đã tồn tại!");
                        parameter.txtName.Clear();
                        return;
                    }
                }
                isSuccessed1 = GoodsDAL.Instance.AddIntoDB(newGoods);
                if (isSuccessed1)
                {
                    MessageBox.Show("Thêm mặt hàng thành công!");
                }
            }
            else
            {
                isSuccessed2 = GoodsDAL.Instance.UpdateOnDB(newGoods);
                if (isSuccessed2)
                {
                    MessageBox.Show("Cập nhật thành công!");
                }
            }
            if (!isSuccessed1 || !isSuccessed2)
            {
                MessageBox.Show("Thực hiện thất bại");
            }
            parameter.Close();
            LoadStkGoods(homeWindow);
        }

        //ImportGoodsWindow
        public void ImportGoods(ImportGoodsWindow parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter.txtImportPrice.Text))
            {
                MessageBox.Show("Vui lòng nhập giá nhập hàng!");
                parameter.txtImportPrice.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(parameter.txtQuantity.Text))
            {
                MessageBox.Show("Vui lòng nhập số lượng hàng nhập!");
                parameter.txtQuantity.Focus();
                return;
            }

            Goods goods = new Goods(int.Parse(parameter.txtIdGoods.Text), parameter.txtName.Text,
                parameter.cboUnit.Text, 1, GoodsDAL.Instance.GetGoods(parameter.txtIdGoods.Text).ImageFile, int.Parse(parameter.txtQuantity.Text));
            bool isSuccessed1 = GoodsDAL.Instance.ImportToDB(goods);

            StockReceipt stockReceipt = new StockReceipt(int.Parse(parameter.txtIdStockReceipt.Text), CurrentAccount.IdAccount,
                DateTime.Parse(parameter.dpImportDate.Text), int.Parse(parameter.txtTotal.Text));
            bool isSuccessed2 = StockReceiptDAL.Instance.AddIntoDB(stockReceipt);

            StockReceiptInfo stockReceiptInfo = new StockReceiptInfo(int.Parse(parameter.txtIdStockReceipt.Text),
                int.Parse(parameter.txtIdGoods.Text), int.Parse(parameter.txtQuantity.Text),
                int.Parse(parameter.txtImportPrice.Text));
            bool isSuccessed3 = StockReceiptInfoDAL.Instance.AddIntoDB(stockReceiptInfo);

            if (isSuccessed1 && isSuccessed2 && isSuccessed3)
            {
                MessageBox.Show("Nhập hàng thành công!");
                parameter.Close();
                LoadStkGoods(homeWindow);
            }
            else
            {
                MessageBox.Show("Thực hiện thất bại!");
            }
        }
        public void CalculateTotal(ImportGoodsWindow parameter)
        {
            int importPriceTmp = 0, quantityTmp = 0;
            int.TryParse(parameter.txtImportPrice.Text, out importPriceTmp);
            int.TryParse(parameter.txtQuantity.Text, out quantityTmp);
            parameter.txtTotal.Text = (importPriceTmp * quantityTmp).ToString();
        }

        public void LoadStkGoods(HomeWindow homeWindow)
        {
            this.homeWindow = homeWindow;
            homeWindow.stkGoods.Children.Clear();
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
                if (CurrentAccount.Type == 2)
                {
                    temp.btnDeleteGoods.IsEnabled = false;
                    temp.btnEditGoods.IsEnabled = false;
                }
                homeWindow.stkGoods.Children.Add(temp);
                i++;
            }
        }
    }
}
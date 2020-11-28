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

namespace FootballFieldManagement.ViewModels
{
    class GoodsViewModel : BaseViewModel
    {
        private string imageFileName;
        private string quantity;
        public string Quantity { get => quantity; set => quantity = value; }
        private string importPrice;
        public string ImportPrice { get => importPrice; set => importPrice = value; }

        //GoodsControl
        public ICommand EditGoodsCommand { get; set; } //show AddGoodsWindow -> edit
        public ICommand ImportGoodsCommand { get; set; } //show ImportGoodsWindow
        public ICommand DeleteGoodsCommand { get; set; } //xóa mặt hàng

        //AddGoodsWindow
        public ICommand SelectImageCommand { get; set; } //chọn ảnh
        public ICommand SaveCommand { get; set; } //lưu thông tin mặt hàng
        public ICommand ExitCommand { get; set; } //thoát khỏi AddGoodsWindow

        //ImportGoodsWindow
        public ICommand ImportCommand { get; set; } //nhập hàng
        public ICommand ExitImportCommand { get; set; } //thoát khỏi ImportGoodsWindow
        public ICommand CalculateTotalCommand { get; set; } //tính tổng tiền

        public GoodsViewModel()
        {
            //GoodsControl
            EditGoodsCommand = new RelayCommand<TextBlock>((parameter) => true, (parameter) => ShowEditGoods(parameter));
            ImportGoodsCommand = new RelayCommand<TextBlock>((parameter) => true, (parameter) => ShowImportGoods(parameter));
            DeleteGoodsCommand = new RelayCommand<TextBlock>((parameter) => true, (parameter) => DeleteGoods(parameter));

            //AddGoodsWindow
            SelectImageCommand = new RelayCommand<Grid>((parameter) => true, (parameter) => ChooseImage(parameter));
            SaveCommand = new RelayCommand<AddGoodsWindow>((parameter) => true, (parameter) => AddGoods(parameter));
            ExitCommand = new RelayCommand<AddGoodsWindow>((parameter) => true, (parameter) => parameter.Close());

            //ImportGoodsWindow
            ImportCommand = new RelayCommand<ImportGoodsWindow>((parameter) => true, (parameter) => ImportGoods(parameter));
            ExitImportCommand = new RelayCommand<ImportGoodsWindow>((parameter) => true, (parameter) => parameter.Close());
            CalculateTotalCommand = new RelayCommand<ImportGoodsWindow>((parameter) => true, (parameter) => CalculateTotal(parameter));

        }

        //GoodsControl
        public void ShowEditGoods(TextBlock parameter)
        {
            List<Goods> goodsList = GoodsDAL.Instance.ConvertDBToList();
            AddGoodsWindow updateWindow = new AddGoodsWindow();
            foreach (var goods in goodsList)
            {
                if (goods.IdGoods.ToString() == parameter.Text)
                {
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
                    break;
                }
            }
            updateWindow.ShowDialog();
        }
        public void ShowImportGoods(TextBlock parameter)
        {
            List<Goods> goodsList = GoodsDAL.Instance.ConvertDBToList();
            List<StockReceipt> stockReceiptList = StockReceiptDAL.Instance.ConvertDBToList();
            ImportGoodsWindow importWindow = new ImportGoodsWindow();
            foreach (var goods in goodsList)
            {
                if (goods.IdGoods.ToString() == parameter.Text)
                {
                    importWindow.txtImportPrice.Focus();
                    importWindow.dpImportDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    try
                    {
                        importWindow.txtIdStockReceipt.Text = (stockReceiptList[stockReceiptList.Count() - 1].IdStockReceipt + 1).ToString();
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
                    break;
                }
            }
            importWindow.ShowDialog();
        }
        public void DeleteGoods(TextBlock txb)
        {
            MessageBoxResult result = MessageBox.Show("Xác nhận xóa hàng hóa?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                string idGoods = txb.Text;
                List<string> idStockReceiptList = StockReceiptInfoDAL.Instance.QueryIdStockReceipt(idGoods);

                bool isSuccessed1 = StockReceiptInfoDAL.Instance.DeleteFromDB(idGoods);
                bool isSuccessed2 = true;
                foreach (var idStockReceipt in idStockReceiptList)
                {
                    isSuccessed2 = StockReceiptDAL.Instance.DeleteFromDB(idStockReceipt);
                }
                bool isSuccessed3 = GoodsDAL.Instance.DeleteFromDB(idGoods);
                if (isSuccessed1 && isSuccessed2 && isSuccessed3 || isSuccessed3)
                {
                    MessageBox.Show("Xoá thành công!");
                }
                else
                {
                    MessageBox.Show("Thực hiện thất bại!");
                }
            }
        }

        //AddGoodsWindow
        public void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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
            foreach(var goods in goodsList)
            {
                if(goods.Name == parameter.txtName.Text)
                {
                    MessageBox.Show("Mặt hàng đã tồn tại!");
                    parameter.txtName.Clear();
                    return;
                }
            }
            byte[] imgByteArr;
            try
            {
                imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
            }
            catch
            {
                imgByteArr = GoodsDAL.Instance.GetGood(parameter.txtIdGoods.Text).ImageFile;
            }
            imageFileName = null;
            Goods newGoods = new Goods(int.Parse(parameter.txtIdGoods.Text), parameter.txtName.Text,
                parameter. cboUnit.Text, double.Parse(parameter.txtUnitPrice.Text), imgByteArr);
            bool isSuccessed1 = true, isSuccessed2 = true;
            if (goodsList.Count == 0 || newGoods.IdGoods > goodsList[goodsList.Count - 1].IdGoods)
            {
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
                parameter.cboUnit.Text, 1, GoodsDAL.Instance.GetGood(parameter.txtIdGoods.Text).ImageFile, int.Parse(parameter.txtQuantity.Text));
            bool isSuccessed1 = GoodsDAL.Instance.ImportToDB(goods);

            StockReceipt stockReceipt = new StockReceipt(int.Parse(parameter.txtIdStockReceipt.Text), 1,
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

    }
}
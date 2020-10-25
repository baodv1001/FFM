using FootballFieldManagement.DAL;
using FootballFieldManagement.Models;
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
    class GoodsViewModel
    {
        private string id;
        public string Id { get => id; set => id = value; }

        public ICommand SaveCommand { get; set; }
        public ICommand ImportCommand { get; set; }
        public ICommand DeleteGoodsCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public GoodsViewModel()
        {
            SaveCommand = new RelayCommand<AddGoodsWindow>((parameter) => true, (parameter) => Add(parameter));
            ImportCommand = new RelayCommand<AddGoodsWindow>((parameter) => true, (parameter) => Add(parameter));
            DeleteGoodsCommand = new RelayCommand<TextBlock>((parameter) => true, (parameter) => DeleteGoods(parameter.Text));
            ExitCommand = new RelayCommand<AddGoodsWindow>((parameter) => true, (parameter) => parameter.Close());
        }
        public void Add(AddGoodsWindow parameter)
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
            foreach(var goods in goodsList)
            {
                if(string.Equals(goods.Name, parameter.txtName.Text))
                {
                    MessageBox.Show("Mặt hàng đã tồn tại");
                    parameter.txtName.Focus();
                    return;
                }
            }
            Goods newGoods = new Goods(int.Parse(parameter.txtIdGoods.Text), parameter.txtName.Text, 1, parameter.cboUnit.Text);
            GoodsDAL.Instance.AddIntoDB(newGoods);
            parameter.Close();
        }
        public void DeleteGoods(string id)
        {
            List<Goods> goodsList = GoodsDAL.Instance.ConvertDBToList();
            foreach (var goods in goodsList)
            {
                if (goods.IdGoods.ToString() == id)
                {
                    GoodsDAL.Instance.DeleteFromDB(goods);
                    break;
                }
            }
        }
    }
}

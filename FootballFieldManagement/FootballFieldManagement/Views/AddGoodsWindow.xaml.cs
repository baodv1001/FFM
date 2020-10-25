using FootballFieldManagement.DAL;
using FootballFieldManagement.Models;
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
namespace FootballFieldManagement.Views
{
    /// <summary>
    /// Interaction logic for AddGoodsWindow.xaml
    /// </summary>
    public partial class AddGoodsWindow : Window
    {
        public AddGoodsWindow()
        {
            InitializeComponent();

            List<Goods> goodsList = GoodsDAL.Instance.ConvertDBToList();
            try
            {
                this.txtIdGoods.Text = (goodsList[goodsList.Count() - 1].IdGoods + 1).ToString();
            }
            catch
            {
                this.txtIdGoods.Text = "1";
            }
        }
    }
}

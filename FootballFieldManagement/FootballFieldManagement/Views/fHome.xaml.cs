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
    /// Interaction logic for fHome.xaml
    /// </summary>
    public partial class fHome : Window
    {
        public fHome()
        {
            InitializeComponent();
        }
        private void SwitchWindow(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);

            GridCursor.Margin = new Thickness(0, (175 + 70 * index), 40, 0);

            switch (index)
            {
                case 0:
                    grdBody_Home.Visibility = Visibility.Visible;
                    break;
                default:
                    grdBody_Home.Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}

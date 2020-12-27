using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
    /// Interaction logic for CustomMessageBoxWindow.xaml
    /// </summary>
    internal partial class CustomMessageBoxWindow : Window
    {
        internal string Caption
        {
            get
            {
                return Title;
            }
            set
            {
                Title = value;
            }
        }

        internal string Message
        {
            get
            {
                return TextBlock_Message.Text;
            }
            set
            {
                TextBlock_Message.Text = value;
            }
        }

        internal string OkButtonText
        {
            get
            {
                return Label_Ok.Content.ToString();
            }
            set => Label_Ok.Content = value.TryAddKeyboardAccellerator();
        }

        internal string CancelButtonText
        {
            get
            {
                return Label_Cancel.Content.ToString();
            }
            set
            {
                Label_Cancel.Content = value.TryAddKeyboardAccellerator();
            }
        }

        internal string YesButtonText
        {
            get
            {
                return Label_Yes.Content.ToString();
            }
            set
            {
                Label_Yes.Content = value.TryAddKeyboardAccellerator();
            }
        }

        internal string NoButtonText
        {
            get
            {
                return Label_No.Content.ToString();
            }
            set
            {
                Label_No.Content = value.TryAddKeyboardAccellerator();
            }
        }

        public MessageBoxResult Result { get; set; }

        internal CustomMessageBoxWindow(string message)
        {
            InitializeComponent();

            Message = message;
            Image_MessageBox.Visibility = System.Windows.Visibility.Collapsed;
            DisplayButtons(MessageBoxButton.OK);
        }

        internal CustomMessageBoxWindow(string message, string caption)
        {
            InitializeComponent();

            Message = message;
            Caption = caption;
            Image_MessageBox.Visibility = System.Windows.Visibility.Collapsed;
            DisplayButtons(MessageBoxButton.OK);
        }

        internal CustomMessageBoxWindow(string message, string caption, MessageBoxButton button)
        {
            InitializeComponent();

            Message = message;
            Caption = caption;
            Image_MessageBox.Visibility = System.Windows.Visibility.Collapsed;

            DisplayButtons(button);
        }

        internal CustomMessageBoxWindow(string message, string caption, MessageBoxImage image)
        {
            InitializeComponent();

            Message = message;
            Caption = caption;
            DisplayImage(image);
            DisplayButtons(MessageBoxButton.OK);
        }

        internal CustomMessageBoxWindow(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            InitializeComponent();

            Message = message;
            Caption = caption;
            Image_MessageBox.Visibility = System.Windows.Visibility.Collapsed;

            DisplayButtons(button);
            DisplayImage(image);
        }

        private void DisplayButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OKCancel:
                    // Hide all but OK, Cancel
                    Button_OK.Visibility = System.Windows.Visibility.Visible;
                    Button_OK.Focus();
                    Button_Cancel.Visibility = System.Windows.Visibility.Visible;

                    Button_Yes.Visibility = System.Windows.Visibility.Collapsed;
                    Button_No.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    // Hide all but Yes, No
                    Button_Yes.Visibility = System.Windows.Visibility.Visible;
                    Button_Yes.Focus();
                    Button_No.Visibility = System.Windows.Visibility.Visible;

                    Button_OK.Visibility = System.Windows.Visibility.Collapsed;
                    Button_Cancel.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNoCancel:
                    // Hide only OK
                    Button_Yes.Visibility = System.Windows.Visibility.Visible;
                    Button_Yes.Focus();
                    Button_No.Visibility = System.Windows.Visibility.Visible;
                    Button_Cancel.Visibility = System.Windows.Visibility.Visible;

                    Button_OK.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                default:
                    // Hide all but OK
                    Button_OK.Visibility = System.Windows.Visibility.Visible;
                    Button_OK.Focus();

                    Button_Yes.Visibility = System.Windows.Visibility.Collapsed;
                    Button_No.Visibility = System.Windows.Visibility.Collapsed;
                    Button_Cancel.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }
        }

        private void DisplayImage(MessageBoxImage image)
        {
            BitmapImage bitmapImage;

            switch (image)
            {
                case MessageBoxImage.Warning:
                    bitmapImage = new BitmapImage(new Uri("pack://application:,,,/FootballFieldManagement;component/Resources/Images/warning.png"));
                    break;
                case MessageBoxImage.Asterisk:
                    bitmapImage = new BitmapImage(new Uri("pack://application:,,,/FootballFieldManagement;component/Resources/Images/succes.png"));
                    break;
                case MessageBoxImage.Error:
                    bitmapImage = new BitmapImage(new Uri("pack://application:,,,/FootballFieldManagement;component/Resources/Images/error.png"));
                    break;
                case MessageBoxImage.Question:
                    bitmapImage = new BitmapImage(new Uri("pack://application:,,,/FootballFieldManagement;component/Resources/Images/help.png"));
                    break;
                default:
                    bitmapImage = new BitmapImage(new Uri("pack://application:,,,/FootballFieldManagement;component/Resources/Images/info.png"));
                    break;
            }

            Image_MessageBox.Source = bitmapImage;
            Image_MessageBox.Visibility = System.Windows.Visibility.Visible;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        private void Button_Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }

        private void Button_No_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }
    }
}

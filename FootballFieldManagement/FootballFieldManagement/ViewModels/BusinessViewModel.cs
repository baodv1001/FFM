using FootballFieldManagement.DAL;
using FootballFieldManagement.Models;
using FootballFieldManagement.Resources.UserControls;
using FootballFieldManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace FootballFieldManagement.ViewModels
{
    class BusinessViewModel : BaseViewModel
    {
        //Biển để set validation
        public DateTime SetDateBooking { get; set; }
        public string TypeField { get; set; }
        public string TimeFrame { get; set; }
        public string Field { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Discount { get; set; }

        DateTime selectedDate;
        public HomeWindow home;
        private FieldButtonControl PickedField;
        private int currentPage;
        private int numberofFields;
        private int maxPage;
        public int CurrentPage { get => currentPage; set => currentPage = value; }
        public int NumberofFields { get => numberofFields; set => numberofFields = value; }
        public int MaxPage { get => maxPage; set => maxPage = value; }

        private ObservableCollection<FootballField> itemSourceField = new ObservableCollection<FootballField>();

        public ObservableCollection<FootballField> ItemSourceField { get => itemSourceField; set { itemSourceField = value; OnPropertyChanged(); } }

        public FootballField SelectedField { get => selectedField; set { selectedField = value; OnPropertyChanged("SelectedField"); } }

        private FootballField selectedField = new FootballField();

        private ObservableCollection<TimeFrame> itemSourceTimeFrame = new ObservableCollection<TimeFrame>();

        public ObservableCollection<TimeFrame> ItemSourceTimeFrame { get => itemSourceTimeFrame; set { itemSourceTimeFrame = value; OnPropertyChanged(); } }

        public TimeFrame SelectedFrame { get => selectedFrame; set { selectedFrame = value; OnPropertyChanged("SelectedFrame"); } }

        private TimeFrame selectedFrame = new TimeFrame();

        //HomeWindow
        public ICommand LoadFieldCommand { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand LastPageCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }
        public ICommand FirstPageCommand { get; set; }
        public ICommand LoadTodayFieldCommand { get; set; }
        public ICommand PickFieldCommand { get; set; }
        public ICommand OpenBookingCommand { get; set; }
        //BookingWindow
        public ICommand LoadAllTimeFrameCommand { get; set; }
        public ICommand HireFieldCommand { get; set; }
        public ICommand LoadFieldTypeBKCommand { get; set; }
        public ICommand LoadFieldNameBKCommand { get; set; }
        public ICommand LoadTimeFrameBKCommand { get; set; }
        public ICommand LoadPriceBKCommand { get; set; }
        //CheckInWindow
        public ICommand CheckInCommand { get; set; }
        public ICommand CancelFieldCommand { get; set; }
        public ICommand ChangeFieldCommand { get; set; }
        public ICommand LoadFieldTypeCICommand { get; set; }
        public ICommand LoadTimeFrameCICommand { get; set; }
        public ICommand LoadFieldNameCICommand { get; set; }
        public ICommand LoadPriceCICommand { get; set; }
        public ICommand TextChangedCommand { get; set; } //discount

        public BusinessViewModel(HomeWindow homeWindow)
        {
            LoadFieldsToView(homeWindow, currentPage * 7);
        }
        public BusinessViewModel()
        {
            currentPage = 0;
            //Home Window
            LoadFieldCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => LoadFieldsToView(parameter, currentPage));
            NextPageCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => GoToNextPage(parameter));
            LastPageCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => GoToLastPage(parameter));
            PreviousPageCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => GoToPreviousPage(parameter));
            FirstPageCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => GoToFirstPage(parameter));
            LoadTodayFieldCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => { this.home = parameter; LoadToday(parameter); });
            PickFieldCommand = new RelayCommand<FieldButtonControl>((parameter) => true, (parameter) => OpenNewWindow(parameter));
            OpenBookingCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => { OpenBookingWindow(parameter); LoadFieldsToView(parameter, currentPage * 7); });
            //Booking Window
            LoadAllTimeFrameCommand = new RelayCommand<ComboBox>((parameter) => true, (parameter) => LoadAllTimeFrame(parameter));
            HireFieldCommand = new RelayCommand<BookingWindow>((parameter) => true, (parameter) => HireField(parameter));
            LoadFieldTypeBKCommand = new RelayCommand<BookingWindow>((parameter) => true, (parameter) => LoadFieldTypeBK(parameter));
            LoadFieldNameBKCommand = new RelayCommand<BookingWindow>((parameter) => true, (parameter) =>
            {
                if (parameter.cboTypeField.SelectedItem != null)
                {
                    LoadFieldName(parameter.cboTypeField.SelectedItem.ToString());
                    parameter.txbPrice.Text = "0";
                }
            });
            LoadTimeFrameBKCommand = new RelayCommand<BookingWindow>((parameter) => true, (parameter) =>
            {
                LoadTimeFrame(parameter.dpSetDate.SelectedDate.ToString()); parameter.cboTime.ItemsSource = itemSourceTimeFrame;
            });
            LoadPriceBKCommand = new RelayCommand<BookingWindow>((parameter) => true, (parameter) => LoadPriceBK(parameter));
            //Checkin Window
            CheckInCommand = new RelayCommand<CheckInWindow>((parameter) => true, (parameter) => CheckInField(parameter));
            CancelFieldCommand = new RelayCommand<CheckInWindow>((parameter) => true, (parameter) => CancelField(parameter));
            ChangeFieldCommand = new RelayCommand<CheckInWindow>((parameter) => true, (parameter) => ChangeField(parameter));
            LoadFieldTypeCICommand = new RelayCommand<CheckInWindow>((parameter) => true, (parameter) =>
            {
                if (parameter != null)
                {
                    LoadFieldTypeCI(parameter);
                }
            });
            LoadFieldNameCICommand = new RelayCommand<CheckInWindow>((parameter) => true, (parameter) =>
            {
                if (parameter.cboTypeField.SelectedItem != null)
                {
                    LoadFieldName(parameter.cboTypeField.SelectedItem.ToString());
                    parameter.txbPrice.Text = "0";
                }
                parameter.cboPickField.ItemsSource = itemSourceField;
            });
            LoadTimeFrameCICommand = new RelayCommand<CheckInWindow>((parameter) => true, (parameter) =>
            {
                LoadTimeFrame(parameter.dpSetDate.SelectedDate.ToString()); parameter.cboTime.ItemsSource = itemSourceTimeFrame;
            });
            LoadPriceCICommand = new RelayCommand<CheckInWindow>((parameter) => true, (parameter) => LoadPriceCI(parameter));
            TextChangedCommand = new RelayCommand<TextBox>((parameter) => true, (parameter) => TextChanged(parameter));
        }

        public void TextChanged(TextBox txtDiscount)
        {
            SeparateThousands(txtDiscount);

            if (string.IsNullOrEmpty(txtDiscount.Text))
            {
                txtDiscount.Text = "0";
            }
        }

        //Booking Window
        public void LoadAllTimeFrame(ComboBox comboBox)
        {
            Thread.Sleep(200);
            if (itemSourceTimeFrame == null)
            {
                List<TimeFrame> timeFrames = TimeFrameDAL.Instance.GetTimeFrame();
                foreach (var timeFrame in timeFrames)
                {
                    itemSourceTimeFrame.Add(timeFrame);
                }
                comboBox.ItemsSource = itemSourceTimeFrame;
            }
        }
        public void LoadPriceBK(BookingWindow bookingWindow)
        {
            if (bookingWindow != null)
            {
                if (bookingWindow.cboTime.SelectedItem != null && bookingWindow.cboTypeField.SelectedItem != null && bookingWindow.cboTime.SelectedItem != null && selectedFrame != null)
                {
                    string type = bookingWindow.cboTypeField.SelectedItem.ToString();
                    string[] temp = type.Split(' ');
                    if (temp.Length > 1)
                        type = temp[1];
                    string price = TimeFrameDAL.Instance.GetPriceOfTimeFrame(selectedFrame.StartTime, selectedFrame.EndTime, type);
                    if (price != null)
                    {
                        bookingWindow.txbPrice.Text = string.Format("{0:N0}", long.Parse(price));
                    }
                }
                else
                {
                    bookingWindow.txbPrice.Text = "0";
                }
            }
        }
        public void HireField(BookingWindow bookingWindow)
        {
            if (bookingWindow.dpSetDate.SelectedDate == null || bookingWindow.dpSetDate.SelectedDate < DateTime.Today)
            {
                bookingWindow.dpSetDate.Focus();
                return;
            }
            if (bookingWindow.cboTypeField.SelectedIndex == -1)
            {
                bookingWindow.cboTypeField.Focus();
                bookingWindow.cboTypeField.Text = "";
                return;
            }
            if ((bookingWindow.dpSetDate.SelectedDate == DateTime.Today && string.Compare(selectedFrame.StartTime, DateTime.Now.ToString("HH:mm")) == -1))
            {
                MessageBox.Show("Không thể đặt sân những ngày đã qua!");
                bookingWindow.dpSetDate.SelectedDate = null;
                bookingWindow.cboTime.SelectedItem = null;
                return;
            }
            if (bookingWindow.cboTime.SelectedIndex == -1)
            {
                bookingWindow.cboTime.Focus();
                bookingWindow.cboTime.Text = "";
                return;
            }
            if (bookingWindow.cboPickField.SelectedItem == null)
            {
                bookingWindow.cboPickField.Focus();
                bookingWindow.cboPickField.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(bookingWindow.txtUserName.Text))
            {
                bookingWindow.txtUserName.Focus();
                bookingWindow.txtUserName.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(bookingWindow.txtPhoneNumber.Text))
            {
                bookingWindow.txtPhoneNumber.Focus();
                bookingWindow.txtPhoneNumber.Text = "";
                return;
            }
            if (!Regex.IsMatch(bookingWindow.txtPhoneNumber.Text, @"^[0-9]+$"))
            {
                bookingWindow.txtPhoneNumber.Focus();
                return;
            }
            if (ConvertToNumber(bookingWindow.txtDiscount.Text) > ConvertToNumber(bookingWindow.txbPrice.Text))
            {
                MessageBox.Show("Không nhập giảm giá lớn hơn giá sân!");
                bookingWindow.txtDiscount.Focus();
                return;
            }
            int idFieldInfo = FieldInfoDAL.Instance.GetMaxIdFieldInfo() + 1;
            FieldInfo fieldInfo = new FieldInfo(idFieldInfo, selectedField.IdField, DateTime.Parse(bookingWindow.dpSetDate.Text + " " + selectedFrame.StartTime), DateTime.Parse(bookingWindow.dpSetDate.Text + " " + selectedFrame.EndTime), 1, bookingWindow.txtPhoneNumber.Text, bookingWindow.txtUserName.Text, bookingWindow.txtMoreInfo.Text, ConvertToNumber(bookingWindow.txtDiscount.Text), ConvertToNumber(bookingWindow.txbPrice.Text));
            if (FieldInfoDAL.Instance.AddIntoDB(fieldInfo))
            {
                MessageBox.Show("Đặt sân thành công!");

                if (!bookingWindow.dpSetDate.IsEnabled)
                {
                    //Chuyển sang sân đã đặt
                    PickedField.bdrOut.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                    PickedField.txbidFieldInfo.Text = idFieldInfo.ToString();
                    PickedField.icn1.Visibility = Visibility.Hidden;
                    PickedField.icn3.Visibility = Visibility.Visible;
                    PickedField.ToolTip = "Check in";
                }
            }
            else
            {
                MessageBox.Show("Đặt sân thất bại!");
            }
            bookingWindow.Close();
        }
        public ObservableCollection<TimeFrame> LoadTimeFrame(string day)
        {
            List<TimeFrame> timeFrames;
            if (day != "")
            {
                selectedDate = DateTime.Parse(day);
                itemSourceTimeFrame.Clear();
                timeFrames = TimeFrameDAL.Instance.GetTimeFrame();
                foreach (var timeFrame in timeFrames)
                {
                    itemSourceTimeFrame.Add(timeFrame);
                }
            }
            return itemSourceTimeFrame;
        }
        public void LoadFieldName(string selectedType)
        {
            if (selectedType != "")
            {

                itemSourceField.Clear();
                string[] temp = selectedType.Split(' ');
                if (temp.Length > 1)
                    selectedType = temp[1];
                List<FootballField> fieldNames = new List<FootballField>();

                if (selectedFrame != null && selectedDate != null)
                {
                    fieldNames = FootballFieldDAL.Instance.GetEmptyField(selectedType, selectedDate.ToShortDateString(), selectedFrame.StartTime, selectedFrame.EndTime);
                }
                foreach (var fieldName in fieldNames)
                {
                    itemSourceField.Add(fieldName);
                }
            }
        }
        public void LoadFieldTypeBK(BookingWindow bookingWindow)
        {
            List<string> typesOfField = FootballFieldDAL.Instance.GetFieldType();
            bookingWindow.cboTypeField.Items.Clear();
            foreach (var typeField in typesOfField)
            {
                bookingWindow.cboTypeField.Items.Add("Sân " + typeField + " người");
            }
        }

        //CheckIn Window
        public void LoadFieldTypeCI(CheckInWindow checkInWindow)
        {
            List<string> typesOfField = FootballFieldDAL.Instance.GetFieldType();
            checkInWindow.cboTypeField.Items.Clear();
            foreach (var typeField in typesOfField)
            {
                checkInWindow.cboTypeField.Items.Add("Sân " + typeField + " người");
            }
        }
        public void ChangeField(CheckInWindow checkInWindow)
        {
            //Đổi sang đặt sân
            checkInWindow.cboPickField.IsEnabled = true;
            checkInWindow.cboTime.IsEnabled = true;
            checkInWindow.txbCheckIn.Text = "Đặt sân";
            checkInWindow.dpSetDate.IsEnabled = true;
            checkInWindow.cboTypeField.IsEnabled = true;
            checkInWindow.txtDiscount.IsEnabled = true;
            checkInWindow.btnCancel.IsEnabled = false;
        }
        public void CancelField(CheckInWindow checkInWindow)
        {
            if (FieldInfoDAL.Instance.DeleteFromDB(PickedField.txbidFieldInfo.Text))
            {
                //Hủy sân đã đặt
                MessageBox.Show("Hủy sân thành công!");
                checkInWindow.Close();
                PickedField.icn3.Visibility = Visibility.Hidden;
                if ((checkInWindow.dpSetDate.SelectedDate < DateTime.Today || (checkInWindow.dpSetDate.SelectedDate == DateTime.Today && string.Compare(selectedFrame.StartTime, DateTime.Now.ToString("HH:mm")) == -1)))
                {
                    PickedField.IsEnabled = false;
                    PickedField.bdrOut.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFCDECDA");
                    PickedField.ToolTip = "Không thể đặt sân";
                }
                else
                {
                    PickedField.bdrOut.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FF27AE60");
                    PickedField.ToolTip = "Đặt sân";
                }
                PickedField.icn1.Visibility = Visibility.Visible;
            }
        }
        public void CheckInField(CheckInWindow checkInWindow)
        {
            if (string.IsNullOrEmpty(checkInWindow.dpSetDate.Text) || checkInWindow.dpSetDate.SelectedDate < DateTime.Today)
            {
                checkInWindow.dpSetDate.Focus();
                return;
            }
            if (checkInWindow.cboTime.SelectedItem == null)
            {
                checkInWindow.cboTime.Focus();
                checkInWindow.cboTime.Text = "";
                return;
            }
            if ((checkInWindow.dpSetDate.SelectedDate == DateTime.Today && string.Compare(selectedFrame.StartTime, DateTime.Now.ToString("HH:mm")) == -1))
            {
                MessageBox.Show("Không thể đặt sân những ngày đã qua!");
                checkInWindow.dpSetDate.SelectedDate = null;
                checkInWindow.cboTime.SelectedItem = null;
                return;
            }
            if (checkInWindow.cboPickField.SelectedItem == null)
            {
                checkInWindow.cboPickField.Focus();
                checkInWindow.cboPickField.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(checkInWindow.txtUserName.Text))
            {
                checkInWindow.txtUserName.Focus();
                checkInWindow.txtUserName.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(checkInWindow.txtPhoneNumber.Text))
            {
                checkInWindow.txtPhoneNumber.Focus();
                checkInWindow.txtPhoneNumber.Text = "";
                return;
            }
            if (!Regex.IsMatch(checkInWindow.txtPhoneNumber.Text, @"^[0-9]+$"))
            {
                checkInWindow.txtPhoneNumber.Focus();
                return;
            }
            if (ConvertToNumber(checkInWindow.txtDiscount.Text) > ConvertToNumber(checkInWindow.txbPrice.Text))
            {
                MessageBox.Show("Không nhập giảm giá lớn hơn giá sân!");
                return;
            }
            if (!checkInWindow.dpSetDate.IsEnabled && (checkInWindow.dpSetDate.SelectedDate > DateTime.Today || (checkInWindow.dpSetDate.SelectedDate == DateTime.Today && string.Compare((DateTime.Parse(selectedFrame.StartTime).Subtract(new TimeSpan(0, 30, 0))).ToString("HH:mm"), DateTime.Now.ToString("HH:mm")) == 1))) // hiện tại < giờ bắt đầu-30 phút thì không được checkin 
            {
                MessageBox.Show("Chưa đến giờ check in !");
                return;
            }

            int status = 2;
            int idFieldInfo = int.Parse(checkInWindow.txbIdFieldInfo.Text);
            if (checkInWindow.cboTime.IsEnabled) // Đã chuyển sang đặt sân
            {
                status = 1;

            }

            FieldInfo fieldInfo = new FieldInfo(idFieldInfo, selectedField.IdField, DateTime.Parse(checkInWindow.dpSetDate.Text + " " + selectedFrame.StartTime), DateTime.Parse(checkInWindow.dpSetDate.Text + " " + selectedFrame.EndTime), status, checkInWindow.txtPhoneNumber.Text, checkInWindow.txtUserName.Text, checkInWindow.txtMoreInfo.Text, ConvertToNumber(checkInWindow.txtDiscount.Text), ConvertToNumber(checkInWindow.txbPrice.Text));
            if (status == 2)
            {
                if (FieldInfoDAL.Instance.UpdateOnDB(fieldInfo))
                {
                    //Chuyển sang sân đang đá
                    MessageBox.Show("Check in thành công!");
                    PickedField.bdrOut.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FF333333");
                    PickedField.icn3.Visibility = Visibility.Hidden;
                    PickedField.icn2.Visibility = Visibility.Visible;
                    PickedField.ToolTip = "Thanh toán";
                }
                else
                {
                    MessageBox.Show("Check in thất bại!");
                }
            }
            else //Nếu đang đặt sân
            {
                if (FieldInfoDAL.Instance.DeleteFromDB(idFieldInfo.ToString()))
                {
                    fieldInfo.IdFieldInfo = FieldInfoDAL.Instance.GetMaxIdFieldInfo() + 1;
                    if (FieldInfoDAL.Instance.AddIntoDB(fieldInfo))
                    {
                        MessageBox.Show("Đổi sân thành công!");
                        LoadFieldsToView(this.home, currentPage * 7);
                    }
                    else
                    {
                        MessageBox.Show("Đổi sân thất bại!");
                    }
                }
                else
                {
                    MessageBox.Show("Đổi sân thất bại!");
                }
            }
            checkInWindow.Close();
        }
        public void LoadPriceCI(CheckInWindow checkInWindow)
        {
            if (checkInWindow != null)
            {
                if (checkInWindow.cboTime.SelectedItem != null && checkInWindow.cboTypeField.SelectedItem != null && selectedFrame != null)
                {
                    string type = checkInWindow.cboTypeField.SelectedItem.ToString();
                    string[] temp = type.Split(' ');
                    if (temp.Length > 1)
                        type = temp[1];
                    string price = TimeFrameDAL.Instance.GetPriceOfTimeFrame(selectedFrame.StartTime, selectedFrame.EndTime, type);
                    if (price != null)
                    {
                        checkInWindow.txbPrice.Text = string.Format("{0:N0}", long.Parse(price));
                    }
                }
                else
                {
                    checkInWindow.txbPrice.Text = "0";
                }
            }
        }

        //Home Window
        public void OpenBookingWindow(HomeWindow homeWindow)
        {
            BookingWindow bookingWindow = new BookingWindow();
            itemSourceTimeFrame.Clear();
            itemSourceField.Clear();
            bookingWindow.cboTime.ItemsSource = itemSourceTimeFrame;
            bookingWindow.cboPickField.ItemsSource = itemSourceField;
            bookingWindow.dpSetDate.IsEnabled = true;
            bookingWindow.cboTypeField.IsEnabled = true;
            bookingWindow.cboTime.IsEnabled = true;
            bookingWindow.cboPickField.IsEnabled = true;
            bookingWindow.dpSetDate.SelectedDate = null;
            bookingWindow.cboTypeField.Text = null;
            bookingWindow.cboTime.Text = null;
            bookingWindow.cboPickField.Text = null;
            bookingWindow.txtUserName.Text = null;
            bookingWindow.txtPhoneNumber.Text = null;
            bookingWindow.txtDiscount.SelectionStart = bookingWindow.txtDiscount.Text.Length - 1;
            bookingWindow.txtDiscount.SelectionLength = 0;
            bookingWindow.ShowDialog();
        }
        public void OpenNewWindow(FieldButtonControl fieldButtonControl)
        {
            PickedField = fieldButtonControl;
            if (fieldButtonControl.icn1.IsVisible)
            {
                //Sân trống
                BookingWindow bookingWindow = new BookingWindow();
                bookingWindow.txbidField.Text = fieldButtonControl.txbidField.Text;
                bookingWindow.dpSetDate.Text = fieldButtonControl.txbDay.Text;
                bookingWindow.cboTypeField.SelectedItem = "Sân " + fieldButtonControl.txbFieldType.Text + " người";
                bookingWindow.txtUserName.Text = null;
                bookingWindow.txtPhoneNumber.Text = null;
                bookingWindow.txtDiscount.SelectionStart = bookingWindow.txtDiscount.Text.Length - 1;
                bookingWindow.txtDiscount.SelectionLength = 0;
                LoadTimeFrame(bookingWindow.dpSetDate.Text);
                for (int i = 0; i < itemSourceTimeFrame.ToList().Count; i++)
                {
                    if (fieldButtonControl.txbendTime.Text == itemSourceTimeFrame[i].EndTime && fieldButtonControl.txbstartTime.Text == itemSourceTimeFrame[i].StartTime)
                    {
                        bookingWindow.cboTime.SelectedItem = itemSourceTimeFrame[i];
                        break;
                    }
                }
                LoadFieldName(fieldButtonControl.txbFieldType.Text);
                for (int i = 0; i < itemSourceField.ToList().Count; i++)
                {
                    if (fieldButtonControl.txbidField.Text == itemSourceField[i].IdField.ToString() && fieldButtonControl.txbFieldType.Text == itemSourceField[i].Type.ToString())
                    {
                        bookingWindow.cboPickField.SelectedItem = itemSourceField[i];
                        break;
                    }
                }
                bookingWindow.txbPrice.Text = fieldButtonControl.txbPrice.Text;
                bookingWindow.ShowDialog();
                return;
            }
            if (fieldButtonControl.icn3.IsVisible)
            {
                //Sân đã đặt 

                home = (HomeWindow)((Grid)((Grid)((Grid)((Grid)((Grid)((ScrollViewer)((StackPanel)((FieldBookingControl)((Grid)((StackPanel)fieldButtonControl.Parent).Parent).Parent).Parent).Parent).Parent).Parent).Parent).Parent).Parent).Parent;
                CheckInWindow checkInWindow = new CheckInWindow();
                FieldInfo fieldInfo = FieldInfoDAL.Instance.GetFieldInfo(PickedField.txbidFieldInfo.Text);
                checkInWindow.txbIdFieldInfo.Text = fieldButtonControl.txbidFieldInfo.Text;
                checkInWindow.dpSetDate.Text = fieldInfo.StartingTime.ToShortDateString();
                checkInWindow.cboTypeField.SelectedItem = "Sân " + fieldButtonControl.txbFieldType.Text + " người";

                LoadTimeFrame(checkInWindow.dpSetDate.Text);
                for (int i = 0; i < itemSourceTimeFrame.ToList().Count; i++)
                {
                    if (fieldButtonControl.txbendTime.Text == itemSourceTimeFrame[i].EndTime && fieldButtonControl.txbstartTime.Text == itemSourceTimeFrame[i].StartTime)
                    {
                        checkInWindow.cboTime.SelectedItem = itemSourceTimeFrame[i];
                        break;
                    }
                }
                LoadFieldName(fieldButtonControl.txbFieldType.Text);
                SelectedField = new FootballField(int.Parse(fieldButtonControl.txbidField.Text), fieldButtonControl.txbFieldName.Text, int.Parse(fieldButtonControl.txbFieldType.Text), 0, " ", 0);
                itemSourceField.Add(SelectedField);
                itemSourceField = new ObservableCollection<FootballField>(itemSourceField.OrderBy(i => i.IdField));
                for (int i = 0; i < itemSourceField.ToList().Count; i++)
                {
                    if (fieldButtonControl.txbidField.Text == itemSourceField[i].IdField.ToString() && fieldButtonControl.txbFieldType.Text == itemSourceField[i].Type.ToString())
                    {
                        checkInWindow.cboPickField.SelectedItem = itemSourceField[i];
                        break;
                    }
                }
                checkInWindow.cboTime.ItemsSource = itemSourceTimeFrame;
                checkInWindow.cboPickField.ItemsSource = itemSourceField;
                checkInWindow.txtUserName.Text = fieldInfo.CustomerName;
                checkInWindow.txtPhoneNumber.Text = fieldInfo.PhoneNumber;
                checkInWindow.txtPhoneNumber.SelectionStart = checkInWindow.txtPhoneNumber.Text.Length - 1;
                checkInWindow.txtPhoneNumber.SelectionLength = 0;
                checkInWindow.txtMoreInfo.Text = fieldInfo.Note;
                checkInWindow.txtDiscount.Text = string.Format("{0:N0}", fieldInfo.Discount);
                checkInWindow.txtDiscount.SelectionStart = checkInWindow.txtDiscount.Text.Length - 1;
                checkInWindow.txtDiscount.SelectionLength = 0;
                checkInWindow.txbPrice.Text = string.Format("{0:N0}", fieldInfo.Price);
                checkInWindow.ShowDialog();

                return;
            }
            if (fieldButtonControl.icn2.IsVisible)
            {
                //Sân đang đá -> Thanh toán
                PayWindow payWindow = new PayWindow();
                FieldInfo fieldInfo = FieldInfoDAL.Instance.GetFieldInfo(PickedField.txbidFieldInfo.Text);
                payWindow.txbtotalGoodsPrice.Text = "0";
                payWindow.txbFieldName.Text = fieldButtonControl.txbFieldName.Text;
                payWindow.txbIdFieldInfo.Text = fieldButtonControl.txbidFieldInfo.Text;
                payWindow.txbCustomerName.Text = fieldInfo.CustomerName;
                payWindow.txbCustomerPhone.Text = fieldInfo.PhoneNumber;
                payWindow.txbFieldPrice.Text = fieldButtonControl.txbPrice.Text;
                payWindow.txtMoreInfo.Text = fieldInfo.Note;
                payWindow.txbDiscount.Text = fieldInfo.Discount.ToString();
                int idBill;
                try
                {
                    idBill = (BillDAL.Instance.GetMaxIdBill() + 1);
                }
                catch
                {
                    idBill = 1;
                }
                Bill bill = new Bill(idBill, CurrentAccount.IdAccount, DateTime.Parse(DateTime.Now.ToShortDateString()),
                    DateTime.Parse(DateTime.Now.ToLongTimeString()), DateTime.Now, 1, 0, fieldInfo.IdFieldInfo, " ");
                if (BillDAL.Instance.AddIntoDB(bill))
                {
                    payWindow.txbIdBill.Text = idBill.ToString();
                    payWindow.ShowDialog();
                    if (payWindow.txbIsPaid.Text == "1") // Thanh toán thành công!
                    {
                        fieldButtonControl.recColor.Visibility = Visibility.Visible;
                        fieldButtonControl.icn2.Visibility = Visibility.Hidden;
                        fieldButtonControl.icn4.Visibility = Visibility.Visible;
                        fieldButtonControl.ToolTip = "Đã thanh toán";
                    }
                }
                else
                {
                    MessageBox.Show("Chưa thể thanh toán!");
                }
                return;
            }
        }
        public void LoadToday(HomeWindow homeWindow)
        {
            //Load lúc mới open window
            currentPage = 0;
            if (homeWindow.dpPickedDate.Text != DateTime.Now.ToShortDateString())
            {
                homeWindow.dpPickedDate.Text = DateTime.Now.ToShortDateString();
            }
            LoadFieldsToView(homeWindow, currentPage * 7);
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(10)
            };
            timer.Tick += (s, e) =>
            {
                LoadFieldsToView(home, currentPage * 7);
            };
            timer.Start();
        }
        public void SetPickedDay(object sender, RoutedEventArgs e)
        {
            //Đặt cái Text có giá trị giống ngày được chọn
            currentPage = 0;
            DatePicker datePicker = sender as DatePicker;
            if (datePicker != null && datePicker.SelectedDate != null)
            {
                datePicker.Text = ((DateTime)datePicker.SelectedDate).ToString();
            }
            else
            {
                datePicker.Text = DateTime.Now.ToShortDateString();
            }
        }
        public void GoToFirstPage(HomeWindow homeWindow)
        {
            currentPage = 0;
            LoadFieldsToView(homeWindow, currentPage * 7); // Load lại
        }
        public void GoToPreviousPage(HomeWindow homeWindow)
        {
            if (currentPage > 0)
            {
                currentPage--;
            }
            LoadFieldsToView(homeWindow, currentPage * 7); // Load lại
        }
        public void GoToLastPage(HomeWindow homeWindow)
        {
            currentPage = maxPage;
            LoadFieldsToView(homeWindow, currentPage * 7); // Load lại
        }
        public void GoToNextPage(HomeWindow homeWindow)
        {
            if (currentPage < maxPage)
            {
                currentPage++;
            }
            LoadFieldsToView(homeWindow, currentPage * 7); // Load lại
        }
        public void LoadFieldsToView(HomeWindow homeWindow, int firstField)
        {
            //Clear hết
            homeWindow.stkField.Children.Clear();
            homeWindow.stkFieldTitle.Children.Clear();

            List<TimeFrame> timeFrames = TimeFrameDAL.Instance.GetTimeFrame();
            List<FieldInfo> fieldInfos;
            List<FootballField> footballFields = FootballFieldDAL.Instance.ConvertDBToList();
            FieldBookingControl fieldBookingControl = new FieldBookingControl();

            //Nếu bị null thì hiển thị ngày hiện tại
            try
            {
                fieldInfos = FieldInfoDAL.Instance.QueryFieldInfoPerDay(DateTime.Parse(homeWindow.dpPickedDate.Text).Year.ToString(), DateTime.Parse(homeWindow.dpPickedDate.Text).Month.ToString(), DateTime.Parse(homeWindow.dpPickedDate.Text).Day.ToString());
            }
            catch
            {
                fieldInfos = FieldInfoDAL.Instance.QueryFieldInfoPerDay(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString());
            }

            //Tìm số lượng sân
            numberofFields = footballFields.Count;
            maxPage = numberofFields / 7;

            for (int i = firstField; i < firstField + 7 && i < numberofFields; i++) // Load các tên sân ( 1 page có 7 sân)
            {
                FieldTitleControl fieldTitleControl = new FieldTitleControl();
                fieldTitleControl.txbName.Text = footballFields[i].Name;
                homeWindow.stkFieldTitle.Children.Add(fieldTitleControl);
            }


            foreach (var timeFrame in timeFrames) // load các khung giờ 
            {
                fieldBookingControl = new FieldBookingControl();
                fieldBookingControl.txbendTime.Text = timeFrame.EndTime;
                fieldBookingControl.txbstartTime.Text = timeFrame.StartTime;

                for (int i = firstField; i < firstField + 7 && i < numberofFields; i++) // Load button trong 1 khung giờ
                {
                    bool flag = false;
                    FieldButtonControl fieldButtonControl = new FieldButtonControl();
                    var fieldInfo = fieldInfos.Find(x => (x.IdField == footballFields[i].IdField && string.Compare(x.StartingTime.ToString("HH:mm"), fieldBookingControl.txbstartTime.Text) == 0));
                    //Kiểm tra FieldInfo nào tồn tại - sân trong giờ đã được sử dụng 
                    if (fieldInfo != null)
                    {
                        switch (fieldInfo.Status)
                        {
                            case 1:
                                fieldButtonControl.icn3.Visibility = Visibility.Visible; // Sân đã đặt
                                fieldButtonControl.bdrOut.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                                fieldButtonControl.ToolTip = "Check In";
                                break;
                            case 2:
                                fieldButtonControl.bdrOut.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FF333333");
                                fieldButtonControl.icn2.Visibility = Visibility.Visible; // Sân đang đá
                                fieldButtonControl.ToolTip = "Thanh toán";
                                break;
                            case 3:
                                fieldButtonControl.recColor.Visibility = Visibility.Visible;
                                fieldButtonControl.icn4.Visibility = Visibility.Visible; // Sân đã thanh toán
                                fieldButtonControl.ToolTip = "Đã thanh toán";
                                break;
                            default:

                                break;
                        }
                        flag = true;
                        fieldButtonControl.txbidFieldInfo.Text = fieldInfo.IdFieldInfo.ToString();
                        fieldInfos.Remove(fieldInfo);
                    }
                    //Những khung giờ đã qua thì icon X
                    if (!flag && (homeWindow.dpPickedDate.SelectedDate < DateTime.Today || (homeWindow.dpPickedDate.SelectedDate == DateTime.Today && string.Compare(fieldBookingControl.txbstartTime.Text, DateTime.Now.ToString("HH:mm")) == -1)))
                    {
                        flag = true;
                        //fieldButtonControl.icn5.Visibility = Visibility.Visible;
                        fieldButtonControl.icn1.Visibility = Visibility.Visible;
                        fieldButtonControl.ToolTip = "Không thể đặt sân";
                        fieldButtonControl.IsEnabled = false;
                        fieldButtonControl.bdrOut.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFCDECDA");
                    }
                    //Nếu không có thì hiện icon còn trống
                    if (!flag)
                    {
                        fieldButtonControl.icn1.Visibility = Visibility.Visible;
                        fieldButtonControl.ToolTip = "Đặt sân";
                    }

                    //Lấy thông tin đặt sân đưa vào từng Button
                    App.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        fieldButtonControl.txbidField.Text = footballFields[i].IdField.ToString();
                        fieldButtonControl.txbstartTime.Text = fieldBookingControl.txbstartTime.Text;
                        fieldButtonControl.txbendTime.Text = fieldBookingControl.txbendTime.Text;
                        fieldButtonControl.txbDay.Text = homeWindow.dpPickedDate.Text;
                        fieldButtonControl.txbFieldType.Text = footballFields[i].Type.ToString();
                        string price = TimeFrameDAL.Instance.GetPriceOfTimeFrame(fieldButtonControl.txbstartTime.Text, fieldButtonControl.txbendTime.Text, fieldButtonControl.txbFieldType.Text);
                        if (price != null)
                        {
                            fieldButtonControl.txbPrice.Text = string.Format("{0:N0}", long.Parse(price));
                        }
                        fieldButtonControl.txbFieldName.Text = footballFields[i].Name;
                    }));

                    fieldBookingControl.stkMain.Children.Add(fieldButtonControl);
                }
                homeWindow.stkField.Children.Add(fieldBookingControl);
            }
            //Xử lý các nút chuyển trang
            homeWindow.btnPreviousPage.IsEnabled = (currentPage != 0);
            homeWindow.btnFirstPage.IsEnabled = (currentPage != 0);
            homeWindow.btnNextPage.IsEnabled = (currentPage != maxPage);
            homeWindow.btnLastPage.IsEnabled = (currentPage != maxPage);
        }
    }
}
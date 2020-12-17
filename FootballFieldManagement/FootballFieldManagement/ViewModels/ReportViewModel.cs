using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using FootballFieldManagement.Views;
using FootballFieldManagement.DAL;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using FootballFieldManagement.Models;
using FootballFieldManagement.Resources.UserControls;
using System.Data;
using FootballFieldManagement.Resources.Template;
using System.Data.SqlClient;

namespace FootballFieldManagement.ViewModels
{
    class ReportViewModel : BaseViewModel
    {
        //Column chart
        private ObservableCollection<string> itemSourceTime = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceTime { get => itemSourceTime; set { itemSourceTime = value; OnPropertyChanged(); } }

        private SeriesCollection seriesCollection;
        public SeriesCollection SeriesCollection { get => seriesCollection; set { seriesCollection = value; OnPropertyChanged(); } }

        private Func<double, string> formatter;
        public Func<double, string> Formatter { get => formatter; set => formatter = value; }

        private string axisYTitle;
        public string AxisYTitle { get => axisYTitle; set { axisYTitle = value; OnPropertyChanged(); } }

        private string axisXTitle;
        public string AxisXTitle { get => axisXTitle; set { axisXTitle = value; OnPropertyChanged(); } }

        private string[] labels;
        public string[] Labels { get => labels; set { labels = value; OnPropertyChanged(); } }

        //Column chart - report tab
        private ObservableCollection<string> report_itemSourceTime = new ObservableCollection<string>();
        public ObservableCollection<string> report_ItemSourceTime { get => report_itemSourceTime; set { report_itemSourceTime = value; OnPropertyChanged(); } }

        private SeriesCollection report_seriesCollection;
        public SeriesCollection report_SeriesCollection { get => report_seriesCollection; set { report_seriesCollection = value; OnPropertyChanged(); } }

        private Func<double, string> report_formatter;
        public Func<double, string> report_Formatter { get => report_formatter; set => report_formatter = value; }

        private string report_axisXTitle;
        public string report_AxisXTitle { get => report_axisXTitle; set { report_axisXTitle = value; OnPropertyChanged(); } }

        private string[] report_labels;
        public string[] report_Labels { get => report_labels; set { report_labels = value; OnPropertyChanged(); } }

        //Pie chart
        private SeriesCollection pieSeriesCollection;
        public SeriesCollection PieSeriesCollection { get => pieSeriesCollection; set { pieSeriesCollection = value; OnPropertyChanged(); } }

        private Func<ChartPoint, string> labelPoint;
        public Func<ChartPoint, string> LabelPoint { get => labelPoint; set => labelPoint = value; }

        //Dashboard
        private string increasingPercent = "0%";
        public string IncreasingPercent { get => increasingPercent; set { increasingPercent = value; OnPropertyChanged(); } }

        private string thisMonthRevenue = "0 đồng";
        public string ThisMonthRevenue { get => thisMonthRevenue; set { thisMonthRevenue = value; OnPropertyChanged(); } }

        private string numOfHiredField;
        public string NumOfHiredField { get => numOfHiredField; set { numOfHiredField = value; OnPropertyChanged(); } }

        private string today;
        public string Today { get => today; set { today = value; OnPropertyChanged(); } }

        private string thisMonth;
        public string ThisMonth { get => thisMonth; set { thisMonth = value; OnPropertyChanged(); } }

        //Xem bill
        private ObservableCollection<string> itemSourceMonthBill = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceMonthBill { get => itemSourceMonthBill; set { itemSourceMonthBill = value; OnPropertyChanged(); } }

        private ObservableCollection<string> itemSourceYearBill = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceYearBill { get => itemSourceYearBill; set { itemSourceYearBill = value; OnPropertyChanged(); } }

        //Xem stock receipt
        private ObservableCollection<string> itemSourceMonthStockReceipt = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceMonthStockReceipt { get => itemSourceMonthStockReceipt; set { itemSourceMonthStockReceipt = value; OnPropertyChanged(); } }

        private ObservableCollection<string> itemSourceYearStockReceipt = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceYearStockReceipt { get => itemSourceYearStockReceipt; set { itemSourceYearStockReceipt = value; OnPropertyChanged(); } }

        //Xem salary record
        private ObservableCollection<string> itemSourceMonthSalaryRecord = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceMonthSalaryRecord { get => itemSourceMonthSalaryRecord; set { itemSourceMonthSalaryRecord = value; OnPropertyChanged(); } }

        private ObservableCollection<string> itemSourceYearSalaryRecord = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceYearSalaryRecord { get => itemSourceYearSalaryRecord; set { itemSourceYearSalaryRecord = value; OnPropertyChanged(); } }

        public ICommand SelectionChangedCommand { get; set; }
        public ICommand InitColumnChartCommand { get; set; }
        public ICommand DataClickColumnChartCommand { get; set; }
        public ICommand InitPieChartCommand { get; set; }
        public ICommand InitDashboardCommand { get; set; }
        public ICommand LoadCommand { get; set; }

        public ICommand Report_SelectionChangedCommand { get; set; }
        public ICommand Report_InitColumnChartCommand { get; set; }

        public ICommand ViewModeCommand { get; set; }
        public ICommand ViewBillByDateCommand { get; set; }
        public ICommand ViewBillByMonthCommand { get; set; }
        public ICommand ViewBillByYearCommand { get; set; }
        public ICommand ViewBillTemplateCommand { get; set; }

        public ICommand ViewModeCommandStockReceipt { get; set; }
        public ICommand ViewStockReceiptByDateCommand { get; set; }
        public ICommand ViewStockReceiptByMonthCommand { get; set; }
        public ICommand ViewStockReceiptByYearCommand { get; set; }
        public ICommand ViewStockReceiptTemplateCommand { get; set; }

        public ICommand UpdateItemSource { get; set; }
        public ICommand ViewSalaryRecordByYearCommand { get; set; }
        public ICommand ViewSalaryRecordTemplateCommand { get; set; }

        public ReportViewModel()
        {
            SelectionChangedCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => UpdateSelectTimeItemSource(parameter));
            InitColumnChartCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => InitColumnChart(parameter));
            InitPieChartCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => InitPieChart(parameter));
            DataClickColumnChartCommand = new RelayCommand<ChartPoint>(parameter => true, parameter => DataClick(parameter));
            LoadCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadDefaultChart(parameter));

            Report_SelectionChangedCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => Report_UpdateSelectTimeItemSource(parameter));
            Report_InitColumnChartCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => Report_InitColumnChart(parameter));

            ViewModeCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => UpdateViewMode(parameter));
            ViewBillByDateCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadBillByDate(parameter));
            ViewBillByMonthCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadBillByMonth(parameter));
            ViewBillByYearCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadBillByYear(parameter));
            ViewBillTemplateCommand = new RelayCommand<BillControl>(parameter => true, parameter => ViewBillTemplate(parameter));

            ViewModeCommandStockReceipt = new RelayCommand<HomeWindow>(parameter => true, parameter => UpdateViewModeStockReceipt(parameter));
            ViewStockReceiptByDateCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadStockReceiptByDate(parameter));
            ViewStockReceiptByMonthCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadStockReceiptByMonth(parameter));
            ViewStockReceiptByYearCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadStockReceiptByYear(parameter));
            ViewStockReceiptTemplateCommand = new RelayCommand<StockReceiptControl>(parameter => true, parameter => ViewStockReceiptTemplate(parameter));

            UpdateItemSource = new RelayCommand<HomeWindow>(parameter => true, parameter => UpdateViewModeSalaryRecord());
            ViewSalaryRecordByYearCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadSalaryRecordByYear(parameter));
            ViewSalaryRecordTemplateCommand = new RelayCommand<SalaryRecordControl>(parameter => true, parameter => ViewSalaryRecordTemplate(parameter));
        }
        public ReportViewModel(HomeWindow homeWindow)
        {
            homeWindow.cboSelectTimePie.SelectedIndex = -1;
            homeWindow.cboSelectPeriod.SelectedIndex = -1;
            LoadDefaultChart(homeWindow);
        }

        public void ViewSalaryRecordTemplate(SalaryRecordControl salaryRecordControl)
        {
            string idSalaryRecord = salaryRecordControl.txbId.Text;
            SalaryRecordTemplate salaryRecordTemplate = new SalaryRecordTemplate();

            //Thông tin salary record
            salaryRecordTemplate.txbIdSalaryRecord.Text = "#" + idSalaryRecord;
            salaryRecordTemplate.txbDate.Text = salaryRecordControl.txbSalaryRecordDate.Text;
            salaryRecordTemplate.txbTotal.Text = salaryRecordControl.txbTotal.Text;

            //Thông tin salary info
            List<Salary> listSalaryInfo = SalaryDAL.Instance.ConvertDBToList();
            int numOfEmployees = listSalaryInfo.Count();
            if (numOfEmployees > 5)
            {
                salaryRecordTemplate.Height += (numOfEmployees - 5) * 31;
            }
            int i = 1;
            foreach (var salaryInfo in listSalaryInfo)
            {
                SalaryInfoControl salaryInfoControl = new SalaryInfoControl();
                Employee employee = EmployeeDAL.Instance.GetEmployee(salaryInfo.IdEmployee.ToString());

                salaryInfoControl.txbOrderNum.Text = i.ToString();
                salaryInfoControl.txbName.Text = employee.Name;
                salaryInfoControl.txbBasicSalary.Text = salaryInfo.SalaryBasic.ToString();
                salaryInfoControl.txbNumOfFault.Text = salaryInfo.NumOfFault.ToString();
                salaryInfoControl.txbNumOfShift.Text = salaryInfo.NumOfShift.ToString();
                salaryInfoControl.txbTotalSalary.Text = salaryInfo.TotalSalary.ToString();

                salaryRecordTemplate.stkSalaryInfo.Children.Add(salaryInfoControl);
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
                salaryRecordTemplate.txbFieldName.Text = dataTable.Rows[0].ItemArray[0].ToString();
                salaryRecordTemplate.txbFieldNameBrand.Text = dataTable.Rows[0].ItemArray[0].ToString();
                salaryRecordTemplate.txbPhoneNumber.Text = dataTable.Rows[0].ItemArray[1].ToString();
                salaryRecordTemplate.txbAddress.Text = dataTable.Rows[0].ItemArray[2].ToString();
            }
            catch
            {

            }
            finally
            {
                connection.conn.Close();
            }

            salaryRecordTemplate.ShowDialog();
        }
        public void LoadSalaryRecordByYear(HomeWindow homeWindow)
        {
            if (homeWindow.cboSelectYearSalaryRecord.SelectedIndex == -1)
            {
                return;
            }
            homeWindow.stkSalaryRecord.Children.Clear();
            string[] tmp = homeWindow.cboSelectYearSalaryRecord.SelectedValue.ToString().Split(' ');
            string selectedYear = tmp[1];
            DataTable dataTable = SalaryRecordDAL.Instance.GetSalaryRecordByYear(selectedYear);
            bool flag = false;
            int temp = 1;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                SalaryRecordControl SalaryRecordControl = new SalaryRecordControl();
                flag = !flag;
                if (flag)
                {
                    SalaryRecordControl.grdMain.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFFFF");
                }
                SalaryRecordControl.txbId.Text = dataTable.Rows[i].ItemArray[0].ToString();
                DateTime SalaryRecordDate = (DateTime)dataTable.Rows[i].ItemArray[1];
                SalaryRecordControl.txbSalaryRecordDate.Text = SalaryRecordDate.ToString("dd/MM/yyyy");
                SalaryRecordControl.txbSalaryRecordTime.Text = SalaryRecordDate.ToString("HH:mm");
                SalaryRecordControl.txbTotal.Text = dataTable.Rows[i].ItemArray[2].ToString();
                homeWindow.stkSalaryRecord.Children.Add(SalaryRecordControl);
                temp++;
            }
        }
        public void UpdateViewModeSalaryRecord()
        {
            itemSourceYearSalaryRecord.Clear();
            int currentYear = DateTime.Now.Year;
            itemSourceYearSalaryRecord.Add("Năm " + (currentYear - 2).ToString());
            itemSourceYearSalaryRecord.Add("Năm " + (currentYear - 1).ToString());
            itemSourceYearSalaryRecord.Add("Năm " + (currentYear).ToString());
        }

        public void ViewStockReceiptTemplate(StockReceiptControl stockReceiptControl)
        {
            //Thông tin bill
            string idStockReceipt = stockReceiptControl.txbId.Text;
            StockReceiptTemplate stockReceiptTemplate = new StockReceiptTemplate();

            stockReceiptTemplate.txbIdStockReceipt.Text = "#" + idStockReceipt;
            stockReceiptTemplate.txbDate.Text = stockReceiptControl.txbStockReceiptDate.Text;
            stockReceiptTemplate.txbTotal.Text = stockReceiptControl.txbTotal.Text;
            stockReceiptTemplate.txbEmployeeName.Text = stockReceiptControl.txbEmployeeName.Text;

            //Load các mặt hàng trong Bill
            List<StockReceiptInfo> listStockReceiptInfo = StockReceiptInfoDAL.Instance.GetStockReceiptInfoById(idStockReceipt);
            int numOfGoods = listStockReceiptInfo.Count();
            if (numOfGoods > 5)
            {
                stockReceiptTemplate.Height += (numOfGoods - 5) * 31;
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
        public void LoadStockReceiptByYear(HomeWindow homeWindow)
        {
            if (homeWindow.cboSelectYearStockReceipt.SelectedIndex == -1)
            {
                return;
            }
            homeWindow.stkStockReceipt.Children.Clear();
            string[] tmp = homeWindow.cboSelectYearStockReceipt.SelectedValue.ToString().Split(' ');
            string selectedYear = tmp[1];
            DataTable dataTable = StockReceiptDAL.Instance.GetStockReceiptByYear(selectedYear);
            bool flag = false;
            int temp = 1;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                StockReceiptControl StockReceiptControl = new StockReceiptControl();
                flag = !flag;
                if (flag)
                {
                    StockReceiptControl.grdMain.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFFFF");
                }
                StockReceiptControl.txbId.Text = dataTable.Rows[i].ItemArray[0].ToString();
                DateTime stockReceiptDate = (DateTime)dataTable.Rows[i].ItemArray[2];
                StockReceiptControl.txbStockReceiptDate.Text = stockReceiptDate.ToString("dd/MM/yyyy");
                StockReceiptControl.txbStockReceiptTime.Text = stockReceiptDate.ToString("HH:mm");
                StockReceiptControl.txbTotal.Text = dataTable.Rows[i].ItemArray[3].ToString();
                string idAccount = dataTable.Rows[i].ItemArray[1].ToString();
                if (string.IsNullOrEmpty(idAccount))
                {
                    StockReceiptControl.txbEmployeeName.Text = "Nhân viên đã nghỉ";
                }
                else
                {
                    StockReceiptControl.txbEmployeeName.Text = EmployeeDAL.Instance.GetEmployeeByIdAccount(idAccount).Name;
                }
                homeWindow.stkStockReceipt.Children.Add(StockReceiptControl);
                temp++;
            }
        }
        public void LoadStockReceiptByMonth(HomeWindow homeWindow)
        {
            if (homeWindow.cboSelectMonthStockReceipt.SelectedIndex == -1)
            {
                return;
            }
            homeWindow.stkStockReceipt.Children.Clear();
            string[] tmp = homeWindow.cboSelectMonthStockReceipt.SelectedValue.ToString().Split(' ');
            string selectedMonth = tmp[1];
            DataTable dataTable = StockReceiptDAL.Instance.GetStockReceiptByMonth(selectedMonth, DateTime.Now.Year.ToString());
            bool flag = false;
            int temp = 1;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                StockReceiptControl StockReceiptControl = new StockReceiptControl();
                flag = !flag;
                if (flag)
                {
                    StockReceiptControl.grdMain.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFFFF");
                }
                StockReceiptControl.txbId.Text = dataTable.Rows[i].ItemArray[0].ToString();
                DateTime stockReceiptDate = (DateTime)dataTable.Rows[i].ItemArray[2];
                StockReceiptControl.txbStockReceiptDate.Text = stockReceiptDate.ToString("dd/MM/yyyy");
                StockReceiptControl.txbStockReceiptTime.Text = stockReceiptDate.ToString("HH:mm");
                StockReceiptControl.txbTotal.Text = dataTable.Rows[i].ItemArray[3].ToString();
                string idAccount = dataTable.Rows[i].ItemArray[1].ToString();
                if (string.IsNullOrEmpty(idAccount))
                {
                    StockReceiptControl.txbEmployeeName.Text = "Nhân viên đã nghỉ";
                }
                else
                {
                    StockReceiptControl.txbEmployeeName.Text = EmployeeDAL.Instance.GetEmployeeByIdAccount(idAccount).Name;
                }
                homeWindow.stkStockReceipt.Children.Add(StockReceiptControl);
                temp++;
            }
        }
        public void LoadStockReceiptByDate(HomeWindow homeWindow)
        {
            if (string.IsNullOrEmpty(homeWindow.dpSelectDateStockReceipt.Text))
            {
                return;
            }
            homeWindow.stkStockReceipt.Children.Clear();
            string selectedDay = DateTime.Parse(homeWindow.dpSelectDateStockReceipt.Text).Day.ToString();
            string selectedMonth = DateTime.Parse(homeWindow.dpSelectDateStockReceipt.Text).Month.ToString();
            string selectedYear = DateTime.Parse(homeWindow.dpSelectDateStockReceipt.Text).Year.ToString();
            DataTable dataTable = StockReceiptDAL.Instance.GetStockReceiptByDate(selectedDay, selectedMonth, selectedYear);
            bool flag = false;
            int temp = 1;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                StockReceiptControl StockReceiptControl = new StockReceiptControl();
                flag = !flag;
                if (flag)
                {
                    StockReceiptControl.grdMain.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFFFF");
                }
                StockReceiptControl.txbId.Text = dataTable.Rows[i].ItemArray[0].ToString();
                DateTime stockReceiptDate = (DateTime)dataTable.Rows[i].ItemArray[2];
                StockReceiptControl.txbStockReceiptDate.Text = stockReceiptDate.ToString("dd/MM/yyyy");
                StockReceiptControl.txbStockReceiptTime.Text = stockReceiptDate.ToString("HH:mm");
                StockReceiptControl.txbTotal.Text = dataTable.Rows[i].ItemArray[3].ToString();
                string idAccount = dataTable.Rows[i].ItemArray[1].ToString();
                if (string.IsNullOrEmpty(idAccount))
                {
                    StockReceiptControl.txbEmployeeName.Text = "Nhân viên đã nghỉ";
                }
                else
                {
                    StockReceiptControl.txbEmployeeName.Text = EmployeeDAL.Instance.GetEmployeeByIdAccount(idAccount).Name;
                }
                homeWindow.stkStockReceipt.Children.Add(StockReceiptControl);
                temp++;
            }
        }
        public void UpdateViewModeStockReceipt(HomeWindow homeWindow)
        {
            homeWindow.dpSelectDateStockReceipt.Visibility = Visibility.Hidden;
            homeWindow.cboSelectMonthStockReceipt.Visibility = Visibility.Hidden;
            homeWindow.cboSelectYearStockReceipt.Visibility = Visibility.Hidden;

            homeWindow.stkStockReceipt.Children.Clear();

            switch (homeWindow.cboSelectViewModeStockReceipt.SelectedIndex)
            {
                case 0:
                    homeWindow.dpSelectDateStockReceipt.Visibility = Visibility.Visible;
                    homeWindow.dpSelectDateStockReceipt.SelectedDate = DateTime.Now;
                    //LoadBillByDate(homeWindow);
                    break;
                case 1:
                    homeWindow.cboSelectMonthStockReceipt.Visibility = Visibility.Visible;
                    itemSourceMonthStockReceipt.Clear();
                    int currentMonth = DateTime.Now.Month;
                    for (int i = 0; i < currentMonth; i++)
                    {
                        itemSourceMonthStockReceipt.Add("Tháng " + (i + 1).ToString());
                    }
                    break;
                case 2:
                    homeWindow.cboSelectYearStockReceipt.Visibility = Visibility.Visible;
                    itemSourceYearStockReceipt.Clear();
                    int currentYear = DateTime.Now.Year;
                    itemSourceYearStockReceipt.Add("Năm " + (currentYear - 2).ToString());
                    itemSourceYearStockReceipt.Add("Năm " + (currentYear - 1).ToString());
                    itemSourceYearStockReceipt.Add("Năm " + (currentYear).ToString());
                    break;
            }
        }

        public void ViewBillTemplate(BillControl billControl)
        {
            //Thông tin bill
            string idBill = billControl.txbId.Text;
            BillTemplate billTemplate = new BillTemplate();
            Bill bill = BillDAL.Instance.GetBill(idBill);
            billTemplate.txbIdBill.Text = "#" + idBill;
            billTemplate.txbInvoiceDate.Text = billControl.txbInvoiceDate.Text;
            billTemplate.txbCheckInTime.Text = bill.CheckInTime.ToString("H:mm");
            billTemplate.txbCheckOutTime.Text = bill.CheckOutTime.ToString("H:mm");
            billTemplate.txbTotal.Text = bill.TotalMoney.ToString();
            
            //Load các mặt hàng trong Bill
            List<BillInfo> listBillInfo = BillInfoDAL.Instance.GetBillInfos(idBill);
            int numOfGoods = listBillInfo.Count();
            if (numOfGoods > 7)
            {
                billTemplate.Height += (numOfGoods - 7) * 35;
            }
            int i = 1;

            BillInfoControl billInfoControl = new BillInfoControl();
            //Thêm sân vào nha
            //billInfoControl.txbOrderNum.Text = i.ToString();
            //billInfoControl.txbName.Text = FieldInfoDAL.Instance.GetFieldInfo(idBill).;
            foreach (var billInfo in listBillInfo)
            {
                Goods goods = GoodsDAL.Instance.GetGoods(billInfo.IdGoods.ToString());
                billInfoControl.txbOrderNum.Text = i.ToString();
                billInfoControl.txbName.Text = goods.Name;
                billInfoControl.txbUnitPrice.Text = goods.UnitPrice.ToString();
                billInfoControl.txbQuantity.Text = billInfo.Quantity.ToString();
                billInfoControl.txbTotal.Text = (goods.UnitPrice * billInfo.Quantity).ToString();

                billTemplate.stkBillInfo.Children.Add(billInfoControl);
                i++;
            }

            //Thông tin khách hàng
            FieldInfo fieldInfo = FieldInfoDAL.Instance.GetFieldInfo(bill.IdFieldInfo.ToString());
            billTemplate.txbCustomerName.Text = fieldInfo.CustumerName;
            billTemplate.txbCustomerPhoneNumber.Text = fieldInfo.PhoneNumber;
            billTemplate.txbDiscount.Text = fieldInfo.Discount.ToString();
            billTemplate.txbTotalBefore.Text = (bill.TotalMoney - fieldInfo.Discount).ToString();

            billTemplate.txbEmployeeName.Text = billControl.txbEmployeeName.Text;
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
        public void LoadBillByYear(HomeWindow homeWindow)
        {
            if (homeWindow.cboSelectYearBill.SelectedIndex == -1)
            {
                return;
            }
            homeWindow.stkBill.Children.Clear();
            string[] tmp = homeWindow.cboSelectYearBill.SelectedValue.ToString().Split(' ');
            string selectedYear = tmp[1];
            DataTable dataTable = BillDAL.Instance.LoadBillByYear(selectedYear);
            bool flag = false;
            int temp = 1;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                BillControl billControl = new BillControl();
                flag = !flag;
                if (flag)
                {
                    billControl.grdMain.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFFFF");
                }
                billControl.txbId.Text = dataTable.Rows[i].ItemArray[0].ToString();
                DateTime invoiceDate = (DateTime)dataTable.Rows[i].ItemArray[2];
                TimeSpan invoiceTime = (TimeSpan)dataTable.Rows[i].ItemArray[3];
                billControl.txbInvoiceDate.Text = invoiceDate.ToString("dd/MM/yyyy");
                billControl.txbTime.Text = invoiceTime.ToString(@"hh\:mm");
                billControl.txbTotal.Text = dataTable.Rows[i].ItemArray[4].ToString();
                string idAccount = dataTable.Rows[i].ItemArray[1].ToString();
                if (string.IsNullOrEmpty(idAccount))
                {
                    billControl.txbEmployeeName.Text = "Nhân viên đã nghỉ";
                }
                else
                {
                    billControl.txbEmployeeName.Text = EmployeeDAL.Instance.GetEmployeeByIdAccount(idAccount).Name;
                }
                homeWindow.stkBill.Children.Add(billControl);
                temp++;
            }
        }
        public void LoadBillByMonth(HomeWindow homeWindow)
        {
            if (homeWindow.cboSelectMonthBill.SelectedIndex == -1)
            {
                return;
            }
            homeWindow.stkBill.Children.Clear();
            string[] tmp = homeWindow.cboSelectMonthBill.SelectedValue.ToString().Split(' ');
            string selectedMonth = tmp[1];
            DataTable dataTable = BillDAL.Instance.LoadBillByMonth(selectedMonth, DateTime.Now.Year.ToString());
            bool flag = false;
            int temp = 1;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                BillControl billControl = new BillControl();
                flag = !flag;
                if (flag)
                {
                    billControl.grdMain.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFFFF");
                }
                billControl.txbId.Text = dataTable.Rows[i].ItemArray[0].ToString();
                DateTime invoiceDate = (DateTime)dataTable.Rows[i].ItemArray[2];
                TimeSpan invoiceTime = (TimeSpan)dataTable.Rows[i].ItemArray[3];
                billControl.txbInvoiceDate.Text = invoiceDate.ToString("dd/MM/yyyy");
                billControl.txbTime.Text = invoiceTime.ToString(@"hh\:mm");
                billControl.txbTotal.Text = dataTable.Rows[i].ItemArray[4].ToString();
                string idAccount = dataTable.Rows[i].ItemArray[1].ToString();
                if (string.IsNullOrEmpty(idAccount))
                {
                    billControl.txbEmployeeName.Text = "Nhân viên đã nghỉ";
                }
                else
                {
                    billControl.txbEmployeeName.Text = EmployeeDAL.Instance.GetEmployeeByIdAccount(idAccount).Name;
                }
                homeWindow.stkBill.Children.Add(billControl);
                temp++;
            }
        }
        public void LoadBillByDate(HomeWindow homeWindow)
        {
            if (string.IsNullOrEmpty(homeWindow.dpSelectDateBill.Text))
            {
                return;
            }
            homeWindow.stkBill.Children.Clear();
            string selectedDay = DateTime.Parse(homeWindow.dpSelectDateBill.Text).Day.ToString();
            string selectedMonth = DateTime.Parse(homeWindow.dpSelectDateBill.Text).Month.ToString();
            string selectedYear = DateTime.Parse(homeWindow.dpSelectDateBill.Text).Year.ToString();
            DataTable dataTable = BillDAL.Instance.LoadBillByDate(selectedDay, selectedMonth, selectedYear);
            bool flag = false;
            int temp = 1;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                BillControl billControl = new BillControl();
                flag = !flag;
                if (flag)
                {
                    billControl.grdMain.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFFFF");
                }
                billControl.txbId.Text = dataTable.Rows[i].ItemArray[0].ToString();
                DateTime invoiceDate = (DateTime)dataTable.Rows[i].ItemArray[2];
                TimeSpan invoiceTime = (TimeSpan)dataTable.Rows[i].ItemArray[3];
                billControl.txbInvoiceDate.Text = invoiceDate.ToString("dd/MM/yyyy");
                billControl.txbTime.Text = invoiceTime.ToString(@"hh\:mm");
                billControl.txbTotal.Text = dataTable.Rows[i].ItemArray[4].ToString();
                string idAccount = dataTable.Rows[i].ItemArray[1].ToString();
                if (string.IsNullOrEmpty(idAccount))
                {
                    billControl.txbEmployeeName.Text = "Nhân viên đã nghỉ";
                }
                else
                {
                    billControl.txbEmployeeName.Text = EmployeeDAL.Instance.GetEmployeeByIdAccount(idAccount).Name;
                }
                homeWindow.stkBill.Children.Add(billControl);
                temp++;
            }
        }
        public void UpdateViewMode(HomeWindow homeWindow)
        {
            homeWindow.dpSelectDateBill.Visibility = Visibility.Hidden;
            homeWindow.cboSelectMonthBill.Visibility = Visibility.Hidden;
            homeWindow.cboSelectYearBill.Visibility = Visibility.Hidden;

            homeWindow.stkBill.Children.Clear();

            switch (homeWindow.cboSelectViewMode.SelectedIndex)
            {
                case 0:
                    homeWindow.dpSelectDateBill.Visibility = Visibility.Visible;
                    homeWindow.dpSelectDateBill.SelectedDate = DateTime.Now;
                    LoadBillByDate(homeWindow);
                    break;
                case 1:
                    homeWindow.cboSelectMonthBill.Visibility = Visibility.Visible;
                    itemSourceMonthBill.Clear();
                    int currentMonth = DateTime.Now.Month;
                    for (int i = 0; i < currentMonth; i++)
                    {
                        itemSourceMonthBill.Add("Tháng " + (i + 1).ToString());
                    }
                    break;
                case 2:
                    homeWindow.cboSelectYearBill.Visibility = Visibility.Visible;
                    itemSourceYearBill.Clear();
                    int currentYear = DateTime.Now.Year;
                    itemSourceYearBill.Add("Năm " + (currentYear - 2).ToString());
                    itemSourceYearBill.Add("Năm " + (currentYear - 1).ToString());
                    itemSourceYearBill.Add("Năm " + (currentYear).ToString());
                    break;
            }
        }

        public void DataClick(ChartPoint p)
        {
            MessageBox.Show("[COMMAND] you clicked " + p.X + ", " + p.Y);
        }
        public void LoadDefaultChart(HomeWindow homeWindow)
        {
            string currentDay = DateTime.Now.Day.ToString();
            string currentMonth = DateTime.Now.Month.ToString();
            string lastMonth = (int.Parse(currentMonth) - 1).ToString();
            string currentYear = DateTime.Now.Year.ToString();
            ThisMonth = DateTime.Now.ToString("MM/yyyy");
            NumOfHiredField = ReportDAL.Instance.QueryRevenueNumOfHiredFieldInMonth(currentMonth, currentYear).ToString() + " lượt";
            ThisMonthRevenue = ReportDAL.Instance.QueryRevenueInMonth(currentMonth, currentYear).ToString() + " đồng";
            try
            {
                IncreasingPercent = (Math.Round((ReportDAL.Instance.QueryRevenueInMonth(currentMonth, currentYear) / ReportDAL.Instance.QueryRevenueInMonth(lastMonth, currentYear) * 100), 2)).ToString() + "%";
            }
            catch
            {
                IncreasingPercent = "100%";
            }
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            timer.Tick += (s, e) =>
            {
                homeWindow.cboSelectTimePie.SelectedIndex = 0;
                homeWindow.cboSelectPeriod.SelectedIndex = 0;
                homeWindow.cboSelectTime.SelectedIndex = DateTime.Now.Month - 1;

                homeWindow.cboSelectPeriod_Report.SelectedIndex = 0;
                homeWindow.cboSelectTime_Report.SelectedIndex = DateTime.Now.Month - 1;

                timer.Stop();
            };
            timer.Start();
        }
        public void InitPieChart(HomeWindow homeWindow)
        {
            labelPoint = chartPoint => string.Format("{0}", chartPoint.Y);
            if (homeWindow.cboSelectTimePie.SelectedIndex == 0)
            {
                string currentDay = DateTime.Now.Day.ToString();
                string currentMonth = DateTime.Now.Month.ToString();
                string currentYear = DateTime.Now.Year.ToString();
                PieSeriesCollection = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "Bán hàng",
                        Values = ReportDAL.Instance.QueryRevenueFromSellingInDay(currentDay, currentMonth, currentYear),
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FF1976D2"),
                        DataLabels = true,
                        FontSize = 16,
                        LabelPoint = labelPoint,
                    },
                    new PieSeries
                    {
                        Title="Sân bóng",
                        Values = ReportDAL.Instance.QueryRevenueFromFieldInDay(currentDay, currentMonth, currentYear),
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FF27AE60"),
                        DataLabels = true,
                        FontSize = 16,
                        LabelPoint = labelPoint,
                    },
                };
            }
            else
            {
                string currentMonth = DateTime.Now.Month.ToString();
                string currentYear = DateTime.Now.Year.ToString();
                PieSeriesCollection = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "Bán hàng",
                        Values = ReportDAL.Instance.QueryRevenueFromSellingInMonth(currentMonth, currentYear),
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FF1976D2"),
                        DataLabels = true,
                        FontSize = 16,
                        LabelPoint = labelPoint,
                    },
                    new PieSeries
                    {
                        Title="Sân bóng",
                        Values = ReportDAL.Instance.QueryRevenueFromFieldInMonth(currentMonth, currentYear),
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FF27AE60"),
                        DataLabels = true,
                        FontSize = 16,
                        LabelPoint = labelPoint,
                    },
                };
            }
        }
        public void InitColumnChart(HomeWindow homeWindow)
        {
            if (homeWindow.cboSelectPeriod.SelectedIndex == 0) //Theo tháng => 31 ngày
            {
                if (homeWindow.cboSelectTime.SelectedIndex != -1)
                {
                    AxisXTitle = "Ngày";
                    string[] tmp = homeWindow.cboSelectTime.SelectedValue.ToString().Split(' ');
                    string selectedMonth = tmp[1];
                    string currentYear = DateTime.Now.Year.ToString();
                    SeriesCollection = new SeriesCollection
                    {
                        new ColumnSeries
                        {
                            Title = "Doanh thu",
                            Fill = (Brush)new BrushConverter().ConvertFrom("#FF1976D2"),
                            Values = ReportDAL.Instance.QueryRevenueByMonth(selectedMonth, currentYear),
                        },
                        new ColumnSeries
                        {
                            Title = "Chi phí",
                            Fill = (Brush)new BrushConverter().ConvertFrom("#FFF44336"),
                            Values = ReportDAL.Instance.QueryOutcomeByMonth(selectedMonth, currentYear),
                        }
                    };
                    Labels = ReportDAL.Instance.QueryDayInMonth(selectedMonth, currentYear);
                    Formatter = value => value.ToString("N");
                }
            }
            else if (homeWindow.cboSelectPeriod.SelectedIndex == 1) //Theo quý => 4 quý
            {
                if (homeWindow.cboSelectTime.SelectedIndex != -1)
                {
                    AxisXTitle = "Quý";
                    string[] tmp = homeWindow.cboSelectTime.SelectedValue.ToString().Split(' ');
                    string selectedYear = tmp[1];
                    SeriesCollection = new SeriesCollection
                    {
                        new ColumnSeries
                        {
                            Title = "Doanh thu",
                            Fill = (Brush)new BrushConverter().ConvertFrom("#FF1976D2"),
                            Values = ReportDAL.Instance.QueryRevenueByQuarter(selectedYear),
                        },
                        new ColumnSeries
                        {
                            Title = "Chi phí",
                            Fill = (Brush)new BrushConverter().ConvertFrom("#FFF44336"),
                            Values = ReportDAL.Instance.QueryOutcomeByQuarter(selectedYear),
                        }
                    };
                    Labels = ReportDAL.Instance.QueryQuarterInYear(selectedYear);
                    Formatter = value => value.ToString("N");
                }
            }
            else
            {
                if (homeWindow.cboSelectTime.SelectedIndex != -1) //Theo năm => 12 tháng
                {
                    AxisXTitle = "Tháng";
                    string[] tmp = homeWindow.cboSelectTime.SelectedValue.ToString().Split(' ');
                    string selectedYear = tmp[1];
                    SeriesCollection = new SeriesCollection
                    {
                        new ColumnSeries
                        {
                            Title = "Doanh thu",
                            Fill = (Brush)new BrushConverter().ConvertFrom("#FF1976D2"),
                            Values = ReportDAL.Instance.QueryRevenueByYear(selectedYear),
                        },
                        new ColumnSeries
                        {
                            Title = "Chi phí",
                            Fill = (Brush)new BrushConverter().ConvertFrom("#FFF44336"),
                            Values = ReportDAL.Instance.QueryOutcomeByYear(selectedYear)
                        }
                    };
                    Labels = ReportDAL.Instance.QueryMonthInYear(selectedYear);
                    Formatter = value => value.ToString("N");
                }
            }
        }
        public void UpdateSelectTimeItemSource(HomeWindow homeWindow)
        {
            ItemSourceTime.Clear();
            if (homeWindow.cboSelectPeriod.SelectedIndex == 0) //Theo tháng
            {
                int currentMonth = DateTime.Now.Month;
                for (int i = 0; i < currentMonth; i++)
                {
                    ItemSourceTime.Add("Tháng " + (i + 1).ToString());
                }
            }
            else
            {
                int currentYear = DateTime.Now.Year;
                ItemSourceTime.Add("Năm " + (currentYear - 2).ToString());
                ItemSourceTime.Add("Năm " + (currentYear - 1).ToString());
                ItemSourceTime.Add("Năm " + (currentYear).ToString());
            }
        }

        public void Report_InitColumnChart(HomeWindow homeWindow)
        {
            if (homeWindow.cboSelectPeriod_Report.SelectedIndex == 0) //Theo tháng => 31 ngày
            {
                if (homeWindow.cboSelectTime_Report.SelectedIndex != -1)
                {
                    report_AxisXTitle = "Ngày";
                    string[] tmp = homeWindow.cboSelectTime_Report.SelectedValue.ToString().Split(' ');
                    string selectedMonth = tmp[1];
                    string currentYear = DateTime.Now.Year.ToString();
                    report_SeriesCollection = new SeriesCollection
                    {
                        new ColumnSeries
                        {
                            Title = "Doanh thu",
                            Fill = (Brush)new BrushConverter().ConvertFrom("#FF1976D2"),
                            Values = ReportDAL.Instance.QueryRevenueByMonth(selectedMonth, currentYear),
                        },
                        new ColumnSeries
                        {
                            Title = "Chi phí",
                            Fill = (Brush)new BrushConverter().ConvertFrom("#FFF44336"),
                            Values = ReportDAL.Instance.QueryOutcomeByMonth(selectedMonth, currentYear),
                        }
                    };
                    report_Labels = ReportDAL.Instance.QueryDayInMonth(selectedMonth, currentYear);
                    report_Formatter = value => value.ToString("N");
                }
            }
            else if (homeWindow.cboSelectPeriod_Report.SelectedIndex == 1) //Theo quý => 4 quý
            {
                if (homeWindow.cboSelectTime_Report.SelectedIndex != -1)
                {
                    report_AxisXTitle = "Quý";
                    string[] tmp = homeWindow.cboSelectTime_Report.SelectedValue.ToString().Split(' ');
                    string selectedYear = tmp[1];
                    report_SeriesCollection = new SeriesCollection
                    {
                        new ColumnSeries
                        {
                            Title = "Doanh thu",
                            Fill = (Brush)new BrushConverter().ConvertFrom("#FF1976D2"),
                            Values = ReportDAL.Instance.QueryRevenueByQuarter(selectedYear),
                        },
                        new ColumnSeries
                        {
                            Title = "Chi phí",
                            Fill = (Brush)new BrushConverter().ConvertFrom("#FFF44336"),
                            Values = ReportDAL.Instance.QueryOutcomeByQuarter(selectedYear),
                        }
                    };
                    report_Labels = ReportDAL.Instance.QueryQuarterInYear(selectedYear);
                    report_Formatter = value => value.ToString("N");
                }
            }
            else
            {
                if (homeWindow.cboSelectTime_Report.SelectedIndex != -1) //Theo năm => 12 tháng
                {
                    report_AxisXTitle = "Tháng";
                    string[] tmp = homeWindow.cboSelectTime_Report.SelectedValue.ToString().Split(' ');
                    string selectedYear = tmp[1];
                    report_SeriesCollection = new SeriesCollection
                    {
                        new ColumnSeries
                        {
                            Title = "Doanh thu",
                            Fill = (Brush)new BrushConverter().ConvertFrom("#FF1976D2"),
                            Values = ReportDAL.Instance.QueryRevenueByYear(selectedYear),
                        },
                        new ColumnSeries
                        {
                            Title = "Chi phí",
                            Fill = (Brush)new BrushConverter().ConvertFrom("#FFF44336"),
                            Values = ReportDAL.Instance.QueryOutcomeByYear(selectedYear)
                        }
                    };
                    report_Labels = ReportDAL.Instance.QueryMonthInYear(selectedYear);
                    report_Formatter = value => value.ToString("N");
                }
            }
        }
        public void Report_UpdateSelectTimeItemSource(HomeWindow homeWindow)
        {
            report_ItemSourceTime.Clear();
            if (homeWindow.cboSelectPeriod_Report.SelectedIndex == 0) //Theo tháng
            {
                int currentMonth = DateTime.Now.Month;
                for (int i = 0; i < currentMonth; i++)
                {
                    report_ItemSourceTime.Add("Tháng " + (i + 1).ToString());
                }
            }
            else
            {
                int currentYear = DateTime.Now.Year;
                report_ItemSourceTime.Add("Năm " + (currentYear - 2).ToString());
                report_ItemSourceTime.Add("Năm " + (currentYear - 1).ToString());
                report_ItemSourceTime.Add("Năm " + (currentYear).ToString());
            }
        }
    }
}

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
        public Func<double, string> Formatter { get => formatter; set { formatter = value; OnPropertyChanged(); } }

        private string axisYTitle;
        public string AxisYTitle { get => axisYTitle; set { axisYTitle = value; OnPropertyChanged(); } }

        private string axisXTitle;
        public string AxisXTitle { get => axisXTitle; set { axisXTitle = value; OnPropertyChanged(); } }

        private string[] labels;
        public string[] Labels { get => labels; set { labels = value; OnPropertyChanged(); } }

        //Column chart - report tab
        private ObservableCollection<string> rpitemSourceTime = new ObservableCollection<string>();
        public ObservableCollection<string> RpItemSourceTime { get => rpitemSourceTime; set { rpitemSourceTime = value; OnPropertyChanged(); } }

        private SeriesCollection rpseriesCollection;
        public SeriesCollection RpSeriesCollection { get => rpseriesCollection; set { rpseriesCollection = value; OnPropertyChanged(); } }

        private Func<double, string> rpformatter;
        public Func<double, string> RpFormatter { get => rpformatter; set => rpformatter = value; }

        private string rpaxisXTitle;
        public string RpAxisXTitle { get => rpaxisXTitle; set { rpaxisXTitle = value; OnPropertyChanged(); } }

        private string[] rplabels;
        public string[] RpLabels { get => rplabels; set { rplabels = value; OnPropertyChanged(); } }

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
        private ObservableCollection<string> itemSourceMonthStR = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceMonthStR { get => itemSourceMonthStR; set { itemSourceMonthStR = value; OnPropertyChanged(); } }

        private ObservableCollection<string> itemSourceYearStR = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceYearStR { get => itemSourceYearStR; set { itemSourceYearStR = value; OnPropertyChanged(); } }

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

        public ICommand RpSelectionChangedCommand { get; set; }
        public ICommand RpInitColumnChartCommand { get; set; }

        public ICommand ViewModeCommand { get; set; }
        public ICommand ViewBillByDateCommand { get; set; }
        public ICommand ViewBillByMonthCommand { get; set; }
        public ICommand ViewBillByYearCommand { get; set; }
        public ICommand ViewBillTemplateCommand { get; set; }

        public ICommand ViewModeCommandStR { get; set; }
        public ICommand ViewStRByDateCommand { get; set; }
        public ICommand ViewStRByMonthCommand { get; set; }
        public ICommand ViewStRByYearCommand { get; set; }
        public ICommand ViewStRTemplateCommand { get; set; }

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

            RpSelectionChangedCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => RpUpdateSelectTimeItemSource(parameter));
            RpInitColumnChartCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => RpInitColumnChart(parameter));

            ViewModeCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => UpdateViewMode(parameter));
            ViewBillByDateCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadBillByDate(parameter));
            ViewBillByMonthCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadBillByMonth(parameter));
            ViewBillByYearCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadBillByYear(parameter));
            ViewBillTemplateCommand = new RelayCommand<BillControl>(parameter => true, parameter => ViewBillTemplate(parameter));

            ViewModeCommandStR = new RelayCommand<HomeWindow>(parameter => true, parameter => UpdateViewModeStR(parameter));
            ViewStRByDateCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadStRByDate(parameter));
            ViewStRByMonthCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadStRByMonth(parameter));
            ViewStRByYearCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadStRByYear(parameter));
            ViewStRTemplateCommand = new RelayCommand<StockReceiptControl>(parameter => true, parameter => ViewStRTemplate(parameter));

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
            salaryRecordTemplate.txbName.Text = salaryRecordControl.txbName.Text;

            //Thông tin salary info
            List<Salary> listSalaryInfo = SalaryDAL.Instance.GetSalaryInfoById(idSalaryRecord);
            int numOfEmployees = listSalaryInfo.Count();
            if (numOfEmployees > 5)
            {
                salaryRecordTemplate.Height += (numOfEmployees - 5) * 31;
            }
            int i = 1;
            foreach (var salaryInfo in listSalaryInfo)
            {
                SalaryInfoControl salaryInfoControl = new SalaryInfoControl();
                Employee employee = EmployeeDAL.Instance.GetEmployeeByIdEmployee(salaryInfo.IdEmployee.ToString());

                salaryInfoControl.txbOrderNum.Text = i.ToString();
                salaryInfoControl.txbName.Text = employee.Name;
                salaryInfoControl.txbBasicSalary.Text = string.Format("{0:N0}", long.Parse(SalarySettingDAL.Instance.GetBaseSalary(employee.Position)));
                salaryInfoControl.txbNumOfFault.Text = salaryInfo.NumOfFault.ToString();
                salaryInfoControl.txbNumOfShift.Text = salaryInfo.NumOfShift.ToString();
                salaryInfoControl.txbTotalSalary.Text = string.Format("{0:N0}", salaryInfo.TotalSalary);

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
                SalaryRecordControl salaryRecordControl = new SalaryRecordControl();
                flag = !flag;
                if (flag)
                {
                    salaryRecordControl.grdMain.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFFFF");
                }
                salaryRecordControl.txbId.Text = dataTable.Rows[i].ItemArray[0].ToString();
                DateTime SalaryRecordDate = (DateTime)dataTable.Rows[i].ItemArray[1];
                salaryRecordControl.txbSalaryRecordDate.Text = SalaryRecordDate.ToString("dd/MM/yyyy");
                salaryRecordControl.txbSalaryRecordTime.Text = SalaryRecordDate.ToString("HH:mm");
                salaryRecordControl.txbTotal.Text = string.Format("{0:N0}", long.Parse(dataTable.Rows[i].ItemArray[2].ToString()));
                salaryRecordControl.txbName.Text = EmployeeDAL.Instance.GetEmployeeByIdAccount(dataTable.Rows[i].ItemArray[3].ToString()).Name;

                homeWindow.stkSalaryRecord.Children.Add(salaryRecordControl);
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

        public void ViewStRTemplate(StockReceiptControl stockReceiptControl)
        {
            //Thông tin stock receipt
            string idStockReceipt = stockReceiptControl.txbId.Text;
            StockReceiptTemplate stockReceiptTemplate = new StockReceiptTemplate();

            stockReceiptTemplate.txbIdStockReceipt.Text = "#" + idStockReceipt;
            stockReceiptTemplate.txbDate.Text = stockReceiptControl.txbStockReceiptDate.Text;
            stockReceiptTemplate.txbTotal.Text = stockReceiptControl.txbTotal.Text;
            stockReceiptTemplate.txbEmployeeName.Text = stockReceiptControl.txbEmployeeName.Text;

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
                stockReceiptInfoControl.txbImportPrice.Text = string.Format("{0:N0}", stockReceiptInfo.ImportPrice);
                stockReceiptInfoControl.txbTotal.Text = string.Format("{0:N0}", stockReceiptInfo.ImportPrice * stockReceiptInfo.Quantity);

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
        public void LoadStRByYear(HomeWindow homeWindow)
        {
            if (homeWindow.cboSelectYearStR.SelectedIndex == -1)
            {
                return;
            }
            homeWindow.stkStockReceipt.Children.Clear();
            string[] tmp = homeWindow.cboSelectYearStR.SelectedValue.ToString().Split(' ');
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
                StockReceiptControl.txbTotal.Text = string.Format("{0:N0}", long.Parse(dataTable.Rows[i].ItemArray[3].ToString()));
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
        public void LoadStRByMonth(HomeWindow homeWindow)
        {
            if (homeWindow.cboSelectMonthStR.SelectedIndex == -1)
            {
                return;
            }
            homeWindow.stkStockReceipt.Children.Clear();
            string[] tmp = homeWindow.cboSelectMonthStR.SelectedValue.ToString().Split(' ');
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
                StockReceiptControl.txbTotal.Text = string.Format("{0:N0}", long.Parse(dataTable.Rows[i].ItemArray[3].ToString()));
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
        public void LoadStRByDate(HomeWindow homeWindow)
        {
            if (string.IsNullOrEmpty(homeWindow.dpSelectDateStR.Text))
            {
                return;
            }
            homeWindow.stkStockReceipt.Children.Clear();
            string selectedDay = DateTime.Parse(homeWindow.dpSelectDateStR.Text).Day.ToString();
            string selectedMonth = DateTime.Parse(homeWindow.dpSelectDateStR.Text).Month.ToString();
            string selectedYear = DateTime.Parse(homeWindow.dpSelectDateStR.Text).Year.ToString();
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
                StockReceiptControl.txbTotal.Text = string.Format("{0:N0}", long.Parse(dataTable.Rows[i].ItemArray[3].ToString()));
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
        public void UpdateViewModeStR(HomeWindow homeWindow)
        {
            homeWindow.dpSelectDateStR.Visibility = Visibility.Hidden;
            homeWindow.cboSelectMonthStR.Visibility = Visibility.Hidden;
            homeWindow.cboSelectYearStR.Visibility = Visibility.Hidden;

            homeWindow.stkStockReceipt.Children.Clear();

            switch (homeWindow.cboSelectViewModeStR.SelectedIndex)
            {
                case 0:
                    homeWindow.dpSelectDateStR.Visibility = Visibility.Visible;
                    homeWindow.dpSelectDateStR.SelectedDate = DateTime.Now;
                    //LoadBillByDate(homeWindow);
                    break;
                case 1:
                    homeWindow.cboSelectMonthStR.Visibility = Visibility.Visible;
                    itemSourceMonthStR.Clear();
                    int currentMonth = DateTime.Now.Month;
                    for (int i = 0; i < currentMonth; i++)
                    {
                        itemSourceMonthStR.Add("Tháng " + (i + 1).ToString());
                    }
                    break;
                case 2:
                    homeWindow.cboSelectYearStR.Visibility = Visibility.Visible;
                    itemSourceYearStR.Clear();
                    int currentYear = DateTime.Now.Year;
                    itemSourceYearStR.Add("Năm " + (currentYear - 2).ToString());
                    itemSourceYearStR.Add("Năm " + (currentYear - 1).ToString());
                    itemSourceYearStR.Add("Năm " + (currentYear).ToString());
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
            billTemplate.txbTotal.Text = string.Format("{0:N0}", bill.TotalMoney);
            billTemplate.txbEmployeeName.Text = billControl.txbEmployeeName.Text;
            //Thông tin khách hàng
            FieldInfo fieldInfo = FieldInfoDAL.Instance.GetFieldInfo(bill.IdFieldInfo.ToString());
            billTemplate.txbCustomerName.Text = fieldInfo.CustomerName;
            billTemplate.txbCustomerPhoneNumber.Text = fieldInfo.PhoneNumber;
            billTemplate.txbDiscount.Text = string.Format("{0:N0}", fieldInfo.Discount);
            billTemplate.txbTotalBefore.Text = string.Format("{0:N0}", bill.TotalMoney + fieldInfo.Discount);

            //Load các mặt hàng trong Bill
            List<BillInfo> listBillInfo = BillInfoDAL.Instance.GetBillInfos(idBill);
            int numOfGoods = listBillInfo.Count();
            if (numOfGoods > 7)
            {
                billTemplate.Height += (numOfGoods - 7) * 35;
            }
            int i = 1;

            BillInfoControl fieldBillInfoControl = new BillInfoControl();
            //Thêm sân vào nha
            fieldBillInfoControl.txbOrderNum.Text = i.ToString();
            i++;
            string idField = fieldInfo.IdField.ToString();
            FootballField field = FootballFieldDAL.Instance.GetFootballFieldById(idField);
            string note = fieldInfo.StartingTime.ToString("HH:mm") + " - " + fieldInfo.EndingTime.ToString("HH:mm");
            fieldBillInfoControl.txbName.Text = string.Format("{0} ({1})", field.Name, note);
            fieldBillInfoControl.txbUnit.Text = "lần";
            fieldBillInfoControl.txbQuantity.Text = "1";
            string str = FieldInfoDAL.Instance.GetPriceByFieldInfoId(fieldInfo.IdFieldInfo.ToString()).ToString();
            fieldBillInfoControl.txbUnitPrice.Text = string.Format("{0:N0}", long.Parse(str));
            fieldBillInfoControl.txbTotal.Text = string.Format("{0:N0}", long.Parse(str));

            billTemplate.stkBillInfo.Children.Add(fieldBillInfoControl);
            foreach (var billInfo in listBillInfo)
            {
                BillInfoControl billInfoControl = new BillInfoControl();
                Goods goods = GoodsDAL.Instance.GetGoods(billInfo.IdGoods.ToString());
                billInfoControl.txbOrderNum.Text = i.ToString();
                billInfoControl.txbName.Text = goods.Name;
                billInfoControl.txbUnitPrice.Text = string.Format("{0:N0}", goods.UnitPrice);
                billInfoControl.txbQuantity.Text = billInfo.Quantity.ToString();
                billInfoControl.txbUnit.Text = goods.Unit;
                billInfoControl.txbTotal.Text = string.Format("{0:N0}", goods.UnitPrice * billInfo.Quantity);

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
                billControl.txbTotal.Text = string.Format("{0:N0}", long.Parse(dataTable.Rows[i].ItemArray[4].ToString()));
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
                billControl.txbTotal.Text = string.Format("{0:N0}", long.Parse(dataTable.Rows[i].ItemArray[4].ToString()));
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
                billControl.txbTotal.Text = string.Format("{0:N0}", long.Parse(dataTable.Rows[i].ItemArray[4].ToString()));
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
            CustomMessageBox.Show("[COMMAND] you clicked " + p.X + ", " + p.Y);
        }
        public void LoadDefaultChart(HomeWindow homeWindow)
        {
            string currentDay = DateTime.Now.Day.ToString();
            string currentMonth = DateTime.Now.Month.ToString();
            string lastMonth = (int.Parse(currentMonth) - 1).ToString();
            string currentYear = DateTime.Now.Year.ToString();
            ThisMonth = DateTime.Now.ToString("MM/yyyy");
            NumOfHiredField = ReportDAL.Instance.QueryRevenueNumOfHiredFieldInMonth(currentMonth, currentYear).ToString() + " lượt";
            ThisMonthRevenue = string.Format("{0:N0}", ReportDAL.Instance.QueryRevenueInMonth(currentMonth, currentYear)) + " đồng";
            try
            {
                double res = 0;
                if (currentMonth != "1")
                {
                    res = ((double)(ReportDAL.Instance.QueryRevenueInMonth(currentMonth, currentYear)) / (double)(ReportDAL.Instance.QueryRevenueInMonth(lastMonth, currentYear))) * 100;
                }
                else
                {
                    res = ((double)(ReportDAL.Instance.QueryRevenueInMonth("1", currentYear)) / (double)(ReportDAL.Instance.QueryRevenueInMonth("12", (int.Parse(currentYear) - 1).ToString()))) * 100;
                }
                IncreasingPercent = Math.Round(res, 2).ToString() + "%";
            }
            catch
            {
                IncreasingPercent = "100%";
            }
            homeWindow.txbRevenueThisMonth.Text = thisMonthRevenue.ToString();
            homeWindow.txbIncreasing.Text = increasingPercent.ToString();
            homeWindow.txbNumOfHiredField.Text = numOfHiredField.ToString();
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            timer.Tick += (s, e) =>
            {
                homeWindow.cboSelectTimePie.SelectedIndex = 0;
                homeWindow.cboSelectPeriod.SelectedIndex = 0;
                homeWindow.cboSelectTime.SelectedIndex = DateTime.Now.Month - 1;

                homeWindow.cboSelectPeriodRp.SelectedIndex = 0;
                homeWindow.cboSelectTimeRp.SelectedIndex = DateTime.Now.Month - 1;

                timer.Stop();
            };
            timer.Start();
        }
        public void InitPieChart(HomeWindow homeWindow)
        {
            labelPoint = chartPoint => string.Format("{0:N0}", chartPoint.Y);
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
                    Formatter = value => string.Format("{0:N0}", value);
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
                    Formatter = value => string.Format("{0:N0}", value);
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
                    Formatter = value => string.Format("{0:N0}", value);
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

        public void RpInitColumnChart(HomeWindow homeWindow)
        {
            if (homeWindow.cboSelectPeriodRp.SelectedIndex == 0) //Theo tháng => 31 ngày
            {
                if (homeWindow.cboSelectTimeRp.SelectedIndex != -1)
                {
                    RpAxisXTitle = "Ngày";
                    string[] tmp = homeWindow.cboSelectTimeRp.SelectedValue.ToString().Split(' ');
                    string selectedMonth = tmp[1];
                    string currentYear = DateTime.Now.Year.ToString();
                    RpSeriesCollection = new SeriesCollection
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
                    RpLabels = ReportDAL.Instance.QueryDayInMonth(selectedMonth, currentYear);
                    RpFormatter = value => string.Format("{0:N0}", value);
                }
            }
            else if (homeWindow.cboSelectPeriodRp.SelectedIndex == 1) //Theo quý => 4 quý
            {
                if (homeWindow.cboSelectTimeRp.SelectedIndex != -1)
                {
                    RpAxisXTitle = "Quý";
                    string[] tmp = homeWindow.cboSelectTimeRp.SelectedValue.ToString().Split(' ');
                    string selectedYear = tmp[1];
                    RpSeriesCollection = new SeriesCollection
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
                    RpLabels = ReportDAL.Instance.QueryQuarterInYear(selectedYear);
                    RpFormatter = value => string.Format("{0:N0}", value);
                }
            }
            else
            {
                if (homeWindow.cboSelectTimeRp.SelectedIndex != -1) //Theo năm => 12 tháng
                {
                    RpAxisXTitle = "Tháng";
                    string[] tmp = homeWindow.cboSelectTimeRp.SelectedValue.ToString().Split(' ');
                    string selectedYear = tmp[1];
                    RpSeriesCollection = new SeriesCollection
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
                    RpLabels = ReportDAL.Instance.QueryMonthInYear(selectedYear);
                    RpFormatter = value => string.Format("{0:N0}", value);
                }
            }
        }
        public void RpUpdateSelectTimeItemSource(HomeWindow homeWindow)
        {
            RpItemSourceTime.Clear();
            if (homeWindow.cboSelectPeriodRp.SelectedIndex == 0) //Theo tháng
            {
                int currentMonth = DateTime.Now.Month;
                for (int i = 0; i < currentMonth; i++)
                {
                    RpItemSourceTime.Add("Tháng " + (i + 1).ToString());
                }
            }
            else
            {
                int currentYear = DateTime.Now.Year;
                RpItemSourceTime.Add("Năm " + (currentYear - 2).ToString());
                RpItemSourceTime.Add("Năm " + (currentYear - 1).ToString());
                RpItemSourceTime.Add("Năm " + (currentYear).ToString());
            }
        }
    }
}

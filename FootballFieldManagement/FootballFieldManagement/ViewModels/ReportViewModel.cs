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
        private string increasingPercent = "0 đồng";
        public string IncreasingPercent { get => increasingPercent; set { increasingPercent = value; OnPropertyChanged(); } }

        private string thisMonthRevenue = "0 đồng";
        public string ThisMonthRevenue { get => thisMonthRevenue; set { thisMonthRevenue = value; OnPropertyChanged(); } }

        private string numOfHiredField;
        public string NumOfHiredField { get => numOfHiredField; set { numOfHiredField = value; OnPropertyChanged(); } }

        private string currentDate;
        public string CurrentDate { get => currentDate; set { currentDate = value; OnPropertyChanged(); } }

        private string currentMonth;
        public string CurrentMonth { get => currentMonth; set { currentMonth = value; OnPropertyChanged(); } }

        //Xem bill
        private ObservableCollection<string> itemSourceMonthBill = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceMonthBill { get => itemSourceMonthBill; set { itemSourceMonthBill = value; OnPropertyChanged(); } }

        private ObservableCollection<string> itemSourceYearBill = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceYearBill { get => itemSourceYearBill; set { itemSourceYearBill = value; OnPropertyChanged(); } }

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
        }
        public ReportViewModel(HomeWindow homeWindow)
        {
            homeWindow.cboSelectTimePie.SelectedIndex = 0;
            homeWindow.cboSelectPeriod.SelectedIndex = 0;
            LoadDefaultChart(homeWindow);
        }

        public void LoadBillByYear(HomeWindow homeWindow)
        {
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
                DateTime invoiceDate = (DateTime)dataTable.Rows[i].ItemArray[2];
                billControl.txbId.Text = dataTable.Rows[i].ItemArray[0].ToString();
                billControl.txbEmployeeName.Text = dataTable.Rows[i].ItemArray[1].ToString();
                billControl.txbInvoiceDate.Text = invoiceDate.ToString("dd/MM/yyyy");
                billControl.txbTime.Text = dataTable.Rows[i].ItemArray[3].ToString();
                billControl.txbTotal.Text = dataTable.Rows[i].ItemArray[4].ToString();

                homeWindow.stkBill.Children.Add(billControl);
                temp++;
            }
        }
        public void LoadBillByMonth(HomeWindow homeWindow)
        {
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
                DateTime invoiceDate = (DateTime)dataTable.Rows[i].ItemArray[2];
                billControl.txbId.Text = dataTable.Rows[i].ItemArray[0].ToString();
                billControl.txbEmployeeName.Text = dataTable.Rows[i].ItemArray[1].ToString();
                billControl.txbInvoiceDate.Text = invoiceDate.ToString("dd/MM/yyyy");
                billControl.txbTime.Text = dataTable.Rows[i].ItemArray[3].ToString();
                billControl.txbTotal.Text = dataTable.Rows[i].ItemArray[4].ToString();

                homeWindow.stkBill.Children.Add(billControl);
                temp++;
            }
        }
        public void LoadBillByDate(HomeWindow homeWindow)
        {
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
                DateTime invoiceDate = (DateTime)dataTable.Rows[i].ItemArray[2];
                billControl.txbId.Text = dataTable.Rows[i].ItemArray[0].ToString();
                billControl.txbEmployeeName.Text = dataTable.Rows[i].ItemArray[1].ToString();
                billControl.txbInvoiceDate.Text = invoiceDate.ToString("dd/MM/yyyy");
                billControl.txbTime.Text = dataTable.Rows[i].ItemArray[3].ToString();
                billControl.txbTotal.Text = dataTable.Rows[i].ItemArray[4].ToString();

                homeWindow.stkBill.Children.Add(billControl);
                temp++;
            }
        }
        public void UpdateViewMode(HomeWindow homeWindow)
        {
            homeWindow.dpSelectDateBill.Visibility = Visibility.Hidden;
            homeWindow.cboSelectMonthBill.Visibility = Visibility.Hidden;
            homeWindow.cboSelectYearBill.Visibility = Visibility.Hidden;

            switch (homeWindow.cboSelectViewMode.SelectedIndex)
            {
                case 0:
                    homeWindow.dpSelectDateBill.Visibility = Visibility.Visible;
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
        public void LoadDefaultChart(HomeWindow parameter)
        {
            string currentDay = DateTime.Now.Day.ToString();
            string currentMonth = DateTime.Now.Month.ToString();
            string lastMonth = (int.Parse(currentMonth) - 1).ToString();
            string currentYear = DateTime.Now.Year.ToString();
            parameter.txbToday.Text = DateTime.Now.ToString("dd/MM/yyyy");
            parameter.txbThisMonth1.Text = parameter.txbThisMonth.Text = DateTime.Now.ToString("MM/yyyy");
            parameter.txbNumOfHiredField.Text = ReportDAL.Instance.QueryRevenueNumOfHiredFieldInMonth(currentMonth, currentYear).ToString() + " lượt";
            parameter.txbThisMonthRevenue.Text = ReportDAL.Instance.QueryRevenueInMonth(currentMonth, currentYear).ToString() + " đồng";
            try
            {
                parameter.txbIncreasingPercent.Text = (Math.Round((ReportDAL.Instance.QueryRevenueInMonth(currentMonth, currentYear) / ReportDAL.Instance.QueryRevenueInMonth(lastMonth, currentYear) * 100), 2)).ToString() + "%";
            }
            catch
            {
                parameter.txbIncreasingPercent.Text = "100%";
            }
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            timer.Tick += (s, e) =>
            {
                parameter.cboSelectTimePie.SelectedIndex = 0;
                parameter.cboSelectPeriod.SelectedIndex = 0;
                parameter.cboSelectTime.SelectedIndex = DateTime.Now.Month - 1;

                parameter.cboSelectPeriod_Report.SelectedIndex = 0;
                parameter.cboSelectTime_Report.SelectedIndex = DateTime.Now.Month - 1;

                timer.Stop();
            };
            timer.Start();
        }
        public void InitPieChart(HomeWindow parameter)
        {
            labelPoint = chartPoint => string.Format("{0}", chartPoint.Y);
            if (parameter.cboSelectTimePie.SelectedIndex == 0)
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
        public void InitColumnChart(HomeWindow parameter)
        {
            if (parameter.cboSelectPeriod.SelectedIndex == 0) //Theo tháng => 31 ngày
            {
                if (parameter.cboSelectTime.SelectedIndex != -1)
                {
                    AxisXTitle = "Ngày";
                    string[] tmp = parameter.cboSelectTime.SelectedValue.ToString().Split(' ');
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
            else if (parameter.cboSelectPeriod.SelectedIndex == 1) //Theo quý => 4 quý
            {
                if (parameter.cboSelectTime.SelectedIndex != -1)
                {
                    AxisXTitle = "Quý";
                    string[] tmp = parameter.cboSelectTime.SelectedValue.ToString().Split(' ');
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
                if (parameter.cboSelectTime.SelectedIndex != -1) //Theo năm => 12 tháng
                {
                    AxisXTitle = "Tháng";
                    string[] tmp = parameter.cboSelectTime.SelectedValue.ToString().Split(' ');
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
        public void UpdateSelectTimeItemSource(HomeWindow parameter)
        {
            ItemSourceTime.Clear();
            if (parameter.cboSelectPeriod.SelectedIndex == 0) //Theo tháng
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

        public void Report_InitColumnChart(HomeWindow parameter)
        {
            if (parameter.cboSelectPeriod_Report.SelectedIndex == 0) //Theo tháng => 31 ngày
            {
                if (parameter.cboSelectTime_Report.SelectedIndex != -1)
                {
                    report_AxisXTitle = "Ngày";
                    string[] tmp = parameter.cboSelectTime_Report.SelectedValue.ToString().Split(' ');
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
            else if (parameter.cboSelectPeriod_Report.SelectedIndex == 1) //Theo quý => 4 quý
            {
                if (parameter.cboSelectTime_Report.SelectedIndex != -1)
                {
                    report_AxisXTitle = "Quý";
                    string[] tmp = parameter.cboSelectTime_Report.SelectedValue.ToString().Split(' ');
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
                if (parameter.cboSelectTime_Report.SelectedIndex != -1) //Theo năm => 12 tháng
                {
                    report_AxisXTitle = "Tháng";
                    string[] tmp = parameter.cboSelectTime_Report.SelectedValue.ToString().Split(' ');
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
        public void Report_UpdateSelectTimeItemSource(HomeWindow parameter)
        {
            report_ItemSourceTime.Clear();
            if (parameter.cboSelectPeriod_Report.SelectedIndex == 0) //Theo tháng
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

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
        public ObservableCollection<string> report_ItemSourceTime { get => itemSourceTime; set { itemSourceTime = value; OnPropertyChanged(); } }

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
        private string todayRevenue = "0 đồng";
        public string TodayRevenue { get => todayRevenue; set => todayRevenue = value; }

        private string thisMonthRevenue = "0 đồng";
        public string ThisMonthRevenue { get => thisMonthRevenue; set => thisMonthRevenue = value; }

        private string numOfHiredField;
        public string NumOfHiredField { get => numOfHiredField; set => numOfHiredField = value; }

        private string currentDate;
        public string CurrentDate { get => currentDate; set => currentDate = value; }

        private string currentMonth;
        public string CurrentMonth { get => currentMonth; set => currentMonth = value; }

        public ICommand SelectionChangedCommand { get; set; }
        public ICommand InitColumnChartCommand { get; set; }
        public ICommand DataClickColumnChartCommand { get; set; }
        public ICommand InitPieChartCommand { get; set; }
        public ICommand InitDashboardCommand { get; set; }
        public ICommand LoadCommand { get; set; }

        public ICommand Report_SelectionChangedCommand { get; set; }
        public ICommand Report_InitColumnChartCommand { get; set; }

        public ReportViewModel()
        {
            InitDashboard();
            SelectionChangedCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => UpdateSelectTimeItemSource(parameter));
            InitColumnChartCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => InitColumnChart(parameter));
            InitPieChartCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => InitPieChart(parameter));
            DataClickColumnChartCommand = new RelayCommand<ChartPoint>(parameter => true, parameter => DataClick(parameter));
            LoadCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadDefaultChart(parameter));

            Report_SelectionChangedCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => Report_UpdateSelectTimeItemSource(parameter));
            Report_InitColumnChartCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => Report_InitColumnChart(parameter));
        }
        public void LoadDefaultChart(HomeWindow parameter)
        {
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
        public void InitDashboard()
        {
            string currentDay = DateTime.Now.Day.ToString();
            string currentMonth = DateTime.Now.Month.ToString();
            string currentYear = DateTime.Now.Year.ToString();
            CurrentDate = DateTime.Now.ToString("dd/MM/yyyy");
            CurrentMonth = DateTime.Now.ToString("MM/yyyy");
            NumOfHiredField = ReportDAL.Instance.QueryRevenueNumOfHiredFieldInMonth(currentMonth, currentYear);
            TodayRevenue = ReportDAL.Instance.QueryRevenueInDay(currentDay, currentMonth, currentYear);
            ThisMonthRevenue = ReportDAL.Instance.QueryRevenueInMonth(currentMonth, currentYear);
        }
        public void DataClick(ChartPoint p)
        {
            MessageBox.Show("[COMMAND] you clicked " + p.X + ", " + p.Y);
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

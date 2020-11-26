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

        public ReportViewModel()
        {
            InitDashboard();
            SelectionChangedCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => UpdateSelectTImeItemSource(parameter));
            InitColumnChartCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => InitColumnChart(parameter));
            InitPieChartCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => InitPieChart(parameter));
            DataClickColumnChartCommand = new RelayCommand<ChartPoint>(parameter => true, parameter => DataClick(parameter));
            LoadCommand = new RelayCommand<HomeWindow>(parameter => true, parameter => LoadDefaultChart(parameter));
        }
        public void LoadDefaultChart(HomeWindow parameter)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += (s, e) =>
            {
                parameter.cboSelectTimePie.SelectedIndex = 0;
                parameter.cboSelectPeriod.SelectedIndex = 0;
                parameter.cboSelectTime.SelectedIndex = DateTime.Now.Month - 1;
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
        public void UpdateSelectTImeItemSource(HomeWindow parameter)
        {
            itemSourceTime.Clear();
            if (parameter.cboSelectPeriod.SelectedIndex == 0) //Theo tháng
            {
                int currentMonth = DateTime.Now.Month;
                for (int i = 0; i < currentMonth; i++)
                {
                    itemSourceTime.Add("Tháng " + (i + 1).ToString());
                }
            }
            else
            {
                int currentYear = DateTime.Now.Year;
                itemSourceTime.Add("Năm " + (currentYear - 2).ToString());
                itemSourceTime.Add("Năm " + (currentYear - 1).ToString());
                itemSourceTime.Add("Năm " + (currentYear).ToString());
            }
        }
    }
}

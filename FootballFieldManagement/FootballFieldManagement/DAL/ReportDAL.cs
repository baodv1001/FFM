using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballFieldManagement.Models;
using LiveCharts;

namespace FootballFieldManagement.DAL
{
    class ReportDAL : DataProvider
    {
        private static ReportDAL instance;

        public static ReportDAL Instance
        {
            get { if (instance == null) instance = new ReportDAL(); return ReportDAL.instance; }
            private set { ReportDAL.instance = value; }
        }
        private ReportDAL()
        {

        }
        //Column chart
        public string[] QueryDayInMonth(string month, string year)
        {
            List<string> res = new List<string>();
            try
            {
                conn.Open();
                string queryString = string.Format("select day(invoiceDate) as day from Bill " +
                    "where month(invoiceDate) = {0} and year(invoiceDate) = {1} group by day(invoiceDate) " +
                    "union select day(dateTimeStockReceipt) as day from StockReceipt where month(dateTimeStockReceipt) =  {0} " +
                    "and year(dateTimeStockReceipt) = {1} group by day(dateTimeStockReceipt)", month, year);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    res.Add(rdr["day"].ToString());
                }
                return res.ToArray();
            }
            catch
            {
                return res.ToArray();
            }
            finally
            {
                conn.Close();
            }
        }
        public string[] QueryMonthInYear(string year)
        {
            List<string> res = new List<string>();
            try
            {
                conn.Open();
                string queryString = string.Format("select month(invoiceDate) as month from Bill where year(invoiceDate) = {0} " +
                    "group by month(invoiceDate) union select month(dateTimeStockReceipt) as month from StockReceipt " +
                    "where year(dateTimeStockReceipt) = {0} group by month(dateTimeStockReceipt)", year);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    res.Add(rdr["month"].ToString());
                }
                return res.ToArray();
            }
            catch
            {
                return res.ToArray();
            }
            finally
            {
                conn.Close();
            }
        }
        public string[] QueryQuarterInYear(string year)
        {
            List<string> res = new List<string>();
            try
            {
                conn.Open();
                string queryString = string.Format("select datepart(quarter, invoiceDate) as quarter from Bill where year(invoiceDate) = {0} " +
                    "group by datepart(quarter, invoiceDate) union select datepart(quarter, dateTimeStockReceipt) as quarter " +
                    "from StockReceipt where year(dateTimeStockReceipt) = {0} group by datepart(quarter, dateTimeStockReceipt)", year);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    res.Add(rdr["quarter"].ToString());
                }
                return res.ToArray();
            }
            catch
            {
                return res.ToArray();
            }
            finally
            {
                conn.Close();
            }
        }

        public ChartValues<long> QueryRevenueByMonth(string month, string year)
        {
            ChartValues<long> res = new ChartValues<long>();
            try
            {
                string[] daysOfMonth = ReportDAL.Instance.QueryDayInMonth(month, year);

                conn.Open();
                string queryString = string.Format("select day(invoiceDate), sum(totalMoney) from Bill where month(invoiceDate) = {0} " +
                    "and year(invoiceDate) = {1} group by day(invoiceDate)", month, year);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                long[] revenue = new long[daysOfMonth.Length];
                int j = 0;
                int numOfRows = dataTable.Rows.Count;
                for (int i = 0; i < daysOfMonth.Length && j < numOfRows; i++)
                {
                    if (daysOfMonth[i] == dataTable.Rows[j].ItemArray[0].ToString())
                    {
                        revenue[i] = long.Parse(dataTable.Rows[j].ItemArray[1].ToString());
                        j++;
                    }
                }
                res = new ChartValues<long>(revenue);
                return res;
            }
            catch
            {
                return res;
            }
            finally
            {
                conn.Close();
            }
        }
        public ChartValues<long> QueryRevenueByYear(string year)
        {
            ChartValues<long> res = new ChartValues<long>();
            try
            {
                string[] monthsOfYear = ReportDAL.Instance.QueryMonthInYear(year);

                conn.Open();
                string queryString = string.Format("select month(invoiceDate), sum(totalMoney) from Bill where year(invoiceDate) = {0} " +
                    "group by month(invoiceDate)", year);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                long[] revenue = new long[monthsOfYear.Length];
                int j = 0;
                int numOfRows = dataTable.Rows.Count;

                for (int i = 0; i < monthsOfYear.Length && j < numOfRows; i++)
                {
                    if (monthsOfYear[i] == dataTable.Rows[j].ItemArray[0].ToString())
                    {
                        revenue[i] = long.Parse(dataTable.Rows[j].ItemArray[1].ToString());
                        j++;
                    }
                }
                res = new ChartValues<long>(revenue);
                return res;
            }
            catch
            {
                return res;
            }
            finally
            {
                conn.Close();
            }
        }
        public ChartValues<long> QueryRevenueByQuarter(string year)
        {
            ChartValues<long> res = new ChartValues<long>();
            try
            {
                string[] quartersOfYear = ReportDAL.Instance.QueryQuarterInYear(year);

                conn.Open();
                string queryString = string.Format("select datepart(quarter, invoiceDate), sum(totalMoney) from Bill " +
                    "where year(invoiceDate) = {0} group by datepart(quarter, invoiceDate)", year);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                long[] revenue = new long[quartersOfYear.Length];
                int j = 0;
                int numOfRows = dataTable.Rows.Count;

                for (int i = 0; i < quartersOfYear.Length && j < numOfRows; i++)
                {
                    if (quartersOfYear[i] == dataTable.Rows[j].ItemArray[0].ToString())
                    {
                        revenue[i] = long.Parse(dataTable.Rows[j].ItemArray[1].ToString());
                        j++;
                    }
                }
                res = new ChartValues<long>(revenue);
                return res;
            }
            catch
            {
                return res;
            }
            finally
            {
                conn.Close();
            }
        }

        public ChartValues<long> QueryOutcomeByMonth(string month, string year)
        {
            ChartValues<long> res = new ChartValues<long>();
            try
            {
                string[] daysOfMonth = ReportDAL.Instance.QueryDayInMonth(month, year);

                conn.Open();
                string queryString = string.Format("select date, sum(total) from(select day(dateTimeStockReceipt) as date, " +
                    "sum(total) as total from StockReceipt where month(dateTimeStockReceipt) = {0} and year(dateTimeStockReceipt) = {1} " +
                    "group by day(dateTimeStockReceipt) union select day(salaryRecordDate) as date, sum(total) as total " +
                    "from SalaryRecord where month(salaryRecordDate) = {0} and year(salaryRecordDate) = {1} " +
                    "group by day(salaryRecordDate)) Expediture group by date", month, year);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                long[] revenue = new long[daysOfMonth.Length];
                int j = 0;
                int numOfRows = dataTable.Rows.Count;

                for (int i = 0; i < daysOfMonth.Length && j < numOfRows; i++)
                {
                    if (daysOfMonth[i] == dataTable.Rows[j].ItemArray[0].ToString())
                    {
                        revenue[i] = long.Parse(dataTable.Rows[j].ItemArray[1].ToString());
                        j++;
                    }
                }
                res = new ChartValues<long>(revenue);
                return res;
            }
            catch
            {
                return res;
            }
            finally
            {
                conn.Close();
            }
        }
        public ChartValues<long> QueryOutcomeByYear(string year)
        {
            ChartValues<long> res = new ChartValues<long>();
            try
            {
                string[] monthsOfYear = ReportDAL.Instance.QueryMonthInYear(year);

                conn.Open();
                string queryString = string.Format("select month, sum(total) from(select month(dateTimeStockReceipt) as month, " +
                    "sum(total) as total from StockReceipt where year(dateTimeStockReceipt) = {0} group by month(dateTimeStockReceipt) " +
                    "union select month(salaryRecordDate) as month, sum(total) as total from SalaryRecord where year(salaryRecordDate) = {0} " +
                    "group by month(salaryRecordDate)) Expenditure group by month", year);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                long[] revenue = new long[monthsOfYear.Length];
                int j = 0;
                int numOfRows = dataTable.Rows.Count;

                for (int i = 0; i < monthsOfYear.Length && j < numOfRows; i++)
                {
                    if (monthsOfYear[i] == dataTable.Rows[j].ItemArray[0].ToString())
                    {
                        revenue[i] = long.Parse(dataTable.Rows[j].ItemArray[1].ToString());
                        j++;
                    }
                }
                res = new ChartValues<long>(revenue);
                return res;
            }
            catch
            {
                return res;
            }
            finally
            {
                conn.Close();
            }
        }
        public ChartValues<long> QueryOutcomeByQuarter(string year)
        {
            ChartValues<long> res = new ChartValues<long>();
            try
            {
                string[] quartersOfYear = ReportDAL.Instance.QueryQuarterInYear(year);

                conn.Open();
                string queryString = string.Format("select quarter, sum(total) from (select datepart(quarter, dateTimeStockReceipt) as quarter, " +
                    "sum(total) as total from StockReceipt where year(dateTimeStockReceipt) = {0} group by datepart(quarter, dateTimeStockReceipt) " +
                    "union select datepart(quarter, salaryRecordDate) as quarter, sum(total) as total from SalaryRecord where year(salaryRecordDate) = {0} " +
                    "group by datepart(quarter, salaryRecordDate)) Expenditure group by quarter", year);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                long[] revenue = new long[quartersOfYear.Length];
                int j = 0;
                int numOfRows = dataTable.Rows.Count;

                for (int i = 0; i < quartersOfYear.Length && j < numOfRows; i++)
                {
                    if (quartersOfYear[i] == dataTable.Rows[j].ItemArray[0].ToString())
                    {
                        revenue[i] = long.Parse(dataTable.Rows[j].ItemArray[1].ToString());
                        j++;
                    }
                }
                res = new ChartValues<long>(revenue);
                return res;
            }
            catch
            {
                return res;
            }
            finally
            {
                conn.Close();
            }
        }

        //Pie chart
        public ChartValues<long> QueryRevenueFromSellingInDay(string day, string month, string year)
        {
            ChartValues<long> res = new ChartValues<long>();
            try
            {
                conn.Open();
                string queryString = string.Format("select sum((BillInfo.quantity * Goods.unitPrice)) as revenue from BillInfo " +
                    "join Goods on BillInfo.idGoods = Goods.idGoods join Bill on Bill.idBill = BillInfo.idBill " +
                    "where year(invoiceDate) = {0} and month(invoiceDate) = {1} and day(invoiceDate) = {2}", year, month, day);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    res.Add(long.Parse(rdr["revenue"].ToString()));
                }
                return res;
            }
            catch
            {
                res.Add(0);
                return res;
            }
            finally
            {
                conn.Close();
            }
        }
        public ChartValues<long> QueryRevenueFromFieldInDay(string day, string month, string year)
        {
            ChartValues<long> res = new ChartValues<long>();
            try
            {
                long totalFromGoods = ReportDAL.instance.QueryRevenueFromSellingInDay(day, month, year)[0];
                conn.Open();
                string queryString = string.Format("select sum(totalMoney) from bill where year(invoiceDate) = {0} " +
                    "and month(invoiceDate) = {1} and day(invoiceDate) = {2}", year, month, day);
                SqlCommand command = new SqlCommand(queryString, conn);
                long total = 0;
                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    total = long.Parse(rdr["revenue"].ToString());
                }
                res.Add(total - totalFromGoods);
                return res;
            }
            catch
            {
                res.Add(0);
                return res;
            }
            finally
            {
                conn.Close();
            }
        }

        public ChartValues<long> QueryRevenueFromSellingInMonth(string month, string year)
        {
            ChartValues<long> res = new ChartValues<long>();
            try
            {
                conn.Open();
                string queryString = string.Format("select sum((BillInfo.quantity * Goods.unitPrice)) as revenue from BillInfo " +
                    "join Goods on BillInfo.idGoods = Goods.idGoods join Bill on Bill.idBill = BillInfo.idBill " +
                    "where year(invoiceDate) = {0} and month(invoiceDate) = {1}", year, month);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    res.Add(long.Parse(rdr["revenue"].ToString()));
                }
                return res;
            }
            catch
            {
                res.Add(0);
                return res;
            }
            finally
            {
                conn.Close();
            }
        }
        public ChartValues<long> QueryRevenueFromFieldInMonth(string month, string year)
        {
            ChartValues<long> res = new ChartValues<long>();
            try
            {
                long totalFromGoods = ReportDAL.instance.QueryRevenueFromSellingInMonth(month, year)[0];
                conn.Open();
                string queryString = string.Format("select sum(totalMoney) from bill where year(invoiceDate) = {0} " +
                    "and month(invoiceDate) = {1}", year, month);
                SqlCommand command = new SqlCommand(queryString, conn);
                long total = 0;
                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    total = long.Parse(rdr["revenue"].ToString());
                }
                res.Add(total - totalFromGoods);
                return res;
            }
            catch
            {
                res.Add(0);
                return res;
            }
            finally
            {
                conn.Close();
            }
        }

        //Dashboard
        public string QueryRevenueInDay(string day, string month, string year)
        {
            string res = "0";
            try
            {
                conn.Open();
                string queryString = string.Format("select sum(totalMoney) as revenue from Bill where year(invoiceDate) = {0} " +
                    "and month(invoiceDate) = {1} and day(invoiceDate) = {2}", year, month, day);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    res = rdr["revenue"].ToString();
                    if (string.IsNullOrEmpty(res))
                    {
                        return "0 đồng";
                    }
                }
                return res + " đồng";
            }
            catch
            {
                return res + " đồng";
            }
            finally
            {
                conn.Close();
            }
        }
        public double QueryRevenueInMonth(string month, string year)
        {
            double res = 0;
            try
            {
                conn.Open();
                string queryString = string.Format("select sum(totalMoney) as revenue from Bill " +
                    "where year(invoiceDate) = {0} and month(invoiceDate) = {1}", year, month);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    res = double.Parse(rdr["revenue"].ToString());
                }
                return res;
            }
            catch
            {
                return res;
            }
            finally
            {
                conn.Close();
            }
        }
        public double QueryRevenueNumOfHiredFieldInMonth(string month, string year)
        {
            double res = 0;
            try
            {
                conn.Open();
                string queryString = string.Format("select count(idFieldInfo) as numOfHiredField from Bill " +
                    "where year(invoiceDate) = {0} and month(invoiceDate) = {1}", year, month);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    res = double.Parse(rdr["numOfHiredField"].ToString());
                }
                return res;
            }
            catch
            {
                return res;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}

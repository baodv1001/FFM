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
                string queryString = "select day(invoiceDate) as day from Bill where month(invoiceDate) = " + month
                    + " and year(invoiceDate) = " + year + " group by day(invoiceDate) union " +
                    "select day(dateTimeStockReceipt) as day from StockReceipt where month(dateTimeStockReceipt) = " + month
                    + " and year(dateTimeStockReceipt) = " + year + " group by day(dateTimeStockReceipt)";
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
                string queryString = "select month(invoiceDate) as month from Bill where year(invoiceDate) = "
                    + year + " group by month(invoiceDate) union " +
                    "select month(dateTimeStockReceipt) as month from StockReceipt where year(dateTimeStockReceipt) = "
                    + year + " group by month(dateTimeStockReceipt)";
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
                string queryString = "select datepart(quarter, invoiceDate) as quarter from Bill where year(invoiceDate) = "
                    + year + " group by datepart(quarter, invoiceDate) union select datepart(quarter, dateTimeStockReceipt) as quarter " +
                    "from StockReceipt where year(dateTimeStockReceipt) = " + year + " group by datepart(quarter, dateTimeStockReceipt)";
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
                string queryString = "select day(invoiceDate), sum(totalMoney) from Bill where month(invoiceDate) = "
                    + month + " and year(invoiceDate) = " + year + " group by day(invoiceDate)";
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                long[] revenue = new long[dataTable.Rows.Count];
                int j = 0;
                int numOfRows = dataTable.Rows.Count;
                for (int i = 0; i < daysOfMonth.Length && j < numOfRows; i++)
                {
                    if (daysOfMonth[i] == dataTable.Rows[j].ItemArray[0].ToString())
                    {
                        revenue[i] = long.Parse(dataTable.Rows[i].ItemArray[1].ToString());
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
                string queryString = "select month(invoiceDate), sum(totalMoney) from Bill where year(invoiceDate) = "
                    + year + " group by month(invoiceDate)";
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                long[] revenue = new long[dataTable.Rows.Count];
                int j = 0;
                int numOfRows = dataTable.Rows.Count;

                for (int i = 0; i < monthsOfYear.Length && j < numOfRows; i++)
                {
                    if (monthsOfYear[i] == dataTable.Rows[j].ItemArray[0].ToString())
                    {
                        revenue[i] = long.Parse(dataTable.Rows[i].ItemArray[1].ToString());
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
                string queryString = "select datepart(quarter, invoiceDate), sum(totalMoney) from Bill where year(invoiceDate) = "
                    + year + " group by datepart(quarter, invoiceDate)";
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                long[] revenue = new long[dataTable.Rows.Count];
                int j = 0;
                int numOfRows = dataTable.Rows.Count;

                for (int i = 0; i < quartersOfYear.Length && j < numOfRows; i++)
                {
                    if (quartersOfYear[i] == dataTable.Rows[j].ItemArray[0].ToString())
                    {
                        revenue[i] = long.Parse(dataTable.Rows[i].ItemArray[1].ToString());
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
                string queryString = "select day(dateTimeStockReceipt), sum(total) from StockReceipt where month(dateTimeStockReceipt) = " + month
                    + "and year(dateTimeStockReceipt) = " + year + " group by day(dateTimeStockReceipt)";
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                long[] revenue = new long[dataTable.Rows.Count];
                int j = 0;
                int numOfRows = dataTable.Rows.Count;

                for (int i = 0; i < daysOfMonth.Length && j < numOfRows; i++)
                {
                    if (daysOfMonth[i] == dataTable.Rows[j].ItemArray[0].ToString())
                    {
                        revenue[i] = long.Parse(dataTable.Rows[i].ItemArray[1].ToString());
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
                string queryString = "select month(dateTimeStockReceipt), sum(total) from StockReceipt where year(dateTimeStockReceipt) = "
                    + year + " group by month(dateTimeStockReceipt)";
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                long[] revenue = new long[dataTable.Rows.Count];
                int j = 0;
                int numOfRows = dataTable.Rows.Count;

                for (int i = 0; i < monthsOfYear.Length && j < numOfRows; i++)
                {
                    if (monthsOfYear[i] == dataTable.Rows[j].ItemArray[0].ToString())
                    {
                        revenue[i] = long.Parse(dataTable.Rows[i].ItemArray[1].ToString());
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
                string queryString = "select datepart(quarter, dateTimeStockReceipt), sum(total) from StockReceipt where year(dateTimeStockReceipt) = "
                    + year + " group by datepart(quarter, dateTimeStockReceipt)";
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                long[] revenue = new long[dataTable.Rows.Count];
                int j = 0;
                int numOfRows = dataTable.Rows.Count;

                for (int i = 0; i < quartersOfYear.Length && j < numOfRows; i++)
                {
                    if (quartersOfYear[i] == dataTable.Rows[j].ItemArray[0].ToString())
                    {
                        revenue[i] = long.Parse(dataTable.Rows[i].ItemArray[1].ToString());
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
                string queryString = "select sum((BillInfo.quantity * Goods.unitPrice)) as revenue from BillInfo " +
                    "join Goods on BillInfo.idGoods = Goods.idGoods join Bill on Bill.idBill = BillInfo.idBill where year(invoiceDate) = "
                    + year + " and month(invoiceDate) = " + month + " and day(invoiceDate) = " + day;
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
                conn.Open();
                string queryString = "select sum(price - discount) as revenue from FootballField join FieldInfo on FootballField.idField = FieldInfo.idField " +
                    "join Bill on Bill.idFieldInfo = FieldInfo.idFieldInfo where year(invoiceDate) = "
                    + year + " and month(invoiceDate) = " + month + " and day(invoiceDate) = " + day;
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
                string queryString = "select sum((BillInfo.quantity * Goods.unitPrice)) as revenue from BillInfo " +
                    "join Goods on BillInfo.idGoods = Goods.idGoods join Bill on Bill.idBill = BillInfo.idBill where year(invoiceDate) = "
                    + year + " and month(invoiceDate) = " + month;
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
                conn.Open();
                string queryString = "select sum(price - discount) as revenue from FootballField join FieldInfo on FootballField.idField = FieldInfo.idField " +
                    "join Bill on Bill.idFieldInfo = FieldInfo.idFieldInfo where year(invoiceDate) = "
                    + year + " and month(invoiceDate) = " + month;
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
                string queryString = "select sum(totalMoney) as revenue from Bill where year(invoiceDate) = "
                    + year + " and month(invoiceDate) = " + month + " and day(invoiceDate) = " + day;
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
                return res;
            }
            finally
            {
                conn.Close();
            }
        }
        public string QueryRevenueInMonth(string month, string year)
        {
            string res = "0";
            try
            {
                conn.Open();
                string queryString = "select sum(totalMoney) as revenue from Bill where year(invoiceDate) = "
                    + year + " and month(invoiceDate) = " + month;
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
                return res;
            }
            finally
            {
                conn.Close();
            }
        }
        public string QueryRevenueNumOfHiredFieldInMonth(string month, string year)
        {
            string res = "0";
            try
            {
                conn.Open();
                string queryString = "select count(idFieldInfo) as numOfHiredField from Bill where year(invoiceDate) = "
                    + year + " and month(invoiceDate) = " + month;
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    res = rdr["numOfHiredField"].ToString();
                    if (string.IsNullOrEmpty(res))
                    {
                        return "0 lượt";
                    }
                }
                return res + " lượt";
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

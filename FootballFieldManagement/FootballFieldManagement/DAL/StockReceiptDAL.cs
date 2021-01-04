using FootballFieldManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FootballFieldManagement.DAL
{
    class StockReceiptDAL : DataProvider
    {
        private static StockReceiptDAL instance;

        public static StockReceiptDAL Instance
        {
            get { if (instance == null) instance = new StockReceiptDAL(); return StockReceiptDAL.instance; }
            private set { StockReceiptDAL.instance = value; }
        }
        private StockReceiptDAL()
        {

        }
        public List<StockReceipt> ConvertDBToList()
        {
            try
            {
                OpenConnection();
                string queryString = "select * from StockReceipt";
                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                List<StockReceipt> stockReceiptList = new List<StockReceipt>();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    int idAccount = -1;
                    if (dataTable.Rows[i].ItemArray[1].ToString() != "")
                    {
                        idAccount = int.Parse(dataTable.Rows[i].ItemArray[1].ToString());
                    }
                    StockReceipt acc = new StockReceipt(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), idAccount,
                        DateTime.Parse(dataTable.Rows[i].ItemArray[2].ToString()), long.Parse(dataTable.Rows[i].ItemArray[3].ToString()));
                    stockReceiptList.Add(acc);
                }
                return stockReceiptList;
            }
            catch
            {
                return new List<StockReceipt>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool AddIntoDB(StockReceipt stockReceipt)
        {
            try
            {
                OpenConnection();
                string queryString = "insert into StockReceipt(idStockReceipt, idAccount, dateTimeStockReceipt, total) " +
                    "values(@idStockReceipt, @idAccount, @dateTimeStockReceipt, @total)";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idStockReceipt", stockReceipt.IdStockReceipt.ToString());
                command.Parameters.AddWithValue("@idAccount", stockReceipt.IdAccount.ToString());
                command.Parameters.AddWithValue("@dateTimeStockReceipt", stockReceipt.DateTimeStockReceipt);
                command.Parameters.AddWithValue("@total", stockReceipt.Total.ToString());

                int rs = command.ExecuteNonQuery();
                if (rs != 1)
                {
                    throw new Exception();
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool UpdateOnDB(StockReceipt stockReceipt)
        {
            try
            {
                OpenConnection();
                string queryString = "update StockReceipt set dateTimeStockReceipt=@dateTimeStockReceipt, total=@total " +
                    "where idStockReceipt =" + stockReceipt.IdStockReceipt.ToString();
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@dateTimeStockReceipt", stockReceipt.DateTimeStockReceipt);
                command.Parameters.AddWithValue("@total", stockReceipt.Total.ToString());
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool DeleteFromDB(string idStockReceipt)
        {
            try
            {
                OpenConnection();
                string queryString = "delete from StockReceipt where idStockReceipt=" + idStockReceipt;
                SqlCommand command = new SqlCommand(queryString, conn);
                int rs = command.ExecuteNonQuery();
                if (rs < 1)
                {
                    throw new Exception();
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool UpdateIdAccount(string idAccount)
        {
            try
            {
                OpenConnection();
                string queryString = "update StockReceipt set idAccount = NULL where idAccount = " + idAccount;
                SqlCommand command = new SqlCommand(queryString, conn);
                int rs = command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public int GetMaxId()
        {
            int res = 0;
            try
            {
                OpenConnection();
                string queryString = "select max(idStockReceipt) as id from StockReceipt";
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataReader rdr = command.ExecuteReader();
                rdr.Read();
                res = int.Parse(rdr["id"].ToString());
                return res;
            }
            catch
            {
                return res;
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataTable GetStockReceiptByDate(string day, string month, string year)
        {
            DataTable dataTable = new DataTable();
            try
            {
                OpenConnection();
                string queryString = string.Format("select * from StockReceipt " +
                    "where year(dateTimeStockReceipt) = {0} and month(dateTimeStockReceipt) = {1} and day(dateTimeStockReceipt) = {2} order by idStockReceipt", year, month, day);

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);
                return dataTable;
            }
            catch
            {
                return dataTable;
            }
            finally
            {
                CloseConnection();
            }
        }
        public DataTable GetStockReceiptByMonth(string month, string year)
        {
            DataTable dataTable = new DataTable();
            try
            {
                OpenConnection();
                string queryString = string.Format("select * from StockReceipt " +
                    "where year(dateTimeStockReceipt) = {0} and month(dateTimeStockReceipt) = {1} order by idStockReceipt", year, month);

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);
                return dataTable;
            }
            catch
            {
                return dataTable;
            }
            finally
            {
                CloseConnection();
            }
        }
        public DataTable GetStockReceiptByYear(string year)
        {
            DataTable dataTable = new DataTable();
            try
            {
                OpenConnection();
                string queryString = string.Format("select * from StockReceipt " +
                    "where year(dateTimeStockReceipt) = {0} order by idStockReceipt", year);

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);
                return dataTable;
            }
            catch
            {
                return dataTable;
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}

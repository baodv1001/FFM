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
            DataTable dt;
            List<StockReceipt> stockReceiptList = new List<StockReceipt>();
            try
            {
                dt = LoadData("StockReceipt");
            }
            catch
            {
                conn.Close();
                dt = LoadData("StockReceipt");
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int idAccount = -1;
                if (dt.Rows[i].ItemArray[1].ToString() != "")
                {
                    idAccount = int.Parse(dt.Rows[i].ItemArray[1].ToString());
                }
                StockReceipt acc = new StockReceipt(int.Parse(dt.Rows[i].ItemArray[0].ToString()), idAccount,
                    DateTime.Parse(dt.Rows[i].ItemArray[2].ToString()), int.Parse(dt.Rows[i].ItemArray[3].ToString()));
                stockReceiptList.Add(acc);
            }
            return stockReceiptList;
        }
        public bool AddIntoDB(StockReceipt stockReceipt)
        {
            try
            {
                conn.Open();
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
                conn.Close();
            }
        }
        public bool DeleteFromDB(string idStockReceipt)
        {
            try
            {
                conn.Open();
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
                conn.Close();
            }
        }
        public bool UpdateIdAccount(string idAccount)
        {
            try
            {
                conn.Open();
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
                conn.Close();
            }
        }
    }
}

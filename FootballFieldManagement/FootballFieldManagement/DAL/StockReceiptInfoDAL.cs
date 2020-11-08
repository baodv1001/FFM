using FootballFieldManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FootballFieldManagement.DAL
{
    class StockReceiptInfoDAL : DataProvider
    {
        private static StockReceiptInfoDAL instance;

        public static StockReceiptInfoDAL Instance
        {
            get { if (instance == null) instance = new StockReceiptInfoDAL(); return StockReceiptInfoDAL.instance; }
            private set { StockReceiptInfoDAL.instance = value; }
        }
        private StockReceiptInfoDAL()
        {

        }
        public bool AddIntoDB(StockReceiptInfo stockReceiptInfo)
        {
            try
            {
                conn.Open();
                string queryString = "insert into StockReceiptInfo(idStockReceipt, idGoods, quantity, importPrice) " +
                    "values(@idStockReceipt, @idGoods, @quantity, @importPrice )";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idStockReceipt", stockReceiptInfo.IdStockReceipt.ToString());
                command.Parameters.AddWithValue("@idGoods", stockReceiptInfo.IdGoods.ToString());
                command.Parameters.AddWithValue("@quantity", stockReceiptInfo.Quantity.ToString());
                command.Parameters.AddWithValue("@importPrice", stockReceiptInfo.ImportPrice.ToString());

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
        public bool DeleteFromDB(string idGoods)
        {
            try
            {
                conn.Open();
                string queryString = "delete from StockReceiptInfo where idGoods=" + idGoods;
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
        public string QueryIdStockReceipt(string idGoods)
        {
            string res = "";
            try
            {
                conn.Open();
                string queryString = "select idStockReceipt from StockReceiptInfo where idGoods=" + idGoods;
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    res = rdr["idStockReceipt"].ToString();
                }

                int rs = command.ExecuteNonQuery();
                if (rs < 1)
                {
                    throw new Exception();
                }
                else
                {
                    return res;
                }
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

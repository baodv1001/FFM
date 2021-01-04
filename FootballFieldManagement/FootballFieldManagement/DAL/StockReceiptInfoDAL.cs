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
                OpenConnection();
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
                CloseConnection();
            }
        }
        public bool DeleteFromDB(string idGoods)
        {
            try
            {
                OpenConnection();
                string queryString = "delete from StockReceiptInfo where idGoods=" + idGoods;
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
        public bool DeleteByIdStock(string idGoods, string idStockReceipt)
        {
            try
            {
                OpenConnection();
                string queryString = string.Format("delete from StockReceiptInfo where idGoods = {0} and idStockReceipt = {1}", idGoods, idStockReceipt);
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
        public bool DeleteByIdStockReceipt(string idStockReceipt)
        {
            try
            {
                OpenConnection();
                string queryString = "delete from StockReceiptInfo where idStockReceipt = " + idStockReceipt;
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
        public bool UpdateOnDB(StockReceiptInfo stockReceiptInfo)
        {
            try
            {
                OpenConnection();
                string queryString = "update StockReceiptInfo set quantity=@quantity, importPrice=@importPrice " +
                    "where idGoods=@idGoods and idStockReceipt=@idStockReceipt";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idGoods", stockReceiptInfo.IdGoods.ToString());
                command.Parameters.AddWithValue("@idStockReceipt", stockReceiptInfo.IdStockReceipt.ToString());
                command.Parameters.AddWithValue("@quantity", stockReceiptInfo.Quantity.ToString());
                command.Parameters.AddWithValue("@importPrice", stockReceiptInfo.ImportPrice.ToString());
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                CustomMessageBox.Show("Thực hiện thất bại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public List<string> QueryIdStockReceipt(string idGoods)
        {
            List<string> res = new List<string>();
            try
            {
                OpenConnection();
                string queryString = "select idStockReceipt from StockReceiptInfo where idGoods=" + idGoods;
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    res.Add(rdr["idStockReceipt"].ToString());
                }
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
        public long CalculateTotalMoney(string idStockReceipt)
        {
            long res = 0;
            try
            {
                OpenConnection();
                string queryString = string.Format("select sum(importPrice * quantity) as total from StockReceiptInfo " +
                    "where idStockReceipt = {0} group by idStockReceipt", idStockReceipt);
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataReader rdr = command.ExecuteReader();
                rdr.Read();
                res = long.Parse(rdr["total"].ToString());
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
        public List<StockReceiptInfo> GetStockReceiptInfoById(string idStockReceipt)
        {
            List<StockReceiptInfo> listStockReceiptInfo = new List<StockReceiptInfo>();
            try
            {
                OpenConnection();
                string queryString = "select * from StockReceiptInfo where idStockReceipt = " + idStockReceipt;
                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    StockReceiptInfo stockReceiptInfo = new StockReceiptInfo(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()),
                        int.Parse(dataTable.Rows[i].ItemArray[1].ToString()), int.Parse(dataTable.Rows[i].ItemArray[2].ToString()),
                        long.Parse(dataTable.Rows[i].ItemArray[3].ToString()));
                    listStockReceiptInfo.Add(stockReceiptInfo);
                }
                return listStockReceiptInfo;
            }
            catch
            {
                return new List<StockReceiptInfo>();
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}

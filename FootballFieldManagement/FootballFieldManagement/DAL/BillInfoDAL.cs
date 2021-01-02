using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FootballFieldManagement.Models;
using System.Data;
using System.Data.SqlClient;

namespace FootballFieldManagement.DAL
{
    class BillInfoDAL : DataProvider
    {
        private static BillInfoDAL instance;

        public static BillInfoDAL Instance
        {
            get { if (instance == null) instance = new BillInfoDAL(); return BillInfoDAL.instance; }
            private set { BillInfoDAL.instance = value; }
        }
        private BillInfoDAL()
        {

        }
        public void DeleteAllBillInfo(string idBill)
        {
            try
            {
                OpenConnection();
                string queryString = "delete from BillInfo where idBill=" + idBill;
                SqlCommand command = new SqlCommand(queryString, conn);
                command.ExecuteNonQuery();
            }
            catch
            {
                CustomMessageBox.Show("Thực hiện thất bại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool DeleteFromDB(BillInfo billInfo)
        {
            try
            {
                OpenConnection();
                string queryString = "delete from BillInfo where idBill=" + billInfo.IdBill.ToString() + " and idGoods=" + billInfo.IdGoods.ToString();
                SqlCommand command = new SqlCommand(queryString, conn);
                if (command.ExecuteNonQuery() < 1)
                {
                    return false;
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

        //Xóa bill info khi xóa goods
        public bool DeleteIdGoods(string idGoods)
        {
            try
            {
                OpenConnection();
                string queryString = "delete from BillInfo where idGoods=" + idGoods;
                SqlCommand command = new SqlCommand(queryString, conn);
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
        public bool AddIntoDB(BillInfo billInfo)
        {
            try
            {
                OpenConnection();
                string queryString = "insert into BillInfo(idBill, idGoods, quantity) values(@idBill, @idGoods, @quantity)";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idBill", billInfo.IdBill);
                command.Parameters.AddWithValue("@idGoods", billInfo.IdGoods);
                command.Parameters.AddWithValue("@quantity", billInfo.Quantity);
                int rs = command.ExecuteNonQuery();
                if (rs == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                CustomMessageBox.Show("Đã tồn tại mặt hàng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool UpdateOnDB(BillInfo billInfo)
        {
            try
            {
                OpenConnection();
                string queryString = "update BillInfo set quantity=@quantity where idGoods=@idGoods and idBill=@idBill";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idGoods", billInfo.IdGoods.ToString());
                command.Parameters.AddWithValue("@idBill", billInfo.IdBill.ToString());
                command.Parameters.AddWithValue("@quantity", billInfo.Quantity.ToString());
                int rs = command.ExecuteNonQuery();
                if (rs != 1)
                {
                    return false;
                    throw new Exception();
                }
                else
                {
                    return true;
                }

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
        public int CountSumMoney(string idBill) // Tính tổng số tiền 
        {
            OpenConnection();
            int sum = 0;
            DataTable dataTable = new DataTable();
            string queryString = "select BillInfo.quantity,unitPrice from BillInfo Inner join Goods on Goods.idGoods = BillInfo.idGoods where idBill=@idBill ";
            SqlCommand commnad = new SqlCommand(queryString, conn);
            commnad.Parameters.AddWithValue("@idBill", idBill);
            SqlDataAdapter adapter = new SqlDataAdapter(commnad);
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                sum += int.Parse(dataTable.Rows[i].ItemArray[0].ToString()) * int.Parse(dataTable.Rows[i].ItemArray[1].ToString());
            }
            CloseConnection();
            return sum;
        }
        public List<BillInfo> GetBillInfos(string idBill)
        {
            List<BillInfo> billInfos = new List<BillInfo>();
            try
            {
                OpenConnection();
                string queryString = "select * from BillInfo where idBill=" + idBill;
                SqlCommand command = new SqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i].ItemArray[0].ToString() == idBill)
                    {
                        BillInfo billInfo = new BillInfo(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), int.Parse(dataTable.Rows[i].ItemArray[1].ToString()), int.Parse(dataTable.Rows[i].ItemArray[2].ToString()));
                        billInfos.Add(billInfo);
                    }
                }
                return billInfos;
            }
            catch
            {
                return billInfos;
            }
            finally
            {
                CloseConnection();
            }
        }
        public List<BillInfo> ConvertDBToList()
        {
            try
            {
                OpenConnection();
                string queryString = "select * from BillInfo";

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                List<BillInfo> billInfos = new List<BillInfo>();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    BillInfo billInfo = new BillInfo(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), int.Parse(dataTable.Rows[i].ItemArray[1].ToString()),
                        int.Parse(dataTable.Rows[i].ItemArray[2].ToString()));
                    billInfos.Add(billInfo);
                }
                return billInfos;
            }
            catch
            {
                return new List<BillInfo>();
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}

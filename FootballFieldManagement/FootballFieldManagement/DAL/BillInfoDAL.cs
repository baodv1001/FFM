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
                conn.Open();
                string queryString = "delete from BillInfo where idBill=" + idBill;
                SqlCommand command = new SqlCommand(queryString, conn);
                command.ExecuteNonQuery();
            }
            catch
            {
                MessageBox.Show("Thực hiện thất bại");
            }
            finally
            {
                conn.Close();
            }
        }
        public bool DeleteFromDB(BillInfo billInfo)
        {
            try
            {
                conn.Open();
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
                conn.Close();
            }
        }

        //Xóa bill info khi xóa goods
        public bool DeleteIdGoods(string idGoods )
        {
            try
            {
                conn.Open();
                string queryString = "delete from BillInfo where idGoods=" + idGoods;
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
                conn.Close();
            }
        }
        public bool AddIntoDB(BillInfo billInfo)
        {
            try
            {
                conn.Open();
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
                MessageBox.Show("Đã tồn tại mặt hàng");
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public bool UpdateOnDB(BillInfo billInfo)
        {
            try
            {
                conn.Open();
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
                MessageBox.Show("Thực hiện thất bại");
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public int CountSumMoney() // Tính tổng số tiền 
        {
            conn.Open();
            int sum = 0;
            DataTable dataTable = new DataTable();
            string queryString = "select BillInfo.quantity,unitPrice from BillInfo Inner join Goods on Goods.idGoods = BillInfo.idGoods ";
            SqlCommand commnad = new SqlCommand(queryString, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(commnad);
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                sum += int.Parse(dataTable.Rows[i].ItemArray[0].ToString()) * int.Parse(dataTable.Rows[i].ItemArray[1].ToString());
            }
            conn.Close();
            return sum;
        }
        public List<BillInfo> GetBillInfos(string idBill)
        {
            List<BillInfo> billInfos = new List<BillInfo>();
            conn.Open();
            string queryString = "select * from BillInfo where idBill=" + idBill;
            SqlCommand command = new SqlCommand(queryString, conn);
            command.ExecuteNonQuery();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i].ItemArray[0].ToString() == idBill)
                {
                    BillInfo billInfo = new BillInfo(int.Parse(dt.Rows[i].ItemArray[0].ToString()), int.Parse(dt.Rows[i].ItemArray[1].ToString()), int.Parse(dt.Rows[i].ItemArray[2].ToString()));
                    billInfos.Add(billInfo);
                }
            }
            return billInfos;
        }
        public List<BillInfo> ConvertDBToList()
        {
            DataTable dt;
            List<BillInfo> billInfos = new List<BillInfo>();
            try
            {

                dt = LoadData("BillInfo");
            }
            catch
            {
                conn.Close();
                dt = LoadData("BillInfo");
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BillInfo billInfo = new BillInfo(int.Parse(dt.Rows[i].ItemArray[0].ToString()), int.Parse(dt.Rows[i].ItemArray[1].ToString()), int.Parse(dt.Rows[i].ItemArray[2].ToString()));
                billInfos.Add(billInfo);
            }
            conn.Close();
            return billInfos;
        }
    }
}

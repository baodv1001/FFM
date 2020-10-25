using FootballFieldManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FootballFieldManagement.DAL
{
    class GoodsDAL : DataProvider
    {
        private static GoodsDAL instance;

        public static GoodsDAL Instance
        {
            get { if (instance == null) instance = new GoodsDAL(); return GoodsDAL.instance; }
            private set { GoodsDAL.instance = value; }
        }
        private GoodsDAL()
        {

        }
        public List<Goods> ConvertDBToList()
        {
            DataTable dt;
            List<Goods> goodsList = new List<Goods>();
            try
            {
                dt = LoadData("Goods");
            }
            catch
            {
                conn.Close();
                dt = LoadData("Goods");
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Goods acc = new Goods(int.Parse(dt.Rows[i].ItemArray[0].ToString()), dt.Rows[i].ItemArray[1].ToString(), int.Parse(dt.Rows[i].ItemArray[2].ToString()), dt.Rows[i].ItemArray[4].ToString(), double.Parse(dt.Rows[i].ItemArray[3].ToString()));
                goodsList.Add(acc);
            }
            return goodsList;
        }
        public void AddIntoDB(Goods goods)
        {
            try
            {
                conn.Open();
                string queryString = "insert into Goods(idGoods, name, amount, price, unit) values(@idGoods, @name, @amount, @price, @unit)";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idGoods", goods.IdGoods.ToString());
                command.Parameters.AddWithValue("@name", goods.Name);
                command.Parameters.AddWithValue("@amount", goods.Amount.ToString());
                command.Parameters.AddWithValue("@price", goods.Price.ToString());
                command.Parameters.AddWithValue("@unit", goods.Unit);

                int rs = command.ExecuteNonQuery();
                if (rs != 1)
                {
                    throw new Exception();
                }
                else
                {
                    MessageBox.Show("Đã thêm thành công!");
                }
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
        public void ImportToDB(Goods goods)
        {
            try
            {
                conn.Open();
                string queryString = "update Goods set idGoods=@idGoods, name=@name, amount=@amount, price=@price, unit=@unit";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idGoods", goods.IdGoods.ToString());
                command.Parameters.AddWithValue("@name", goods.Name);
                command.Parameters.AddWithValue("@amount", goods.Amount.ToString());
                command.Parameters.AddWithValue("@price", goods.Price.ToString());
                command.Parameters.AddWithValue("@unit", goods.Unit);
                int rs = command.ExecuteNonQuery();
                if (rs != 1)
                {
                    throw new Exception();
                }
                else
                {
                    MessageBox.Show("Đã nhập thành công!");
                }
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
        public void DeleteFromDB(Goods goods)
        {
            try
            {
                conn.Open();
                string queryString = "delete from Goods where idGoods=" + goods.IdGoods.ToString();
                SqlCommand command = new SqlCommand(queryString, conn);
                if (command.ExecuteNonQuery() < 1)
                { 
                    throw new Exception();
                }
                else
                {
                    MessageBox.Show("Đã xóa thành công!");
                }
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
    }
}

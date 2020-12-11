using FootballFieldManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
                Goods acc = new Goods(int.Parse(dt.Rows[i].ItemArray[0].ToString()), dt.Rows[i].ItemArray[1].ToString(),
                    dt.Rows[i].ItemArray[2].ToString(), double.Parse(dt.Rows[i].ItemArray[3].ToString()),
                    Convert.FromBase64String(dt.Rows[i].ItemArray[4].ToString()), int.Parse(dt.Rows[i].ItemArray[5].ToString()));
                goodsList.Add(acc);
            }
            return goodsList;
        }
        public bool AddIntoDB(Goods goods)
        {
            try
            {
                conn.Open();
                string queryString = "insert into Goods(idGoods, name, unit, unitPrice, imageFile, quantity) " +
                    "values(@idGoods, @name, @unit, @unitPrice, @imageFile, @quantity)";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idGoods", goods.IdGoods.ToString());
                command.Parameters.AddWithValue("@name", goods.Name);
                command.Parameters.AddWithValue("@unit", goods.Unit);
                command.Parameters.AddWithValue("@unitPrice", goods.UnitPrice.ToString());
                command.Parameters.AddWithValue("@imageFile", Convert.ToBase64String(goods.ImageFile));
                command.Parameters.AddWithValue("@quantity", goods.Quantity.ToString());

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
        public bool UpdateOnDB(Goods goods)
        {
            try
            {
                conn.Open();
                string queryString = "update Goods set name=@name, unit=@unit, unitPrice=@unitPrice, imageFile=@imageFile " +
                    "where idGoods =" + goods.IdGoods.ToString();
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@name", goods.Name);
                command.Parameters.AddWithValue("@unit", goods.Unit);
                command.Parameters.AddWithValue("@unitPrice", goods.UnitPrice.ToString());
                command.Parameters.AddWithValue("@imageFile", Convert.ToBase64String(goods.ImageFile));
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
        public bool ImportToDB(Goods goods)
        {
            try
            {
                conn.Open();
                string queryString = "update Goods set quantity = quantity + @quantity where idGoods=" + goods.IdGoods.ToString();
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@quantity", goods.Quantity.ToString());

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
                string queryString = "delete from Goods where idGoods=" + idGoods;
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
        public Goods GetGoods(string idGoods) // lấy thông tin hàng hóa khi biết id 
        {
            try
            {
                conn.Open();
                string queryString = "select * from Goods where idGoods = " + idGoods;

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                Goods res = new Goods(int.Parse(idGoods), dataTable.Rows[0].ItemArray[1].ToString(), 
                    dataTable.Rows[0].ItemArray[2].ToString(), double.Parse(dataTable.Rows[0].ItemArray[3].ToString()),
                    Convert.FromBase64String(dataTable.Rows[0].ItemArray[4].ToString()));

                return res;
            }
            catch
            {
                return new Goods();
            }
            finally
            {
                conn.Close();
            }
        }
    }
}

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
                    (byte[])dt.Rows[i].ItemArray[5], int.Parse(dt.Rows[i].ItemArray[4].ToString()));
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
                command.Parameters.AddWithValue("@imageFile", goods.ImageFile);
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
                string queryString = "update Goods set idGoods=@idGoods, name=@name, unit=@unit, unitPrice=@unitPrice, imageFile=@imageFile " +
                    "where idGoods =" + goods.IdGoods.ToString();
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idGoods", goods.IdGoods.ToString());
                command.Parameters.AddWithValue("@name", goods.Name);
                command.Parameters.AddWithValue("@unit", goods.Unit);
                command.Parameters.AddWithValue("@unitPrice", goods.UnitPrice.ToString());
                command.Parameters.AddWithValue("@imageFile", goods.ImageFile);
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
                string queryString = "update Goods set quantity=@quantity where idGoods=" + goods.IdGoods.ToString();
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
        public Goods GetGood(string idGood) // lấy thông tin hàng hóa khi biết id 
        {
            foreach (var good in ConvertDBToList())
            {
                if (good.IdGoods.ToString() == idGood)
                {
                    return good;
                }
            }
            return null;
        }
    }
}

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
            List<Goods> goodsList = new List<Goods>();
            try
            {
                conn.Open();
                string queryString = "select * from Goods where isDeleted = 0";

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Goods acc = new Goods(int.Parse(dt.Rows[i].ItemArray[0].ToString()), dt.Rows[i].ItemArray[1].ToString(),
                        dt.Rows[i].ItemArray[2].ToString(), long.Parse(dt.Rows[i].ItemArray[3].ToString()),
                        Convert.FromBase64String(dt.Rows[i].ItemArray[4].ToString()), int.Parse(dt.Rows[i].ItemArray[5].ToString()),
                        int.Parse(dt.Rows[i].ItemArray[6].ToString()));
                    goodsList.Add(acc);
                }
                return goodsList;
            }
            catch
            {
                return goodsList;
            }
            finally
            {
                conn.Close();
            }
        }
        public DataTable LoadDatatable()
        {
            try
            {
                conn.Open();
                string queryString = "select * from Goods where isDeleted = 0";

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch
            {
                return new DataTable();
            }
            finally
            {
                conn.Close();
            }
        }
        public bool AddIntoDB(Goods goods)
        {
            try
            {
                conn.Open();
                string queryString = "insert into Goods(idGoods, name, unit, unitPrice, imageFile, quantity, isDeleted) " +
                    "values(@idGoods, @name, @unit, @unitPrice, @imageFile, @quantity, @isDeleted)";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idGoods", goods.IdGoods.ToString());
                command.Parameters.AddWithValue("@name", goods.Name);
                command.Parameters.AddWithValue("@unit", goods.Unit);
                command.Parameters.AddWithValue("@unitPrice", goods.UnitPrice.ToString());
                command.Parameters.AddWithValue("@imageFile", Convert.ToBase64String(goods.ImageFile));
                command.Parameters.AddWithValue("@quantity", goods.Quantity.ToString());
                command.Parameters.AddWithValue("@isDeleted", goods.IsDeleted.ToString());

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
        public bool UpdateOnDB(Goods goods)
        {
            try
            {
                conn.Open();
                string queryString = "update Goods set name=@name, unit=@unit, unitPrice=@unitPrice, imageFile=@imageFile,quantity=@quantity " +
                    "where idGoods =" + goods.IdGoods.ToString();
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@name", goods.Name);
                command.Parameters.AddWithValue("@unit", goods.Unit);
                command.Parameters.AddWithValue("@unitPrice", goods.UnitPrice.ToString());
                command.Parameters.AddWithValue("@imageFile", Convert.ToBase64String(goods.ImageFile));
                command.Parameters.AddWithValue("@quantity", goods.Quantity);
                
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
        public bool ImportToDB(Goods goods)
        {
            try
            {
                conn.Open();
                string queryString = "update Goods set quantity = quantity + @quantity where idGoods=" + goods.IdGoods.ToString();
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@quantity", goods.Quantity.ToString());
                command.ExecuteNonQuery();
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
        public bool DeleteFromDB(string idGoods)
        {
            try
            {
                conn.Open();
                string queryString = "update Goods set isDeleted = 1 where idGoods = " + idGoods;
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
                    dataTable.Rows[0].ItemArray[2].ToString(), long.Parse(dataTable.Rows[0].ItemArray[3].ToString()),
                    Convert.FromBase64String(dataTable.Rows[0].ItemArray[4].ToString()), int.Parse(dataTable.Rows[0].ItemArray[5].ToString()),
                    int.Parse(dataTable.Rows[0].ItemArray[6].ToString()));

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
        public int GetMaxId()
        {
            int res = 0;
            try
            {
                conn.Open();
                string queryString = "select max(idGoods) as id from Goods";
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
                conn.Close();
            }
        }
        public bool isExistGoodsName(string goodsName)
        {
            try
            {
                conn.Open();
                string query = @"select * from Goods where name = '" + goodsName + "'";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count == 0)
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
                return true;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}

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
    class BillDAL : DataProvider
    {
        private static BillDAL instance;

        public static BillDAL Instance
        {
            get { if (instance == null) instance = new BillDAL(); return BillDAL.instance; }
            private set { BillDAL.instance = value; }
        }
        private BillDAL()
        {

        }
        public bool DeleteFromDB(string idBill)
        {
            try
            {
                conn.Open();
                string queryString = "delete from Bill where idBill=" + idBill;
                SqlCommand command = new SqlCommand(queryString, conn);
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
        public bool AddIntoDB(Bill bill)
        {
            try
            {
                conn.Open();
                string queryString = "insert into Bill(idBill, idAccount, invoiceDate,checkInTime,checkOutTime,status,totalMoney,idFieldInfo ) values(@idBill, @idAccount, @invoiceDate,@checkInTime,@checkOutTime,@status,@totalMoney,@idFieldInfo)";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idBill", bill.IdBill.ToString());
                command.Parameters.AddWithValue("@idAccount", bill.IdAccount.ToString());
                command.Parameters.AddWithValue("@invoiceDate", bill.InvoiceDate);
                command.Parameters.AddWithValue("@checkInTime", bill.CheckInTime);
                command.Parameters.AddWithValue("@checkOutTime", bill.CheckOutTime);
                command.Parameters.AddWithValue("@status", bill.Status);
                command.Parameters.AddWithValue("@totalMoney", bill.TotalMoney.ToString());
                command.Parameters.AddWithValue("@idFieldInfo", bill.IdFieldInfo.ToString());
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
        public bool UpdateOnDB(Bill bill)
        {
            try
            {
                conn.Open();
                string queryString = "update Bill set checkOutTime=@checkOutTime,status=@status,totalMoney=@totalMoney,note=@note where idBill=@idBill";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idBill", bill.IdBill.ToString());
                command.Parameters.AddWithValue("@checkOutTime", bill.CheckOutTime);
                command.Parameters.AddWithValue("@status", bill.Status);
                command.Parameters.AddWithValue("@totalMoney", bill.TotalMoney);
                command.Parameters.AddWithValue("@note", bill.Note);
                int rs = command.ExecuteNonQuery();
                if (rs == 1)
                {
                    return true;

                }
                else
                    return false;

            }
            catch
            {
                CustomMessageBox.Show("Thực hiện thất bại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public List<Bill> ConvertDBToList()
        {
            try
            {
                List<Bill> bills = new List<Bill>();
                conn.Open();
                string queryString = "select * from Bill";

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    int idAccount = -1;
                    if (dataTable.Rows[i].ItemArray[1].ToString() != "")
                    {
                        idAccount = int.Parse(dataTable.Rows[i].ItemArray[1].ToString());
                    }
                    Bill bill = new Bill(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), idAccount, DateTime.Parse(dataTable.Rows[i].ItemArray[2].ToString()), DateTime.Parse(dataTable.Rows[i].ItemArray[3].ToString()), DateTime.Parse(dataTable.Rows[i].ItemArray[4].ToString()), int.Parse(dataTable.Rows[i].ItemArray[5].ToString()), long.Parse(dataTable.Rows[i].ItemArray[6].ToString()), int.Parse(dataTable.Rows[i].ItemArray[7].ToString()), dataTable.Rows[i].ItemArray[8].ToString());
                    bills.Add(bill);
                }
                return bills;
            }
            catch
            {
                return new List<Bill>();
            }
            finally
            {
                conn.Close();
            }
        }
        public Bill GetBill(string idBill)
        {
            try
            {
                conn.Open();
                string queryString = "select * from Bill where idBill = " + idBill;

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                Bill res;
                if (string.IsNullOrEmpty(dataTable.Rows[0].ItemArray[1].ToString()))
                {
                    res = new Bill(int.Parse(idBill), 0, DateTime.Parse(dataTable.Rows[0].ItemArray[2].ToString()),
                        DateTime.Parse(dataTable.Rows[0].ItemArray[3].ToString()), DateTime.Parse(dataTable.Rows[0].ItemArray[4].ToString()),
                        int.Parse(dataTable.Rows[0].ItemArray[5].ToString()), long.Parse(dataTable.Rows[0].ItemArray[6].ToString()),
                        int.Parse(dataTable.Rows[0].ItemArray[7].ToString()), dataTable.Rows[0].ItemArray[8].ToString());
                }
                else
                {
                    res = new Bill(int.Parse(idBill), int.Parse(dataTable.Rows[0].ItemArray[1].ToString()),
                        DateTime.Parse(dataTable.Rows[0].ItemArray[2].ToString()), DateTime.Parse(dataTable.Rows[0].ItemArray[3].ToString()),
                        DateTime.Parse(dataTable.Rows[0].ItemArray[4].ToString()), int.Parse(dataTable.Rows[0].ItemArray[5].ToString()),
                        long.Parse(dataTable.Rows[0].ItemArray[6].ToString()), int.Parse(dataTable.Rows[0].ItemArray[7].ToString()),
                        dataTable.Rows[0].ItemArray[8].ToString());
                }
                return res;
            }
            catch
            {
                return new Bill();
            }
            finally
            {
                conn.Close();
            }
        }
        //Sau khi xóa nhân viên => xóa Account => update idAccount về NULL 
        public bool UpdateIdAccount(string idAccount)
        {
            try
            {
                conn.Open();
                string queryString = "update Bill set idAccount = NULL where idAccount = " + idAccount;
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

        public DataTable LoadBillByDate(string day, string month, string year)
        {
            DataTable dataTable = new DataTable();
            try
            {
                conn.Open();
                string queryString = string.Format("select idBill, idAccount, invoiceDate, checkOutTime, totalMoney " +
                    "from Bill where year(invoiceDate) = {0} and month(invoiceDate) = {1} and day(invoiceDate) = {2} order by idBill", year, month, day);

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
                conn.Close();
            }
        }
        public DataTable LoadBillByMonth(string month, string year)
        {
            DataTable dataTable = new DataTable();
            try
            {
                conn.Open();
                string queryString = string.Format("select idBill, idAccount, invoiceDate, checkOutTime, totalMoney " +
                    "from Bill where year(invoiceDate) = {0} and month(invoiceDate) = {1} order by idBill", year, month);

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
                conn.Close();
            }
        }
        public DataTable LoadBillByYear(string year)
        {
            DataTable dataTable = new DataTable();
            try
            {
                conn.Open();
                string queryString = string.Format("select idBill, idAccount, invoiceDate, checkOutTime, totalMoney " +
                    "from Bill where year(invoiceDate) = {0} order by idBill", year);

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
                conn.Close();
            }
        }
        public int GetMaxIdBill()
        {
            int res = 0;
            try
            {
                conn.Open();
                string queryString = @"Select max(idBill) From Bill";

                SqlCommand command = new SqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                res = int.Parse(dataTable.Rows[0].ItemArray[0].ToString());
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
            return res;
        }
    }
}

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
        public void DeleteFromDB(Bill bill)
        {
            try
            {
                conn.Open();
                string queryString = "delete from Bill where idBill=" + bill.IdBill.ToString();
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
        public void AddIntoDB(Bill bill)
        {
            try
            {
                conn.Open();
                string queryString = "insert into Bill(idBill, idAccount, invoiceDate,checkInTime,checkOutTime,status,totalMoney,idFieldInfo ) values(@idBill, @idAccount, @invoiceDate,@checkInTime,@checkOutTime,@status,@totalMoney,@idFieldInfo)";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idBill", bill.IdBill.ToString());
                command.Parameters.AddWithValue("@idAccount", bill.IdAccount.ToString());
                command.Parameters.AddWithValue("@invoiceDate", bill.InvoiceDate.ToString());
                command.Parameters.AddWithValue("@checkInTime", bill.CheckInTime.ToString());
                command.Parameters.AddWithValue("@checkOutTime", bill.CheckOutTime.ToString());
                command.Parameters.AddWithValue("@status", bill.Status);
                command.Parameters.AddWithValue("@totalMoney", bill.TotalMoney.ToString());
                command.Parameters.AddWithValue("@idFieldInfo", bill.IdFieldInfo.ToString());
                int rs = command.ExecuteNonQuery();
                if (rs == 1)
                {
                    MessageBox.Show("Đã thêm!");
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
                MessageBox.Show("Thực hiện thất bại");
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public List<Bill> ConvertDBToList()
        {
            DataTable dt;
            List<Bill> bills = new List<Bill>();
            try
            {
                dt = LoadData("Bill");
            }
            catch
            {
                conn.Close();
                dt = LoadData("Bill");
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int idAccount = -1;
                if (dt.Rows[i].ItemArray[1].ToString() != "")
                {
                    idAccount = int.Parse(dt.Rows[i].ItemArray[1].ToString());
                }
                Bill bill = new Bill(int.Parse(dt.Rows[i].ItemArray[0].ToString()), idAccount, DateTime.Parse(dt.Rows[i].ItemArray[2].ToString()), DateTime.Parse(dt.Rows[i].ItemArray[3].ToString()), DateTime.Parse(dt.Rows[i].ItemArray[4].ToString()), int.Parse(dt.Rows[i].ItemArray[5].ToString()), long.Parse(dt.Rows[i].ItemArray[6].ToString()), int.Parse(dt.Rows[i].ItemArray[7].ToString()), dt.Rows[i].ItemArray[8].ToString());
                bills.Add(bill);
            }
            conn.Close();
            return bills;
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
                string queryString = string.Format("select idBill, name, invoiceDate, checkOutTime, totalMoney from Bill join Account" +
                    " on Bill.idAccount = Account.idAccount join Employee on Account.idAccount = Employee.idAccount " +
                    "where year(invoiceDate) = {0} and month(invoiceDate) = {1} and day(invoiceDate) = {2} order by idBill", year, month, day);

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
                string queryString = string.Format("select idBill, name, invoiceDate, checkOutTime, totalMoney from Bill join Account" +
                    " on Bill.idAccount = Account.idAccount join Employee on Account.idAccount = Employee.idAccount " +
                    "where year(invoiceDate) = {0} and month(invoiceDate) = {1} order by idBill", year, month);

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
                string queryString = string.Format("select idBill, name, invoiceDate, checkOutTime, totalMoney from Bill join Account" +
                    " on Bill.idAccount = Account.idAccount join Employee on Account.idAccount = Employee.idAccount " +
                    "where year(invoiceDate) = {0} order by idBill", year);

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
    }
}

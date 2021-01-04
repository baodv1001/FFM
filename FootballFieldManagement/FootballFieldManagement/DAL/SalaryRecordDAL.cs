using FootballFieldManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballFieldManagement.DAL
{
    class SalaryRecordDAL : DataProvider
    {
        private static SalaryRecordDAL instance;

        public static SalaryRecordDAL Instance
        {
            get { if (instance == null) instance = new SalaryRecordDAL(); return SalaryRecordDAL.instance; }
            private set { SalaryRecordDAL.instance = value; }
        }
        private SalaryRecordDAL()
        {
        }
        public bool AddIntoDB(SalaryRecord record)
        {
            try
            {
                OpenConnection();
                string query = @"insert into SalaryRecord (idSalaryRecord, salaryRecordDate, total, idAccount) values(@idSalaryRecord, @salaryRecordDate, @total, @idAccount)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idSalaryRecord", record.IdSalaryRecord.ToString());
                cmd.Parameters.AddWithValue("@salaryRecordDate", record.SalaryRecordDate);
                cmd.Parameters.AddWithValue("@total", record.Total.ToString());
                cmd.Parameters.AddWithValue("@idAccount", record.IdAccount.ToString());
                if (cmd.ExecuteNonQuery() < 1)
                    return false;
                else
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
        public DataTable GetSalaryRecordByYear(string year)
        {
            DataTable dataTable = new DataTable();
            try
            {
                OpenConnection();
                string queryString = string.Format("select * from SalaryRecord " +
                    "where year(salaryRecordDate) = {0} order by idSalaryRecord", year);

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
                CloseConnection();
            }
        }
        public SalaryRecord GetSalaryRecordById(string idSalaryRecord)
        {
            try
            {
                OpenConnection();
                string queryString = "select * from SalaryRecord where idSalaryRecord = " + idSalaryRecord;

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                SalaryRecord res = new SalaryRecord(int.Parse(idSalaryRecord), DateTime.Parse(dataTable.Rows[0].ItemArray[1].ToString()),
                    int.Parse(dataTable.Rows[0].ItemArray[2].ToString()), int.Parse(dataTable.Rows[0].ItemArray[3].ToString()));
                return res;
            }
            catch
            {
                return new SalaryRecord();
            }
            finally
            {
                CloseConnection();
            }
        }
        public int SetID()
        {
            try
            {
                OpenConnection();
                string queryString = "select max(idSalaryRecord) from SalaryRecord";
                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows[0].ItemArray[0].ToString() != "")
                {
                    return int.Parse(dataTable.Rows[0].ItemArray[0].ToString()) + 1;
                }
                else
                {
                    return 1;
                }
            }
            catch
            {
                return -1;
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}

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
        public DataTable GetSalaryRecordByYear(string year)
        {
            DataTable dataTable = new DataTable();
            try
            {
                conn.Open();
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
                conn.Close();
            }
        }
        public SalaryRecord GetSalaryRecordById(string idSalaryRecord)
        {
            try
            {
                conn.Open();
                string queryString = "select * from SalaryRecord where idSalaryRecord = " + idSalaryRecord;

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                SalaryRecord res = new SalaryRecord(int.Parse(idSalaryRecord), DateTime.Parse(dataTable.Rows[0].ItemArray[1].ToString()),
                    int.Parse(dataTable.Rows[0].ItemArray[2].ToString()));
                return res;
            }
            catch
            {
                return new SalaryRecord();
            }
            finally
            {
                conn.Close();
            }
        }
    }
}

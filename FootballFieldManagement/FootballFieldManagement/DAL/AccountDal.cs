using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FootballFieldManagement.Models;

namespace FootballFieldManagement.DAL
{
    class AccountDAL : DataProvider
    {
        private static AccountDAL instance;

        public static AccountDAL Instance
        {
            get { if (instance == null) instance = new AccountDAL(); return AccountDAL.instance; }
            private set { AccountDAL.instance = value; }
        }
        private AccountDAL()
        {

        }
        public List<Account> ConvertDBToList()
        {
            DataTable dt;
            List<Account> accounts = new List<Account>();
            try
            {
                dt = LoadData("Account");
            }
            catch
            {
                conn.Close();
                dt = LoadData("Account");
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Account acc = new Account(int.Parse(dt.Rows[i].ItemArray[0].ToString()), dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[2].ToString(), int.Parse(dt.Rows[i].ItemArray[3].ToString()));
                accounts.Add(acc);
            }
            return accounts;
        }
        public void AddIntoDB(Account account)
        {
            conn.Open();
            string query = "INSERT INTO Account(idAccount,username, password, type) VALUES(@idAccount,@username, @password, @type)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@idAccount", account.IdAccount.ToString());
            cmd.Parameters.AddWithValue("@username", account.Username);
            cmd.Parameters.AddWithValue("@password", account.Password);
            cmd.Parameters.AddWithValue("@type", account.Type.ToString());
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public bool DeleteAccount(string idAccount)
        {
            try
            {
                conn.Open();
                string query = "delete from Account where IdAccount = " + idAccount;
                SqlCommand command = new SqlCommand(query, conn);
                if (command.ExecuteNonQuery() > 0)
                    return true;
                else
                {
                    return false;
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
        public bool UpdatePassword(string username, string password)
        {
            try
            {
                conn.Open();
                string query = "update Account set password=@password where username = @username";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                if (command.ExecuteNonQuery() > 0)
                    return true;
                else
                {
                    return false;
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
        public bool UpdateType(Account account)
        {
            try
            {
                conn.Open();
                string query = "update Account set type=@type where IdAccount = " + account.IdAccount;
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@type  ", account.Type);
                if (command.ExecuteNonQuery() > 0)
                    return true;
                else
                {
                    return false;
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
        public bool IsExistUserName(string username)
        {
            try
            {
                conn.Open();
                string queryString = "select * from Account where username = '" + username +"'";
                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if(dataTable.Rows.Count > 0)
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
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public int SetNewID()
        {
            try
            {
                conn.Open();
                string queryString = "select max(idAccount) from Account";
                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    return int.Parse(dataTable.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}

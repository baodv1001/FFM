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
    class FootballFieldDAL : DataProvider
    {
        private static FootballFieldDAL instance;
        public static FootballFieldDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new FootballFieldDAL();
                return FootballFieldDAL.instance;
            }
            private set
            {
                FootballFieldDAL.instance = value;
            }
        }

        FootballFieldDAL()
        {

        }
        public List<FootballField> ConvertDBToList()
        {
            DataTable dt;
            List<FootballField> footballFields = new List<FootballField>();
            try
            {
                dt = LoadData("FootballField");
            }
            catch
            {
                dt = null;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                FootballField footballField = new FootballField(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), int.Parse(dt.Rows[i].ItemArray[2].ToString()), int.Parse(dt.Rows[i].ItemArray[3].ToString())
                    , dt.Rows[i].ItemArray[5].ToString());
                footballFields.Add(footballField);
            }
            return footballFields;
        }
        public bool AddIntoDB(FootballField footballField)
        {
            try
            {
                conn.Open();
                string query = "insert into FootballField(idField, name, type, status, note) values (@idField, @name, @type, @status, @note)";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@idField", footballField.IdField.ToString());
                command.Parameters.AddWithValue("@name", footballField.Name);
                command.Parameters.AddWithValue("@type", footballField.Type.ToString());
                command.Parameters.AddWithValue("@status", footballField.Status.ToString());
                command.Parameters.AddWithValue("@note", footballField.Note);
                int rs = command.ExecuteNonQuery();
                if (rs != 1)
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
        public bool DeleteField(string idField)
        {
            try
            {
                conn.Open();
                string query = @"delete from FootballField where idField = " + idField;
                SqlCommand command = new SqlCommand(query, conn);
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
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public bool UpdateField(FootballField footballField)
        {
            try
            {
                conn.Open();
                string query = @"update FootballField set idField = @idField, name = @name, type = @type, status = @status where idField = " + footballField.IdField.ToString();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@idField", footballField.IdField.ToString());
                command.Parameters.AddWithValue("@name", footballField.Name);
                command.Parameters.AddWithValue("@type", footballField.Type.ToString());
                command.Parameters.AddWithValue("@status", footballField.Status.ToString());
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
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public List<string> GetFieldType()
        {
            try
            {
                conn.Open();
                string query = @"select distinct(type) from FootballField";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                List<string> listTmp = new List<string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    listTmp.Add(dt.Rows[i].ItemArray[0].ToString());
                }
                return listTmp;
            }
            catch
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
        }
        public bool CheckFieldName(string fieldName)
        {
            try
            {
                conn.Open();
                string query = @"select * from FootballField where name = '@fieldName'";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@fieldName", fieldName);
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

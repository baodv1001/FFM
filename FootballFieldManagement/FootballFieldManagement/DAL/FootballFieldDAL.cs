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
            try
            {
                conn.Open();
                string queryString = "select * from FootballField";
                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                List<FootballField> footballFields = new List<FootballField>();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    FootballField footballField = new FootballField(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()),
                        dataTable.Rows[i].ItemArray[1].ToString(), int.Parse(dataTable.Rows[i].ItemArray[2].ToString()), int.Parse(dataTable.Rows[i].ItemArray[3].ToString())
                        , dataTable.Rows[i].ItemArray[4].ToString());
                    footballFields.Add(footballField);
                }
                return footballFields;
            }
            catch
            {
                return new List<FootballField>();
            }
            finally
            {
                conn.Close();
            }
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
                string query = @"select distinct(type) from FootballField order by type ASC";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                List<string> listTmp = new List<string>();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    listTmp.Add(dataTable.Rows[i].ItemArray[0].ToString());
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
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count == 0)
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
        public FootballField GetFootballFieldById(string idField)
        {
            try
            {
                conn.Open();
                string queryString = "select * from FootballField where idField = " + idField;

                SqlCommand command = new SqlCommand(queryString, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                FootballField res = new FootballField(int.Parse(idField), dataTable.Rows[0].ItemArray[1].ToString(),
                    int.Parse(dataTable.Rows[0].ItemArray[2].ToString()), int.Parse(dataTable.Rows[0].ItemArray[3].ToString()),
                    dataTable.Rows[0].ItemArray[4].ToString());
                return res;
            }
            catch
            {
                return new FootballField();
            }
            finally
            {
                conn.Close();
            }
        }
        public List<FootballField> GetNamesPerType(string type)
        {
            List<FootballField> res = new List<FootballField>();
            try
            {
                conn.Open();
                string queryString = @"Select *
                                       From FootballField
                                       Where type =@type
                                       Order by type ASC ";
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@type", type);
                command.ExecuteNonQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    FootballField footballField = new FootballField(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), dataTable.Rows[i].ItemArray[1].ToString(), int.Parse(dataTable.Rows[i].ItemArray[2].ToString()), int.Parse(dataTable.Rows[i].ItemArray[3].ToString()), dataTable.Rows[i].ItemArray[4].ToString());
                    res.Add(footballField);
                }
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
        public List<FootballField> GetEmptyField(string type, string day, string startTime, string endTime)
        {
            List<FootballField> footballFields = new List<FootballField>();
            try
            {
                conn.Open();
                string query = @"Select idField,name from FootballField
                                 Where FootballField.type=@type
                                 Except
                                 Select FieldInfo.idField,FootballField.name from FieldInfo
                                 Join FootballField on FieldInfo.idField=FootballField.idField
                                 Where convert(varchar(10), startingTime, 103)=@day and convert(varchar(5), startingTime, 108)=@startTime and convert(varchar(5), endingTime, 108) =@endTime and FootballField.type=@type";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@day", day);
                command.Parameters.AddWithValue("@startTime", startTime);
                command.Parameters.AddWithValue("@endTime", endTime);
                command.Parameters.AddWithValue("@type", type);
                command.ExecuteNonQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    FootballField footballField = new FootballField(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), dataTable.Rows[i].ItemArray[1].ToString(), int.Parse(type), 0, " ");
                    footballFields.Add(footballField);
                }
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
            return footballFields;
        }
    }
}
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace kllmp.org.msql
{
    public class Sql
    {
        #region Properties
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }
        public CommandType CommandType { get; set; }
        #endregion

        #region Constructors
        public Sql(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
            this.CommandTimeout = 180;
            this.CommandType = CommandType.Text;
        }
        public Sql(string server, string database, string user, string password)
        {
            this.ConnectionString = $"Data Source={server};Initial Catalog={database};Persist Security Info=True;User ID={user};Password={password};Integrated Security=False";
            this.CommandTimeout = 180;
            this.CommandType = CommandType.Text;
        }
        public Sql(string ConnectionString, int CommandTimeout)
        {
            this.ConnectionString = ConnectionString;
            this.CommandTimeout = CommandTimeout;
            this.CommandType = CommandType.Text;
        }
        public Sql(string ConnectionString, CommandType CommandType)
        {
            this.ConnectionString = ConnectionString;
            this.CommandTimeout = 180;
            this.CommandType = CommandType;
        }
        public Sql(string ConnectionString, int CommandTimeout, CommandType CommandType)
        {
            this.ConnectionString = ConnectionString;
            this.CommandTimeout = CommandTimeout;
            this.CommandType = CommandType;
        }
        #endregion

        string GetMethodMain(MethodBase? method)
        {
            return $"{method?.DeclaringType?.FullName}.{method?.Name}";
        }

        public int Exec(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                int rows;
                using (SqlConnection db = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, db))
                    {
                        cmd.CommandType = CommandType;
                        cmd.CommandTimeout = CommandTimeout;

                        if (parameters != null && parameters.Length > 0)
                            cmd.Parameters.AddRange(parameters);

                        db.Open();
                        rows = cmd.ExecuteNonQuery();
                    }
                }
                return rows;
            }
            catch (SqlException ex) { throw new Exception($"SqlException: {ex} At {GetMethodMain(MethodBase.GetCurrentMethod())}"); }
            catch (Exception ex) { throw new Exception($"Exception: {ex} At {GetMethodMain(MethodBase.GetCurrentMethod())}"); }
        }

        public DataTable ExecDataTable(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                DataSet dataSet = ExecDataSet(query, parameters);
                return (dataSet.Tables.Count > 0) ? dataSet.Tables[0] : new DataTable();
            }
            catch (SqlException ex) { throw new Exception($"SqlException: {ex} At {GetMethodMain(MethodBase.GetCurrentMethod())}"); }
            catch (Exception ex) { throw new Exception($"Exception: {ex} At {GetMethodMain(MethodBase.GetCurrentMethod())}"); }
        }

        public DataSet ExecDataSet(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                DataSet dataSet = new DataSet();
                using (SqlConnection db = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, db))
                    {
                        cmd.CommandType = CommandType;
                        cmd.CommandTimeout = CommandTimeout;

                        if (parameters != null && parameters.Length > 0)
                            cmd.Parameters.AddRange(parameters);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            db.Open();
                            adapter.Fill(dataSet);
                        }
                    }
                }
                return dataSet;
            }
            catch (SqlException ex) { throw new Exception($"SqlException: {ex} At {GetMethodMain(MethodBase.GetCurrentMethod())}"); }
            catch (Exception ex) { throw new Exception($"Exception: {ex} At {GetMethodMain(MethodBase.GetCurrentMethod())}"); }
        }
    }
}

using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace kllmp.org.msql
{
    public class Sql : ISql
    {
        #region Properties
        private int _CommandTimeout = 180;
        private string _ConnectionString = string.Empty;
        private CommandType _CommandType = CommandType.Text;
        
        public int CommandTimeout { set => _CommandTimeout = value; }
        public CommandType CommandType { set => _CommandType = value; }
        public string ConnectionString { set => _ConnectionString = value; }
        #endregion

        #region Constructors
        public Sql(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
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
                using (SqlConnection db = new SqlConnection(_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, db))
                    {
                        cmd.CommandType = _CommandType;
                        cmd.CommandTimeout = _CommandTimeout;

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
                using (SqlConnection db = new SqlConnection(_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, db))
                    {
                        cmd.CommandType = _CommandType;
                        cmd.CommandTimeout = _CommandTimeout;

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

        public List<T> ExecDataList<T>(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                using (DataTable dataTable = ExecDataTable(query, parameters))
                {
                    List<T> list = new List<T>();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        T item = DBRowToObject<T>(row);
                        list.Add(item);
                    }
                    return list;
                }

            }
            catch (SqlException ex) { throw new Exception($"SqlException: {ex} At {GetMethodMain(MethodBase.GetCurrentMethod())}"); }
            catch (Exception ex) { throw new Exception($"Exception: {ex} At {GetMethodMain(MethodBase.GetCurrentMethod())}"); }
        }

        public SqlRecord ExecReader(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                SqlRecord record;
                SqlConnection con = new SqlConnection(_ConnectionString);
                
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = _CommandType;
                    cmd.CommandTimeout = _CommandTimeout;

                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    record = new SqlRecord(ref con, ref reader);
                }

                return record;
            }
            catch (SqlException ex) { throw new Exception($"SqlException: {ex} At {GetMethodMain(MethodBase.GetCurrentMethod())}"); }
            catch (Exception ex) { throw new Exception($"Exception: {ex} At {GetMethodMain(MethodBase.GetCurrentMethod())}"); }
        }

        public static T DBRowToObject<T>(DataRow row)
        {
            Type type = typeof(T);
            T item = Activator.CreateInstance<T>();
            List<PropertyInfo> properties = type.GetProperties().ToList();
            foreach (DataColumn column in row.Table.Columns)
            {
                var property = properties.FirstOrDefault(x => x.Name.ToLower() == column.ColumnName.ToLower());
                if (property != null)
                    property.SetValue(item, row[column.ColumnName].GetType() == typeof(DBNull) ? null : row[column.ColumnName], null);
            }
            return item;
        }

        public static int DBInt(object value) => !DBNull.Value.Equals(value) ? Convert.ToInt32(value) : 0;
        public static long DBLong(object value) => !DBNull.Value.Equals(value) ? Convert.ToInt64(value) : 0L;
        public static string DBString(object value) => !DBNull.Value.Equals(value) ? value.ToString()! : string.Empty;
        public static decimal DBDecimal(object value) => !DBNull.Value.Equals(value) ? Convert.ToDecimal(value) : 0M;
        public static bool DBBool(object value) => !DBNull.Value.Equals(value) && Convert.ToBoolean(value);
    }
}

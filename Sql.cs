using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Collections.Concurrent;

namespace kllmp.org.msql
{
    public class Sql : ISql
    {
        #region Properties

        private int _commandTimeout = 180;
        private string _connectionString = string.Empty;
        private CommandType _commandType = CommandType.Text;
        private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> _propertyCache = new();

        #endregion


        #region Constructors

        public Sql(string ConnectionString)
        {
            _connectionString = ConnectionString;
            _commandType = CommandType.Text;
        }

        public Sql(string ConnectionString, int CommandTimeout)
        {
            _connectionString = ConnectionString;
            _commandTimeout = CommandTimeout;
            _commandType = CommandType.Text;
        }

        public Sql(string ConnectionString, CommandType CommandType)
        {
            _connectionString = ConnectionString;
            _commandType = CommandType;
        }

        public Sql(string ConnectionString, int CommandTimeout, CommandType CommandType)
        {
            _connectionString = ConnectionString;
            _commandTimeout = CommandTimeout;
            _commandType = CommandType;
        }

        #endregion


        #region Builder
        public Sql AddCommandType(CommandType commandType)
        {
            _commandType = commandType;
            return this;
        }
        public Sql AddCommandTimeout(int commandTimeout)
        {
            _commandTimeout = commandTimeout;
            return this;
        }

        public Sql AddSettings(string connectionString, int commandTimeout, CommandType commandType)
        {
            _connectionString = connectionString;
            _commandTimeout = commandTimeout;
            _commandType = commandType;
            return this;
        }

        public ISql Build()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Connection string cannot be null or empty.");
            if (_commandTimeout <= 0)
                throw new ArgumentOutOfRangeException(nameof(_commandTimeout), "Command timeout must be greater than zero.");
            return this;
        }

        #endregion


        #region Settings
        /// <summary>
        /// Establece la cadena de conexión para las operaciones SQL
        /// </summary>
        /// <param name="connectionString"></param>
        public void SetConnectionString(string connectionString)
            => _connectionString = connectionString;

        /// <summary>
        /// Establece el tipo de comando SQL que se utilizará para las operaciones.
        /// </summary>
        /// <param name="commandType"></param>
        public void SetCommandType(CommandType commandType)
            => _commandType = commandType;

        /// <summary>
        /// Establece el tiempo de espera del comando SQL en segundos. Este valor determina cuánto tiempo esperará el comando antes de cancelar la operación si no se completa.
        /// </summary>
        /// <param name="commandTimeout"></param>
        public void SetCommandTimeout(int commandTimeout)
            => _commandTimeout = commandTimeout;
        #endregion


        /// <summary>
        /// Ejecuta un comando SQL y devuelve el número de filas afectadas.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int Exec(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                using var db = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(query, db)
                {
                    CommandType = _commandType,
                    CommandTimeout = _commandTimeout
                };

                if (parameters?.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                db.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new Exception($"SqlException: {ex.Message} At {GetMethodMain(MethodBase.GetCurrentMethod())}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message} At {GetMethodMain(MethodBase.GetCurrentMethod())}", ex);
            }
        }


        /// <summary>
        /// Ejecuta un comando SQL y devuelve una DataTable que contiene los resultados.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataTable ExecDataTable(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                using var db = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(query, db)
                {
                    CommandType = _commandType,
                    CommandTimeout = _commandTimeout
                };

                if (parameters?.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                using var adapter = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                db.Open();
                adapter.Fill(dataTable);
                return dataTable;
            }
            catch (SqlException ex)
            {
                throw new Exception($"SqlException: {ex.Message} At {GetMethodMain(MethodBase.GetCurrentMethod())}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message} At {GetMethodMain(MethodBase.GetCurrentMethod())}", ex);
            }
        }


        /// <summary>
        /// Ejecuta un comando SQL y devuelve un DataSet que contiene los resultados.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet ExecDataSet(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                var dataSet = new DataSet();
                using var db = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(query, db)
                {
                    CommandType = _commandType,
                    CommandTimeout = _commandTimeout
                };

                if (parameters?.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                using var adapter = new SqlDataAdapter(cmd);
                db.Open();
                adapter.Fill(dataSet);
                return dataSet;
            }
            catch (SqlException ex)
            {
                throw new Exception($"SqlException: {ex.Message} At {GetMethodMain(MethodBase.GetCurrentMethod())}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message} At {GetMethodMain(MethodBase.GetCurrentMethod())}", ex);
            }
        }


        /// <summary>
        /// Ejecuta un comando SQL y devuelve una lista de objetos de tipo T, donde T es una clase con propiedades que coinciden con las columnas del conjunto de resultados.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<T> ExecDataList<T>(string query, SqlParameter[]? parameters = null) 
            where T : class
        {
            try
            {
                using var dataTable = ExecDataTable(query, parameters);
                var list = new List<T>(dataTable.Rows.Count);
                foreach (DataRow row in dataTable.Rows)
                    list.Add(DBRowToObject<T>(row));
                return list;
            }
            catch (SqlException ex)
            {
                throw new Exception($"SqlException: {ex.Message} At {GetMethodMain(MethodBase.GetCurrentMethod())}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception: {ex.Message} At {GetMethodMain(MethodBase.GetCurrentMethod())}", ex);
            }
        }


        /// <summary>
        /// Ejecuta un comando SQL y devuelve una colección enumerable de objetos de tipo T, donde T es una clase con propiedades que coinciden con las columnas del conjunto de resultados.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<T> LazyExecute<T>(string query, params SqlParameter[] parameters)
            where T : class
        {
            using var db = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(query, db)
            {
                CommandType = _commandType,
                CommandTimeout = _commandTimeout
            };

            if (parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            db.Open();
            using var reader = cmd.ExecuteReader();

            var type = typeof(T);
            //var propertyMap = type.GetProperties().ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);
            var propertyMap = _propertyCache.GetOrAdd(type, t =>
                t.GetProperties().ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase)
            );

            while (reader.Read())
            {
                var item = Activator.CreateInstance<T>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (propertyMap.TryGetValue(reader.GetName(i), out var prop))
                    {
                        var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        prop.SetValue(item, value);
                    }
                }
                yield return item;
            }
        }


        /// <summary>
        /// Convierte un DataRow en un objeto de tipo T, donde T es una clase con propiedades que coinciden con las columnas del DataRow.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T DBRowToObject<T>(DataRow row)
            where T : class
        {
            Type type = typeof(T);
            T item = Activator.CreateInstance<T>();
            
            //var propertyMap = type.GetProperties().ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);
            var propertyMap = _propertyCache.GetOrAdd(type, t =>
                t.GetProperties().ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase)
            );
            foreach (DataColumn column in row.Table.Columns)
            {
                if (propertyMap.TryGetValue(column.ColumnName, out var property))
                {
                    var value = row[column];
                    property.SetValue(item, value == DBNull.Value ? null : value);
                }
            }
            return item;
        }


        static string GetMethodMain(MethodBase? method)
            => $"{method?.DeclaringType?.FullName}.{method?.Name}";

        public static int DBInt(object value) => !DBNull.Value.Equals(value) ? Convert.ToInt32(value) : 0;
        
        public static long DBLong(object value) => !DBNull.Value.Equals(value) ? Convert.ToInt64(value) : 0L;
        
        public static string DBString(object value) => !DBNull.Value.Equals(value) ? value.ToString()! : string.Empty;
        
        public static decimal DBDecimal(object value) => !DBNull.Value.Equals(value) ? Convert.ToDecimal(value) : 0M;
        
        public static bool DBBool(object value) => !DBNull.Value.Equals(value) && Convert.ToBoolean(value);
    }
}

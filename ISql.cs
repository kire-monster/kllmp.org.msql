using System.Data;
using System.Data.SqlClient;

namespace kllmp.org.msql
{
    public interface ISql
    {
        int Exec(string query, SqlParameter[]? parameters = null);
        DataSet ExecDataSet(string query, SqlParameter[]? parameters = null);
        DataTable ExecDataTable(string query, SqlParameter[]? parameters = null);
        List<T> ExecDataList<T>(string query, SqlParameter[]? parameters = null) where T : class;
        IEnumerable<T> LazyExecute<T>(string query, params SqlParameter[] parameters) where T : class;
        void SetConnectionString(string connectionString);
        void SetCommandType(CommandType commandType);
        void SetCommandTimeout(int commandTimeout);
    }
}

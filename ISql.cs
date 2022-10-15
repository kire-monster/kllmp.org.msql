using System.Data;
using System.Data.SqlClient;

namespace kllmp.org.msql
{
    public interface ISql
    {
        int Exec(string query, SqlParameter[]? parameters = null);
        DataTable ExecDataTable(string query, SqlParameter[]? parameters = null);
        DataSet ExecDataSet(string query, SqlParameter[]? parameters = null);
        List<T> ExecDataList<T>(string query, SqlParameter[]? parameters = null);
    }
}

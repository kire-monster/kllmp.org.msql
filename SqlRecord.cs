using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;

namespace kllmp.org.msql
{
    public class SqlRecord : IDisposable
    {
        private readonly SqlDataReader reader;
        private readonly SqlConnection connection;


        private Component component = new Component();
        private bool disposed = false;

        public SqlRecord(ref SqlConnection connection, ref SqlDataReader reader)
        {
            this.reader = reader;
            this.connection = connection;
        }

        public IDataRecord Record
        {
            get => reader;
        }

        public bool Fetch() => reader.Read();

        public bool NextResult() => reader.NextResult();

        #region Disposing
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                    component.Dispose();

                reader.Close();
                connection.Close();

                disposed = true;
            }
        }

        ~SqlRecord() => Dispose(disposing: false);
        #endregion
    }
}
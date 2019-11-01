using System.Data.Common;
using System.Data.SqlClient;

namespace ApexVoxApi.Providers
{
    public class MigrationConnectionProvider : IConnectionProvider
    {
        private readonly string _connectionString;

        public MigrationConnectionProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbConnection OpenDDRConnection()
        {
            SqlConnection conn = new SqlConnection(_connectionString);

            return conn;
        }
    }
}

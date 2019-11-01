using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using ApexVoxApi.ElasticDatabaseClient;
using ApexVoxApi.TenantProviders;

namespace ApexVoxApi.Providers
{
    public class ConnectionProvider : IConnectionProvider
    {
        private readonly ITenantProvider _tenantProvider;
        private readonly IConfiguration _configuration;

        public ConnectionProvider(ITenantProvider tenantProvider, IConfiguration configuration)
        {
            _tenantProvider = tenantProvider;
            _configuration = configuration;
        }

        public DbConnection OpenDDRConnection()
        {
            var connectionString = _configuration.GetConnectionString("ShardMapManager");
            SqlConnectionStringBuilder connStrBldr = new SqlConnectionStringBuilder(connectionString);

            SqlConnectionStringBuilder emptyCon = new SqlConnectionStringBuilder
            {
                UserID = connStrBldr.UserID,
                Password = connStrBldr.Password,
                ApplicationName = connStrBldr.ApplicationName
            };

            ShardMapManager smm;
            if (!ShardMapManagerFactory.TryGetSqlShardMapManager(connStrBldr.ConnectionString, ShardMapManagerLoadPolicy.Lazy, out smm))
            {
                return null;
            }

            ListShardMap<long> shardMap;
            if (!smm.TryGetListShardMap<long>(ElasticScaleConstants.ShardMapName, out shardMap))
            {
                return null;
            }

            var tenantId = _tenantProvider.GetTenantId();
            SqlConnection conn = shardMap.OpenConnectionForKey(tenantId, emptyCon.ConnectionString, ConnectionOptions.Validate);

            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SET CONTEXT_INFO " + tenantId;
            cmd.CommandType = System.Data.CommandType.Text;

            cmd.ExecuteNonQuery();

            return conn;
        }
    }
}

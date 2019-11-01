using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.Extensions.Options;
using System;
using System.Data.SqlClient;
using ApexVoxApi.Infrastructure;

namespace ApexVoxApi.ElasticDatabaseClient
{
    public class ShardingManager : IShardingManager
    {
        private readonly ShardingManagerConnectionStringOptions _shardingManagerOptions;

        public ShardingManager(IOptions<ShardingManagerConnectionStringOptions> shardingManagerOptions)
        {
            _shardingManagerOptions = shardingManagerOptions.Value;
        }

        public void CreateShardingIfNotExists()
        {
            ShardMapManager smm;
            if (!ShardMapManagerFactory.TryGetSqlShardMapManager(_shardingManagerOptions.ShardMapManager, ShardMapManagerLoadPolicy.Lazy, out smm))
            {
                smm = ShardMapManagerFactory.CreateSqlShardMapManager(_shardingManagerOptions.ShardMapManager);
            }

            if (!smm.TryGetListShardMap<long>(ElasticScaleConstants.ShardMapName, out _))
            {
                smm.CreateListShardMap<long>(ElasticScaleConstants.ShardMapName);
            }
        }

        public void RegisterCreateTenantIfNotExists(long tennatId, string tenantName)
        {
            SqlConnectionStringBuilder smConnectionString = new SqlConnectionStringBuilder(_shardingManagerOptions.ShardMap);

            ShardMapManager smm;
            if (!ShardMapManagerFactory.TryGetSqlShardMapManager(_shardingManagerOptions.ShardMapManager, ShardMapManagerLoadPolicy.Lazy, out smm))
            {
                throw new InvalidOperationException("Error accessing shard map manager database");
            }

            ListShardMap<long> sm;
            if (!smm.TryGetListShardMap<long>(ElasticScaleConstants.ShardMapName, out sm))
            {
                throw new InvalidOperationException("Error accessing list shard map of the sharding map manager database");
            }

            smConnectionString.InitialCatalog = tenantName;
            DbUtils.CreateDatabseIfNotExist(smConnectionString.ConnectionString);

            ShardLocation shardLocation = new ShardLocation(smConnectionString.DataSource, smConnectionString.InitialCatalog);

            Shard shard;
            if (!sm.TryGetShard(shardLocation, out shard))
            {
                shard = sm.CreateShard(shardLocation);
            }

            if (!sm.TryGetMappingForKey(tennatId, out _))
            {
                sm.CreatePointMapping(tennatId, shard);
            }
        }

        public void RegisterNewTenantIfNotExists(long tenantId)
        {
            SqlConnectionStringBuilder smConnectionString = new SqlConnectionStringBuilder(_shardingManagerOptions.ShardMap);

            ShardMapManager smm;
            if (!ShardMapManagerFactory.TryGetSqlShardMapManager(_shardingManagerOptions.ShardMapManager, ShardMapManagerLoadPolicy.Lazy, out smm))
            {
                throw new InvalidOperationException("Error accessing shard map manager database");
            }

            ListShardMap<long> sm;
            if (!smm.TryGetListShardMap<long>(ElasticScaleConstants.ShardMapName, out sm))
            {
                throw new InvalidOperationException("Error accessing list shard map of the sharding map manager database");
            }

            ShardLocation shardLocation = new ShardLocation(smConnectionString.DataSource, smConnectionString.InitialCatalog);

            Shard shard;
            if (!sm.TryGetShard(shardLocation, out shard))
            {
                shard = sm.CreateShard(shardLocation);
            }

            if (!sm.TryGetMappingForKey(tenantId, out _))
            {
                sm.CreatePointMapping(tenantId, shard);
            }
        }
    }
}

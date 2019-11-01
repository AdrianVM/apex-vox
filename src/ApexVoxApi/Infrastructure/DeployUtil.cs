using ApexVoxApi.ElasticDatabaseClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ApexVoxApi.Infrastructure
{
    public class DeployUtil
    {
        private readonly string _shardMapManagerConnString;
        private readonly string _tenantConnectionString;

        public DeployUtil(
            string shardMapManagerConnString,
            string tenantConnectionString)
        {
            _shardMapManagerConnString = shardMapManagerConnString;
            _tenantConnectionString = tenantConnectionString;
        }

        public void DeployShardManagerDbIfNotExist()
        {
            var services = new ServiceCollection();
            services.Configure<ShardingManagerConnectionStringOptions>(shardingOptions =>
            {
                shardingOptions.ShardMapManager = _shardMapManagerConnString;
            });

            services.AddSingleton<IShardingManager, ShardingManager>();

            services.Configure<ShardingManagerConnectionStringOptions>(shardingOptions =>
            {
                shardingOptions.ShardMapManager = _shardMapManagerConnString;
                shardingOptions.ShardMap = _tenantConnectionString;
            });

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    //Check if shard map manager db exists and if not creates it
                    DbUtils.CreateDatabseIfNotExist(_shardMapManagerConnString);

                    var shardManager = scope.ServiceProvider.GetRequiredService<IShardingManager>();

                    shardManager.CreateShardingIfNotExists();
                }
            }
        }

        public void DeployTenantDbIfNotExist(long tenantId)
        {
            var services = new ServiceCollection();
            services.Configure<ShardingManagerConnectionStringOptions>(shardingOptions =>
            {
                shardingOptions.ShardMapManager = _shardMapManagerConnString;
            });

            services.AddSingleton<IShardingManager, ShardingManager>();

            services.Configure<ShardingManagerConnectionStringOptions>(shardingOptions =>
            {
                shardingOptions.ShardMapManager = _shardMapManagerConnString;
                shardingOptions.ShardMap = _tenantConnectionString;
            });

            services.AddDbContext<ApexVoxContext>(options =>
            {
                SqlConnection conn = GetSqlConnectionWithContextInfoSet(_tenantConnectionString, tenantId);

                options.UseSqlServer(conn);
            });

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    //Check if shard map manager db exists and if not creates it
                    DbUtils.CreateDatabseIfNotExist(_tenantConnectionString);

                    var shardManager = scope.ServiceProvider.GetRequiredService<IShardingManager>();

                    shardManager.RegisterNewTenantIfNotExists(tenantId);

                    using (var context = serviceProvider.GetService<ApexVoxContext>())
                    {
                        context.Database.Migrate();
                    }
                }
            }
        }

        public static SqlConnection GetSqlConnectionWithContextInfoSet(string tenantConnectionString, long tenantId)
        {

            var conn = new SqlConnection(tenantConnectionString);

            conn.Open();

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SET CONTEXT_INFO " + tenantId;
            cmd.ExecuteNonQuery();
            return conn;
        }
    }
}

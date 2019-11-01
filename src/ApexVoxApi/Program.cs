using System.Linq;
using ApexVoxApi.Infrastructure;
using ApexVoxApi.Seed;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApexVoxApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var deploy = args.Any(x => x == "/deploy");
            if (deploy) args = args.Except(new[] { "/deploy" }).ToArray();

            var host = CreateWebHostBuilder(args).Build();

            if (deploy)
            {
                long tenatId = 1;
                var config = host.Services.GetRequiredService<IConfiguration>();
                var shardMapManagerConnString = config.GetConnectionString("ShardMapManager");
                var shardMapConnString = config.GetConnectionString("ShardMap");

                var deployUtil = new DeployUtil(shardMapManagerConnString, shardMapConnString);
                deployUtil.DeployShardManagerDbIfNotExist();
                deployUtil.DeployTenantDbIfNotExist(tenatId);

                var userSeeder = new UserSeeder(shardMapConnString);
                userSeeder.SeedDefaultUser(tenatId);

                var productSeeder = new ProductSeeder(shardMapConnString);
                productSeeder.SeedProducts(tenatId);

                return;
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

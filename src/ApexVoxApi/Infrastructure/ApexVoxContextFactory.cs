using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using ApexVoxApi.Providers;

namespace ApexVoxApi.Infrastructure
{
    public class ApexVoxContextFactory : IDesignTimeDbContextFactory<ApexVoxContext>
    {
        public ApexVoxContext CreateDbContext(string[] args)
        {
            // Get environment
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            //Set default value for environment as development
            if (string.IsNullOrEmpty(environment))
            {
                environment = "development";
            }

            // Build config
            string applicationSettingsFullPath = Path.Combine(Directory.GetCurrentDirectory());
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(applicationSettingsFullPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApexVoxContext>();
            var connectionProvider = new MigrationConnectionProvider(config.GetConnectionString("ShardMap"));
            var connectionString = connectionProvider.OpenDDRConnection();
            optionsBuilder.UseSqlServer(connectionString);
            return new ApexVoxContext(optionsBuilder.Options);
        }
    }
}

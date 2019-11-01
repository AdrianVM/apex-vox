using ApexVoxApi.Infrastructure;
using ApexVoxApi.Models;
using ApexVoxApi.Services;
using ApexVoxApi.TenantProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;
using System.Linq;

namespace ApexVoxApi.Seed
{
    public class ProductSeeder
    {
        private readonly string _connectionString;
        public ProductSeeder(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SeedProducts(long tenantId)
        {
            var services = new ServiceCollection();

            services.AddScoped<ICurrentUser, SeedDataCurrentUser>();
            services.AddScoped<ITenantProvider, SeedTenantProvider>();

            services.AddDbContext<ApexVoxContext>(options =>
            {
                SqlConnection conn = DeployUtil.GetSqlConnectionWithContextInfoSet(_connectionString, tenantId);

                options.UseSqlServer(conn);
            });


            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApexVoxContext>();

                    for (var i=1; i<=10; i++)
                    {
                        var product = CreateProduct(i, tenantId);

                        if (!context.Products.Any(x=>x.Name.Equals(product.Name)))
                        {
                            context.Products.Add(product);
                        }
                    }

                    context.SaveChanges();
                }
            }
        }

        private Product CreateProduct(long productSufix, long tenantId)
        {
            return new Product()
            {
                Name = "Product" + productSufix,
                TenantId = tenantId
            };
        }
    }
}

using ApexVoxApi.Infrastructure;
using ApexVoxApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ApexVoxApi.Seed
{
    public class UserSeeder
    {
        private readonly string _connectionString;

        public UserSeeder(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal void SeedDeaultTenant(long tenantId, TenantContext context)
        {
            var tenant = new Tenant()
            {
                Id = tenantId,
                Name="ApexVox"
            };

            if(!context.Tenants.Any(x=>x.Id == tenantId))
            {
                context.Database.OpenConnection();
                try
                {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Tenants ON");
                    context.Tenants.Add(tenant);
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Tenants OFF");
                }
                finally
                {
                    context.Database.CloseConnection();
                }
            }
        }

        internal void SeedDefaultUser(long tenantId)
        {
            var services = new ServiceCollection();

            services.AddScoped<ITenantContext, TenantContext>();

            services.AddDbContext<TenantContext>((options) =>
            {
                options.UseSqlServer(_connectionString);
            });

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<TenantContext>();

                    SeedDeaultTenant(tenantId, context);

                    var user = new User()
                    {
                        FirstName = "Apex",
                        LastName = "Vox",
                        Password = "password",
                        Username = "admin",
                        Role = "admin",
                        TenantId = tenantId
                    };

                    if (!context.Users.Any(x=>x.Username == user.Username))
                    {
                        context.Users.Add(user);
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}

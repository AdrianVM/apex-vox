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

                    context.Database.Migrate();

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

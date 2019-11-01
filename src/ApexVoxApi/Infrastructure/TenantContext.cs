using ApexVoxApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ApexVoxApi.Infrastructure
{
    public class TenantContext : DbContext, ITenantContext
    {
        public TenantContext(DbContextOptions<TenantContext> options):
                base(options)
        {

        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}

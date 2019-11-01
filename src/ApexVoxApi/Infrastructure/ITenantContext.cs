using ApexVoxApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ApexVoxApi.Infrastructure
{
    public interface ITenantContext
    {
        DbSet<Tenant> Tenants { get; set; }
        DbSet<User> Users { get; set; }

        int SaveChanges();
    }
}

using Microsoft.EntityFrameworkCore;
using ApexVoxApi.Models;

namespace ApexVoxApi.Infrastructure
{
    public interface IUnitOfWork
    {
        DbSet<Product> Products { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Tenant> Tenants { get; set; }

        int SaveChanges();
        void Migrate();
    }
}

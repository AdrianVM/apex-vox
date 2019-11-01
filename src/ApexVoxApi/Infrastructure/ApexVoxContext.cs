using Microsoft.EntityFrameworkCore;
using ApexVoxApi.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ApexVoxApi.TenantProviders;
using ApexVoxApi.Services;

namespace ApexVoxApi.Infrastructure
{
    public class ApexVoxContext: DbContext, IUnitOfWork
    {
        private readonly ITenantProvider _tenantProvider;

        public ApexVoxContext(DbContextOptions<ApexVoxContext> options)
            : base(options)
        {
        }

        public ApexVoxContext(DbContextOptions<ApexVoxContext> options, ITenantProvider tenantProvider)
            : base(options)
        {
            _tenantProvider = tenantProvider;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tenant> Tenants { get; set; }

        public void Migrate()
        {
            this.Database.Migrate();
        }

        public override int SaveChanges()
        {
            ProcessEntities();

            return base.SaveChanges();
        }

        private void ProcessEntities()
        {
            var modifiedEntries = ChangeTracker.Entries()
                    .Where(x => x.State == EntityState.Added
                                || x.State == EntityState.Modified
                                || x.State == EntityState.Deleted);

            if (modifiedEntries.Any())
            {
                var tenantId = _tenantProvider.GetTenantId();

                foreach (var entry in modifiedEntries)
                {
                    SetTenantIdIfNeeded(tenantId, entry);
                }
            }
        }

        private void SetTenantIdIfNeeded(long tenantId, EntityEntry entry)
        {
            if (entry.State == EntityState.Added)
            {
                Entry(entry.Entity).Property("TenantId").CurrentValue = tenantId;
            }
        }
    }
}

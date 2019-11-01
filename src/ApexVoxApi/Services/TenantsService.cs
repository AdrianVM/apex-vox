using System;
using System.Linq;
using ApexVoxApi.ElasticDatabaseClient;
using ApexVoxApi.Infrastructure;
using ApexVoxApi.Models;

namespace ApexVoxApi.Services
{
    public class TenantsService: ITenantsService
    {
        private readonly IShardingManager _shardingManager;
        private readonly ITenantContext _tenantContext;

        public TenantsService(IShardingManager shardingManager, ITenantContext tenantContext)
        {
            _shardingManager = shardingManager;
            _tenantContext = tenantContext;
        }

        public void CreateSharding()
        {
            _shardingManager.CreateShardingIfNotExists();
        }

        public Tenant GetTenantByName(string tenantName)
        {
            if (string.IsNullOrEmpty(tenantName))
            {
                throw new ArgumentException("Tenant name cannot be null or empty");
            }

            var tenant = _tenantContext.Tenants.SingleOrDefault(x=>x.Name == tenantName);

            return tenant;
        }

        public void RegisterNewTenant(string tenantName)
        {
            if (string.IsNullOrEmpty(tenantName))
            {
                throw new ArgumentException("Tenant name cannot be null or empty");
            }

            Tenant tenant = SaveNewTenant(tenantName);

            try
            {
                _shardingManager.RegisterNewTenantIfNotExists(tenant.Id);
            }
            catch (InvalidOperationException)
            {
                _tenantContext.Tenants.Remove(tenant);
            }
        }

        public void RegisterNewTenantCreateDb(string tenantName)
        {
            if (string.IsNullOrEmpty(tenantName))
            {
                throw new ArgumentException("Tenant name cannot be null or empty");
            }

            Tenant tenant = SaveNewTenant(tenantName);

            try
            {
                _shardingManager.RegisterCreateTenantIfNotExists(tenant.Id, tenantName);
            }
            catch (InvalidOperationException)
            {
                _tenantContext.Tenants.Remove(tenant);
            }
        }

        private Tenant SaveNewTenant(string tenantName)
        {
            var tenant = new Tenant()
            {
                Name = tenantName
            };

            _tenantContext.Tenants.Add(tenant);

            _tenantContext.SaveChanges();
            return tenant;
        }
    }
}

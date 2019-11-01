using ApexVoxApi.Models;

namespace ApexVoxApi.Services
{
    public interface ITenantsService
    {
        Tenant GetTenantByName(string tenantName);

        void RegisterNewTenant(string tenantName);

        void RegisterNewTenantCreateDb(string tenantName);

        void CreateSharding();
    }
}

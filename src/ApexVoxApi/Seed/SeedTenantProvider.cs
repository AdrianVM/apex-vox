using ApexVoxApi.TenantProviders;

namespace ApexVoxApi.Seed
{
    internal class SeedTenantProvider: ITenantProvider
    {
        public long GetTenantId()
        {
            return 1;
        }
    }
}
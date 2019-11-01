namespace ApexVoxApi.ElasticDatabaseClient
{
    public interface IShardingManager
    {
        void CreateShardingIfNotExists();
        void RegisterNewTenantIfNotExists(long tennatId);
        void RegisterCreateTenantIfNotExists(long tennatId, string tenantName);
    }
}

using ApexVoxApi.Services;

namespace ApexVoxApi.Seed
{
    internal class SeedDataCurrentUser: ICurrentUser
    {
        long ICurrentUser.Id => 1;
    }
}
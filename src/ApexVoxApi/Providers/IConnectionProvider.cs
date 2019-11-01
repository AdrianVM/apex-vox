using System.Data.Common;

namespace ApexVoxApi.Providers
{
    public interface IConnectionProvider
    {
        DbConnection OpenDDRConnection();
    }
}

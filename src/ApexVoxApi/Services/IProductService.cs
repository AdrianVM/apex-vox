using ApexVoxApi.Models;
using System.Collections.Generic;

namespace ApexVoxApi.Services
{
    public interface IProductService
    {
        IList<Product> LoadData(Tenant tenant);
    }
}

using System.Collections.Generic;
using ApexVoxApi.Models;

namespace ApexVoxApi.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(long id);
        User Create(string tenantName);
    }
}

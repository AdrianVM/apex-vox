using ApexVoxApi.Helpers;
using Microsoft.AspNetCore.Http;

namespace ApexVoxApi.Services
{
    public class HttpContextCurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

       public long Id
        {
            get
            {
                HttpContext httpContext = _httpContextAccessor.HttpContext;
                
                return httpContext.User.GetId();
            }
        }
    }
}
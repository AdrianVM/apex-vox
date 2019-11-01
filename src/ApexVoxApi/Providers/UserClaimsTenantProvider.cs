using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace ApexVoxApi.TenantProviders
{
    public class UserClaimsTenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserClaimsTenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public long GetTenantId()
        {
            var userClaims = _contextAccessor.HttpContext.User.Claims;

            var tenantClaim = userClaims.FirstOrDefault(c => c.Type == "TenantId");

            if (tenantClaim == null)
            {
                throw new InvalidOperationException("Missing tenant id in user claims");
            }

            if (!long.TryParse(tenantClaim.Value, out long tenantClaimId))
            {
                throw new InvalidOperationException("Tenant Id provided in wrong format");
            }

            return tenantClaimId;
        }
    }
}

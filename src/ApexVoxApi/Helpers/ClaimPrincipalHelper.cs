using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApexVoxApi.Helpers
{
    public static class ClaimsPrincipalHelper
    {
        public static long GetId(this ClaimsPrincipal claimsPrincipal)
        {
            var idClaimValue = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Id)?.Value;
            if (idClaimValue == null)
            {
                throw new InvalidOperationException("Claim 'id' not found in user claims");
            }

            if (!long.TryParse(idClaimValue, out long id))
            {
                throw new InvalidOperationException($"Unable to parse claim 'id' from provided value '{idClaimValue}'");
            }

            return id;
        }
    }
}

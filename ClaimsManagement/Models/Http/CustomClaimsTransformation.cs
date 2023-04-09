using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace ClaimsManagement.Models.Http;

public class CustomClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentException();

        var claimType = "newClaim";
        if (!principal.HasClaim(x => x.Type.Equals(claimType)))
        {
            var identity = principal.Identity as ClaimsIdentity;
            identity?.AddClaim(new Claim(claimType, "newClaimValue"));
        }

        return Task.FromResult(principal);
    }
}
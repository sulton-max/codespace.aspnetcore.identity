using System.Security.Claims;

namespace ExternalIdentity.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetAccessToken(this ClaimsPrincipal principal)
    {
        return principal.Claims.FirstOrDefault(x => x.Type == "access_token")?.Value;
    }
}
using CookingCalendarApi.Auth;
using System.Security.Claims;

namespace CookingCalendarApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return GetClaimStringItemOrEmpty(claimsPrincipal, AuthConstants.UserId);
        }

        private static string GetClaimStringItemOrEmpty(ClaimsPrincipal claimsPrincipal, string claimName)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == claimName)?.Value ?? string.Empty;
        }
    }
}

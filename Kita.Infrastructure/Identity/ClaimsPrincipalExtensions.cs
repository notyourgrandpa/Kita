using System.Security.Claims;

namespace Kita.Infrastructure.Persistence.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("User Id claim not found.");
        }
    }
}

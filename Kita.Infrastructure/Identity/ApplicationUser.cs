using Kita.Domain.Inventory;
using Kita.Domain.Sales;
using Microsoft.AspNetCore.Identity;

namespace Kita.Infrastructure.Identity
{
    public class ApplicationUser: IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? Position { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

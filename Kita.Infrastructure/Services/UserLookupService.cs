using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Kita.Application.Services;
using Kita.Infrastructure.Persistence;

namespace Kita.Infrastructure.Services
{
    public class UserLookupService : IUserLookupService
    {
        private readonly ApplicationDbContext _db;

        public UserLookupService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Dictionary<string, string>> GetUserNamesAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null)
                return new Dictionary<string, string>();

            var idList = ids.Where(i => !string.IsNullOrEmpty(i)).Distinct().ToList();
            if (idList.Count == 0)
                return new Dictionary<string, string>();

            var users = await _db.Users
                .Where(u => idList.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync(cancellationToken);

            return users.ToDictionary(u => u.Id, u => u.UserName ?? string.Empty);
        }
    }
}

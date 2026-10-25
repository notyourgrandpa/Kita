using Kita.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Kita.Infrastructure.Persistence
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<T> GetQuery<T>(
            IQueryable<T> inputQuery,
            Specification<T> spec)
            where T : class
        {
            var query = inputQuery;

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);

            if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if (spec.IsPagingEnabled)
                query = query.Skip(spec.Skip!.Value).Take(spec.Take!.Value);

            query = spec.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}

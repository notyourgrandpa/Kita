using Kita.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Infrastructure.Persistence.Querying.Sorting
{
    public class EfSortBuilder: ISortBuilder
    {
        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> source, string? sortColumn, string? sortDirection) where TEntity : class
        {
            if (sortColumn == null)
                return source;
            var param = Expression.Parameter(typeof(TEntity), "x");

            Expression property = Expression.PropertyOrField(param, sortColumn);

            var lambda = Expression.Lambda(property, param);

            string methodName =
                sortDirection?.ToUpper() == "DESC"
                ? "OrderByDescending"
                : "OrderBy";

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(TEntity), property.Type },
                source.Expression,
                Expression.Quote(lambda));

            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}

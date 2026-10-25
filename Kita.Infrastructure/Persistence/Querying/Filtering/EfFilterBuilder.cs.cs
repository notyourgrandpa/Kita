using Kita.Application.Common.Interfaces;
using Kita.Application.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Infrastructure.Persistence.Querying.Filtering
{
    public class EfFilterBuilder: IFilterBuilder
    {
        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> source, List<FilterCondition> filters) where TEntity : class
        {
            var param = Expression.Parameter(typeof(TEntity), "x");

            Expression? combinedExpression = null;
            foreach (FilterCondition filter in filters)
            {
                Expression property = Expression.PropertyOrField(param, filter.Field);
                Expression constant = Expression.Constant(Convert.ChangeType(filter.Value, property.Type));

                Expression? condition = filter.Operator switch
                {
                    FilterOperator.Contains => Expression.Call(property, typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!, constant),
                    FilterOperator.StartsWith => Expression.Call(property, typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) })!, constant),
                    FilterOperator.Equals => Expression.Equal(property, constant),
                    FilterOperator.GreaterThan => Expression.GreaterThan(property, constant),
                    FilterOperator.LessThan => Expression.LessThan(property, constant),
                    _ => null,
                };

                if (condition != null)
                {
                    combinedExpression = combinedExpression == null ? condition : Expression.AndAlso(combinedExpression, condition);
                }
            }

            if (combinedExpression == null)
            {
                return source;
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(combinedExpression, param);

            return source.Where(lambda);
        }
    }
}

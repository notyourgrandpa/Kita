using Kita.Application.Common.Paging;
using Kita.Application.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Common.Interfaces
{
    public interface IQueryExecutor
    {
        Task<PagingResult<TDto>> ExecuteAsync<TEntity, TDto>(PagingQuery query, Expression<Func<TEntity, TDto>> selector) where TEntity : class;
    }
}

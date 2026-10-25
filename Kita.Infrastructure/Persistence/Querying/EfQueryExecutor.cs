using Kita.Application.Common.Interfaces;
using Kita.Application.Common.Paging;
using Kita.Application.QueryParameters;
using Kita.Infrastructure.Persistence.Querying.Filtering;
using Kita.Infrastructure.Persistence.Querying.Sorting;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Infrastructure.Persistence.Querying
{
    public class EfQueryExecutor : IQueryExecutor
    {
        private readonly IApplicationDbContext _context;
        private readonly EfFilterBuilder _filterBuilder;
        private readonly EfSortBuilder _sortBuilder;

        public EfQueryExecutor(IApplicationDbContext context, EfFilterBuilder filterBuilder, EfSortBuilder sortBuilder)
        {
            _context = context;
            _filterBuilder = filterBuilder;
            _sortBuilder = sortBuilder;
        }

        public async Task<PagingResult<TDto>> ExecuteAsync<TEntity,TDto>(PagingQuery query, Expression<Func<TEntity, TDto>> selector) where TEntity : class
        {
            IQueryable<TEntity> source = _context.Set<TEntity>().AsNoTracking();

            source = _filterBuilder.Apply(source, query.Filters ?? new List<FilterCondition>());
            source = _sortBuilder.Apply(source, query.SortColumn, query.SortDirection);

            var count = await source.CountAsync();

            var pageData = await source
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(selector)
                .ToListAsync();


            return new PagingResult<TDto>(
                pageData,
                query.PageIndex,
                query.PageSize,
                count
            );
        }
    }
}

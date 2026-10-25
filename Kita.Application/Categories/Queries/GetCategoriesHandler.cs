using Kita.Application.Common.Interfaces;
using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.QueryParameters;
using Kita.Domain.Categories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Categories.Queries
{
    public class GetCategoriesHandler: IRequestHandler<GetCategoriesQuery, PagingResult<CategoryDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFilterBuilder _filterBuilder;
        private readonly ISortBuilder _sortBuilder;

        public GetCategoriesHandler(IApplicationDbContext context, IFilterBuilder filterBuilder, ISortBuilder sortBuilder)
        {
            _context = context;
            _filterBuilder = filterBuilder;
            _sortBuilder = sortBuilder;
        }

        public async Task<PagingResult<CategoryDTO>> Handle(GetCategoriesQuery command, CancellationToken cancellationToken = default)
        {
            var query = command.Query;
            var categoriesQuery = _context.Categories
                .AsNoTracking()
                .AsQueryable();

            categoriesQuery = _filterBuilder.Apply(categoriesQuery, query.Filters ?? new List<FilterCondition>());

            var isCustomSort = query.SortColumn?.ToLower() == "totalproducts";

            if (isCustomSort)
            {
                categoriesQuery = query.SortDirection?.ToLower() == "desc"
                    ? categoriesQuery.OrderByDescending(x => x.Products.Count())
                    : categoriesQuery.OrderBy(x => x.Products.Count());
            }
            else
            {
                categoriesQuery = _sortBuilder.Apply(categoriesQuery, query.SortColumn, query.SortDirection);
            }

            var items = await categoriesQuery
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new CategoryDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    TotalProducts = x.Products.Count()
                })
                .ToListAsync(cancellationToken);

            return new PagingResult<CategoryDTO>(items, query.PageIndex, query.PageSize, await categoriesQuery.CountAsync());
        }
    }
}

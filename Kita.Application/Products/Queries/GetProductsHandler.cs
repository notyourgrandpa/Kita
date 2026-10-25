using Kita.Application.Common.Interfaces;
using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Domain.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Products.Queries
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductDTO>>
    {
        private readonly IApplicationDbContext _context;

        public GetProductsHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProductDTO>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var query = request.ProductQuery;
            IQueryable<Product> products = _context.Products
                .Include(p => p.Category)
                .AsNoTracking();

            if (query.CategoryId != null)
            {
                products = products.Where(p => p.CategoryId == query.CategoryId);
            }

            if (query.IsActive != null)
            {
                products = products.Where(p => p.IsActive == query.IsActive);
            }

            if (query.UnitId != null)
            {
                products = products.Where(p => ((int)p.Unit) == query.UnitId);
            }

            // For dynamic filtering
            //Expression<Func<Product, bool>>? predicate = null;
            //if (query.FilterColumn != null && query.FilterQuery != null) 
            //{
            //    ParameterExpression param = Expression.Parameter(typeof(Product), "p");
            //    var property = Expression.PropertyOrField(param, query.FilterColumn);
            //    var constant = Expression.Constant(query.FilterQuery);

            //    predicate = Expression.Lambda<Func<Product, bool>>(
            //        Expression.Call(property, typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!, constant),
            //        param
            //    );
            //}

            //if(predicate != null)
            //{
            //    products = products.Where(predicate);
            //}

            if (query.FilterQuery != null)
            {
                products = products.Where(p =>
                p.Name.Contains(query.FilterQuery)
                );
            }

            products = (query.SortColumn, query.SortOrder?.ToLower()) switch
            {
                ("name", "asc") => products.OrderBy(p => p.Name),
                ("name", "desc") => products.OrderByDescending(p => p.Name),
                ("costPrice", "asc") => products.OrderBy(p => p.CostPrice),
                ("costPrice", "desc") => products.OrderByDescending(p => p.CostPrice),
                ("sellingPrice", "asc") => products.OrderBy(p => p.SellingPrice),
                ("sellingPrice", "desc") => products.OrderByDescending(p => p.SellingPrice),
                ("categoryName", "asc") => products.OrderByDescending(p => p.CategoryId),
                ("categoryName", "desc") => products.OrderByDescending(p => p.CategoryId),
                ("stock", "asc") => products.OrderBy(p => p.Stock),
                ("stock", "desc") => products.OrderByDescending(p => p.Stock),
                ("minimumStock", "asc") => products.OrderBy(p => p.MinimumStock),
                ("minimumStock", "desc") => products.OrderByDescending(p => p.MinimumStock),
                ("isActive", "asc") => products.OrderBy(p => p.IsActive),
                ("isActive", "desc") => products.OrderByDescending(p => p.IsActive),
                ("unit", "asc") => products.OrderBy(p => p.Unit),
                ("unit", "desc") => products.OrderByDescending(p => p.Unit),
                ("dateAdded", "asc") => products.OrderBy(p => p.DateAdded),
                ("dateAdded", "desc") => products.OrderByDescending(p => p.DateAdded),
                ("lastUpdated", "asc") => products.OrderBy(p => p.LastUpdated),
                ("lastUpdated", "desc") => products.OrderByDescending(p => p.LastUpdated),
                _ => products.OrderBy(p => p.Name)
            };

            var result = new PagedResult<ProductDTO>(
                await products
                    .Skip((query.PageIndex) * query.PageSize)
                    .Take(query.PageSize)
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category!.Name,
                        CostPrice = p.CostPrice,
                        SellingPrice = p.SellingPrice,
                        Stock = p.Stock,
                        MinimumStock = p.MinimumStock,
                        IsActive = p.IsActive,
                        Unit = p.Unit,
                        UnitName = p.Unit.ToString(),
                        DateAdded = p.DateAdded,
                        LastUpdated = p.LastUpdated
                    })
                    .ToListAsync(),
                await products.CountAsync(),
                query.PageIndex,
                query.PageSize
                );
            return result;
        }
    }
}

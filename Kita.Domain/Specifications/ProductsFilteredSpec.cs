using Kita.Domain.Abstractions;
using Kita.Domain.Products;
using System.Linq.Expressions;

namespace Kita.Domain.Specification
{
    public class ProductsFilteredSpec : Specification<Product>
    {
        public ProductsFilteredSpec(int pageIndex, int pageSize, string sortColumn, string sortOrder, string filterColumn, string? filterQuery, int? categoryId, bool? isActive)
        {
            Criteria = p =>
                (!categoryId.HasValue || p.CategoryId == categoryId) &&
                (string.IsNullOrEmpty(filterQuery) || p.Name.Contains(filterQuery)) &&
                (!isActive.HasValue || p.IsActive == isActive);

            Includes.Add(p => p.Category!);


            if(!string.IsNullOrWhiteSpace(sortColumn) && ProductSortMap.Map.TryGetValue(sortColumn, out var sortExpression))
            {
                if(sortOrder == "asc")
                    OrderBy = sortExpression;
                else if(sortOrder == "desc")
                    OrderByDescending = sortExpression;
            }

            Skip = pageIndex * pageSize;
            Take = pageSize;
            IsPagingEnabled = true;
        }
    }
    
    public static class ProductSortMap
    {
        public static readonly IReadOnlyDictionary<string, Expression<Func<Product, object>>> Map = new Dictionary<string, Expression<Func<Product, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Name"] = p => p.Name,
            ["CategoryName"] = p => p.Category!.Name,
            ["Category"] = p => p.CategoryId,
            ["CostPrice"] = p => p.CostPrice,
            ["SellingPrice"] = p => p.SellingPrice,
            ["Stock"] = p => p.Stock,
            ["DateAdded"] = p => p.DateAdded,
            ["DateUpdated"] = p => p.LastUpdated,
        };
    }
}

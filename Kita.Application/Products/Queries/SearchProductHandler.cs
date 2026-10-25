using Kita.Application.Common.Interfaces;
using Kita.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Products.Queries
{
    public class SearchProductHandler : IRequestHandler<SearchProductQuery, IEnumerable<ProductDTO>>
    {
        private readonly IApplicationDbContext _context;

        public SearchProductHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDTO>> Handle(SearchProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Where(c => c.Name.Contains(request.Term))
                .OrderBy(c => c.Name)
                .Take(20) // limit results for performance
                .Select(c => new ProductDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    SellingPrice = c.SellingPrice
                })
                .ToListAsync();

            return product;
        }
    }
}

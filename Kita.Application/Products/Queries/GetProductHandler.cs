using Kita.Application.Common.Interfaces;
using Kita.Application.DTOs;
using Kita.Domain.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Products.Queries
{
    public class GetProductHandler : IRequestHandler<GetProductQuery, ProductDTO>
    {
        private readonly IApplicationDbContext _context;

        public GetProductHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductDTO> Handle(GetProductQuery request, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if(product == null)
            {
                throw new KeyNotFoundException($"Product {request.Id} not found!");
            }


            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                ProductCode = product.ProductCode,
                SellingPrice = product.SellingPrice,
                CostPrice = product.CostPrice,
                IsActive = product.IsActive,
                UnitName = product.Unit.ToString(),
                Unit = product.Unit,
                CategoryId = product.CategoryId,
                CategoryName = product.Category!.Name,
                Stock = product.Stock,
                MinimumStock = product.MinimumStock,
            };
        }
    }
}

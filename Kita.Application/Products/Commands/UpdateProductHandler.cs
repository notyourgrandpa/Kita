using Kita.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Products.Commands
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        public UpdateProductHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(request.ProductId);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");
            }

            product.Name = request.ProductUpdateDTO.Name;
            product.ProductCode = request.ProductUpdateDTO.ProductCode;
            product.CostPrice = request.ProductUpdateDTO.CostPrice;
            product.SellingPrice = request.ProductUpdateDTO.SellingPrice;
            product.IsActive = request.ProductUpdateDTO.IsActive;
            product.Unit = request.ProductUpdateDTO.Unit;
            product.CategoryId = request.ProductUpdateDTO.CategoryId;
            product.MinimumStock = request.ProductUpdateDTO.MinimumStock;
            product.SetUpdated(DateTime.UtcNow);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

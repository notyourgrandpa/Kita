using Kita.Application.Common.Interfaces;
using Kita.Domain.Abstractions;
using Kita.Domain.Products;
using Kita.Domain.Sales;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Sales_Transactions.Commands
{
    public class VoidSalesTransactionHandler : IRequestHandler<VoidSalesTransactionCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public VoidSalesTransactionHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(VoidSalesTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _context.SalesTransactions
                .Include(t => t.SalesTransactionItems)
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                throw new KeyNotFoundException("Selected transaction not found.");
            }

            if (transaction.IsVoided)
            {
                throw new InvalidOperationException("Transaction is already voided.");
            }


            // Mark as voided
            transaction.IsVoided = true;
            transaction.VoidedAt = DateTime.UtcNow;
            transaction.VoidedByUserId = request.UserId;

            // Reverse inventory changes
            foreach (var item in transaction.SalesTransactionItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity; // restore stock
                    product.SetUpdated(DateTime.UtcNow);
                }
            }

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}

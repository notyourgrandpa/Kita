using Kita.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Sales_Transactions.Commands
{
    public class DeleteSalesTransactionHandler : IRequestHandler<DeleteSalesTransactionCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public DeleteSalesTransactionHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteSalesTransactionCommand request, CancellationToken cancellationToken)
        {
            var salesTransaction = await _context.SalesTransactions.FindAsync(request.SalesTransactionId);

            if(salesTransaction == null)
            {
                throw new KeyNotFoundException($"Sales Transaction {request.SalesTransactionId} not found.");
            }

            _context.SalesTransactions.Remove(salesTransaction);

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}

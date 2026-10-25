using Kita.Application.Common.Interfaces;
using Kita.Domain.Sales;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Sales_Transactions.Query
{
    public class GetSalesTransactionHandler : IRequestHandler<GetSalesTransactionQuery, SalesTransaction>
    {
        private readonly IApplicationDbContext _context;

        public GetSalesTransactionHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SalesTransaction> Handle(GetSalesTransactionQuery request, CancellationToken cancellationToken)
        {
            var salesTransaction = await _context.SalesTransactions.FindAsync(request.SalesTransactionId);

            if(salesTransaction == null)
            {
                throw new KeyNotFoundException($"Sales Transaction {request.SalesTransactionId} not found.");
            }

            return salesTransaction;
        }
    }
}

using Kita.Application.Common.Interfaces;
using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Sales_Transactions.Query
{
    public class GetSalesTransactionsHandler : IRequestHandler<GetSalesTransactionsQuery, PagedResult<SalesTransactionsDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserLookupService _userLookup;

        public GetSalesTransactionsHandler(IApplicationDbContext context, IUserLookupService userLookup)
        {
            _context = context;
            _userLookup = userLookup;
        }

        public async Task<PagedResult<SalesTransactionsDTO>> Handle(GetSalesTransactionsQuery request, CancellationToken cancellationToken)
        {
            var parameter = request.Parameter;
            var salesTransaction = _context.SalesTransactions
                .AsNoTracking();

            salesTransaction = (parameter.SortColumn, parameter.SortOrder?.ToLower()) switch
            {
                ("totalAmount", "asc") => salesTransaction.OrderBy(s => s.TotalAmount),
                ("totalAmount", "desc") => salesTransaction.OrderByDescending(s => s.TotalAmount),
                ("paymentMethod", "asc") => salesTransaction.OrderBy(s => s.PaymentMethod),
                ("paymentMethod", "desc") => salesTransaction.OrderByDescending(s => s.PaymentMethod),
                ("processedBy", "asc") => salesTransaction.OrderBy(s => s.ProcessedByUserId),
                ("processedBy", "desc") => salesTransaction.OrderByDescending(s => s.ProcessedByUserId),
                ("date", "asc") => salesTransaction.OrderBy(s => s.Date),
                ("date", "desc") => salesTransaction.OrderByDescending(s => s.Date),
                _ => salesTransaction.OrderBy(s => s.TotalAmount)
            };

            // Project minimal fields and batch lookup usernames from IUserLookupService
            var items = await salesTransaction.Select(s => new
            {
                s.Id,
                s.TotalAmount,
                s.PaymentMethod,
                s.ProcessedByUserId,
                s.Date
            }).ToListAsync(cancellationToken);

            var userIds = items.Select(i => i.ProcessedByUserId).Where(id => !string.IsNullOrEmpty(id)).Distinct();
            var names = await _userLookup.GetUserNamesAsync(userIds, cancellationToken);

            var dtos = items.Select(i => new SalesTransactionsDTO
            {
                Id = i.Id,
                TotalAmount = i.TotalAmount,
                PaymentMethod = i.PaymentMethod,
                ProcessedByUserName = names.TryGetValue(i.ProcessedByUserId, out var n) ? n : null,
                Date = i.Date
            }).ToList();

            return new PagedResult<SalesTransactionsDTO>(dtos, await salesTransaction.CountAsync(cancellationToken), parameter.PageIndex, parameter.PageSize);
        }
    }
}

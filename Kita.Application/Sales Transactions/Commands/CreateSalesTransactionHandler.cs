using Kita.Application.Common.Interfaces;
using Kita.Application.DTOs;
using Kita.Application.Services;
using Kita.Domain.Abstractions;
using Kita.Domain.Inventory;
using Kita.Domain.Products;
using Kita.Domain.Sales;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Sales_Transactions.Commands
{
    public class CreateSalesTransactionHandler : IRequestHandler<CreateSalesTransactionCommand, SalesTransactionsDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IInventoryLogService _inventoryLogService;

        public CreateSalesTransactionHandler(IApplicationDbContext context, IInventoryLogService inventoryLogService)
        {
            _context = context;
            _inventoryLogService = inventoryLogService;
        }

        public async Task<SalesTransactionsDTO> Handle(CreateSalesTransactionCommand request, CancellationToken cancellationToken)
        {
            var salesTransactionDTO = request.transactionCreateDTO;
            var productIds = salesTransactionDTO.Items.Select(i => i.ProductId).ToList();

            var validIds = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            var invalidIds = productIds.Except(validIds).ToList();
            if (invalidIds.Any())
                throw new InvalidOperationException($"Invalid product IDs: {string.Join(", ", invalidIds)}");

            var salesTransaction = new SalesTransaction
            {
                CustomerId = salesTransactionDTO.CustomerId,
                PaymentMethod = salesTransactionDTO.PaymentMethod,
                ProcessedByUserId = request.UserId,
                Date = DateTime.UtcNow,
                TotalAmount = salesTransactionDTO.Items.Sum(i => i.Quantity * i.UnitPrice)
            };

            foreach (var row in salesTransactionDTO.Items)
            {
                SalesTransactionItem salesTransactionItem = new()
                {
                    ProductId = row.ProductId,
                    Quantity = row.Quantity,
                    UnitPrice = row.UnitPrice,
                    Subtotal = row.Quantity * row.UnitPrice
                };
                salesTransaction.SalesTransactionItems.Add(salesTransactionItem);

                InventoryLogCreateDto inventoryLog = new()
                {
                    ProductId = row.ProductId,
                    Quantity = row.Quantity,
                    ChangeType = InventoryChangeType.StockOut
                };
                await _inventoryLogService.CreateInventoryLogAsync(inventoryLog);
            }

            await _context.SalesTransactions.AddAsync(salesTransaction);

            await _context.SaveChangesAsync();

            return new SalesTransactionsDTO
            {
                Id = salesTransaction.Id,
                TotalAmount = salesTransaction.TotalAmount,
                PaymentMethod = salesTransaction.PaymentMethod,
                ProcessedByUserId = salesTransaction.ProcessedByUserId,
                //ProcessedByUserName = salesTransaction.ProcessedByUser.UserName!,
                Date = salesTransaction.Date,
            };
        }
    }
}

using Kita.Application.Common.Interfaces;
using Kita.Application.Products.Commands;
using Kita.Domain.Abstractions;
using Kita.Domain.Inventory;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Inventory_Logs.Commands
{
    public class VoidInventoryLogHandler : IRequestHandler<VoidInventoryLogCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public VoidInventoryLogHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(VoidInventoryLogCommand request, CancellationToken cancellationToken)
        {

            var inventoryLog = await _context.InventoryLogs
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == request.Id);

            if (inventoryLog == null)
                throw new KeyNotFoundException($"Inventory Log {request.Id} not found.");

            if (inventoryLog.IsVoided)
                throw new InvalidOperationException("This inventory log is already voided.");

            // Void the log
            inventoryLog.IsVoided = true;
            inventoryLog.VoidedAt = DateTime.UtcNow;
            inventoryLog.VoidedByUserId = request.UserId;

            var logs = _context.InventoryLogs
                .Where(l => l.ProductId == inventoryLog.ProductId && l.Id != request.Id)
                .OrderBy(l => l.Date)
                .ThenBy(l => l.Id);

            int runningStock = 0;

            foreach (var log in logs)
            {
                if (log.IsVoided)
                    continue;

                log.OldStock = runningStock;

                switch (log.ChangeType)
                {
                    case InventoryChangeType.StockIn:
                        runningStock += log.Quantity;
                        break;

                    case InventoryChangeType.StockOut:
                        runningStock -= log.Quantity;
                        break;

                    case InventoryChangeType.Adjustment:
                        runningStock = log.Quantity; // absolute adjustment
                        break;

                    default:
                        throw new InvalidOperationException("Unknown inventory change type.");
                }
            }

            inventoryLog.Product!.Stock = runningStock;

            await _context.SaveChangesAsync();
            return Unit.Value;
        }
    }
}

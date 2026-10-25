using Kita.Application.Common.Interfaces;
using Kita.Application.DTOs;
using Kita.Domain.Inventory;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Inventory_Logs.Queries
{
    public class GetInventoryLogHandler : IRequestHandler<GetInventoryLogQuery, InventoryLogDTO>
    {
        private readonly IApplicationDbContext _context;

        public GetInventoryLogHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryLogDTO> Handle(GetInventoryLogQuery request, CancellationToken cancellationToken)
        {
            var inventoryLog = await _context.InventoryLogs
                .Include(i => i.Product)
                .Select(i => new InventoryLogDTO
                {
                    Id = i.Id,
                    ProductName = i.Product!.Name,
                    Date = i.Date,
                    ChangeType = i.ChangeType,
                    IsVoided = i.IsVoided,
                    Quantity = i.Quantity,
                    Remarks =  i.Remarks
                })
                .FirstOrDefaultAsync(i => i.Id == request.InventoryLogId);
            if (inventoryLog == null)
            {
                throw new KeyNotFoundException($"Inventory Log {request.InventoryLogId} not found.");
            }

            return inventoryLog;
        }
    }
}

using Kita.Application.Common.Interfaces;
using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Inventory_Logs.Queries
{
    public class GetInventoryLogsHandler : IRequestHandler<GetInventoryLogsQuery, PagedResult<InventoryLogDTO>>
    {
        private readonly IApplicationDbContext _context;

        public GetInventoryLogsHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<InventoryLogDTO>> Handle(GetInventoryLogsQuery request, CancellationToken cancellationToken)
        {
            var queryParameter = request.QueryParameter;
            var inventoryLogs = _context.InventoryLogs.AsNoTracking();

            inventoryLogs = (queryParameter.SortColumn, queryParameter.SortOrder?.ToLower()) switch
            {
                ("date", "asc") => inventoryLogs.OrderBy(i => i.Date),
                ("date", "desc") => inventoryLogs.OrderByDescending(i => i.Date),
                ("productName", "asc") => inventoryLogs.OrderBy(i => i.ProductId),
                ("productName", "desc") => inventoryLogs.OrderByDescending(i => i.ProductId),
                ("quantity", "asc") => inventoryLogs.OrderBy(i => i.Quantity),
                ("quantity", "desc") => inventoryLogs.OrderByDescending(i => i.Quantity),
                ("changeType", "asc") => inventoryLogs.OrderBy(i => i.ChangeType),
                ("changeType", "desc") => inventoryLogs.OrderByDescending(i => i.ChangeType),
                ("remarks", "asc") => inventoryLogs.OrderBy(i => i.Remarks),
                ("remarks", "desc") => inventoryLogs.OrderByDescending(i => i.Remarks),
                _ => inventoryLogs.OrderBy(i => i.Id)
            };

            var result = new PagedResult<InventoryLogDTO>(
                await inventoryLogs
                .Skip(queryParameter.PageSize * queryParameter.PageIndex)
                    .Take(queryParameter.PageSize)
                    .Select(i => new InventoryLogDTO
                    {
                        Id = i.Id,
                        Date = i.Date,
                        ChangeType = i.ChangeType,
                        IsVoided = i.IsVoided,
                        Quantity = i.Quantity,
                        Remarks = i.Remarks,
                        ProductName = i.Product!.Name,
                        ProductId = i.ProductId
                    })
                    .ToListAsync(), 
                await inventoryLogs.CountAsync(), 
                queryParameter.PageIndex, 
                queryParameter.PageSize);

            return result;
        }
    }
}

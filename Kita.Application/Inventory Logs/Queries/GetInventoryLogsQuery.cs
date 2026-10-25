using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.QueryParameters;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Inventory_Logs.Queries
{
    public class GetInventoryLogsQuery : IRequest<PagedResult<InventoryLogDTO>>
    {
        public required QueryParameter QueryParameter { get; set; }
    }
}

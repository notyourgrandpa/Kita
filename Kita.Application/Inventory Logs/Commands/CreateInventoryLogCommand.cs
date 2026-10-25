using Kita.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Inventory_Logs.Commands
{
    public class CreateInventoryLogCommand : IRequest<InventoryLogDTO>
    {
        public required InventoryLogCreateDto InventoryLogDto { get; set; }
    }
}

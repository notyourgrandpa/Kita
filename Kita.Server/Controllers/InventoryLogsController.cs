using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.Inventory_Logs.Commands;
using Kita.Application.Inventory_Logs.Queries;
using Kita.Application.Products.Commands;
using Kita.Application.QueryParameters;
using Kita.Application.Services;
using Kita.Domain.Inventory;
using Kita.Infrastructure.Persistence.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryLogsController : ControllerBase
    {
        private readonly IInventoryLogService _inventoryLogService;
        private readonly IMediator _mediator;

        public InventoryLogsController(IInventoryLogService inventoryLogService, IMediator mediator)
        {
            _inventoryLogService = inventoryLogService;
            _mediator = mediator;
        }
        // GET: api/InventoryLogs
        // GET: api/InventoryLogs/?pageIndex=0&pageSize=10
        // GET: api/InventoryLogs/?pageIndex=0&pageSize=10&sortColumn=name&sortOrder=asc
        [HttpGet]
        public async Task<PagedResult<InventoryLogDTO>> GetInventoryLogs([FromQuery]QueryParameter parameter)
        {
            return await _mediator.Send(new GetInventoryLogsQuery { QueryParameter = parameter});
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryLogDTO>> GetInventoryLogAsync(int id)
        {
            var inventoryLog = await _mediator.Send(new GetInventoryLogQuery { InventoryLogId = id });

            return inventoryLog;
        }

        [HttpPost]
        public async Task<ActionResult<InventoryLogDTO>> CreateInventoryLogAsync(InventoryLogCreateDto inventoryLogDto)
        {
            InventoryLogDTO inventoryLog = await _mediator.Send(new CreateInventoryLogCommand { InventoryLogDto = inventoryLogDto });

            return CreatedAtAction("GetInventoryLogAsync", new { id = inventoryLog.Id }, inventoryLog);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> VoidInventoryLogAsync(int id)
        {
            await _mediator.Send(new VoidInventoryLogCommand { Id = id, UserId = User.GetUserId() });
            return NoContent();
        }
    }
}

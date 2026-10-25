using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.QueryParameters;
using Kita.Application.Sales_Transactions.Commands;
using Kita.Application.Sales_Transactions.Query;
using Kita.Application.Services;
using Kita.Domain.Sales;
using Kita.Infrastructure.Persistence.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kita.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesTransactionsController: ControllerBase
    {
        private readonly IMediator _mediator;

        public SalesTransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // GET: api/SalesTransactions
        // GET: api/SalesTransactions/?pageIndex=0&pageSize=10
        // GET: api/SalesTransactions/?pageIndex=0&pageSize=10&sortColumn=name&sortOrder=asc
        [HttpGet]
        public async Task<PagedResult<SalesTransactionsDTO>> GetSalesTransactions([FromQuery]QueryParameter query)
        {
            return await _mediator.Send(new GetSalesTransactionsQuery { Parameter = query });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SalesTransaction>> GetSalesTransactionAsync(int id)
        {
            var salesTransaction = await _mediator.Send(new GetSalesTransactionQuery { SalesTransactionId = id });

            return salesTransaction;
        }

        [HttpPost]
        public async Task<ActionResult<SalesTransaction>> CreateSalesTransactionAsync(SalesTransactionCreateDTO salesTransactionLogDto)
        {
            try
            {
                var salesTransactionDto = await _mediator.Send(new CreateSalesTransactionCommand { transactionCreateDTO = salesTransactionLogDto, UserId = User.GetUserId() });

            return CreatedAtAction("GetSalesTransactionAsync", new { id = salesTransactionDto.Id }, salesTransactionDto);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }

        //We should not use this but for development, it's fine. So, I should just leave it here
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesTransactionAsync(int id)
        {
            await _mediator.Send(new DeleteSalesTransactionCommand { SalesTransactionId = id });

            return NoContent();
        }

        [HttpPost("{id}/void")]
        public async Task<IActionResult> VoidSalesTransaction(int id)
        {
            await _mediator.Send(new VoidSalesTransactionCommand { SalesTransactionId = id, UserId = User.GetUserId() });

            return NoContent();
        }
    }
}

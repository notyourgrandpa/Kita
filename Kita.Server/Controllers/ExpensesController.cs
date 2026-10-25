using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.QueryParameters;
using Kita.Application.Services;
using Kita.Domain.Expenses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<ExpenseDTO>>> GetExpensesAsync([FromQuery] QueryParameter parameters)
        {
            return await _expenseService.GetExpensesAsync(parameters);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpenseById(int id)
        {
            return await _expenseService.GetExpenseByIdAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateExpenseAsync(ExpenseCreateDTO expenseCreateDTO)
        {
            var expenseDTO = await _expenseService.CreateExpenseAsync(expenseCreateDTO);
            return CreatedAtAction("GetExpenseById", new { id = expenseDTO.Id }, expenseDTO);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateExpenseAsync(int id, ExpenseUpdateDTO expenseDTO)
        {
            if (id != expenseDTO.Id)
            {
                return BadRequest();
            }
            await _expenseService.UpdateExpenseAsync(id, expenseDTO);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteExpenseAsync(int id)
        {
            await _expenseService.DeleteExpenseAsync(id);
            return NoContent();
        }
    }
}

using Kita.Application.Customers;
using Kita.Application.Customers.Queries;
using Kita.Application.DTOs;
using Kita.Application.Services;
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
    public class CustomersController: ControllerBase
    {
        private readonly IMediator _mediator;
        public CustomersController(IMediator mediator) 
        { 
            _mediator = mediator;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> Search(string term)
        {
            var customers = await _mediator.Send(new SearchCustomersQuery() { Term = term});

            return Ok(customers);
        }
    }
}

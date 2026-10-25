using Kita.Domain.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kita.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UnitsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult GetUnitsAsync()
        {
            var units = Enum.GetValues<UnitOfMeasure>()
                .Select(unit => new
                {
                    value = (int)unit,
                    name = unit.ToString()
                });

            return Ok(units);
        }
    }
}

using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.Products.Commands;
using Kita.Application.Products.Queries;
using Kita.Application.QueryParameters;
using Kita.Application.Services;
using Kita.Domain.Products;
using Kita.Server.Data;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kita.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMediator _mediator;

        public ProductsController(IProductService productService, IMediator mediator)
        {
            _productService = productService;
            _mediator = mediator;
        }

        // GET: api/Products
        // GET: api/Products/?pageIndex=0&pageSize=10
        // GET: api/Products/?pageIndex=0&pageSize=10&sortColumn=name&sortOrder=asc
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductDTO>>> GetProductsAsync([FromQuery]ProductQueryParameter parameters)
        {
            return Ok(await _mediator.Send(new GetProductsQuery { ProductQuery = parameters } ));
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductAsync(int id)
        {
            return await _mediator.Send(new GetProductQuery { Id = id });
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "RegisteredUser")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductAsync(int id, ProductUpdateDTO product)
        {
            if (id != product.Id)
                return BadRequest(new { message = "ID mismatch between route and body." });

            var updated = await _mediator.Send(new UpdateProductCommand
            {
                ProductId = id,
                ProductUpdateDTO = product
            });

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "RegisteredUser")]
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> PostProduct(ProductCreateDTO product)
        {
            var productDTO = await _mediator.Send(new CreateProductCommand { ProductCreateDTO = product });

            return CreatedAtAction(nameof(GetProductAsync), new { id = productDTO.Id }, productDTO);
        }

        // DELETE: api/Products/5
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var deleted = await _mediator.Send(new DeleteProductCommand { ProductId = id });

            return NoContent();
        }

        private async Task<bool> ProductExists(int id)
        {
            return await _productService.ProductExistsAsync(id);
        }

        [Route("IsDupeProduct")]
        public async Task<bool> IsDupeProduct(Product product)
        {
            return await _productService.IsDupeProduct(product);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Search(string term)
        {
            return Ok(await _mediator.Send(new SearchProductQuery { Term = term }));
        }
    }
}

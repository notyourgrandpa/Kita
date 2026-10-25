using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kita.Server.Data;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Kita.Application.Services;
using Kita.Application.DTOs;
using Kita.Domain.Categories;
using Kita.Application.Common.Paging;
using MediatR;
using Kita.Application.Categories.Commands;
using Kita.Application.QueryParameters;
using Kita.Application.Categories.Queries;

namespace Kita.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMediator _mediator;

        public CategoriesController(ICategoryService categoryService, IMediator mediator)
        {
            _categoryService = categoryService;
            _mediator = mediator;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<PagingResult<CategoryDTO>>> GetCategoriesAsync(
            [FromQuery] PagingQuery query)
        {
            return Ok(await _mediator.Send(new GetCategoriesQuery
            {
                Query = query
            }));
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryAsync(int id)
        {
            return await _mediator.Send(new GetCategoryQuery { Id = id });
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> PutCategoryAsync(int id, CategoryUpdateDTO category)
        {
            var updateCommand = new UpdateCategoryCommand() { Id = id, Name = category.Name };
            return Ok(await _mediator.Send(updateCommand));
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "RegisteredUser")]
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> PostCategoryAsync(CategoryCreateInputDTO category)
        {
            var newCategory = await _mediator.Send(new CreateCategoryCommand(category.Name));

            return CreatedAtAction("GetCategoryAsync", new { id = newCategory.Id }, newCategory);
        }

        // DELETE: api/Categories/5
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            await _mediator.Send(new DeleteCategoryCommand(id));

            return NoContent();
        }

        // POST: api/Categories/Archive/5
        [Authorize(Roles = "Administrator")]
        [HttpPost("Archive/{id}")]
        public async Task<IActionResult> ArchiveCategoryAsync(int id)
        {
            await _mediator.Send(new ArchiveCategoryCommand(id));

            return NoContent();
        }

        // POST: api/Categories/Unarchive/5
        [Authorize(Roles = "Administrator")]
        [HttpPost("Unarchive/{id}")]
        public async Task<IActionResult> UnarchiveCategoryAsync(int id)
        {
            await _mediator.Send(new UnarchiveCategoryCommand(id));

            return NoContent();
        }


        [HttpPost]
        [Route("IsDupeField")]
        public async Task<bool> IsDupeField(int categoryId, string fieldName, string fieldValue)
        {
            //// Default approach (using strongly-typed LAMBA expressions)
            //switch (fieldName)
            //{
            //    case "name":
            //        return _context.Categories.Any(
            //            c => c.Name == fieldValue && c.Id != categoryId);

            //    default:
            //        return false;
            //}

            // Alternative approach (using System.Linq.Dynamic.Core)
            return await _categoryService.IsDupeField<Category>(categoryId, fieldName, fieldValue);
        }
    }
}
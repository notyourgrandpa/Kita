using FluentValidation;
using Kita.Application.Common.Interfaces;
using Kita.Application.DTOs;
using Kita.Domain.Categories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Categories.Commands
{
    public class CreateCategoryHandler: IRequestHandler<CreateCategoryCommand, CategoryDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidator<CreateCategoryCommand> _validator;

        public CreateCategoryHandler(IApplicationDbContext context, IValidator<CreateCategoryCommand> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<CategoryDTO> Handle(CreateCategoryCommand command, CancellationToken cancellationToken = default)
        {
            await _validator.ValidateAndThrowAsync(command);

            var category = new Category 
            { 
                Name = command.Name 
            };

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return new CategoryDTO { Id = category.Id, Name =  category.Name };
        }
    }
}

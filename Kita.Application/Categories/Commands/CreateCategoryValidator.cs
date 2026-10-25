using FluentValidation;
using Kita.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Categories.Commands
{
    public class CreateCategoryValidator: AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryValidator(ICategoryRepository repository)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MustAsync(async (name, ct) => 
                    !await repository.CategoryExists(name, ct))
                .WithMessage("Category already exist!");
        }
    }
}

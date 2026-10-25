using Kita.Application.Common.Interfaces;
using Kita.Domain.Categories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Categories.Commands
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Unit>
    {
        private readonly IApplicationDbContext _context; 

        public UpdateCategoryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken = default)
        {
            Category? category = await _context.Categories.FindAsync(request.Id, cancellationToken);

            if(category == null)
            {
                throw new KeyNotFoundException($"Cannot find Category with id {request.Id}");
            }

            category.Name = request.Name;

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}

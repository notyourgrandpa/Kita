using Kita.Application.Common.Interfaces;
using Kita.Domain.Categories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Categories.Commands
{
    public class ArchiveCategoryHandler : IRequestHandler<ArchiveCategoryCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public ArchiveCategoryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(ArchiveCategoryCommand request, CancellationToken cancellationToken)
        {
            Category? category = await _context.Categories.FindAsync(request.Id, cancellationToken);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category {request.Id} not found.");
            }

            category.IsActive = false;

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}

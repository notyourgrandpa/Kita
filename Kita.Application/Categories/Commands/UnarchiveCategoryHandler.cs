using Kita.Application.Common.Interfaces;
using Kita.Domain.Categories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kita.Application.Categories.Commands
{
    public class UnarchiveCategoryHandler : IRequestHandler<UnarchiveCategoryCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UnarchiveCategoryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UnarchiveCategoryCommand request, CancellationToken cancellationToken)
        {
            Category? category = await _context.Categories.FindAsync(request.Id, cancellationToken);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category {request.Id} not found.");
            }

            category.IsActive = true;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
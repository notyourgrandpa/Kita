using Kita.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Categories.Commands
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCategoryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id);

            if(category == null)
            {
                return Unit.Value;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}

using Kita.Application.Common.Interfaces;
using Kita.Application.DTOs;
using Kita.Domain.Categories;
using Kita.Domain.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IApplicationDbContext _context;
        public CategoryRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CategoryExists(string categoryName, CancellationToken ct = default)
        {
            return await _context.Categories.AnyAsync(x => x.Name == categoryName, ct) ;
        }
    }
}

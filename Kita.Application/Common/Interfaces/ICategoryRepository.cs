using Kita.Domain.Categories;
using Kita.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Common.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<bool> CategoryExists(string categoryName, CancellationToken ct);
    }
}

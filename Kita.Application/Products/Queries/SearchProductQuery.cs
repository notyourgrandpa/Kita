using Kita.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Products.Queries
{
    public class SearchProductQuery : IRequest<IEnumerable<ProductDTO>>
    {
        public string Term { get; set; } = string.Empty;
    }
}

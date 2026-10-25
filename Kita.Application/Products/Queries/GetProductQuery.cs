using Kita.Application.DTOs;
using Kita.Domain.Products;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Products.Queries
{
    public class GetProductQuery: IRequest<ProductDTO>
    {
        public int Id { get; set; }
    }
}

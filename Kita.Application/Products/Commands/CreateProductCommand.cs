using Kita.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Products.Commands
{
    public class CreateProductCommand : IRequest<ProductDTO>
    {
        public required ProductCreateDTO ProductCreateDTO { get; set; }
    }
}

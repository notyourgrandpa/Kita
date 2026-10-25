using Kita.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Products.Commands
{
    public class UpdateProductCommand : IRequest<bool>
    {
        public int ProductId { get; set; }
        public ProductUpdateDTO ProductUpdateDTO { get; set; } = null!;
    }
}

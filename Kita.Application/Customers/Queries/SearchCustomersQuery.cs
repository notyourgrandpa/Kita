using Kita.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Customers.Queries
{
    public class SearchCustomersQuery : IRequest<IEnumerable<CustomerDTO>>
    {
        public string Term { get; set; } = string.Empty;
    }
}

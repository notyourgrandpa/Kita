using Kita.Application.Common.Interfaces;
using Kita.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Customers.Queries
{
    public class SearchCustomersHandler: IRequestHandler<SearchCustomersQuery, IEnumerable<CustomerDTO>>
    {
        private readonly IApplicationDbContext _context;

        public SearchCustomersHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerDTO>> Handle(SearchCustomersQuery searchCustomersQuery, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .Where(x => x.Name.Contains(searchCustomersQuery.Term))
                .OrderBy(x => x.Name)
                .Take(20)
                .Select(x => new CustomerDTO
                {
                    Id = x.Id,
                    ContactNumber = x.ContactNumber,
                    Name = x.Name,
                }).ToListAsync();
        }
    }
}

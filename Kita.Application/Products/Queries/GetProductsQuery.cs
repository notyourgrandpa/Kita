using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.QueryParameters;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Products.Queries
{
    public class GetProductsQuery: IRequest<PagedResult<ProductDTO>>
    {
        public required ProductQueryParameter ProductQuery;
    }
}

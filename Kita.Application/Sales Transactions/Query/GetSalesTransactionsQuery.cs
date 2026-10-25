using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.QueryParameters;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Sales_Transactions.Query
{
    public class GetSalesTransactionsQuery : IRequest<PagedResult<SalesTransactionsDTO>>
    {
        public required QueryParameter Parameter { get; set; }
    }
}

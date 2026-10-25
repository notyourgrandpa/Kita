using Kita.Domain.Inventory;
using Kita.Domain.Sales;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Sales_Transactions.Query
{
    public class GetSalesTransactionQuery : IRequest<SalesTransaction>
    {
        public int SalesTransactionId { get; set;}
    }
}

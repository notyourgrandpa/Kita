using Kita.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Sales_Transactions.Commands
{
    public class CreateSalesTransactionCommand : IRequest<SalesTransactionsDTO>
    {
        public required SalesTransactionCreateDTO transactionCreateDTO { get; set; }
        public required string UserId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.DTOs
{
    public class ExpenseCreateDTO
    {
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = String.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}

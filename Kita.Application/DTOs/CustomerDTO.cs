using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.DTOs
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string Name { get; set;  } = string.Empty;
        public string? ContactNumber { get; set; } = string.Empty;
    }
}

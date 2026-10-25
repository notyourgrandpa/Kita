using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.QueryParameters
{
    public class ProductQueryParameter: QueryParameter
    {
        public int? CategoryId { get; set; }
        public bool? IsActive { get; set; }
        public int? UnitId { get; set; }
    }
}

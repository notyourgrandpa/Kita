using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.QueryParameters
{
    public class QueryParameter
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string? SortColumn { get; set; } = null;
        public string? SortOrder { get; set; } = null;
        public string? FilterColumn { get; set; } = null;
        public string? FilterQuery { get; set; } = null;
    }
}

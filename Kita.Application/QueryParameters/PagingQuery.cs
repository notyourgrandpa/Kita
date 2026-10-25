using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.QueryParameters
{
    public class PagingQuery
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }

        public List<FilterCondition>? Filters { get; set; }
    }

    public class FilterCondition
    {
        public string Field { get; set; } = default!;
        public FilterOperator Operator { get; set; }
        public string Value { get; set; } = default!;
    }

    public enum FilterOperator
    {
        Contains,
        StartsWith,
        Equals,
        GreaterThan,
        LessThan
    }
}

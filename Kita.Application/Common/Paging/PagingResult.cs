using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Common.Paging
{
    public class PagingResult<T>
    {
        public List<T> Data { get; private set; }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public string? FilterColumn { get; set; }
        public string? FilterQuery { get; set; }

        public bool HasPreviousPage => PageIndex > 0;
        public bool HasNextPage => (PageIndex + 1) < TotalPages;

        public PagingResult(
            List<T> data,
            int pageIndex,
            int pageSize,
            int totalCount)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }
}

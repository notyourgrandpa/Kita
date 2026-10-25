using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.QueryParameters;
using MediatR;

namespace Kita.Application.Categories.Queries
{
    public class GetCategoriesQuery: IRequest<PagingResult<CategoryDTO>>
    {
        public PagingQuery Query { get; set; } = new();
    }
}

using Kita.Domain.Categories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Categories.Queries
{
    public class GetCategoryQuery: IRequest<Category>
    {
        public int Id { get; set; }
    }
}
